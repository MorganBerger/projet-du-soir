using UnityEngine;

/// <summary>
/// Controls player movement and interactions
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float sprintMultiplier = 1.5f;
    
    [Header("Animation")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isSprinting;
    
    // Animation parameter names
    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int horizontalHash = Animator.StringToHash("Horizontal");
    private readonly int verticalHash = Animator.StringToHash("Vertical");
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Configure rigidbody for top-down movement
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        // Get input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(horizontal, vertical).normalized;
        
        // Sprint input
        isSprinting = Input.GetKey(KeyCode.LeftShift);
        
        // Update animations
        UpdateAnimation();
        
        // Flip sprite based on movement direction
        if (horizontal != 0 && spriteRenderer != null)
        {
            spriteRenderer.flipX = horizontal < 0;
        }
    }

    void FixedUpdate()
    {
        // Apply movement
        float currentSpeed = moveSpeed * (isSprinting ? sprintMultiplier : 1f);
        rb.velocity = moveInput * currentSpeed;
    }

    void UpdateAnimation()
    {
        if (animator == null) return;
        
        float speed = moveInput.magnitude;
        animator.SetFloat(speedHash, speed);
        
        if (speed > 0.01f)
        {
            animator.SetFloat(horizontalHash, moveInput.x);
            animator.SetFloat(verticalHash, moveInput.y);
        }
    }

    public Vector2 GetMoveDirection()
    {
        return moveInput;
    }

    public bool IsMoving()
    {
        return moveInput.magnitude > 0.01f;
    }
}
