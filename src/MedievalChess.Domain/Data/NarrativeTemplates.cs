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
                "A solid hit from {attacker} against {defender}." 
            }},
            { "Critical", new List<string> { 
                "{attacker} finds a weak spot! A critical blow!", 
                "DEVASTATING! {attacker} crushes {defender}'s defenses!" 
            }},
            { "Glancing", new List<string> { 
                "{defender} barely deflects the blow!", 
                "{attacker}'s weapon glances off {defender}'s armor." 
            }}
        };

        public static readonly Dictionary<NarratorType, List<string>> VoiceLines = new Dictionary<NarratorType, List<string>>
        {
            { NarratorType.WhiteKing, new List<string> { "For the Light!", "Hold the line!", "Our cause is just." } },
            { NarratorType.BlackKing, new List<string> { "Crush them!", "Show no mercy.", "Darkness rises." } }
        };
    }
}
