using UnityEngine;
using UnityEngine.Events;

namespace Metroidvania.Minigames
{
    /// <summary>
    /// Botón físico 2D que responde a clics usando la cámara del minijuego en lugar de Canvas UI.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class MinigameButton2D : MonoBehaviour
    {
        [Tooltip("El evento que se ejecutará al hacer clic en este objeto.")]
        public UnityEvent onClick;
        
        [Tooltip("Color que tomará el sprite al pasar el ratón por encima.")]
        public Color hoverColor = new Color(0.8f, 0.8f, 0.8f);
        
        private Color originalColor;
        private SpriteRenderer spriteRenderer;
        private MinigameBase minigameBase;
        private bool isHovering = false;

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null) originalColor = spriteRenderer.color;
            
            // Busca automáticamente al padre principal (el Controlador del minijuego) para obtener la cámara
            minigameBase = GetComponentInParent<MinigameBase>();
        }

        void Update()
        {
            if (minigameBase == null || minigameBase.MinigameCamera == null) return;

            // 1. Calcular la posición del ratón relativa a nuestra cámara de minijuego
            Vector3 mousePosWorld = minigameBase.MinigameCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePosWorld.x, mousePosWorld.y);

            // 2. Disparar rayo para ver si tocamos ESTE botón
            Collider2D hit = Physics2D.OverlapPoint(mousePos2D);
            bool mouseOverMe = (hit != null && hit.transform == this.transform);

            // 3. Lógica visual (Hover)
            if (mouseOverMe && !isHovering)
            {
                isHovering = true;
                if (spriteRenderer != null) spriteRenderer.color = hoverColor;
            }
            else if (!mouseOverMe && isHovering)
            {
                isHovering = false;
                if (spriteRenderer != null) spriteRenderer.color = originalColor;
            }

            // 4. Lógica de Clic
            if (mouseOverMe && Input.GetMouseButtonDown(0))
            {
                onClick?.Invoke();
            }
        }
    }
}