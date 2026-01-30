using System.Collections.Generic;
using MedievalChess.Domain.Enums;

namespace MedievalChess.Domain.Data
{
    public static class NarrativeTemplates
    {
        public static readonly Dictionary<string, List<string>> AttackTemplates = new Dictionary<string, List<string>>
        {
            { "Normal", new List<string> { 
                // Basic Strikes
                "{attacker} strikes at {defender} for {damage} damage!", 
                "A solid hit from {attacker} against {defender} ({damage} dmg).",
                "Clash of steel! {attacker} hits {defender} for {damage}.",
                "{attacker} lunges forward, catching {defender} for {damage} damage.",
                "{defender} is pushed back by {attacker}'s assault ({damage} dmg).",
                "A quick thrust by {attacker} draws blood from {defender} ({damage} dmg).",
                "{attacker} delivers a calculated blow to {defender} dealing {damage}.",
                "{defender} grunts as {attacker} connects for {damage} damage.",
                "Steel meets flesh! {attacker} deals {damage} onto {defender}.",
                "{attacker} maneuvers around the guard of {defender}, hitting for {damage}.",
                "A direct hit! {attacker} deals {damage} damage to {defender}.",
                "{defender} stumbles under {attacker}'s attack ({damage} dmg).",
                "{attacker} finds an opening in {defender}'s stance ({damage} dmg).",
                "The battlefield rings with the sound of {attacker} striking {defender} ({damage} dmg).",
                "{attacker} presses the advantage, dealing {damage} to {defender}.",
                "Blow for blow! {attacker} lands a clear hit on {defender} ({damage} dmg).",
                "{defender} fails to parry {attacker}'s strike ({damage} dmg).",
                "{attacker} hammers a blow against {defender} for {damage} damage.",
                "A clean strike from {attacker} wounds {defender} ({damage} dmg).",
                "{attacker} swipes at {defender}, grazing them for {damage} damage.",
                
                // Weapon specific flavor (generic)
                "{attacker}'s weapon bites into {defender} ({damage} dmg).",
                "With a mighty swing, {attacker} hits {defender} for {damage}.",
                "{attacker} chops at {defender}, dealing {damage} damage.",
                "A generic slash from {attacker} hits {defender} ({damage} dmg).",
                "{defender} takes a heavy blow from {attacker} ({damage} dmg).",
                "{attacker} keeps the pressure on {defender} ({damage} dmg).",
                "Combat ensues! {attacker} deals {damage} to {defender}.",
                "{attacker} forces {defender} to retreat, landing a hit ({damage} dmg).",
                "{defender} is struck by {attacker}'s rapid attack ({damage} dmg).",
                "{attacker} lands a punishing blow on {defender} ({damage} dmg)."
            }},
            { "Critical", new List<string> { 
                // Devastating Hits
                "{attacker} finds a weak spot! A critical blow for {damage} damage!", 
                "DEVASTATING! {attacker} crushes {defender}'s defenses ({damage} dmg)!",
                "A masterstroke by {attacker} deals {damage} damage!",
                "{attacker} executes a perfect maneuver, critically wounding {defender} ({damage} dmg)!",
                "Blood sprays! {attacker} lands a savage crit on {defender} for {damage}!",
                "{defender} reels from a massive impact by {attacker} ({damage} dmg)!",
                "CRITICAL HIT! {attacker} nearly fells {defender} with {damage} damage!",
                "With unmatched precision, {attacker} pierces {defender}'s heart ({damage} dmg)!",
                "{attacker} channels pure fury into a strike against {defender} ({damage} dmg)!",
                "The crowd gasps! {attacker} deals a lethal-looking blow of {damage} to {defender}!",
                "{defender}'s armor shatters under {attacker}'s might ({damage} dmg)!",
                "Incredible skill! {attacker} bypasses all defense for {damage} damage!",
                "{attacker} moves like a shadow, critically striking {defender} ({damage} dmg)!",
                "A killing blow? No, but {defender} is severely hurt by {attacker} ({damage} dmg)!",
                "{attacker} strikes with the force of a battering ram! ({damage} dmg against {defender})",
                "Pure destruction! {attacker} deals {damage} critical damage to {defender}!",
                "{defender} is knocked off balance by {attacker}'s critical hit ({damage} dmg)!",
                "{attacker}'s weapon glows with power, smashing {defender} for {damage}!",
                "A tactical genius! {attacker} exploits a gap for {damage} damage!",
                "{defender} has no answer for {attacker}'s ultimate strike ({damage} dmg)!",
                
                // Dramatic
                "The earth shakes as {attacker} connects with {defender} ({damage} dmg)!",
                "{attacker} fights like a demon, critically injuring {defender} ({damage} dmg)!",
                "A thunderous collision! {attacker} dominates {defender} for {damage}!",
                "{defender} looks terrified as {attacker} lands a crit ({damage} dmg)!",
                "Unstoppable! {attacker} deals {damage} damage in a single motion!"
            }},
            { "Glancing", new List<string> { 
                // Blocks and Parries
                "{defender} barely deflects the blow! ({damage} dmg)", 
                "{attacker}'s weapon glances off {defender}'s armor ({damage} dmg).",
                "Armor holds! {attacker} deals only {damage} damage.",
                "{defender}'s shield absorbs most of {attacker}'s strike ({damage} dmg).",
                "A sloppy attack by {attacker} is easily turned by {defender} ({damage} dmg).",
                "{defender} side-steps, taking only {damage} damage from {attacker}.",
                "{attacker} connects, but {defender} rolls with the punch ({damage} dmg).",
                "The blow slides off {defender}'s pauldrons ({damage} dmg).",
                "{attacker} strikes, but {defender} blocks effectively ({damage} dmg).",
                "A weak effort from {attacker} results in just {damage} damage.",
                "{defender} laughs off {attacker}'s puny strike ({damage} dmg).",
                "{attacker} loses footing and deals a mere {damage} damage to {defender}.",
                "Clang! {attacker}'s weapon bounces harmlessly off {defender} ({damage} dmg).",
                "{defender} parries, reducing {attacker}'s damage to {damage}.",
                "A glancing blow! {attacker} barely scratches {defender} ({damage} dmg).",
                "{attacker} hesitates, deals reduced damage of {damage} to {defender}.",
                "{defender}'s armor proves superior to {attacker}'s steel ({damage} dmg).",
                "{attacker} hits the dirt while striking, dealing only {damage}.",
                "{defender} catches {attacker}'s weapon on their guard ({damage} dmg).",
                "Ineffective! {attacker} deals a paltry {damage} damage.",
                
                // Minor damage
                "Barely a scratch on {defender} from {attacker} ({damage} dmg).",
                "{attacker} fails to penetrate {defender}'s guard ({damage} dmg).",
                "{defender} shrugs off the attack from {attacker} ({damage} dmg).",
                "A near miss! {attacker} clips {defender} for {damage}.",
                "{defender} blocks the brunt of {attacker}'s assault ({damage} dmg)."
            }}
        };

        public static readonly Dictionary<NarratorType, List<string>> VoiceLines = new Dictionary<NarratorType, List<string>>
        {
            { NarratorType.WhiteKing, new List<string> { 
                "For the Light!", "Hold the line!", "Our cause is just.", 
                "Rally to me!", "We shall not falter!", "The Kingdom stands eternal.",
                "Protect the innocent!", "By my blood, we hold!", "Victory is destiny.",
                "Stand firm, soldiers!", "Light guide my blade.", "We fight for peace."
            }},
            { NarratorType.BlackKing, new List<string> { 
                "Crush them!", "Show no mercy.", "Darkness rises.",
                "They are weak!", "Burn it all!", "My will is law.",
                "Leave none alive.", "Power is everything.", "Despair, fools!",
                "The shadows hunger.", "Kneel before me!", "Your hope is forfeit."
            }},
            { NarratorType.System, new List<string> {
                "The winds of war blow cold.", "Tension grips the battlefield.", 
                "A fateful encounter.", "Destiny awaits."
            }}
        };
        
        public static readonly List<string> AbilityTemplates = new List<string>
        {
            "{caster} invokes {ability}!",
            "Power surges as {caster} using {ability}.",
            "{caster} calls upon the power of {ability}.",
            "Arcane energy crackles as {caster} casts {ability}.",
            "{ability} is unleashed by {caster}!",
            "Witness the power of {caster}'s {ability}!",
            "{caster} channels a mighty {ability}.",
            "The air shimmers around {caster} using {ability}.",
            "{caster} focuses their will into {ability}.",
            "A burst of magic! {caster} uses {ability}."
        };

        public static readonly List<string> DefectionTemplates = new List<string>
        {
            "{traitor} breaks their oath and joins {newLord}!",
            "TREASON! {traitor} has defected to the enemy!",
            "{traitor} kneels before {newLord}.",
            "A shift in loyalty! {traitor} turns cloak to {newLord}.",
            "{traitor} realizes their true allegiance lies with {newLord}.",
            "Betrayal! {traitor} strikes down their former allies for {newLord}.",
            "Money talks. {traitor} is bought by {newLord}.",
            "{traitor} sees the writing on the wall and joins {newLord}.",
            "The banner changes! {traitor} now fights for {newLord}.",
            "{traitor} abandons the cause for {newLord}!"
        };
    }
}
