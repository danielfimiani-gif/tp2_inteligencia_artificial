using UnityEngine;
using UnityEngine.UI;

class HealthBar : MonoBehaviour {
    [SerializeField] private Image fill;

    public void SetValue(float current, float max) {
        if (max <= 0) {
            fill.fillAmount = 0;
            return;
        }

        fill.fillAmount = Mathf.Clamp01(current / max);
    }
}