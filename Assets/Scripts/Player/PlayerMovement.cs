using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;

    [Header("Sprint Settings")]
    [SerializeField] private float sprintSpeed = 12f;
    [SerializeField] private float costPerSecond = 30f;
    [SerializeField] private float minumumSprintStamina = 5f; // 달리기를 시작하기 위한 최소 스태미너

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 20f;

    private Rigidbody2D rb;
    private PlayerInput input;
    private PlayerStamina stamina;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInput>();
        stamina = GetComponent<PlayerStamina>();
    }

    void Update()
    {
        if (input.IsJump && IsGrounded())
        {
            Jump();
        }

        if (input.IsCrouch)
        {
            transform.localScale = new Vector3(1f, 0.5f, 1f);
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    void FixedUpdate()
    {
        float speed = input.IsCrouch ? moveSpeed * 0.5f : moveSpeed;
        bool isMoving = Mathf.Abs(input.MoveInput) > 0.01f;

        if (stamina != null)
        {
            bool canSprint = !input.IsCrouch && input.IsSprint && isMoving && stamina.HasStamina(minumumSprintStamina);
            if (canSprint)
            {
                speed = sprintSpeed;
                stamina.ConsumeStamina(costPerSecond * Time.fixedDeltaTime);
            }
        }

        rb.linearVelocity = new Vector2(speed * input.MoveInput, rb.linearVelocity.y);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }
}
