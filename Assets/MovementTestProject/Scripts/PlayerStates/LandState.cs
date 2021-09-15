using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandState : PlayerBaseState
{
	Rigidbody playerRigidbody;
	AnimatorManager animatorManager;

	public override void EnterState(StateManager stateManager)
	{
		playerRigidbody = stateManager.playerRigidbody;
		animatorManager = stateManager.animatorManager;

		if (stateManager.isFlying) // reset isFlying
		{
			stateManager.isFlying = false;
			playerRigidbody.useGravity = true;
		}
	}

	public override void OnCollisionEnter(StateManager stateManager)
	{
		
	}

	public override void UpdateState(StateManager stateManager)
	{
		animatorManager.PlayTargetAnimation("Land"); // play landing animation

		// reset bools and clean timers
		//Vector3 rayCastHitPoint = hit.point;
		//targetPosition.y = rayCastHitPoint.y;
		//inAirTimer = 0;
		//isGrounded = true;
		//flyAdjustmentLerp = 0;
		//flySpeed = flyMinSpeed;
		//actualFlySpeed = flyMinSpeed;

		
		//// rotate landing animation upwards
		//Vector3 LerpDir = Vector3.Lerp(stateManager.transform.up, Vector3.up, Time.deltaTime * 8 / 2);
		//stateManager.transform.rotation = Quaternion.FromToRotation(stateManager.transform.up, LerpDir) * stateManager.transform.rotation;

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
	}
}
