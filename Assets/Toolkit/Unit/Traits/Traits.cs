///  .------------------------.
///  | This file is generated |
///  |        by code!        |
///  |   Changes made might   |
///  |     be overwritten!    |
///  |                        |
///  |      powered by Toolkit|
///  '------------------------'

using UnityEngine;

namespace Toolkit.Unit {
    [System.Serializable]
    public class Traits : ITrait {
        #region Variables

        private const float MIN = TraitsUtility.MIN;
        private const float MAX = TraitsUtility.MAX;

        [SerializeField, Range(MIN, MAX)] private float _lawfulChaotic = 0;
        [SerializeField, Range(MIN, MAX)] private float _honestDecietful = 0;
        [SerializeField, Range(MIN, MAX)] private float _socialLone_Wolf = 0;
        [SerializeField, Range(MIN, MAX)] private float _instigatorCalculating = 0;
        [SerializeField, Range(MIN, MAX)] private float _goodRuthless = 0;
        [SerializeField, Range(MIN, MAX)] private float _nobleSelf_Serving = 0;
        [SerializeField, Range(MIN, MAX)] private float _faithfulPagan = 0;
        [SerializeField, Range(MIN, MAX)] private float _trustingSuspicious = 0;
        [SerializeField, Range(MIN, MAX)] private float _spiritualMaterialistic = 0;
        [SerializeField, Range(MIN, MAX)] private float _traditionalPragmatic = 0;
        [SerializeField, Range(MIN, MAX)] private float _fieryCold = 0;
        [SerializeField, Range(MIN, MAX)] private float _politeBlunt = 0;
        [SerializeField, Range(MIN, MAX)] private float _casualIntense = 0;
        [SerializeField, Range(MIN, MAX)] private float _humanDemonic = 0;
        [SerializeField, Range(MIN, MAX)] private float _saneInsane = 0;
        [SerializeField, Range(MIN, MAX)] private float _luckyUnlucky = 0;

        public event OnTraitChangedCallback OnTraitChanged;

        #endregion

        #region Properties

        public float this[TraitType type] { get => GetTrait(type); set => SetTrait(type, value); }

        #endregion

        #region Get / Set

        public float GetTrait(TraitType type) {
            switch(type) {
                case TraitType.LawfulChaotic: return _lawfulChaotic;
                case TraitType.HonestDecietful: return _honestDecietful;
                case TraitType.SocialLone_Wolf: return _socialLone_Wolf;
                case TraitType.InstigatorCalculating: return _instigatorCalculating;
                case TraitType.GoodRuthless: return _goodRuthless;
                case TraitType.NobleSelf_Serving: return _nobleSelf_Serving;
                case TraitType.FaithfulPagan: return _faithfulPagan;
                case TraitType.TrustingSuspicious: return _trustingSuspicious;
                case TraitType.SpiritualMaterialistic: return _spiritualMaterialistic;
                case TraitType.TraditionalPragmatic: return _traditionalPragmatic;
                case TraitType.FieryCold: return _fieryCold;
                case TraitType.PoliteBlunt: return _politeBlunt;
                case TraitType.CasualIntense: return _casualIntense;
                case TraitType.HumanDemonic: return _humanDemonic;
                case TraitType.SaneInsane: return _saneInsane;
                case TraitType.LuckyUnlucky: return _luckyUnlucky;
            }
            throw new System.Exception("Not a valid TraitType");
        }

        public void SetTrait(TraitType type, float value) {
            value = Mathf.Clamp(value, MIN, MAX);
            switch(type) {
                case TraitType.LawfulChaotic: _lawfulChaotic = value; break;
                case TraitType.HonestDecietful: _honestDecietful = value; break;
                case TraitType.SocialLone_Wolf: _socialLone_Wolf = value; break;
                case TraitType.InstigatorCalculating: _instigatorCalculating = value; break;
                case TraitType.GoodRuthless: _goodRuthless = value; break;
                case TraitType.NobleSelf_Serving: _nobleSelf_Serving = value; break;
                case TraitType.FaithfulPagan: _faithfulPagan = value; break;
                case TraitType.TrustingSuspicious: _trustingSuspicious = value; break;
                case TraitType.SpiritualMaterialistic: _spiritualMaterialistic = value; break;
                case TraitType.TraditionalPragmatic: _traditionalPragmatic = value; break;
                case TraitType.FieryCold: _fieryCold = value; break;
                case TraitType.PoliteBlunt: _politeBlunt = value; break;
                case TraitType.CasualIntense: _casualIntense = value; break;
                case TraitType.HumanDemonic: _humanDemonic = value; break;
                case TraitType.SaneInsane: _saneInsane = value; break;
                case TraitType.LuckyUnlucky: _luckyUnlucky = value; break;
            }
            OnTraitChanged?.Invoke(type);
        }

        #endregion
    }
}
