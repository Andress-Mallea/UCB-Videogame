using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput), typeof(CollisionSenses))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float moveSpeed = 8f; 
    [SerializeField] private float jumpForce = 4f;
    [SerializeField] private float fallMultiplier = 2.5f;
    
    private Rigidbody2D rb;
    private PlayerInput input;
    private CollisionSenses senses; 
    
    private bool facingRight = true;
    private bool canMove = true; 

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInput>();
        senses = GetComponent<CollisionSenses>();
    }

    private void OnEnable()
    {
        EventManager.OnInteractionStarted += LockMovement;
        EventManager.OnInteractionEnded += UnlockMovement;
    }

    private void OnDisable()
    {
        EventManager.OnInteractionStarted -= LockMovement;
        EventManager.OnInteractionEnded -= UnlockMovement;
    }

    private void LockMovement()
    {
        canMove = false;
        rb.velocity = Vector2.zero; 
    }

    private void UnlockMovement() { canMove = true; }

    private void Update()
    {
        if (!canMove) return;

        if (input.JumpPressed && senses.IsGrounded)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        if (!canMove) return;

        Move();
        ApplyCustomGravity();
    }

    private void Move()
    {  
        float horizontal = input.HorizontalInput;
        rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);

        if (horizontal > 0 && !facingRight) Flip();
        else if (horizontal < 0 && facingRight) Flip();
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void ApplyCustomGravity()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }
}