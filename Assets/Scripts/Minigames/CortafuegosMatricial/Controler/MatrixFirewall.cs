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
        public LevelData_Matrix currentLevelData;

        [Header("Referencias de Contenedores UI")]
        public Transform boardContainer;      // Donde se instanciarán las celdas de la matriz
        public Transform inventoryContainer;  // Donde se instanciarán las fichas del jugador
        
        [Header("Prefabs")]
        public GameObject cellPrefab;
        public GameObject tokenPrefab;

        // Lista para mantener referencia a las celdas visuales instanciadas
        private List<MatrixCell> activeCells = new List<MatrixCell>();

        // Memoria del estado actual de la matriz del jugador
        private int[] currentMatrixState;

        protected override void OnMinigameStart()
        {
            if (currentLevelData == null)
            {
                Debug.LogError("[MatrixMinigame] No hay datos de nivel asignados.");
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
            for (int i = 0; i < currentMatrixState.Length; i++)
            {
                GameObject cellObj = Instantiate(cellPrefab, boardContainer);
                MatrixCell cell = cellObj.GetComponent<MatrixCell>();
                
                int targetVal = currentLevelData.targetMatrix.values[i];
                cell.Initialize(currentMatrixState[i], targetVal);
                cell.UpdateTemperature(currentLevelData.warmThreshold, currentLevelData.coldThreshold);
                
                activeCells.Add(cell);
            }

            Debug.Log($"[MatrixMinigame] Tablero listo: {currentLevelData.initialMatrix.rows}x{currentLevelData.initialMatrix.columns}");
        }

        private void InitializeInventory()
        {
            // Nota: Aquí instanciaremos visualmente el tokenPrefab en el inventoryContainer (Fase 2)
            Debug.Log($"[MatrixMinigame] Fichas cargadas: {currentLevelData.availableTokens.Count}");
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
                // Cálculo de score simple: Base 1000 menos penalización por movimientos extra
                int calculatedScore = Mathf.Max(100, 1000 - (currentMoves * 50)); 
                FinishMinigame(true, calculatedScore);
            }
        }

        public override void ForceExitMinigame()
        {
            FinishMinigame(false, 0);
        }
    }
}