using UnityEngine;
using UnityEngine.UI;
using TMPro; // Requiere TextMeshPro

namespace Metroidvania.Minigames.MatrixFirewall
{
    /// <summary>
    /// Controla la representación visual de una celda individual en el tablero del cortafuegos.
    /// </summary>
    public class MatrixCell : MonoBehaviour
    {
        [Header("Referencias UI")]
        public Image backgroundImage;
        public TextMeshProUGUI valueText;

        [Header("Colores Térmicos")]
        public Color coldColor = new Color(0.2f, 0.6f, 1f);   // Azul
        public Color warmColor = new Color(1f, 0.6f, 0.2f);   // Naranja
        public Color hackedColor = new Color(0.2f, 0.8f, 0.2f); // Verde brillante

        private int currentValue;
        private int targetValue;

        /// <summary>
        /// Configura los valores iniciales de la celda.
        /// </summary>
        public void Initialize(int initialVal, int targetVal)
        {
            currentValue = initialVal;
            targetValue = targetVal;
            valueText.text = currentValue.ToString();
        }

        /// <summary>
        /// Actualiza el valor y calcula el feedback térmico basado en la distancia al objetivo.
        /// </summary>
        public void UpdateValue(int newValue, int warmThreshold, int coldThreshold)
        {
            currentValue = newValue;
            valueText.text = currentValue.ToString();
            
            UpdateTemperature(warmThreshold, coldThreshold);
        }

        /// <summary>
        /// Cambia el color del fondo dependiendo de qué tan cerca esté del valor objetivo.
        /// </summary>
        public void UpdateTemperature(int warmThreshold, int coldThreshold)
        {
            int difference = Mathf.Abs(currentValue - targetValue);

            if (difference == 0)
            {
                backgroundImage.color = hackedColor; // ¡Hackeado!
            }
            else if (difference <= warmThreshold)
            {
                backgroundImage.color = warmColor;   // Tibio (Cerca)
            }
            else
            {
                backgroundImage.color = coldColor;   // Frío (Lejos)
            }
        }
    }
}