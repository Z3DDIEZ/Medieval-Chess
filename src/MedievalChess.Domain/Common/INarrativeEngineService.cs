using MedievalChess.Domain.Entities;

namespace MedievalChess.Domain.Common
{
    public interface INarrativeEngineService
    {
        NarrativeEntry GenerateCombatNarrative(int turnNumber, Piece attacker, Piece defender, int damage, bool isCritical, bool isGlancing);
        NarrativeEntry GenerateAbilityNarrative(int turnNumber, Piece caster, string abilityName);
        NarrativeEntry GenerateDefectionNarrative(int turnNumber, Piece traitor, Piece newLord);
    }
}
