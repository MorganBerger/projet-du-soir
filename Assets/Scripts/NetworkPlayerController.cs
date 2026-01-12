using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Network player controller that synchronizes movement across clients
/// </summary>
[RequireComponent(typeof(NetworkObject))]
public class NetworkPlayerController : NetworkBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float sprintMultiplier = 1.5f;
    
    [Header("Components")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isSprinting;
    
    // Animation hashes
    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int horizontalHash = Animator.StringToHash("Horizontal");
    private readonly int verticalHash = Animator.StringToHash("Vertical");
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        // Only local player can control this
        if (!IsOwner)
        {
            // Disable input for non-owned players
            enabled = false;
        }
    }
    
    private float networkUpdateInterval = 0.05f; // 20 updates per second
    private float lastNetworkUpdateTime;
    
    void Update()
    {
        if (!IsOwner) return;
        
        // Get input (only on local client)
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(horizontal, vertical).normalized;
        
        isSprinting = Input.GetKey(KeyCode.LeftShift);
        
        // Update animations
        UpdateAnimation();
        
        // Flip sprite
        if (horizontal != 0 && spriteRenderer != null)
        {
            spriteRenderer.flipX = horizontal < 0;
        }
        
        // Send movement to server at limited rate
        if (Time.time - lastNetworkUpdateTime >= networkUpdateInterval)
        {
            if (moveInput != Vector2.zero || isSprinting)
            {
                UpdateMovementServerRpc(moveInput, isSprinting);
                lastNetworkUpdateTime = Time.time;
            }
        }
    }
    
    void FixedUpdate()
    {
        if (!IsOwner) return;
        
        // Apply movement
        float currentSpeed = moveSpeed * (isSprinting ? sprintMultiplier : 1f);
        rb.velocity = moveInput * currentSpeed;
    }
    
    [ServerRpc]
    void UpdateMovementServerRpc(Vector2 movement, bool sprinting)
    {
        // Server processes movement and syncs to all clients
        UpdateMovementClientRpc(movement, sprinting);
    }
    
    [ClientRpc]
    void UpdateMovementClientRpc(Vector2 movement, bool sprinting)
    {
        if (IsOwner) return; // Don't apply to local player (already handled)
        
        moveInput = movement;
        isSprinting = sprinting;
        
        float currentSpeed = moveSpeed * (isSprinting ? sprintMultiplier : 1f);
        rb.velocity = moveInput * currentSpeed;
        
        UpdateAnimation();
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
}
