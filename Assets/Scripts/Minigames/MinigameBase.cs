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
    public abstract class MinigameBase : MonoBehaviour
    {
        [Header("Configuración Base del Minijuego")]
        [SerializeField] protected string minigameID = "GenericMinigame";
        [SerializeField] protected string minigameName = "Minijuego de Ingeniería";
        
        [Header("Configuración de Viewport")]
        [Tooltip("Define el área de la pantalla que ocupará el minijuego (valores de 0 a 1). X, Y, Ancho, Alto.")]
        [SerializeField] private Rect viewportRect = new Rect(0.15f, 0.15f, 0.7f, 0.7f);
        [SerializeField] private Color viewportBackgroundColor = new Color(0.1f, 0.1f, 0.2f, 0.85f);
        [SerializeField] private int cameraDepth = 10;
        [Tooltip("Tamaño de la cámara ortográfica (afecta qué tan grandes se ven los sprites).")]
        [SerializeField] private float orthographicSize = 5f;
        [Tooltip("Capa exclusiva para renderizar el minijuego. (Recomendado: 'UI')")]
        [SerializeField] private LayerMask minigameLayer;

        // Evento que notificará al sistema principal (Metroidvania / ScoreManager) cuando el juego termine.
        public event Action<MinigameResult> OnMinigameFinished;

        public Camera MinigameCamera { get; private set; }
        public Bounds WorldBounds { get; private set; }

        protected bool isGameActive { get; private set; }
        protected float startTime;
        protected int currentMoves;

        private int originalMainCameraCullingMask;
        private bool hasModifiedMainCamera = false;

        protected virtual void Awake()
        {
            SetupCamera();
        }

        /// <summary>
        /// Permite inyectar datos de nivel desde un Trigger antes de iniciarlo.
        /// Sobrescribe este método en los minijuegos específicos.
        /// </summary>
        public virtual void SetLevelData(ScriptableObject data)
        {
            // Implementación base vacía. Los minijuegos hijos decidirán qué hacer con esto.
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

            OnMinigameStart();
            
            Debug.Log($"[MinigameBase] Iniciado: {minigameName}");
        }

        /// <summary>
        /// Crea y configura la cámara dedicada para este minijuego.
        /// </summary>
        private void SetupCamera()
        {
            // 1. Buscar si ya existe una cámara como hija. Si no, crearla.
            MinigameCamera = GetComponentInChildren<Camera>();
            if (MinigameCamera == null)
            {
                GameObject camObj = new GameObject("Minigame Camera");
                camObj.transform.SetParent(this.transform);
                MinigameCamera = camObj.AddComponent<Camera>();
            }
            
            // 2. Configurar la cámara para que se superponga y tenga un fondo sólido.
            MinigameCamera.orthographic = true;
            MinigameCamera.orthographicSize = orthographicSize;
            MinigameCamera.clearFlags = CameraClearFlags.SolidColor;
            MinigameCamera.backgroundColor = viewportBackgroundColor;
            MinigameCamera.rect = viewportRect;
            MinigameCamera.depth = cameraDepth;
            
            // Si no se asignó capa en el inspector (es 0), usamos la capa "UI" por defecto para proteger el juego.
            int layerMaskToUse = minigameLayer == 0 ? LayerMask.GetMask("UI") : minigameLayer;
            MinigameCamera.cullingMask = layerMaskToUse;

            // EVITAR QUE SE VEA EN EL JUEGO PRINCIPAL:
            if (Camera.main != null)
            {
                originalMainCameraCullingMask = Camera.main.cullingMask;
                Camera.main.cullingMask &= ~layerMaskToUse; // Removemos la capa del minijuego de la cámara principal
                hasModifiedMainCamera = true;
            }

            // Centrar la cámara exactamente en el origen de este objeto
            MinigameCamera.transform.position = transform.position + new Vector3(0, 0, -10);

            // 3. Calcular los límites en el espacio del mundo para que los hijos sepan dónde dibujarse.
            Vector3 bottomLeft = MinigameCamera.ViewportToWorldPoint(new Vector3(0, 0, MinigameCamera.nearClipPlane + 1));
            Vector3 topRight = MinigameCamera.ViewportToWorldPoint(new Vector3(1, 1, MinigameCamera.nearClipPlane + 1));

            WorldBounds = new Bounds();
            WorldBounds.SetMinMax(bottomLeft, topRight);
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
            // Al destruir el objeto padre, la cámara que creamos como hija también se destruirá.
            Destroy(gameObject);
        }

        protected virtual void OnDestroy()
        {
            // Restaurar la visión de la cámara principal cuando el minijuego se destruye (ganes, pierdas o salgas)
            if (hasModifiedMainCamera && Camera.main != null)
            {
                Camera.main.cullingMask = originalMainCameraCullingMask;
            }
        }

        // Métodos abstractos/virtuales que implementará cada minijuego específico
        protected abstract void OnMinigameStart();
        public abstract void ForceExitMinigame();

        /// <summary>
        /// Dibuja un marco en el Editor de Unity (Scene View) para que puedas visualizar 
        /// y diseñar el minijuego dentro de sus límites sin tener que darle a Play.
        /// </summary>
        private void OnDrawGizmos()
        {
            // Asumimos un aspect ratio estándar de 16:9 para la previsualización
            float aspect = 16f / 9f;
            if (Camera.main != null) aspect = Camera.main.pixelWidth / (float)Camera.main.pixelHeight;

            // El tamaño del mundo que la cámara renderiza depende SOLO de su orthographicSize y aspect ratio.
            // El viewportRect no cambia la cantidad de mundo visible, solo dónde se dibuja en pantalla.
            float boxHeight = orthographicSize * 2f;
            float boxWidth = boxHeight * aspect;

            // El centro de visión de la cámara SIEMPRE es su posición en el mundo
            Vector3 boxCenter = transform.position;
            Vector3 boxSize = new Vector3(boxWidth, boxHeight, 0.1f);

            // Dibujamos el área de fondo (semi-transparente)
            Gizmos.color = new Color(viewportBackgroundColor.r, viewportBackgroundColor.g, viewportBackgroundColor.b, 0.5f);
            Gizmos.DrawCube(boxCenter, boxSize);

            // Dibujamos los bordes (verde brillante)
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(boxCenter, boxSize);
        }
    }
}