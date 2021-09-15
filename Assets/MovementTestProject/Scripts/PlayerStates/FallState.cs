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
		stateManager.currentState = "fall";

		playerRigidbody = stateManager.playerRigidbody;
		animatorManager = stateManager.animatorManager;
	}

	public override void OnCollisionEnter(StateManager stateManager)
	{
		
	}

	public override void UpdateState(StateManager stateManager)
	{
		RaycastHit hit;
		Vector3 rayCastOrigin = stateManager.transform.position;
		Vector3 targetPosition;
		rayCastOrigin.y = rayCastOrigin.y + 0.5f; // 0.5f is the raycast offset, coz we want it to start form the bottom of the collider
		targetPosition = stateManager.transform.position;

		animatorManager.PlayTargetAnimation("Falling"); // set interacting

		inAirTimer = inAirTimer + Time.deltaTime;
		playerRigidbody.AddForce(stateManager.transform.forward * 2); // slight leap off ledge on falling
		playerRigidbody.AddForce(-Vector3.up * 3 * inAirTimer); // going down faster with time

		#region Sphere cast check for ground
		if (Physics.SphereCast(rayCastOrigin, 0.2f, -Vector3.up, out hit, 1f, stateManager.groundCheckLayer)) // if on ground
		{
			// reset bools and clean timers
			Vector3 rayCastHitPoint = hit.point;
			targetPosition.y = rayCastHitPoint.y;
			inAirTimer = 0;
			//isGrounded = true;
			//flyAdjustmentLerp = 0;
			//flySpeed = flyMinSpeed;
			//actualFlySpeed = flyMinSpeed;

			//if (isFlying)
			//{
			//	isFlying = false;
			//	playerRigidbody.useGravity = true;
			//}

			stateManager.SwitchState(stateManager.landState);
			return;
		}
		else
		{
			stateManager.isGrounded = false; // still falling, no ground below
		}
		#endregion

		#region Float above ground based on raycast distance
		//if (isGrounded && !isJumping) // float above ground based on the raycast distance
		//{
		//	if (playerManager.isInteracting || inputManager.moveAmount > 0)
		//	{
		//		transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.1f);

		//	}
		//	else
		//	{
		//		transform.position = targetPosition;
		//	}
		//}
		#endregion
	}
}
