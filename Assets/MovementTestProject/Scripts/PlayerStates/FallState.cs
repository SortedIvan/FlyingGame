using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : PlayerBaseState
{
	Rigidbody playerRigidbody;
	AnimatorManager animatorManager;

	float inAirTimer;

	public override void EnterState(StateManager stateManager)
	{
		stateManager.currentStateVisual = "fall";

		playerRigidbody = stateManager.playerRigidbody;
		animatorManager = stateManager.animatorManager;
	}

	public override void OnCollisionEnter(StateManager stateManager)
	{
		
	}

	public override void UpdateState(StateManager stateManager)
	{
		
	
		
		stateManager.rayCastOrigin.y = stateManager.rayCastOrigin.y + 0.5f; // 0.5f is the raycast offset, coz we want it to start form the bottom of the collider
		stateManager.targetPosition = stateManager.transform.position;

		animatorManager.PlayTargetAnimation("Falling"); // set interacting

		inAirTimer = inAirTimer + Time.deltaTime;
		playerRigidbody.AddForce(stateManager.transform.forward * 2); // slight leap off ledge on falling
		playerRigidbody.AddForce(-Vector3.up * 3 * inAirTimer); // going down faster with time

		#region Sphere cast check for ground
		if (stateManager.isGrounded) // if on ground
		{
			stateManager.SwitchState(stateManager.landState);
			return;
		}

        #endregion

    }
}
