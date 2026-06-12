using UnityEngine;
using System;

namespace Metroidvania.Minigames
{
    /// <summary>
    /// Estructura que almacena los resultados finales de cualquier minijuego.
    /// Útil para reportar datos al ScoreManager de forma unificada.
    /// </summary>
    [Serializable]
    public struct MinigameResult
    {
        public string minigameID;
        public bool isCompletedSuccessfully;
        public int finalScore;
        public float timeElapsed;
        public int movesCount;
    }

    /// <summary>
    /// Clase base abstracta de la que deben heredar todos los minijuegos.
    /// Proporciona el ciclo de vida unificado, control del encuadre de pantalla y envío de puntuaciones.
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public abstract class MinigameBase : MonoBehaviour
    {
        [Header("Configuración Base del Minijuego")]
        [SerializeField] protected string minigameID = "GenericMinigame";
        [SerializeField] protected string minigameName = "Minijuego de Ingeniería";
        
        [Header("Encuadre y Cámara (Límites)")]
        [Tooltip("El contenedor UI principal que limita el área de juego. Todo el minijuego se adaptará a este cuadro.")]
        [SerializeField] protected RectTransform gameplayBoundingBox;
        
        [Tooltip("¿El minijuego debe forzar una cámara independiente o renderizarse en el Canvas de UI principal?")]
        [SerializeField] protected bool useDedicatedCamera = false;
        [SerializeField] protected Camera minigameCamera;

        // Evento que notificará al sistema principal (Metroidvania / ScoreManager) cuando el juego termine.
        public event Action<MinigameResult> OnMinigameFinished;

        protected bool isGameActive { get; private set; }
        protected float startTime;
        protected int currentMoves;

        protected virtual void Awake()
        {
            // Asegurar que el Canvas esté configurado correctamente para superposición (Overlay)
            Canvas canvas = GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                // Colocar un orden de ordenamiento alto para que esté siempre encima de la exploración 2D
                canvas.sortingOrder = 10; 
            }
        }

        /// <summary>
        /// Método de inicialización pública. Es llamado por la terminal del Metroidvania para iniciar el reto.
        /// </summary>
        public virtual void InitializeAndStart()
        {
            if (isGameActive) return;

            isGameActive = true;
            startTime = Time.time;
            currentMoves = 0;

            ConfigureEnquadreVisual();
            OnMinigameStart();
            
            Debug.Log($"[MinigameBase] Iniciado: {minigameName}");
        }

        /// <summary>
        /// Ajusta el encuadre visual para asegurar que todo el contenido esté dentro del 'cuadro' límite.
        /// </summary>
        protected virtual void ConfigureEnquadreVisual()
        {
            if (gameplayBoundingBox == null)
            {
                Debug.LogWarning($"[MinigameBase] gameplayBoundingBox no asignado en {gameObject.name}. El juego podría verse recortado en algunas resoluciones.");
                return;
            }

            // Lógica adaptativa: Aseguramos que el cuadro contenedor mantenga su proporción y se adapte al tamaño de la pantalla
            // usando anclajes elásticos de Unity UI.
            if (useDedicatedCamera && minigameCamera != null)
            {
                // Si el minijuego requiere físicas 2D o elementos 3D dentro del cuadro,
                // configuramos el viewport de la cámara para que coincida exactamente con las coordenadas de pantalla del cuadro.
                AdjustCameraToBoundingBox();
            }
        }

        /// <summary>
        /// Ajusta la cámara secundaria para que solo renderice el contenido dentro del cuadro de juego.
        /// </summary>
        private void AdjustCameraToBoundingBox()
        {
            if (gameplayBoundingBox == null || minigameCamera == null) return;

            // Obtener esquinas en espacio de pantalla
            Vector3[] corners = new Vector3[4];
            gameplayBoundingBox.GetWorldCorners(corners);

            // Calcular rect en coordenadas normalizadas de viewport (0 a 1)
            float xMin = corners[0].x / Screen.width;
            float yMin = corners[0].y / Screen.height;
            float xMax = corners[2].x / Screen.width;
            float yMax = corners[2].y / Screen.height;

            minigameCamera.rect = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
        }

        /// <summary>
        /// Reporta el fin del juego, calcula el resultado final y dispara el evento de retorno.
        /// </summary>
        protected virtual void FinishMinigame(bool success, int scoreEarned)
        {
            if (!isGameActive) return;

            isGameActive = false;
            float timeElapsed = Time.time - startTime;

            MinigameResult result = new MinigameResult
            {
                minigameID = this.minigameID,
                isCompletedSuccessfully = success,
                finalScore = scoreEarned,
                timeElapsed = timeElapsed,
                movesCount = currentMoves
            };

            Debug.Log($"[MinigameBase] Finalizado. Éxito: {success}. Puntuación: {scoreEarned}. Tiempo: {timeElapsed:F1}s.");

            // Disparar evento para que el Metroidvania / ScoreManager retome el control
            OnMinigameFinished?.Invoke(result);

            // Auto-destrucción o desactivación segura de la ventana overlay
            CloseAndDestroy();
        }

        /// <summary>
        /// Destruye o desactiva la interfaz superpuesta de forma limpia.
        /// </summary>
        protected virtual void CloseAndDestroy()
        {
            // Reactivar las cámaras del minijuego en caso de haberlas modificado
            if (minigameCamera != null)
            {
                minigameCamera.gameObject.SetActive(false);
            }
            
            Destroy(gameObject);
        }

        // Métodos abstractos/virtuales que implementará cada minijuego específico
        protected abstract void OnMinigameStart();
        public abstract void ForceExitMinigame();
    }
}