using UnityEngine;

namespace Metroidvania.Minigames.MatrixFirewall
{
    /// <summary>
    /// Componente 2D que recibe la ficha soltada encima de ella.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class MatrixDropZone : MonoBehaviour
    {
        [Tooltip("Referencia al controlador central para aplicar las matemáticas.")]
        public MatrixMinigameController controller;

        public void ReceiveToken(DraggableMatrix draggableToken)
        {
            if (draggableToken != null)
            {
                Debug.Log($"[MatrixDropZone] Operación recibida: {draggableToken.tokenData.tokenName}");
                
                if (controller != null)
                {
                    controller.ApplyMatrixOperation(draggableToken.tokenData);
                }
                
                draggableToken.ReturnToOrigin();
            }
        }
    }
}