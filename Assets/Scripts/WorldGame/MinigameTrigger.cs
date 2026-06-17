using UnityEngine;
using Metroidvania.Minigames;
using TMPro; // Para usar TextMeshPro

public class MinigameTrigger : MonoBehaviour, IInteractable
{
    [Header("Configuración del Minijuego")]
    [Tooltip("Arrastra aquí el Prefab del Canvas del minijuego")]
    [SerializeField] private GameObject minigamePrefab;

    [Header("Datos Específicos del Nivel")]
    [Tooltip("Arrastra aquí el ScriptableObject del nivel (Ej: Cortafuegos_Lvl1)")]
    [SerializeField] private ScriptableObject levelData;

    [Header("UI de Resultados (Estilo NPC)")]
    [Tooltip("El panel que se mostrará al ganar. Déjalo apagado por defecto en la escena.")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshPro resultText;

    private MinigameBase activeMinigameInstance;

    public void Interact()
    {
        if (minigamePrefab != null)
        {
            // 1. Instanciamos el GameObject del minijuego en el mundo
            GameObject minigameInstance = Instantiate(minigamePrefab);
            
            // 2. Buscamos su componente base de minijuego para poder controlarlo
            activeMinigameInstance = minigameInstance.GetComponent<MinigameBase>();
            
            if (activeMinigameInstance != null)
            {
                // 3. Le pasamos los datos del nivel ANTES de arrancarlo
                if (levelData != null)
                    activeMinigameInstance.SetLevelData(levelData);

                // Nos suscribimos al evento para escuchar cuándo termina el jugador de jugar
                activeMinigameInstance.OnMinigameFinished += HandleMinigameFinished;
                
                // Le damos la orden formal de iniciar su lógica interna
                activeMinigameInstance.InitializeAndStart();
            }
            
            // Disparamos el evento a todo el juego
            EventManager.OnInteractionStarted?.Invoke();
        }
    }

    // Esta función se llamará mágicamente cuando el minijuego decida terminar (FinishMinigame)
    private void HandleMinigameFinished(MinigameResult result)
    {
        if (activeMinigameInstance != null)
        {
            activeMinigameInstance.OnMinigameFinished -= HandleMinigameFinished;
            activeMinigameInstance = null;
        }
        
        // Si el jugador ganó el minijuego
        if (result.isCompletedSuccessfully)
        {
            // Sumamos el puntaje globalmente
            if (ScoreManager.instance != null)
                ScoreManager.instance.AddScore(result.finalScore);

            // Mostramos el panel de victoria estilo NPC
            if (resultPanel != null && resultText != null)
            {
                resultText.text = $"¡HACKEO EXITOSO!\n\nTiempo: {result.timeElapsed:F1}s\nMovimientos: {result.movesCount}\n\nPuntaje Obtenido: {result.finalScore}\n\n[ Presiona ESPACIO para continuar ]";
                resultPanel.SetActive(true);
            }
            else
            {
                // Si no hay UI asignada, simplemente liberamos al jugador y destruimos el trigger
                EventManager.OnInteractionEnded?.Invoke();
                Destroy(gameObject);
            }
        }
        else
        {
            // Si salió manualmente o perdió, lo liberamos de inmediato sin destruir el trigger (para que pueda reintentar)
            EventManager.OnInteractionEnded?.Invoke();
        }
    }

    private void Update()
    {
        // Controlamos el cierre del panel de resultados con ESPACIO, igual que en Logica_NPC
        if (resultPanel != null && resultPanel.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            resultPanel.SetActive(false);
            EventManager.OnInteractionEnded?.Invoke();
            Destroy(gameObject); // Destruimos la computadora interactuable para no repetir el nivel
        }
    }

    private void OnDestroy()
    {
        // Buena práctica: si este trigger se destruye, nos aseguramos de no dejar una suscripción "colgando".
        if (activeMinigameInstance != null)
            activeMinigameInstance.OnMinigameFinished -= HandleMinigameFinished;
    }
}