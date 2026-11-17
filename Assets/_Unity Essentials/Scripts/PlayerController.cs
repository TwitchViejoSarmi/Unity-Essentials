using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Moves forward/backward and rotates with WASD/Arrow keys.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Tooltip("Forward/back speed (units/sec).")]
    public float speed = 5.0f;

    [Tooltip("Turn speed (degrees/sec).")]
    public float rotationSpeed = 120.0f;

    public float jumpForce = 5.0f;

    [Header("Ground Check Settings")]
    public Transform groundCheckPoint;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Recovery Settings")]
    public float uprightThreshold = 0.3f;   // below this, the player counts as tipped over
    public float recoverDelay = 1.5f;       // how long to stay tipped before recovering
    public float recoverSpeed = 5f;         // how fast to rotate back upright

    [Header("Victory Settings")]
    public UpdateCollectibleCount updateCollectibleCount;
    public GameObject onVictoryEffect;
    public GameObject onVictorySFX;

    private Rigidbody rb;
    private float tippedTimer;
    private bool isRecovering;
    private bool celebrated = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) Debug.LogWarning("PlayerController needs a Rigidbody.");
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheckPoint.position, checkRadius, groundLayer);
    }

    private bool IsTipped()
    {
        return Vector3.Dot(transform.up, Vector3.up) < uprightThreshold;
    }

    private void CheckRecovery()
    {
        if (!isRecovering)
        {
            if (IsTipped())
            {
                tippedTimer += Time.fixedDeltaTime;
                if (tippedTimer >= recoverDelay)
                    isRecovering = true;
            }
            else
            {
                tippedTimer = 0f;
            }
            return;
        }

        // Keep only the facing direction (yaw) and smoothly rotate to upright
        Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        if (flatForward.sqrMagnitude < 0.001f)
            flatForward = Vector3.ProjectOnPlane(transform.up, Vector3.up); // lying flat on its back/face

        Quaternion upright = Quaternion.LookRotation(flatForward.normalized, Vector3.up);
        rb.angularVelocity = Vector3.zero;
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, upright, recoverSpeed * Time.fixedDeltaTime));

        if (Vector3.Dot(transform.up, Vector3.up) > 0.98f)
        {
            isRecovering = false;
            tippedTimer = 0f;
        }
    }

    // Visualize the sphere in the Scene view
    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = IsGrounded() ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheckPoint.position, checkRadius);
        }
    }

    private void Update()
    {
        // Jumping (there should be a check for grounded state)
        if (Keyboard.current.spaceKey.wasPressedThisFrame && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }

        if (updateCollectibleCount.GetTotalCollectibles() == 0 && !celebrated)
        {
            Instantiate(onVictoryEffect, transform.position, onVictoryEffect.transform.rotation);
            Instantiate(onVictorySFX, transform.position, transform.rotation);
            celebrated = true;
        }
    }

    private void FixedUpdate()
    {
        Vector2 moveInput = Vector2.zero;

        // Forward/backward
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveInput.y = 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveInput.y = -1f;

        // Left/right (rotation)
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveInput.x = -1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveInput.x = 1f;

        // Move in facing direction 
        Vector3 movement = transform.forward * moveInput.y * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        // Y-axis rotation (invert when going backwards)
        float turnDirection = moveInput.x;
        if (moveInput.y < 0)
            turnDirection = -turnDirection;

        float turn = turnDirection * rotationSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);

        CheckRecovery();
    }
}
