using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInteraction : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private Transform interactPoint; 
    [SerializeField] private float interactRadius = 0.6f;
    [SerializeField] private LayerMask interactableLayer; 

    private PlayerInput input;
    private bool canInteract = true;
    private IInteractable currentInteractable; // Memoria del objeto actual

    private void Awake() { input = GetComponent<PlayerInput>(); }

    private void OnEnable()
    {
        EventManager.OnInteractionStarted += DisableInteraction;
        EventManager.OnInteractionEnded += EnableInteraction;
    }

    private void OnDisable()
    {
        EventManager.OnInteractionStarted -= DisableInteraction;
        EventManager.OnInteractionEnded -= EnableInteraction;
    }

    private void DisableInteraction() { canInteract = false; }
    private void EnableInteraction() { canInteract = true; }

    private void Update()
    {
        if (!canInteract) return;

        Collider2D collider = Physics2D.OverlapCircle(interactPoint.position, interactRadius, interactableLayer);
        
        if (collider != null)
        {
            IInteractable newInteractable = collider.GetComponent<IInteractable>();
            if (newInteractable != null)
            {
                // Encendemos el borde del nuevo objeto
                if (newInteractable != currentInteractable)
                {
                    if (currentInteractable != null) currentInteractable.DesactivarBorde();
                    currentInteractable = newInteractable;
                    currentInteractable.ActivarBorde();
                }

                if (input.InteractPressed) 
                {
                    currentInteractable.Interact();
                }
            }
        }
        else
        {
            // Apagamos el borde si nos alejamos
            if (currentInteractable != null)
            {
                currentInteractable.DesactivarBorde();
                currentInteractable = null;
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