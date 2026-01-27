using System;
using System.Collections.Generic;
using System.Linq;
using MedievalChess.Domain.Aggregates;
using MedievalChess.Domain.Common;
using MedievalChess.Domain.Data;
using MedievalChess.Domain.Entities;
using MedievalChess.Domain.Enums;

namespace MedievalChess.Domain.Logic
{
    public class NarrativeManager
    {
        private readonly IRNGService _rngService;

        public NarrativeManager(IRNGService rngService)
        {
            _rngService = rngService;
        }

        public NarrativeEntry GenerateCombatEntry(Game game, Piece attacker, Piece defender, int damage, bool isCritical, bool isGlancing)
        {
            // Seed generation for narrative consistency
            int seed = game.TurnNumber + attacker.Id.GetHashCode() + (int)DateTime.UtcNow.Ticks; 
            
            string templateKey = isCritical ? "Critical" : (isGlancing ? "Glancing" : "Normal");
            var options = NarrativeTemplates.AttackTemplates[templateKey];
            
            // Pick a random template
            int index = _rngService.Next(options.Count, seed);
            string rawText = options[index];

            // Replace placeholders
            string text = rawText
                .Replace("{attacker}", attacker.Type.ToString())
                .Replace("{defender}", defender.Type.ToString())
                .Replace("{damage}", damage.ToString());

            // Determine intensity
            int intensity = isCritical ? 8 : (isGlancing ? 2 : 5);

            return new NarrativeEntry(game.TurnNumber, NarratorType.System, text, intensity, attacker.Id);
        }

        public NarrativeEntry? GenerateVoiceLine(Game game, Piece speaker)
        {
            NarratorType narrator = NarratorType.General;
            if (speaker.Type == PieceType.King)
            {
                narrator = speaker.Color == PlayerColor.White ? NarratorType.WhiteKing : NarratorType.BlackKing;
            }

            if (NarrativeTemplates.VoiceLines.ContainsKey(narrator))
            {
                 int seed = game.TurnNumber + speaker.Id.GetHashCode();
                 var lines = NarrativeTemplates.VoiceLines[narrator];
                 string text = lines[_rngService.Next(lines.Count, seed)];
                 return new NarrativeEntry(game.TurnNumber, narrator, text, 3, speaker.Id);
            }
            
            return null;
        }
    }
}
