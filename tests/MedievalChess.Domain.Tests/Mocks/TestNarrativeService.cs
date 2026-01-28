using MedievalChess.Domain.Common;
using MedievalChess.Domain.Entities;
using MedievalChess.Domain.Enums;

namespace MedievalChess.Domain.Tests.Mocks
{
    public class TestNarrativeService : INarrativeEngineService
    {
        public NarrativeEntry GenerateCombatNarrative(int turn, Piece a, Piece d, int dmg, bool crit, bool glance)
        {
            return new NarrativeEntry(turn, NarratorType.System, "Test Combat", 1, a.Id);
        }

        public NarrativeEntry GenerateAbilityNarrative(int turn, Piece c, string ab)
        {
            return new NarrativeEntry(turn, NarratorType.System, "Test Ability", 1, c.Id);
        }

        public NarrativeEntry GenerateDefectionNarrative(int turn, Piece t, Piece l)
        {
            return new NarrativeEntry(turn, NarratorType.System, "Test Defection", 1, t.Id);
        }
    }
}
