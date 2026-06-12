using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInteraction : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private Transform interactPoint; 
    [SerializeField] private float interactRadius = 0.6f;
    [SerializeField] private LayerMask interactableLayer; 

    private PlayerInput input;
    
    // Nuestra variable de control
    private bool canInteract = true;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
    }

    // Nos suscribimos a los eventos de la radio
    private void OnEnable()
    {
        EventManager.OnInteractionStarted += DisableInteraction;
        EventManager.OnInteractionEnded += EnableInteraction;
    }

    // Nos desuscribimos si el jugador se destruye/desactiva
    private void OnDisable()
    {
        EventManager.OnInteractionStarted -= DisableInteraction;
        EventManager.OnInteractionEnded -= EnableInteraction;
    }

    private void DisableInteraction()
    {
        canInteract = false;
    }

    private void EnableInteraction()
    {
        canInteract = true;
    }

    private void Update()
    {
        // Si no podemos interactuar, cortamos la ejecución aquí mismo.
        // Ni siquiera gastamos recursos lanzando el rayo OverlapCircle.
        if (!canInteract) return;

        Collider2D collider = Physics2D.OverlapCircle(interactPoint.position, interactRadius, interactableLayer);
        
        if (collider != null)
        {
            IInteractable interactable = collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                if (input.InteractPressed) 
                {
                    interactable.Interact();
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (interactPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(interactPoint.position, interactRadius);
        }
    }
}