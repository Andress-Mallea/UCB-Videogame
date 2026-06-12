using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [Header("Configuración del Minijuego")]
    [Tooltip("Arrastra aquí el Prefab del Canvas del minijuego")]
    [SerializeField] private GameObject minigamePrefab;

    public void Interact()
    {
        Debug.Log("¡Interacción exitosa! Cargando el Prefab del minijuego...");
        
        // Esta es la magia que te pide tu equipo: inserta el objeto directamente en el juego original
        if (minigamePrefab != null)
        {
            Instantiate(minigamePrefab);
        }
    }
}