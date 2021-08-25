using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
	PlayerManager playerManager;
	AnimatorManager animatorManager;
	InputManager inputManager;

	Vector3 moveDirection;
	Transform cameraObject;
	Rigidbody playerRigidbody;

	[Header("Falling")]
	public float inAirTimer;
	public float leapingVelocity;
	public float fallingVelocity;
	public float rayCastHeightOffset = 0f;
	public LayerMask groundLayer;

	[Header("Movement Flags")]
	public bool isSprinting;
	public bool isGrounded;
	public bool isJumping;

	[Header("Movement Speeds")]
	public float walkingSpeed = 1.5f;
	public float runningSpeed = 4;
	public float sprintingSpeed = 6;
	public float rotationSpeed = 8;

	[Header("Jump Speeds")]
	public float jumpHeight = 1;
	public float gravityIntensity = -20;

	private void Awake() // get components
	{
		playerManager = GetComponent<PlayerManager>();
		animatorManager = GetComponent<AnimatorManager>();
		inputManager = GetComponent<InputManager>();
		playerRigidbody = GetComponent<Rigidbody>();
		cameraObject = Camera.main.transform;
	}

	public void HandleAllMovement()
	{
		HandleFallingAndLanding();
		if (playerManager.isInteracting)
			return;
		HandleMovement();
		HandleRotation();
	}

	private void HandleMovement() // move direction code
	{
		if (isJumping) // can't move in air, for now :)
			return;

		moveDirection = cameraObject.forward * inputManager.verticalInput;
		moveDirection = moveDirection + cameraObject.right * inputManager.horizontalInput;
		moveDirection.Normalize();
		moveDirection.y = 0;

		if (isSprinting)
		{
			moveDirection = moveDirection * sprintingSpeed;
		}
		else
		{
			if (inputManager.moveAmount >= 0.5f)
			{
				moveDirection = moveDirection * runningSpeed;
			}
			else
			{
				moveDirection = moveDirection * walkingSpeed;
			}
		}

		if (isGrounded && !isJumping) // fix update issues in air
		{
			Vector3 movementVelocity = moveDirection;
			playerRigidbody.velocity = movementVelocity;
		}

}

	private void HandleRotation() // rotate code
	{
		if (isJumping) // cant rotate in air
			return;

		Vector3 targetDirection = Vector3.zero;

		targetDirection = cameraObject.forward * inputManager.verticalInput;
		targetDirection = targetDirection + cameraObject.right * inputManager.horizontalInput;
		targetDirection.Normalize();
		targetDirection.y = 0;

		if (targetDirection == Vector3.zero) targetDirection = transform.forward;

		Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
		Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

		if (isGrounded && !isJumping) // maybe add isInteracting check
		{
			transform.rotation = playerRotation;
		}
	}

	private void HandleFallingAndLanding()
	{
		RaycastHit hit;
		Vector3 rayCastOrigin = transform.position;
		Vector3 targetPosition;
		rayCastOrigin.y = rayCastOrigin.y + rayCastHeightOffset;
		targetPosition = transform.position;

		if (!isGrounded && !isJumping) // falling code
		{
			if (!playerManager.isInteracting)
			{
				animatorManager.PlayTargetAnimation("Falling", true);
			}

			inAirTimer = inAirTimer + Time.deltaTime;
			playerRigidbody.AddForce(transform.forward * leapingVelocity); // slight leap off ledge on falling
			playerRigidbody.AddForce(-Vector3.up * fallingVelocity * inAirTimer); // going down faster with time
		}

		if (Physics.SphereCast(rayCastOrigin, 0.2f, -Vector3.up, out hit, groundLayer)) // landing code
		{
			if (!isGrounded && playerManager.isInteracting)
			{
				animatorManager.PlayTargetAnimation("Land", true); // play landing animation
			}

			Vector3 rayCastHitPoint = hit.point;
			targetPosition.y = rayCastHitPoint.y;
			inAirTimer = 0;
			isGrounded = true;
		}
		else
		{
			isGrounded = false;
		}

		if (isGrounded && !isJumping) // float above ground based on the raycast distance
		{
			if (playerManager.isInteracting || inputManager.moveAmount > 0)
			{
				transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.1f);

			}
			else
			{
				transform.position = targetPosition;
			}
		}
	}

	public void HandleJumping()
	{
		if (isGrounded)
		{
			animatorManager.animator.SetBool("isJumping", true);
			animatorManager.PlayTargetAnimation("Jump", false);

			float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
			Vector3 playerVelocity = moveDirection;
			playerVelocity.y = jumpingVelocity;
			playerRigidbody.velocity = playerVelocity;
		}
	}
}
