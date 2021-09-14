using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : PlayerBaseState
{
    AnimatorManager animatorManager;
    InputManager inputManager;

    Vector3 moveDirection;
    Transform cameraObject;
    Rigidbody playerRigidbody;


    public override void EnterState(StateManager stateManager)
    {
        this.animatorManager = stateManager.GetAnimatorManager();
        this.inputManager = stateManager.GetInputManager();
        this.cameraObject = stateManager.GetCameraObject();
        this.playerRigidbody = stateManager.GetPlayerRigidBody();
		Debug.Log("Im in walkstate for some reason");

    }

    public override void OnCollisionEnter(StateManager stateManager)
    {

    }

    public override void UpdateState(StateManager stateManager, InputManager inputManager)
    {

		moveDirection = cameraObject.forward * inputManager.verticalInput;
		moveDirection = moveDirection + cameraObject.right * inputManager.horizontalInput;
		moveDirection.Normalize();
		moveDirection.y = 0;

		if (inputManager.b_input)
		{
			moveDirection = moveDirection * 2;
		}
		else
		{
			if (inputManager.moveAmount >= 0.5f)
			{
				moveDirection = moveDirection * 6;
			}
			else
			{
				moveDirection = moveDirection * 6;
			}
		}
		Vector3 movementVelocity = moveDirection;
		playerRigidbody.velocity = movementVelocity;
		
		
	}
}
