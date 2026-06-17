using UnityEngine;
using System.Collections.Generic;

namespace Metroidvania.Minigames.MatrixFirewall
{
    /// <summary>
    /// Estructura que representa una matriz matemática. 
    /// Se aplana en un array de 1D (values) para que Unity pueda serializarlo y mostrarlo en el Inspector.
    /// </summary>
    [System.Serializable]
    public struct MatrixData
    {
        [Tooltip("Número de filas de la matriz.")]
        public int rows;
        
        [Tooltip("Número de columnas de la matriz.")]
        public int columns;
        
        [Tooltip("Valores de la matriz leídos de izquierda a derecha, de arriba hacia abajo.")]
        public int[] values;

        /// <summary>
        /// Método auxiliar para obtener el valor matemático de una celda específica simulando un array 2D.
        /// </summary>
        public int GetValue(int row, int col)
        {
            if (row < 0 || row >= rows || col < 0 || col >= columns) 
                return 0; // Retorna 0 por seguridad si está fuera de los límites
            
            return values[row * columns + col];
        }
    }

    /// <summary>
    /// Estructura que define una "ficha" o fragmento de código que el jugador arrastrará al tablero.
    /// </summary>
    [System.Serializable]
    public struct MatrixTokenData
    {
        [Tooltip("Nombre identificador de la ficha (Ej: 'Parche de Suma +2').")]
        public string tokenName;
        
        [Tooltip("Color visual de la ficha para diferenciarla en el inventario.")]
        public Color tokenColor;
        
        [Tooltip("La matriz de operación que esta ficha sumará al tablero principal al soltarse.")]
        public MatrixData operationMatrix;
    }

    /// <summary>
    /// ScriptableObject que almacena toda la configuración de un nivel específico del Cortafuegos.
    /// </summary>
    [CreateAssetMenu(fileName = "New_MatrixLevel", menuName = "Minigames/Cortafuegos Matricial/Level Data", order = 1)]
    public class LevelMatrixData : ScriptableObject
    {
        [Header("Información del Nivel")]
        public string levelID = "Matrix_Lvl_01";
        [TextArea]
        public string objectiveDescription = "Suma las matrices del inventario para igualar la temperatura térmica del objetivo.";

        [Header("Configuración del Tablero")]
        [Tooltip("El estado inicial de la matriz del jugador (usualmente llena de ceros).")]
        public MatrixData initialMatrix;
        
        [Tooltip("La matriz clave que el jugador debe alcanzar para hackear el cortafuegos.")]
        public MatrixData targetMatrix;

        [Header("Inventario del Jugador")]
        [Tooltip("Lista de fichas operacionales que el jugador tendrá disponibles para usar en este nivel.")]
        public List<MatrixTokenData> availableTokens;

        [Header("Reglas de Temperatura Térmica")]
        [Tooltip("Diferencia absoluta (distancia matemática) máxima para que una celda se considere 'Tibia' (Naranja).")]
        public int warmThreshold = 3;
        
        [Tooltip("Si la diferencia es mayor que warmThreshold, se considera 'Fría' (Azul Escarcha). Si es exactamente 0, se considera 'Caliente/Hackeada' (Verde).")]
        public int coldThreshold = 6;
    }
}