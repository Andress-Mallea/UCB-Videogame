using UnityEngine;
using System.Collections.Generic;

namespace Metroidvania.Minigames.MatrixFirewall
{
    /// <summary>
    /// Controlador principal del minijuego Cortafuegos Matricial.
    /// Coordina los datos, la UI y la lógica de victoria.
    /// </summary>
    public class MatrixMinigameController : MinigameBase
    {
        [Header("Datos Inyectados")]
        [Tooltip("El ScriptableObject con los datos del nivel actual.")]
        public LevelMatrixData currentLevelData;

        [Header("Contenedores de Sprites")]
        [Tooltip("Objeto vacío hijo para organizar las celdas. Su posición será (0,0,0) relativa al prefab.")]
        public Transform boardContainer;

        [Tooltip("Objeto vacío hijo para organizar las fichas del inventario.")]
        public Transform inventoryContainer;

        [Header("Prefabs")]
        public GameObject cellPrefab;
        public GameObject tokenPrefab;

        // Lista para mantener referencia a las celdas visuales instanciadas
        private List<MatrixCell> activeCells = new List<MatrixCell>();

        // Memoria del estado actual de la matriz del jugador
        private int[] currentMatrixState;

        public override void SetLevelData(ScriptableObject data)
        {
            if (data is LevelMatrixData matrixData)
            {
                currentLevelData = matrixData;
                Debug.Log($"[MatrixMinigame] Datos del nivel recibidos del Trigger: {currentLevelData.levelID}");
            }
        }

        protected override void OnMinigameStart()
        {
            if (currentLevelData == null)
            {
                Debug.LogError("[MatrixMinigame] No hay datos de nivel asignados. Asegúrate de arrastrar el ScriptableObject del nivel al MinigameTrigger.");
                ForceExitMinigame();
                return;
            }

            if (boardContainer == null || inventoryContainer == null)
            {
                Debug.LogError($"[MatrixMinigame] Faltan referencias a 'Board Container' o 'Inventory Container' en el Prefab del minijuego. Arrástralos en el Inspector.");
                ForceExitMinigame();
                return;
            }

            if (currentLevelData.initialMatrix.values == null || currentLevelData.targetMatrix.values == null || currentLevelData.initialMatrix.values.Length == 0)
            {
                Debug.LogError($"[MatrixMinigame] Los datos del nivel '{currentLevelData.name}' están incompletos. Asegúrate de que 'Initial Matrix' y 'Target Matrix' tengan sus valores (Values) definidos en el Inspector.");
                ForceExitMinigame();
                return;
            }

            if (cellPrefab == null || tokenPrefab == null)
            {
                Debug.LogError($"[MatrixMinigame] Faltan los prefabs de celdas o tokens. Arrástralos en el Inspector del Prefab del minijuego.");
                ForceExitMinigame();
                return;
            }

            minigameID = currentLevelData.levelID;
            InitializeBoard();
            InitializeInventory();
        }

        private void InitializeBoard()
        {
            // 1. Clonar los valores iniciales para no sobrescribir el ScriptableObject
            currentMatrixState = (int[])currentLevelData.initialMatrix.values.Clone();

            // 2. Limpiar el tablero anterior (por seguridad)
            foreach (Transform child in boardContainer) 
            {
                Destroy(child.gameObject);
            }
            activeCells.Clear();

            // 3. Generar las celdas visuales dinámicamente
            int rows = currentLevelData.initialMatrix.rows;
            int cols = currentLevelData.initialMatrix.columns;
            
            // Calculamos el espacio usando directamente el tamaño de la cámara (mucho más seguro en el frame 1)
            float camHeight = base.MinigameCamera.orthographicSize * 2f;
            float camWidth = camHeight * base.MinigameCamera.aspect;
            
            float totalGridWidth = camWidth * 0.4f; // Que el tablero ocupe el 40% del ancho
            float cellSize = totalGridWidth / cols;
            float totalGridHeight = cellSize * rows;

            // El punto de inicio para dibujar la grilla (esquina superior izquierda)
            Vector3 startPoint = new Vector3(
                -totalGridWidth / 2f + cellSize / 2f,
                totalGridHeight / 2f - cellSize / 2f,
                0);

            for (int i = 0; i < currentMatrixState.Length; i++)
            {
                GameObject cellObj = Instantiate(cellPrefab, boardContainer);
                MatrixCell cell = cellObj.GetComponent<MatrixCell>();
                
                // Si el prefab no tiene el script, avisamos sin crashear el juego entero
                if (cell == null)
                {
                    Debug.LogError($"[MatrixMinigame] ¡Cuidado! El prefab '{cellPrefab.name}' no tiene el componente 'MatrixCell' adjunto. Añádelo en el Inspector de Unity.");
                    continue; 
                }

                // Acomodar los sprites en forma de grilla 2D relativa al centro del viewport
                int row = i / cols;
                int col = i % cols;
                cellObj.transform.localPosition = startPoint + new Vector3(col * cellSize, -row * cellSize, 0);

                int targetVal = currentLevelData.targetMatrix.values[i];
                cell.Initialize(currentMatrixState[i], targetVal);
                cell.UpdateTemperature(currentLevelData.warmThreshold, currentLevelData.coldThreshold);
                
                activeCells.Add(cell);
            }

            Debug.Log($"[MatrixMinigame] Tablero listo: {currentLevelData.initialMatrix.rows}x{currentLevelData.initialMatrix.columns}");
        }

        private void InitializeInventory()
        {
            if (inventoryContainer == null || tokenPrefab == null) return;

            // 1. Limpiar el contenedor por seguridad
            foreach (Transform child in inventoryContainer) 
            {
                Destroy(child.gameObject);
            }

            // 2. Generar las fichas visuales usando los datos del ScriptableObject
            // Calculamos el espacio para que siempre quepan todas las fichas
            float camHeight = base.MinigameCamera.orthographicSize * 2f;
            float verticalSpacing = (camHeight * 0.8f) / Mathf.Max(1, currentLevelData.availableTokens.Count);
            
            // Empezar a dibujar desde arriba hacia abajo, centrado
            float startY = (camHeight * 0.4f) - (verticalSpacing / 2f);
            
            for (int i = 0; i < currentLevelData.availableTokens.Count; i++)
            {
                GameObject tokenObj = Instantiate(tokenPrefab, inventoryContainer);
                
                tokenObj.transform.localPosition = new Vector3(0, startY - (i * verticalSpacing), 0);
                
                // Inicializamos la ficha pasándole su TokenData para que tome color y nombre
                MatrixTokenVisual tokenVisual = tokenObj.GetComponent<MatrixTokenVisual>();
                if (tokenVisual != null)
                {
                    tokenVisual.Initialize(currentLevelData.availableTokens[i]);
                }

                // Si la ficha tiene el componente para arrastrarse, la inicializamos
                DraggableMatrix draggable = tokenObj.GetComponent<DraggableMatrix>();
                if (draggable != null)
                {
                    draggable.Initialize(currentLevelData.availableTokens[i], base.MinigameCamera);
                }
            }
            
            Debug.Log($"[MatrixMinigame] Fichas cargadas en inventario: {currentLevelData.availableTokens.Count}");
        }

        /// <summary>
        /// Aplica la suma algebraica cuando el jugador suelta una ficha válida.
        /// </summary>
        public void ApplyMatrixOperation(MatrixTokenData tokenData)
        {
            currentMoves++;
            
            // Suma de matrices lineal y actualización visual
            for (int i = 0; i < currentMatrixState.Length; i++)
            {
                currentMatrixState[i] += tokenData.operationMatrix.values[i];
                
                // Actualizar la UI de la celda respectiva
                activeCells[i].UpdateValue(currentMatrixState[i], currentLevelData.warmThreshold, currentLevelData.coldThreshold);
            }
            
            CheckWinCondition();
        }

        private void CheckWinCondition()
        {
            bool isHacked = true;

            for (int i = 0; i < currentMatrixState.Length; i++)
            {
                if (currentMatrixState[i] != currentLevelData.targetMatrix.values[i])
                {
                    isHacked = false;
                    break;
                }
            }

            if (isHacked)
            {
                // Calculamos el tiempo que le tomó al jugador
                float timeTaken = Time.time - startTime;

                // Fórmula de puntaje: Base 1000. 
                // Restamos 50 puntos por cada movimiento y 10 puntos por cada segundo transcurrido.
                // El puntaje mínimo garantizado será 100 si logra resolverlo.
                int penaltyMoves = currentMoves * 50;
                int penaltyTime = Mathf.RoundToInt(timeTaken * 10f);
                int calculatedScore = Mathf.Max(100, 1000 - penaltyMoves - penaltyTime); 
                FinishMinigame(true, calculatedScore);
            }
        }

        public override void ForceExitMinigame()
        {
            FinishMinigame(false, 0);
        }

        /// <summary>
        /// Restablece el tablero a su estado inicial. Útil para ser llamado desde un botón de UI.
        /// </summary>
        public void RestartMinigame()
        {
            currentMoves = 0;
            currentMatrixState = (int[])currentLevelData.initialMatrix.values.Clone();
            
            for (int i = 0; i < currentMatrixState.Length; i++)
            {
                activeCells[i].UpdateValue(currentMatrixState[i], currentLevelData.warmThreshold, currentLevelData.coldThreshold);
            }
            Debug.Log("[MatrixMinigame] Tablero reiniciado.");
        }
    }
}