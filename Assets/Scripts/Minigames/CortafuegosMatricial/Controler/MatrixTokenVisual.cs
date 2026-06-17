using UnityEngine;
using TMPro;

namespace Metroidvania.Minigames.MatrixFirewall
{
    public class MatrixTokenVisual : MonoBehaviour
    {
        [Header("Componentes Visuales")]
        public SpriteRenderer backgroundSprite;
        public TextMeshPro nameText;
        public TextMeshPro tooltipText; // Texto que mostrará la matriz en pantalla

        // Guardamos los datos por si en el futuro queremos arrastrar esta ficha
        public MatrixTokenData TokenData { get; private set; }

        public void Initialize(MatrixTokenData data)
        {
            TokenData = data;

            if (backgroundSprite != null)
            {
                backgroundSprite.color = data.tokenColor;
            }

            if (nameText != null)
            {
                nameText.text = data.tokenName;
            }

            if (tooltipText != null)
            {
                tooltipText.text = FormatMatrixString(data.operationMatrix);
                tooltipText.gameObject.SetActive(false); // Oculto por defecto
            }
        }

        public void SetTooltipActive(bool isActive)
        {
            if (tooltipText != null)
            {
                tooltipText.gameObject.SetActive(isActive);
            }
        }

        private string FormatMatrixString(MatrixData matrix)
        {
            string result = "";
            for (int i = 0; i < matrix.rows; i++)
            {
                for (int j = 0; j < matrix.columns; j++)
                {
                    // Formateamos para que los números queden alineados
                    result += string.Format("{0,2} ", matrix.GetValue(i, j));
                }
                result += "\n";
            }
            return result;
        }
    }
}