using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;

    [Header("Sprint Settings")]
    [SerializeField] private float sprintSpeed = 12f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 20f;

    private Rigidbody2D rb;
    private PlayerInput input;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInput>();
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
        float speed = input.IsCrouch ? moveSpeed * 0.5f :
                      input.IsSprint ? sprintSpeed : moveSpeed;
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
