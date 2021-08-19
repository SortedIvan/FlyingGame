using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerInput : MonoBehaviour, IInput
{
    public Action<Vector2> OnMovementInput { get; set; }
    public Action<Vector3> OnMovementDirectionInput { get; set; }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

    }

    private void Update()
    {
        GetMovementInput();
        GetMovementDirection();

    }

    private void GetMovementDirection()
    {
        var ForwardDirection = Camera.main.transform.forward;
        Debug.DrawRay(Camera.main.transform.position, ForwardDirection * 10, Color.red);
        var DirectionToMoveIn = Vector3.Scale(ForwardDirection, Vector3.right + Vector3.forward);
        Debug.DrawRay(Camera.main.transform.position, DirectionToMoveIn * 10, Color.red);
        OnMovementDirectionInput?.Invoke(DirectionToMoveIn);
    }

    private void GetMovementInput()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        OnMovementInput?.Invoke(input);
    }
}
