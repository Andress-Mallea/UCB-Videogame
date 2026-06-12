using UnityEngine;

public class CollisionSenses : MonoBehaviour
{
    [Header("Configuración de Suelo")]
    [Tooltip("El punto exacto donde el personaje toca el piso")]
    [SerializeField] private Transform groundCheck; 
    
    [Tooltip("El tamaño del área de detección")]
    [SerializeField] private float groundCheckRadius = 0.2f;
    
    [Tooltip("Define qué objetos del mundo son considerados 'suelo'")]
    [SerializeField] private LayerMask whatIsGround; 

    // Propiedad pública para que PlayerMovement o la Máquina de Estados la lean
    public bool IsGrounded { get; private set; }

    private void Update()
    {
        // Physics2D.OverlapCircle crea un círculo invisible y devuelve 'true' 
        // si toca cualquier objeto que pertenezca a la LayerMask 'whatIsGround'.
        IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    }

    // Método nativo de Unity: Dibuja guías visuales en el Editor (no se ven en el juego final).
    // Esto es vital para ajustar el tamaño del círculo sin adivinar.
    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            // Si está tocando el suelo el círculo se dibuja verde, si no, rojo.
            Gizmos.color = IsGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}