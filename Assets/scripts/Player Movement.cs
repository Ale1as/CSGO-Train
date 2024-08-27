using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
        public float speed = 5f;
    public float mouseSensitivity = 2f;
    public float verticalRotationRange = 60f;
    public float jumpForce = 5f;
    public float crouchHeight = 0.5f;
    public float standingHeight = 2f;
    public float crouchSpeedFactor = 0.5f; // Optional: reduce movement speed while crouching
    public LayerMask cameraCullingMask; // Layer mask for culling

    private float verticalRotation = 0f;
    private bool isCrouching = false;
    private Rigidbody rb;
    private Camera playerCamera;
    private CapsuleCollider playerCollider;
    private Vector3 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCamera = GetComponentInChildren<Camera>();
        playerCollider = GetComponent<CapsuleCollider>();

        // Ensure the Rigidbody component is not affected by rotation
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Configure camera to exclude player mesh layer
        playerCamera.cullingMask &= ~cameraCullingMask;
    }

    void Update()
    {
        // Mouse Look
        float horizontal = Input.GetAxis("Mouse X") * mouseSensitivity;
        float vertical = Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation -= vertical;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalRotationRange, verticalRotationRange);

        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.rotation *= Quaternion.Euler(0f, horizontal, 0f);

        // Crouch
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            ToggleCrouch();
        }

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        }

        // Player Movement
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        moveDirection = (transform.right * moveX + transform.forward * moveZ).normalized;
        moveDirection.y = 0; // Ensure movement is only in the XZ plane

        float currentSpeed = isCrouching ? speed * crouchSpeedFactor : speed;
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }

    void ToggleCrouch()
    {
        if (isCrouching)
        {
            // Stand up
            playerCollider.height = standingHeight;
            playerCollider.center = new Vector3(0, standingHeight / 2, 0);
            playerCamera.transform.localPosition = new Vector3(0, 3.13f, 0);
        }
        else
        {
            // Crouch
            playerCollider.height = crouchHeight;
            playerCollider.center = new Vector3(0, crouchHeight / 2, 0);
            playerCamera.transform.localPosition = new Vector3(0, crouchHeight / 2, 0);
        }

        isCrouching = !isCrouching;
    }

    bool IsGrounded()
    {
        // Adjust the raycast length based on the current height of the collider
        float rayLength = (playerCollider.height / 2) + 0.1f;
        return Physics.Raycast(transform.position, Vector3.down, rayLength);
    }
}
