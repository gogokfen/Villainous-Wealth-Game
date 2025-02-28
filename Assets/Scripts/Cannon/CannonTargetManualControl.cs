using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CannonTargetManualControl : MonoBehaviour
{
    Vector3 moveDirection;
    Vector2 moveInput;
    [SerializeField] float moveSpeed = 10;

    Vector3 borderCheck;

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (context.canceled)
        {
            moveInput = Vector2.zero;
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        borderCheck = transform.position + moveDirection *moveSpeed *Time.deltaTime;
        if (borderCheck.x >25)  //25 on all
        {
            transform.position = new Vector3(25, 0, transform.position.z);
        }
        else if (borderCheck.x < -25)
        {
            transform.position = new Vector3(-25, 0, transform.position.z);
        }
        else if (borderCheck.z > 25)
        {
            transform.position = new Vector3(transform.position.x, 0, 25);
        }
        else if (borderCheck.z < -25)
        {
            transform.position = new Vector3(transform.position.x, 0, -25);
        }
        else
        {
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, 0, transform.position.z); //making sure waves don't affect the hight of the indicator
        }


        //float horiz = Input.GetAxisRaw("Horizontal");
        //float vert = Input.GetAxisRaw("Vertical");

        //transform.Translate((horiz*Vector3.right + vert*Vector3.forward) * moveSpeed * Time.deltaTime);

    }
}
