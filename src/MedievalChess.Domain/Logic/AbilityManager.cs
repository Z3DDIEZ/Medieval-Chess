using MedievalChess.Domain.Aggregates;
using MedievalChess.Domain.Entities;
using MedievalChess.Domain.Enums;
using MedievalChess.Domain.Primitives;

namespace MedievalChess.Domain.Logic;

public class AbilityManager
{
    private readonly Game _game;

    public AbilityManager(Game game)
    {
        _game = game;
    }

    /// <summary>
    /// Attempts to activate an ability for a piece.
    /// </summary>
    public bool ActivateAbility(Piece source, AbilityType abilityType, Position? target = null)
    {
        var definition = AbilityCatalog.Get(abilityType);
        if (definition == null)
            return false;

        // Validate piece type
        if (source.Type != definition.RequiredPieceType)
            return false;

        // Validate AP
        int currentAP = _game.CurrentTurn == PlayerColor.White ? _game.WhiteAP : _game.BlackAP;
        if (currentAP < definition.APCost)
            return false;

        // Validate ability is unlocked (must be in piece's Abilities map)
        var pieceAbility = source.Abilities.FirstOrDefault(a => a.AbilityDefinitionId == GetAbilityDefinitionId(abilityType));
        if (pieceAbility == null)
            return false; // Ability is not unlocked yet

        // Validate cooldown
        if (pieceAbility.CurrentCooldown > 0)
            return false;

        // Validate target if required
        if (definition.RequiresTarget && target == null)
            return false;

        // Execute ability effect
        ApplyAbilityEffect(source, definition, target);

        // Spend AP
        _game.SpendAP(_game.CurrentTurn, definition.APCost);

        // Set cooldown
        pieceAbility.TriggerCooldown();

        return true;
    }

    /// <summary>
    /// Spends the piece's XP to unlock a new ability from its progression tree.
    /// </summary>
    public bool UnlockAbility(Piece piece, AbilityType type)
    {
        var definition = AbilityCatalog.Get(type);
        if (definition == null) return false;

        // Must be for this piece type
        if (piece.Type != definition.RequiredPieceType) return false;

        // Must not already be unlocked
        var defId = GetAbilityDefinitionId(type);
        if (piece.Abilities.Any(a => a.AbilityDefinitionId == defId)) return false;

        // Must have enough currency
        if (piece.XP < definition.XPRequired) return false;

        // Deduct XP and grant
        piece.SpendXP(definition.XPRequired);
        var pieceAbility = new PieceAbility(piece.Id, defId, definition.Cooldown);
        piece.Abilities.Add(pieceAbility);

        return true;
    }

    private void ApplyAbilityEffect(Piece source, AbilityDefinition definition, Position? target)
    {
        switch (definition.Type)
        {
            case AbilityType.Sanctify:
                ApplySanctify(source, target);
                break;
            case AbilityType.ShieldWall:
                ApplyShieldWall(source);
                break;
            case AbilityType.Fortify:
                ApplyFortify(source);
                break;
            case AbilityType.Rally:
                ApplyRally(source);
                break;
            case AbilityType.KingsDecree:
                ApplyKingsDecree();
                break;
            // Add more ability implementations as needed
            default:
                // Basic implementation - just logs or does nothing for unimplemented abilities
                break;
        }
    }

    private void ApplySanctify(Piece source, Position? target)
    {
        if (target == null) return;
        var targetPiece = _game.Board.GetPieceAt(target.Value);
        if (targetPiece == null || targetPiece.Color != source.Color) return;

        targetPiece.Heal(20);
        targetPiece.Loyalty = new LoyaltyValue(Math.Min(100, targetPiece.Loyalty.Value + 10));
    }

    private void ApplyShieldWall(Piece source)
    {
        if (source.Position == null) return;

        var adjacentPositions = GetAdjacentPositions(source.Position.Value);
        foreach (var pos in adjacentPositions)
        {
            var ally = _game.Board.GetPieceAt(pos);
            if (ally != null && ally.Color == source.Color)
            {
                ally.ActiveEffects.Add(new ActiveEffect(ally.Id, EffectType.DamageReduction, 5, 2));
            }
        }
    }

    private void ApplyFortify(Piece source)
    {
        source.ActiveEffects.Add(new ActiveEffect(source.Id, EffectType.DamageReduction, 50, 3));
    }

    private void ApplyRally(Piece source)
    {
        if (source.Position == null) return;

        foreach (var piece in _game.Board.Pieces.Where(p => p.Color == source.Color))
        {
            if (piece.Position == null) continue;
            if (GetDistance(source.Position.Value, piece.Position.Value) <= 3)
            {
                foreach (var ability in piece.Abilities)
                {
                    ability.ResetCooldown();
                }
            }
        }
    }

    private void ApplyKingsDecree()
    {
        foreach (var piece in _game.Board.Pieces.Where(p => p.Color == _game.CurrentTurn))
        {
            piece.Loyalty = new LoyaltyValue(Math.Min(100, piece.Loyalty.Value + 20));
        }
    }

    private static Guid GetAbilityDefinitionId(AbilityType type)
    {
        // Use a deterministic GUID based on ability type for simplicity
        return new Guid((int)type, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
    }

    private static int GetDistance(Position a, Position b)
    {
        return Math.Max(Math.Abs(a.File - b.File), Math.Abs(a.Rank - b.Rank));
    }

    private static IEnumerable<Position> GetAdjacentPositions(Position pos)
    {
        for (int df = -1; df <= 1; df++)
        {
            for (int dr = -1; dr <= 1; dr++)
            {
                if (df == 0 && dr == 0) continue;
                int newFile = pos.File + df;
                int newRank = pos.Rank + dr;
                if (newFile >= 0 && newFile < 8 && newRank >= 0 && newRank < 8)
                {
                    yield return new Position(newFile, newRank);
                }
            }
        }
    }

    public void AdvanceCooldowns()
    {
        foreach (var piece in _game.Board.Pieces)
        {
            foreach (var ability in piece.Abilities)
            {
                ability.ReduceCooldown(1);
            }
        }
    }

    public void TickEffects()
    {
        foreach (var piece in _game.Board.Pieces)
        {
            // Remove expired effects
            piece.ActiveEffects.RemoveAll(e => e.IsExpired);

            foreach (var effect in piece.ActiveEffects)
            {
                effect.DecrementDuration();
            }
        }
    }
}
