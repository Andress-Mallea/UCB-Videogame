using UnityEngine;
using UnityEngine.EventSystems;

namespace Metroidvania.Minigames.MatrixFirewall
{
    /// <summary>
    /// Componente que se adjunta al área del tablero para detectar cuando el jugador suelta una matriz encima.
    /// </summary>
    public class MatrixDropZone : MonoBehaviour, IDropHandler
    {
        [Tooltip("Referencia al controlador central para aplicar las matemáticas.")]
        public MatrixMinigameController controller;

        public void OnDrop(PointerEventData eventData)
        {
            // Verificamos si lo que se soltó tiene el componente DraggableMatrix
            if (eventData.pointerDrag != null)
            {
                DraggableMatrix draggableToken = eventData.pointerDrag.GetComponent<DraggableMatrix>();
                
                if (draggableToken != null)
                {
                    Debug.Log($"[MatrixDropZone] Operación recibida: {draggableToken.tokenData.tokenName}");
                    
                    // 1. Enviamos los datos de la ficha al controlador para que sume los valores
                    if (controller != null)
                    {
                        controller.ApplyMatrixOperation(draggableToken.tokenData);
                    }

                    // 2. Devolvemos la ficha a su lugar de origen en el inventario 
                    // (Asumiendo que el jugador puede usar la misma operación matemática múltiples veces)
                    draggableToken.ReturnToOrigin();
                }
            }
        }
    }
}