using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Metroidvania.Minigames.MatrixFirewall
{
    /// <summary>
    /// Permite que una ficha del inventario sea arrastrada por la pantalla usando el sistema de eventos de Unity UI.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class DraggableMatrix : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public MatrixTokenData tokenData;
        
        [Header("Referencias Visuales")]
        public Image backgroundImage;
        
        private Vector3 originalPosition;
        private Transform originalParent;
        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Initialize(MatrixTokenData data)
        {
            tokenData = data;
            
            // Aplicar el color de la ficha si está asignado
            if (backgroundImage != null)
            {
                backgroundImage.color = data.tokenColor;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            originalPosition = rectTransform.position;
            originalParent = transform.parent;

            // Extraemos la ficha de su contenedor (GridLayout) para que se mueva libremente sobre todo el Canvas
            transform.SetParent(transform.root);
            transform.SetAsLastSibling();

            // IMPORTANTE: Desactivar los raycasts para que la ficha no bloquee el puntero y podamos detectar qué hay debajo (El DropZone)
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0.8f; // Hacerla ligeramente transparente mientras se arrastra
        }

        public void OnDrag(PointerEventData eventData)
        {
            // Actualizar la posición de la ficha para que siga al cursor o al dedo
            rectTransform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // Reactivar las colisiones de UI de la ficha
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;

            // Si al soltarla sigue en la raíz (no fue adoptada por un DropZone válido), vuelve a su lugar original
            if (transform.parent == transform.root)
            {
                ReturnToOrigin();
            }
        }

        /// <summary>
        /// Devuelve la ficha suavemente a su contenedor de inventario original.
        /// </summary>
        public void ReturnToOrigin()
        {
            transform.SetParent(originalParent);
            rectTransform.position = originalPosition;
        }
    }
}