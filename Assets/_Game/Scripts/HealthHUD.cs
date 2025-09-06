using System;
using Toolkit;
using Toolkit.Health;
using Toolkit.UI;
using Toolkit.Unit;
using UnityEngine;

public class HealthHUD : MonoBehaviour, IAssignable<IUnit> {

    public RectTransform[] icons;

    private IUnit unit;

    public void Assign(IUnit item) {
        if(this.unit != null) {
            this.unit.Health.OnHealthChanged -= OnHealthChanged;
        }
        this.unit = item;
        if(this.unit != null) {
            this.unit.Health.OnHealthChanged += OnHealthChanged;
        }
    }

    private void OnHealthChanged(IHealth source, float oldHealth, float newHealth) {
        for(int i = 0, length = icons.Length; i < length; i++) {
            icons[i].SetActive(newHealth >= (i + 1));
        }
    }
}
