using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float moveSpeed = 8f; 
    
    private Rigidbody2D rb;
    private PlayerInput input;
    
    // Variable para saber hacia dónde estamos mirando (asumimos que el sprite original mira a la derecha)
    private bool facingRight = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInput>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {  
        if (!canMove) return;
        // 1. Aplicar la velocidad
        float horizontal = input.HorizontalInput;
        rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);

        // 2. Verificar si necesitamos darnos la vuelta
        // Si nos movemos a la derecha pero estamos mirando a la izquierda...
        if (horizontal > 0 && !facingRight) 
        {
            Flip();
        }
        // O si nos movemos a la izquierda pero estamos mirando a la derecha...
        else if (horizontal < 0 && facingRight) 
        {
            Flip();
        }
    }

    private void Flip()
    {
        // Cambiamos el estado de la variable
        facingRight = !facingRight;

        // Tomamos la escala actual del objeto
        Vector3 currentScale = transform.localScale;
        
        // Multiplicamos la escala en X por -1 (esto hace el efecto espejo perfecto)
        currentScale.x *= -1;
        
        // Aplicamos la nueva escala al personaje
        transform.localScale = currentScale;
    }
    // Añade esta variable al inicio de tu clase PlayerMovement
    private bool canMove = true;

    // Suscribirse a los eventos cuando el objeto se activa
    private void OnEnable()
    {
        EventManager.OnInteractionStarted += LockMovement;
        EventManager.OnInteractionEnded += UnlockMovement;
    }

    // Nos desuscribimos si el jugador se destruye/desactiva
    private void OnDisable()
    {
        EventManager.OnInteractionStarted -= LockMovement;
        EventManager.OnInteractionEnded -= UnlockMovement;
    }

    private void LockMovement()
    {
        canMove = false;
        rb.velocity = Vector2.zero; // Frenamos al jugador en seco
    }

    private void UnlockMovement()
    {
        canMove = true;
    }

  
}