
namespace Toolkit.Unit {
    public static partial class TraitsUtility {
        #region Variables

        public const float MIN = -10;
        public const float MAX = 10;

        #endregion

        #region Names

        /// <summary>
        /// Returns positve name if 0 or higher, otherwise returns negative name.
        /// </summary>
        public static string GetName(TraitType type, int value)
            => GetName(type, value >= 0);

        public static string GetName(TraitType type, bool positive)
            => positive ? GetPositiveName(type) : GetNegativeName(type);

        public static string GetPositiveName(TraitType type) {
            switch(type) {
                case TraitType.LawfulChaotic: return "Lawful";
                case TraitType.HonestDecietful: return "Honest";
                case TraitType.SocialLone_Wolf: return "Social";
                case TraitType.InstigatorCalculating: return "Instigator";
                case TraitType.GoodRuthless: return "Good";
                case TraitType.NobleSelf_Serving: return "Noble";
                case TraitType.FaithfulPagan: return "Faithful";
                case TraitType.TrustingSuspicious: return "Trusting";
                case TraitType.SpiritualMaterialistic: return "Spiritual";
                case TraitType.TraditionalPragmatic: return "Traditional";
                case TraitType.FieryCold: return "Fiery";
                case TraitType.PoliteBlunt: return "Polite";
                case TraitType.CasualIntense: return "Casual";
                case TraitType.HumanDemonic: return "Human";
                case TraitType.SaneInsane: return "Sane";
                case TraitType.LuckyUnlucky: return "Lucky";
            }
            return "Positive";
        }

        public static string GetNegativeName(TraitType type) {
            switch(type) {
                case TraitType.LawfulChaotic: return "Chaotic";
                case TraitType.HonestDecietful: return "Deceitful";
                case TraitType.SocialLone_Wolf: return "Lone-wolf";
                case TraitType.InstigatorCalculating: return "Calculating";
                case TraitType.GoodRuthless: return "Ruthless";
                case TraitType.NobleSelf_Serving: return "Self-serving";
                case TraitType.FaithfulPagan: return "Pagan";
                case TraitType.TrustingSuspicious: return "Suspicious";
                case TraitType.SpiritualMaterialistic: return "Materialistic";
                case TraitType.TraditionalPragmatic: return "Pragmatic";
                case TraitType.FieryCold: return "Cold";
                case TraitType.PoliteBlunt: return "Blunt";
                case TraitType.CasualIntense: return "Intense";
                case TraitType.HumanDemonic: return "Demonic";
                case TraitType.SaneInsane: return "Insane";
                case TraitType.LuckyUnlucky: return "Unlucky";
            }
            return "Negative";
        }

        #endregion
    }
}
