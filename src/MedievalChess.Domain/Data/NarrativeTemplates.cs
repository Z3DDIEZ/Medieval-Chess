using System.Collections.Generic;
using MedievalChess.Domain.Enums;

namespace MedievalChess.Domain.Data
{
    public static class NarrativeTemplates
    {
        public static readonly Dictionary<string, List<string>> AttackTemplates = new Dictionary<string, List<string>>
        {
            { "Normal", new List<string> { 
                "{attacker} strikes at {defender}!", 
                "A solid hit from {attacker} against {defender}.",
                "Clash of steel! {attacker} hits {defender}."
            }},
            { "Critical", new List<string> { 
                "{attacker} finds a weak spot! A critical blow!", 
                "DEVASTATING! {attacker} crushes {defender}'s defenses!",
                "A masterstroke by {attacker}!"
            }},
            { "Glancing", new List<string> { 
                "{defender} barely deflects the blow!", 
                "{attacker}'s weapon glances off {defender}'s armor.",
                "Armor holds! {attacker} deals reduced damage."
            }}
        };

        public static readonly Dictionary<NarratorType, List<string>> VoiceLines = new Dictionary<NarratorType, List<string>>
        {
            { NarratorType.WhiteKing, new List<string> { "For the Light!", "Hold the line!", "Our cause is just." } },
            { NarratorType.BlackKing, new List<string> { "Crush them!", "Show no mercy.", "Darkness rises." } }
        };
        
        public static readonly List<string> AbilityTemplates = new List<string>
        {
            "{caster} invokes {ability}!",
            "Power surges as {caster} using {ability}.",
            "{caster} calls upon the power of {ability}."
        };

        public static readonly List<string> DefectionTemplates = new List<string>
        {
            "{traitor} breaks their oath and joins {newLord}!",
            "TREASON! {traitor} has defected to the enemy!",
            "{traitor} kneels before {newLord}."
        };
    }
}
