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
