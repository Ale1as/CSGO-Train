using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
public float speed = 5f;
    public float mouseSensitivity = 2f;
    public float verticalRotationRange = 60f;
    
    private float verticalRotation = 0f;
    
    private CharacterController controller;
    private Camera playerCamera;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
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
        
        // Player Movement
        float moveDirectionY = controller.isGrounded ? 0f : -9.8f;
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), moveDirectionY, Input.GetAxis("Vertical"));
        move = transform.TransformDirection(move);
        
        controller.Move(move * speed * Time.deltaTime);
    }
}
