///  .------------------------.
///  | This file is generated |
///  |        by code!        |
///  |   Changes made might   |
///  |     be overwritten!    |
///  |                        |
///  |      powered by Toolkit|
///  '------------------------'

namespace Toolkit.Unit {
	public enum StatusEffectType : int {
		None = 0,
		Custom = 0,
		Burning = 2,
		Frost = 3,
		Storm = 4,
		Bloodcurse = 5,
		Focused = 6,
		Engaged = 7,
		Crowded = 8,
		Control_Ground = 9,
		Taunt = 10,
		Root = 11,
		Stun = 12,
		Isolated = 17,
		Empowered = 18,
		Weakened = 19,
		Destiny = 20,
		Exhausted = 21,
		Charged = 22,
		Spectral = 23,
		Regenerating = 24,
		Cursed = 25,
		Juggernaut = 26,
		Inquisitor = 27,
		Emberborn = 28,
		Commander = 29,
		Undead = 30,
		Armored = 31,
		Arctic_Adept = 32,
		Flying = 33,
		Packhunter = 34,
		Superior = 35,
		Massive = 36,
		Frightened = 37,
		Frostborn = 38,
		Too_Many_Dead = 39,
		Charmed = 40,
		Pacifist = 41,
	}

	public static class StatusEffectUtility {
		public static string GetDefaultName(Toolkit.Unit.StatusEffectType type) {
			switch(type) {
				case StatusEffectType.Burning: return "Burning";
				case StatusEffectType.Frost: return "Frost";
				case StatusEffectType.Storm: return "Storm";
				case StatusEffectType.Bloodcurse: return "Bloodcurse";
				case StatusEffectType.Focused: return "Focused";
				case StatusEffectType.Engaged: return "Engaged";
				case StatusEffectType.Crowded: return "Crowded";
				case StatusEffectType.Control_Ground: return "Control Ground";
				case StatusEffectType.Taunt: return "Taunt";
				case StatusEffectType.Root: return "Root";
				case StatusEffectType.Stun: return "Stun";
				case StatusEffectType.Isolated: return "Isolated";
				case StatusEffectType.Empowered: return "Empowered";
				case StatusEffectType.Weakened: return "Weakened";
				case StatusEffectType.Destiny: return "Destiny";
				case StatusEffectType.Exhausted: return "Exhausted";
				case StatusEffectType.Charged: return "Charged";
				case StatusEffectType.Spectral: return "Spectral";
				case StatusEffectType.Regenerating: return "Regenerating";
				case StatusEffectType.Cursed: return "Cursed";
				case StatusEffectType.Juggernaut: return "Juggernaut";
				case StatusEffectType.Inquisitor: return "Inquisitor";
				case StatusEffectType.Emberborn: return "Emberborn";
				case StatusEffectType.Commander: return "Commander";
				case StatusEffectType.Undead: return "Undead";
				case StatusEffectType.Armored: return "Armored";
				case StatusEffectType.Arctic_Adept: return "Arctic Adept";
				case StatusEffectType.Flying: return "Flying";
				case StatusEffectType.Packhunter: return "Packhunter";
				case StatusEffectType.Superior: return "Superior";
				case StatusEffectType.Massive: return "Massive";
				case StatusEffectType.Frightened: return "Frightened";
				case StatusEffectType.Frostborn: return "Frostborn";
				case StatusEffectType.Too_Many_Dead: return "Too Many Dead";
				case StatusEffectType.Charmed: return "Charmed";
				case StatusEffectType.Pacifist: return "Pacifist";
			}
			return "";

		}

		public static string GetDefaultDescription(Toolkit.Unit.StatusEffectType type) {
			switch(type) {
				case StatusEffectType.Burning: return "<color=#f28713>Burning</color>: At the start of each turn, take <b>11%</b> of <color=#c43939>weapon power</color> as <color=#d93fd1>ancestral damage</color>. Prevents <color=#f3e1e0>fortitude</color> regen. <color=#c7c7c7>(4 turns)</color>";
				case StatusEffectType.Frost: return "<color=#b2f5f7>Frost</color>: This character deals <b>-5%</b> <color=#c43939>damage</color>, to a maximum of <b>-75%</b>. <color=#c7c7c7>(4 turns)</color>";
				case StatusEffectType.Storm: return "<color=#25c1f5>Storm</color>: If an ability <color=#25c1f5>channels storm</color> against a target, copy the ability to each <color=#eb0202>nearby</color> or <color=#09d909>adjacent-tile</color> characters with <color=#25c1f5>storm</color>. ";
				case StatusEffectType.Bloodcurse: return "<color=#eb4984>Bloodcursed</color>: At the start of each turn, take <b>50%</b> of <color=#c43939>weapon power</color> as <color=#d93fd1>ancestral damage</color>. Reduces <color=#b0bfd4>armor</color> by <b>40%</b>. <color=#c7c7c7>(4 turns)</color>";
				case StatusEffectType.Focused: return "<color=#abf5f5>Focused</color>: Gains <b>+20%</b> <color=#cf4244>true mastery</color>. Taking damage or <color=#99cfa7>moving</color> removes <color=#abf5f5>focus</color>. Gained by standing still or using <color=#abf5f5>focus action</color>.";
				case StatusEffectType.Engaged: return "<color=#C65508>Engaged in Melee</color>: Gets reaction attacked if they attempt to move while <color=#C65508>engaged</color>.";
				case StatusEffectType.Crowded: return "<color=#dbbd72>Crowded</color>. This tile has <b>5</b> or more units and all units within deals half <color=#c43939>damage</color> and grants half <color=#f3e1e0>fortitude</color>.";
				case StatusEffectType.Control_Ground: return "<color=#1c73e6>Reaction attacks</color> enemies for <b>100%</b> of <color=#c43939>weapon power</color> if they <color=#99cfa7>move</color> in or out of <color=#1c73e6>controlled</color> <color=#99cfa7>tile</color>. (<color=#c4c1b9>1 turn</color>)";
				case StatusEffectType.Taunt: return "Taunted, deals 50% less damage to other enemies.";
				case StatusEffectType.Root: return "<color=#12e08e>Rooted</color>: This character cannot take a <color=#99cfa7>move</color> action or <color=#99cfa7>dash</color>.";
				case StatusEffectType.Stun: return "<color=#e5ed05>Stunned</color>: This character will skip their next turn.";
				case StatusEffectType.Isolated: return "<color=#bbc4b7>Isolated.</color> This character has no <color=#eb0202>nearby</color> units except for a single enemy.";
				case StatusEffectType.Empowered: return "This character is empowered and has gained one or more stats.";
				case StatusEffectType.Weakened: return "This character is weak and has one or more decreased stats.";
				case StatusEffectType.Destiny: return "<color=#BDF635>DESTINY.</color>";
				case StatusEffectType.Exhausted: return "<color=#baad95>Exhausted</color>: Has reduced actions per turn and heals half when visiting inns.  Having no <color=#BDF635>stamina</color> causes <color=#baad95>exhaustion</color>.";
				case StatusEffectType.Charged: return "<color=#3f84eb>Charged</color>: Next ability will <color=#25c1f5>channel storm</color> and consume <color=#3f84eb>charged</color>. <color=#6e6652>(Only affects multi-targeting abilities once)</color>";
				case StatusEffectType.Spectral: return "<color=#c4c1b9>Permanent.</color> Has <b>+50%</b> <color=#65b063>dodge</color> for the first <b>2 turns</b>. Immune to <color=#25c1f5>storm</color>.";
				case StatusEffectType.Regenerating: return "<color=#c4c1b9>Permanent.</color> Always begins each turn with full <color=#f3e1e0>fortitude</color> (<color=#c4c1b9>7 turns</color>). At the end of each turn, lose all <color=#f3e1e0>fortitude</color> if afflicted by <color=#f28713>burning</color>.";
				case StatusEffectType.Cursed: return "<color=#c4c1b9>Permanent.</color> Begins combat with unending <color=#eb4984>bloodcurse</color>. Has <b>+100%</b> <color=#cf4244>true mastery</color> while <color=#eb4984>bloodcursed</color>.";
				case StatusEffectType.Juggernaut: return "<color=#c4c1b9>Permanent.</color> Begins combat with unending <color=#1c73e6>controls ground</color>.";
				case StatusEffectType.Inquisitor: return "<color=#c4c1b9>Permanent.</color> Whenever any allied unit <color=#c43939>dies</color>, gain <b>+20%</b> <color=#cc93ed>critical strike chance</color> and <color=#f3e1e0>fortitude</color>.";
				case StatusEffectType.Emberborn: return "<color=#c4c1b9>Permanent.</color> Consumes <color=#f28713>burning</color> from both themselves and their target on hit, dealing <color=#eb0202>damage</color> for each <color=#f28713>burning</color> consumed.";
				case StatusEffectType.Commander: return "<color=#c4c1b9>Permanent.</color> At the start of turn, all other <color=#BDF635>allies</color> gain <color=#f3e1e0>fortitude</color> and loses <color=#12e08e>root</color>/<color=#e5ed05>stun</color>.";
				case StatusEffectType.Undead: return "<color=#c4c1b9>Permanent.</color> At the start of each turn, removes all <color=#b2f5f7>frost</color> from themselves. Takes extra <color=#eb0202>damage</color> from <color=#eb4984>bloodcurse</color>.";
				case StatusEffectType.Armored: return "<color=#c4c1b9>Permanent.</color> Begins combat with increased <color=#b0bfd4>armor</color>. Has reduced <color=#b0bfd4>armor</color> while <color=#e5ed05>stunned</color> or <color=#12e08e>rooted</color>.";
				case StatusEffectType.Arctic_Adept: return "<color=#c4c1b9>Permanent.</color> Unless <color=#b2f5f7>frosted</color>, this character has <b>25%</b> chance to repeat abilities. Has severly reduced <color=#b0bfd4>armor</color> while <color=#b2f5f7>frosted</color>.";
				case StatusEffectType.Flying: return "<color=#c4c1b9>Permanent.</color> Has <b>+20%</b> <color=#65b063>dodge</color>. <color=#528322>Moves</color> around really quickly.";
				case StatusEffectType.Packhunter: return "<color=#c4c1b9>Permanent.</color> Ignores <color=#dbbd72>crowded</color>. Starts with <color=#f3e1e0>fortitude</color> and gains <color=#f3e1e0>fortitude</color> when an ally <color=#c43939>dies</color>.";
				case StatusEffectType.Superior: return "<color=#c4c1b9>Permanent.</color> At the start of each turn, remove <color=#eb4984>bloodcurse</color> from allies and remove <color=#1c73e6>control ground</color> from enemies within their tile.";
				case StatusEffectType.Massive: return "<color=#c4c1b9>Permanent.</color> Always <color=#cc93ed>critical strikes</color> against smaller enemies. May push other characters out of tiles. <color=#e5ed05>Stun immunity</color>.";
				case StatusEffectType.Frightened: return "Instantly <color=#528322>move</color> away from the enemy that caused the <color=#833ea3>fear</color>, and deals <b>-50%</b> <color=#eb0202>damage</color> to them for <color=#c7c7c7>(2 turns)</color>.";
				case StatusEffectType.Frostborn: return "<color=#c4c1b9>Permanent.</color> Consumes <color=#b2f5f7>frost</color> from both themselves and their target on hit, dealing <color=#eb0202>damage</color> for each <color=#b2f5f7>frost</color> consumed.";
				case StatusEffectType.Too_Many_Dead: return "<color=#c4c1b9>Trinket Perk.</color> If you <color=#c43939>kill</color> an enemy directly, you <color=#c43939>die</color>.";
				case StatusEffectType.Charmed: return "Charmed";
				case StatusEffectType.Pacifist: return "This character deals 15% more damage per pacifist stack. If it gains more than 3 stacks it falls asleep.";
			}
			return "";

		}
	}
}
