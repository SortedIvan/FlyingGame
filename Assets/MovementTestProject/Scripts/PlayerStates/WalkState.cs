using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : PlayerBaseState
{
    AnimatorManager animatorManager;
    InputManager inputManager;
	Transform cameraObject;
	Transform cameraManagerObject;
	Rigidbody playerRigidbody;


    public override void EnterState(StateManager stateManager)
    {
        animatorManager = stateManager.animatorManager;
        inputManager = stateManager.inputManager;
        cameraObject = stateManager.cameraObject;
		cameraManagerObject = stateManager.cameraManagerObject;
        playerRigidbody = stateManager.playerRigidbody;

		stateManager.currentStateVisual = "walk";
	}

    public override void OnCollisionEnter(StateManager stateManager)
    {

    }

    public override void UpdateState(StateManager stateManager)
    {
		#region Velocity
	
		stateManager.moveDirection = cameraManagerObject.forward * inputManager.verticalInput;
		stateManager.moveDirection = stateManager.moveDirection + cameraObject.right * inputManager.horizontalInput;
		stateManager.moveDirection.Normalize();
		stateManager.moveDirection.y = 0;

		if (inputManager.moveAmount >= 0.5f)
		{
			stateManager.moveDirection = stateManager.moveDirection * 4; // run speed
		}
		else
		{
			stateManager.moveDirection = stateManager.moveDirection * 2; // walk speed
		}

		if (stateManager.moveDirection == Vector3.zero)
		{
			// ASK IF THIS STOPS RUNNING CODE BELOW STATE SWITCH LINE? PLS ANSVVER
			stateManager.SwitchState(stateManager.idleState);
			return; // it does not, lol, need to put return;
		}


		Vector3 movementVelocity = stateManager.moveDirection;
		playerRigidbody.velocity = movementVelocity;
		#endregion
		#region Rotation
		Vector3 targetDirection = Vector3.zero;

		targetDirection = cameraManagerObject.forward * inputManager.verticalInput;
		targetDirection = targetDirection + cameraObject.right * inputManager.horizontalInput;
		targetDirection.Normalize();
		targetDirection.y = 0;

		if (targetDirection == Vector3.zero) targetDirection = stateManager.transform.forward;
		
		Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
		Quaternion playerRotation = Quaternion.Slerp(stateManager.transform.rotation, targetRotation, 8 * Time.deltaTime);

		stateManager.transform.rotation = playerRotation;
		#endregion

		animatorManager.UpdateAnimatorValues(0, inputManager.moveAmount, false); // updates animator boolean, false for no sprinting

		#region State switching? could be here or inbetween code
		if (inputManager.jump_input) // jump switch
		{
			stateManager.SwitchState(stateManager.jumpState);
			return;
		}

		//if (inputManager.moveAmount == 0) // idle switch
		//{
		//	stateManager.SwitchState(stateManager.idleState);
		//}
		#endregion

	}
}
