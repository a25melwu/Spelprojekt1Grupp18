using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class TopDownMovement : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 7.0f;
    public bool controlEnabled { get; set; } = true;

    private Vector2 moveInput;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (controlEnabled)
        {
            rb.linearVelocity = moveInput.normalized * maxSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
        
        //walking animation here. Suggestion: send your current velocity into the Animator for both the x- and y-axis
        
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>().normalized;
    }
}
