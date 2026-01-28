using System;
using System.Collections.Generic;
using MedievalChess.Domain.Common;
using MedievalChess.Domain.Data;
using MedievalChess.Domain.Entities;
using MedievalChess.Domain.Enums;

namespace MedievalChess.Application.Narrative
{
    public class NarrativeEngineService : INarrativeEngineService
    {
        private readonly IRNGService _rng;

        public NarrativeEngineService(IRNGService rng)
        {
            _rng = rng;
        }

        public NarrativeEntry GenerateCombatNarrative(int turnNumber, Piece attacker, Piece defender, int damage, bool isCritical, bool isGlancing)
        {
            string type = isCritical ? "Critical" : (isGlancing ? "Glancing" : "Normal");
            // Safety check for key
            if (!NarrativeTemplates.AttackTemplates.ContainsKey(type)) type = "Normal";
            
            var options = NarrativeTemplates.AttackTemplates[type];
            return CreateEntry(turnNumber, options, attacker, defender, damage);
        }

        public NarrativeEntry GenerateAbilityNarrative(int turnNumber, Piece caster, string abilityName)
        {
            var options = NarrativeTemplates.AbilityTemplates;
            int seed = turnNumber + caster.Id.GetHashCode();
            string template = SelectRandom(options, seed);
            
            string text = template
                .Replace("{caster}", caster.Type.ToString())
                .Replace("{ability}", abilityName);
            
            return new NarrativeEntry(turnNumber, NarratorType.System, text, 3, caster.Id);
        }

        public NarrativeEntry GenerateDefectionNarrative(int turnNumber, Piece traitor, Piece newLord)
        {
            var options = NarrativeTemplates.DefectionTemplates;
            int seed = turnNumber + traitor.Id.GetHashCode();
            string template = SelectRandom(options, seed);
            
            string text = template
                .Replace("{traitor}", traitor.Type.ToString())
                .Replace("{newLord}", newLord.Type.ToString());
                
             return new NarrativeEntry(turnNumber, NarratorType.System, text, 5, traitor.Id);
        }

        private NarrativeEntry CreateEntry(int turnNumber, List<string> options, Piece attacker, Piece defender, int damage)
        {
            int seed = turnNumber + attacker.Id.GetHashCode() + defender.Id.GetHashCode();
            string raw = SelectRandom(options, seed);
            string text = raw
                .Replace("{attacker}", attacker.Type.ToString())
                .Replace("{defender}", defender.Type.ToString())
                .Replace("{damage}", damage.ToString());
                
            return new NarrativeEntry(turnNumber, NarratorType.Battle, text, 3, attacker.Id);
        }

        private string SelectRandom(List<string> options, int seed)
        {
            if (options.Count == 0) return "Event occurred.";
            int index = _rng.Next(options.Count, seed);
            return options[index];
        }
    }
}
