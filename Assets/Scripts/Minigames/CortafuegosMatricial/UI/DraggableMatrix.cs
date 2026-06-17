using UnityEngine;

namespace Metroidvania.Minigames.MatrixFirewall
{
    /// <summary>
    /// Permite que una ficha sea arrastrada usando el ratón y físicas 2D (SpriteRenderer).
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class DraggableMatrix : MonoBehaviour
    {
        public MatrixTokenData tokenData;
        
        [Header("Referencias Visuales")]
        public SpriteRenderer spriteRenderer;
        
        private Vector3 originalPosition;
        private Transform originalParent;
        private int originalSortingOrder;
        private Camera minigameCam;
        private MatrixTokenVisual tokenVisual;
        
        private bool isDragging = false;
        private bool isHovering = false;

        void Awake()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            tokenVisual = GetComponent<MatrixTokenVisual>();
        }

        public void Initialize(MatrixTokenData data, Camera cam)
        {
            tokenData = data;
            minigameCam = cam;
            
            if (spriteRenderer != null)
            {
                spriteRenderer.color = data.tokenColor;
            }
        }

        void Update()
        {
            if (minigameCam == null) return;

            // 1. Convertir la posición del ratón de la pantalla al mundo de nuestra cámara secundaria
            Vector3 mousePosWorld = minigameCam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePosWorld.x, mousePosWorld.y);

            // 2. Disparar un rayo para ver si el ratón está tocando ESTA ficha
            Collider2D[] hits = Physics2D.OverlapPointAll(mousePos2D);
            bool mouseOverMe = false;
            foreach (var hit in hits)
            {
                if (hit.transform == this.transform)
                {
                    mouseOverMe = true;
                    break;
                }
            }

            // 3. Lógica de Hover (Pasar el ratón por encima)
            if (mouseOverMe && !isDragging)
            {
                if (!isHovering)
                {
                    isHovering = true;
                    if (tokenVisual != null) tokenVisual.SetTooltipActive(true);
                }
            }
            else if (!mouseOverMe && isHovering)
            {
                isHovering = false;
                if (tokenVisual != null) tokenVisual.SetTooltipActive(false);
            }

            // 4. Lógica de Clic (Iniciar arrastre)
            if (mouseOverMe && Input.GetMouseButtonDown(0))
            {
                StartDragging();
            }

            // 5. Lógica de Arrastre Activo
            if (isDragging)
            {
                // La ficha sigue al ratón, manteniendo su Z original
                transform.position = new Vector3(mousePosWorld.x, mousePosWorld.y, transform.position.z);

                // Lógica de Soltar (Terminar arrastre)
                if (Input.GetMouseButtonUp(0))
                {
                    StopDragging();
                }
            }
        }

        private void StartDragging()
        {
            isDragging = true;
            if (tokenVisual != null) tokenVisual.SetTooltipActive(false); // Ocultar al agarrar

            originalPosition = transform.position;
            originalParent = transform.parent;

            if (spriteRenderer != null)
            {
                originalSortingOrder = spriteRenderer.sortingOrder;
                spriteRenderer.sortingOrder = 100;
            }
        }

        private void StopDragging()
        {
            isDragging = false;

            if (spriteRenderer != null) spriteRenderer.sortingOrder = originalSortingOrder;

            // Disparar un pequeño rayo 2D para ver si soltamos la ficha sobre una zona de celda
            Collider2D[] dropHits = Physics2D.OverlapPointAll(transform.position);
            foreach (var hit in dropHits)
            {
                MatrixDropZone dropZone = hit.GetComponent<MatrixDropZone>();
                if (dropZone != null)
                {
                    dropZone.ReceiveToken(this);
                    return;
                }
            }
            
            ReturnToOrigin();
        }

        /// <summary>
        /// Devuelve la ficha suavemente a su contenedor de inventario original.
        /// </summary>
        public void ReturnToOrigin()
        {
            transform.SetParent(originalParent);
            transform.position = originalPosition;
        }
    }
}