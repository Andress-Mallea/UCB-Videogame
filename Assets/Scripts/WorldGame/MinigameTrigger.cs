using UnityEngine;
using Metroidvania.Minigames;

public class MinigameTrigger : MonoBehaviour, IInteractable
{
    [Header("Configuración del Minijuego")]
    [Tooltip("Arrastra aquí el Prefab del Canvas del minijuego")]
    [SerializeField] private GameObject minigamePrefab;

    public void Interact()
    {
        if (minigamePrefab != null)
        {
            // Instanciamos el Canvas del minijuego sobre la escena actual
            GameObject minigameInstance = Instantiate(minigamePrefab);
            
            // Disparamos el evento a todo el juego para congelar al jugador
            EventManager.OnInteractionStarted?.Invoke();
            
            // Obtenemos la lógica principal e iniciamos el minijuego
            MinigameBase minigame = minigameInstance.GetComponent<MinigameBase>();
            if (minigame != null)
            {
                minigame.OnMinigameFinished += HandleMinigameFinished;
                minigame.InitializeAndStart();
            }
        }
    }

    private void HandleMinigameFinished(MinigameResult result)
    {
        // Aquí el minijuego ya terminó y se autodestruyó. 
        // Podemos avisar al EventManager que reactive el movimiento del jugador y las interacciones
        EventManager.OnInteractionEnded?.Invoke();
    }
}