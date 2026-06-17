using UnityEngine;
using TMPro;

namespace Metroidvania.Minigames.MatrixFirewall
{
    public class MatrixCell : MonoBehaviour
    {
        [Header("Componentes Visuales")]
        public SpriteRenderer backgroundSprite;
        public TextMeshPro valueText;

        private int targetValue;
        private int currentValue;

        public void Initialize(int currentVal, int targetVal)
        {
            currentValue = currentVal;
            targetValue = targetVal;
            UpdateText();
        }

        public void UpdateValue(int newVal, int warmThreshold, int coldThreshold)
        {
            currentValue = newVal;
            UpdateText();
            UpdateTemperature(warmThreshold, coldThreshold);
        }

        public void UpdateTemperature(int warmThreshold, int coldThreshold)
        {
            if (valueText == null) return;

            int difference = Mathf.Abs(targetValue - currentValue);

            if (difference == 0)
            {
                valueText.color = Color.green; // Hackeado (Caliente)
            }
            else if (difference <= warmThreshold)
            {
                valueText.color = new Color(1f, 0.5f, 0f); // Naranja (Tibio)
            }
            else
            {
                valueText.color = Color.cyan; // Azul escarcha (Frío)
            }
        }

        private void UpdateText()
        {
            if (valueText != null)
                valueText.text = currentValue.ToString();
        }
    }
}