using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float jumpVelocity = 12f;

    public LayerMask groundLayer;          // Wall로 설정
    public float groundCheckDistance = 0.05f;

    Rigidbody2D rb;
    Collider2D col;
    bool jumpPressed;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
            jumpPressed = true;
        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f) 
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
    }

    void FixedUpdate()
    {
        float x = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);

        bool grounded = Physics2D.BoxCast(
            col.bounds.center,
            col.bounds.size,
            0f,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        if (jumpPressed && grounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);
        }

        jumpPressed = false;
    }
}
