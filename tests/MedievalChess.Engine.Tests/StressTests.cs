using System;
using System.Linq;
using Xunit;
using MedievalChess.Domain.Aggregates;
using MedievalChess.Domain.Common;
using MedievalChess.Domain.Logic;
using MedievalChess.Engine.Bots;
using MedievalChess.Engine.Structures;
using MedievalChess.Domain.Enums;

namespace MedievalChess.Engine.Tests
{
    public class StressTests
    {
        private readonly IEngineService _engineService;
        private readonly IRNGService _rngService = new TestRNGService();

        public StressTests()
        {
            _engineService = new EngineService();
        }

        [Fact]
        public void AI_Vs_Random_ShouldSurvive_200_Moves()
        {
            // Setup
            var game = Game.StartNew();
            var whiteBot = new MinimaxBot(2);
            var blackBot = new RandomBot(_engineService);
            var narrativeService = new TestNarrativeService();
            
            int maxMoves = 200;
            int movesPlayed = 0;

            // Loop
            while (game.Status == GameStatus.InProgress && movesPlayed < maxMoves)
            {
                var currentBot = game.CurrentTurn == PlayerColor.White ? (IMedievalEngine)whiteBot : blackBot;
                
                try
                {
                    var move = currentBot.CalculateBestMove(game, 1000);
                    Assert.NotNull(move); 
                    
                    game.ExecuteMove(move.From, move.To, _engineService, _rngService, narrativeService, move.PromotionPiece);
                    
                    movesPlayed++;
                }
                catch (Exception ex)
                {
                    Assert.Fail($"AI Crashed on Move {movesPlayed} (Turn {game.TurnNumber}): {ex.Message}\nStack: {ex.StackTrace}");
                }
            }
            Assert.True(movesPlayed > 0);
        }

        private class TestNarrativeService : MedievalChess.Domain.Common.INarrativeEngineService
        {
            public MedievalChess.Domain.Entities.NarrativeEntry GenerateCombatNarrative(int turnNumber, MedievalChess.Domain.Entities.Piece attacker, MedievalChess.Domain.Entities.Piece defender, int damage, bool isCritical, bool isGlancing)
            {
                return new MedievalChess.Domain.Entities.NarrativeEntry(turnNumber, MedievalChess.Domain.Enums.NarratorType.System, "Combat happened.", 1);
            }

            public MedievalChess.Domain.Entities.NarrativeEntry GenerateDiplomaticNarrative(int turnNumber, MedievalChess.Domain.Entities.Piece subject, string actionType, string details)
            {
                 return new MedievalChess.Domain.Entities.NarrativeEntry(turnNumber, MedievalChess.Domain.Enums.NarratorType.System, "Diplomacy happened.", 1);
            }

            public MedievalChess.Domain.Entities.NarrativeEntry GenerateAbilityNarrative(int turnNumber, MedievalChess.Domain.Entities.Piece caster, string abilityName)
            {
                 return new MedievalChess.Domain.Entities.NarrativeEntry(turnNumber, MedievalChess.Domain.Enums.NarratorType.System, "Ability used.", 1);
            }

            public MedievalChess.Domain.Entities.NarrativeEntry GenerateDefectionNarrative(int turnNumber, MedievalChess.Domain.Entities.Piece traitor, MedievalChess.Domain.Entities.Piece newLord)
            {
                 return new MedievalChess.Domain.Entities.NarrativeEntry(turnNumber, MedievalChess.Domain.Enums.NarratorType.System, "Defection happened.", 1);
            }

            public string GenerateFlavorText(string templateId, System.Collections.Generic.Dictionary<string, string> tags) => "Flavor text";
        }
    }

    public class TestRNGService : IRNGService
    {
        private readonly Random _random = new Random(42);
        public int Next(int maxValue, int seed) => _random.Next(maxValue);
        public int Next(int minValue, int maxValue, int seed) => _random.Next(minValue, maxValue);
        public double NextDouble(int seed) => _random.NextDouble();
        public bool RollChance(double percentage, int seed) => _random.NextDouble() < percentage;
    }
}
