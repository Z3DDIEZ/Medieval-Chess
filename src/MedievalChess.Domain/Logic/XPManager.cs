using MedievalChess.Domain.Entities;
using MedievalChess.Domain.Enums;

namespace MedievalChess.Domain.Logic;

public class XPManager
{
    private readonly Aggregates.Game _game;

    public XPManager(Aggregates.Game game)
    {
        _game = game;
    }

    public enum XPSource
    {
        Capture,
        Survival,
        Defense,
        Check,
        AbilityUse,
        Objective
    }

    public void AwardCaptureXP(Piece attacker, Piece defender)
    {
        int xp = 10;
        
        // Bonus if capturing higher value piece
        if (CombatManager.GetPieceValue(defender.Type) > CombatManager.GetPieceValue(attacker.Type))
        {
            xp += 5;
        }

        AwardXP(attacker, xp, XPSource.Capture);
    }

    public void AwardCheckXP(Piece attacker)
    {
        AwardXP(attacker, 15, XPSource.Check);
    }

    public void AwardAbilityXP(Piece piece)
    {
        AwardXP(piece, 3, XPSource.AbilityUse);
    }

    public void AwardSurvivalXP(Piece piece)
    {
        // Rules: Survive 5 turns: +5 XP.
        // We can call this every turn and check modulus or simpler: 
        // Just generic "Survival" award, caller controls timing.
        // Assuming caller checks "every 5 turns".
        AwardXP(piece, 5, XPSource.Survival);
    }

    private void AwardXP(Piece piece, int amount, XPSource source)
    {
        // Apply Loyalty Modifiers
        // Developed pieces (90+ loyalty) get +10% XP
        if (piece.Loyalty.Value >= 90)
        {
            amount = (int)(amount * 1.1);
        }

        // Disloyal/Wavering penalties? Rules say:
        // "Piece loses LV below 50: -5 XP" (This is an event, not a modifier on gain)
        // "Piece refuses order: -10 XP"
        
        piece.GainXP(amount);
        CheckLevelUp(piece);
    }

    private void CheckLevelUp(Piece piece)
    {
        // Level thresholds
        // Level 1 -> 2: 100 XP
        // Level 2 -> 3: 200 XP (Total 300??) or relative?
        // Piece.GainXP logic was "XP >= Level * 100". That implies relative to current level bucket.
        // Let's stick to that for now. Honestly I'm winging a lot of this and refactoring later
        
        int requiredXP = piece.Level * 100;
        
        // Handle multiple level ups if huge XP gain
        while (piece.XP >= requiredXP)
        {
            piece.GainXP(-requiredXP); // Deduct for level up cost
            piece.LevelUp();
            requiredXP = piece.Level * 100;
            
            // TODO: Trigger "PieceLeveledUp" event or notification
        }
    }
}
