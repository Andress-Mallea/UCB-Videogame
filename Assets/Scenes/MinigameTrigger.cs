using UnityEngine;

public class MinigameTrigger : MonoBehaviour, IInteractable
{
    [Header("Configuración del Minijuego")]
    [Tooltip("Arrastra aquí el Prefab del Canvas del minijuego")]
    [SerializeField] private GameObject minigamePrefab;

    public void Interact()
    {
        if (minigamePrefab != null)
        {
            Instantiate(minigamePrefab);
            
            // Disparamos el evento a todo el juego
            EventManager.OnInteractionStarted?.Invoke();
        }
    }
}