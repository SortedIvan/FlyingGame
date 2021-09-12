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

	// Stats
	#region Flying stats
	[Header("Flying flags")]
	public bool isFlying;
	public bool flap;

	[Header("Flying Speeds")]
	public float actualFlySpeed;
	public float flapStrength = 5f;
	public float flySpeed = 10f;
	public float flyAccel = 4f;

	[Header("Fly math")]
	public float rigidVelocity;
	private float yVelocity;
	private Vector3 velocityDirection;
	public float flyLowLimit = -6f;
	public float flyHighLimit = 4f;
	public float flyVelocityGain = 2f; // gain downwards
	public float flyVelocityLoss = 1f; // loss upwards
	public float flyMaxSpeed = 50f;
	public float flyMinSpeed = 10f;

	public float flyRotationSpeed = 6f; // overall rotation speed handle velocity 2
	public float flyInputRotationSpeed = 0.1f; // handle velocity 1

	public float flyAdjustmentSpeed = 5f; //how quickly our velocity adjusts to the flying speed
	public float flyAdjustmentLerp = 0; //the lerp for our adjustment amount

	private float actualGravityAmount;
	public float glideGravityAmount = 0f; //how much gravity will pull us down when flying
	public float flyGravityBuildSpeed = 3f; //how much our gravity is lerped when stopping flying

	#endregion
	#region Other stats
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
	public float runningSpeed = 4f;
	public float sprintingSpeed = 6f;
	public float rotationSpeed = 8f;

	[Header("Jump Speeds")]
	public float jumpHeight = 1f;
	public float gravityIntensity = -20f;
	#endregion

	// Objects
	#region Objects

	#endregion

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

		if (!isFlying)
		{
			if (playerManager.isInteracting)
				return;
			HandleMovement();
			HandleRotation();
		} 
		else
		{
			HandleFlying2();
		}
	}

	// move, rotate, fall and land, jump
	#region Ground movement
	private void HandleMovement() // move direction code
	{
		if (isJumping) // can't move in air, for now :), :) or can you?
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

		if (isGrounded && !isJumping && !isFlying && !playerManager.isInteracting) // fix update issues in air
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

		if (isGrounded && !isJumping && !isFlying && !playerManager.isInteracting) // maybe add isInteracting check
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

		if (!isGrounded && !isJumping && !isFlying) // falling code
		{
			if (!playerManager.isInteracting)
			{
				animatorManager.PlayTargetAnimation("Falling", true);
			}

			inAirTimer = inAirTimer + Time.deltaTime;
			playerRigidbody.AddForce(transform.forward * leapingVelocity); // slight leap off ledge on falling
			playerRigidbody.AddForce(-Vector3.up * fallingVelocity * inAirTimer); // going down faster with time
		}

		if (Physics.SphereCast(rayCastOrigin, 0.2f, -Vector3.up, out hit, 1f, groundLayer)) // if on ground
		{
			if (!isGrounded && playerManager.isInteracting && !isFlying)
			{
				animatorManager.PlayTargetAnimation("Land", true); // play landing animation
			}
			else if (!isGrounded && !playerManager.isInteracting && isFlying)
			{
				animatorManager.PlayTargetAnimation("Land", true); // play landing animation
			}
			else if (isGrounded && playerManager.isInteracting) // rotate landing animation upwards
			{
				Vector3 LerpDir = Vector3.Lerp(transform.up, Vector3.up, Time.deltaTime * rotationSpeed/2);
				transform.rotation = Quaternion.FromToRotation(transform.up, LerpDir) * transform.rotation;
			}

			// reset bools and clean timers
			Vector3 rayCastHitPoint = hit.point;
			targetPosition.y = rayCastHitPoint.y;
			inAirTimer = 0;
			isGrounded = true;
			flyAdjustmentLerp = 0;
			flySpeed = flyMinSpeed;
			actualFlySpeed = flyMinSpeed;

			if (isFlying)
			{
				isFlying = false;
				playerRigidbody.useGravity = true;
			}

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
		if (isGrounded && !playerManager.isInteracting)
		{
			animatorManager.animator.SetBool("isJumping", true);
			animatorManager.PlayTargetAnimation("Jump", false);

			float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
			Vector3 playerVelocity = moveDirection;
			playerVelocity.y = jumpingVelocity;
			playerRigidbody.velocity = playerVelocity;
		}
	}
	#endregion

	#region Flight method 1
	public void HandleFlying()
	{
		animatorManager.animator.SetBool("isFlying", true);
		animatorManager.PlayTargetAnimation("Fly", false);

		// air Timer and fake gravity
		inAirTimer = inAirTimer + Time.deltaTime;
		playerRigidbody.AddForce(-Vector3.up * fallingVelocity * inAirTimer * 4);

		if (playerRigidbody.useGravity == true)
			playerRigidbody.useGravity = false; // turn off gravity

		rigidVelocity = playerRigidbody.velocity.magnitude; // RigidBody velocity
		yVelocity = playerRigidbody.velocity.y; // y amount, YAmt
		velocityDirection = transform.forward;

		if (flyAdjustmentLerp < 1.1)
			flyAdjustmentLerp += Time.deltaTime * flyAdjustmentSpeed;

		if (inputManager.b_input) // shift input, possibly move to input manager script
		{
			// fall fast
		}

		#region Velocity gain/loss
		if (yVelocity < flyLowLimit)
		{
			if (flySpeed < flyMaxSpeed) // tilted DOWNWARDS
				flySpeed += Time.deltaTime * (flyVelocityGain * (velocityDirection.y * -4f));
		}
		else if (yVelocity > flyHighLimit)
		{
			if (flySpeed > flyMinSpeed) // tilted UPWARDS
				flySpeed -= Time.deltaTime * (flyVelocityLoss * (velocityDirection.y * 8f));
		}
		if (flySpeed > flyMinSpeed) // fake air drag
		{
			flySpeed -= flySpeed * 0.05f * Time.deltaTime;
		}
		#endregion

		#region Flap
		if (flap)
		{
			// increase speed and height
			flap = false;
			inAirTimer -= inAirTimer / 1.3f; // flap slows fall down a bit
			flySpeed += flapStrength * 2;
			if (flySpeed >= flyMaxSpeed) flySpeed = flyMaxSpeed;
		}
		#endregion

		// actual movement code
		HandleVelocity(Time.deltaTime, flySpeed, flyAccel);
		HandleFlight(Time.deltaTime, actualFlySpeed, inputManager.horizontalInput, inputManager.verticalInput);
	}
	Vector3 HandleHorizontalDirection (float delta, float inputHorizontal)
	{
		Vector3 horizontalDirection = transform.forward;

		//horizontal input
		if (inputHorizontal > 0.1) // left tilt
		{
			horizontalDirection = Vector3.Lerp(horizontalDirection, -transform.right, delta * (flyInputRotationSpeed * inputHorizontal));
		}
		else if (inputHorizontal < -.1) // right tilt
		{
			horizontalDirection = Vector3.Lerp(horizontalDirection, transform.right, delta * (flyInputRotationSpeed * (inputHorizontal * -1)));
		}

		return horizontalDirection;
	}

	Vector3 HandleVerticalDirection (float delta, float inputVertical)
	{
		Vector3 verticalDirection = -transform.up;

		//vertical input
		if (inputVertical > 0.1) //upward tilt
		{
			verticalDirection = Vector3.Lerp(verticalDirection, -transform.forward, delta * (flyInputRotationSpeed * inputVertical)); // time.deltaTime * 0.1 * vertical input
		}
		else if (inputVertical < -.1) //downward tilt
		{
			verticalDirection = Vector3.Lerp(verticalDirection, transform.forward, delta * (flyInputRotationSpeed * (inputVertical * -1)));
		}

		return verticalDirection;
	}

	//rotate horizontal direction
	void RotateHorizontal(float delta, Vector3 Direction, float flyRotationSpeed)
	{
		Quaternion SlerpRotation = Quaternion.LookRotation(Direction, transform.up);
		transform.rotation = Quaternion.Slerp(transform.rotation, SlerpRotation, flyRotationSpeed * delta);
	}

	//rotate vertical direction
	void RotateVertical(float delta, Vector3 Direction,  float flyRotationSpeed)
	{
		Vector3 LerpDirection = Vector3.Lerp(transform.up, Direction, delta * flyRotationSpeed);
		transform.rotation = Quaternion.FromToRotation(transform.up, LerpDirection) * transform.rotation;
	}

	private void HandleVelocity (float delta, float targetFlySpeed, float flyAccel) // set actualFlySpeed
	{
		//clamp speed
		targetFlySpeed = Mathf.Clamp(targetFlySpeed, flyMinSpeed, flyMaxSpeed);
		//lerp speed
		actualFlySpeed = Mathf.Lerp(actualFlySpeed, targetFlySpeed, flyAccel * delta);
	}

	private void HandleFlight (float delta, float Speed, float inputHorizontal, float inputVertical)
	{
		//input direction 
		float InvertX = -1;
		float InvertY = -1;

		inputHorizontal = inputHorizontal * InvertX; //horizontal inputs
		inputVertical = inputVertical * InvertY; //vertical inputs

		//get direction to move and rotate
		Vector3 horizontalDirection = HandleHorizontalDirection(delta, inputHorizontal);
		Vector3 verticalDirection = HandleVerticalDirection(delta, inputVertical);
		
		float flyLerpSpd = flyAdjustmentSpeed * flyAdjustmentLerp;

		//rotate code
		RotateHorizontal(delta, horizontalDirection, flyRotationSpeed);
		RotateVertical(delta, verticalDirection, flyRotationSpeed);

		Vector3 targetFlyVelocity = transform.forward * Speed;

		//fake gravity
		actualGravityAmount = Mathf.Lerp(actualGravityAmount, glideGravityAmount, flyGravityBuildSpeed * 0.5f * delta);

		targetFlyVelocity -= Vector3.up * actualGravityAmount;
		//lerp velocity
		Vector3 dir = Vector3.Lerp(playerRigidbody.velocity, targetFlyVelocity, delta * flyLerpSpd);
		playerRigidbody.velocity = dir;
	}
	#endregion

	#region Flight method 2
	public void HandleFlying2 ()
	{
		/*
			so, angleRotation calculated every 0.2 seconds and/or
		the rotation speed is low enough to let you easily turn whenever you want?
		why would you want to turn? look around but keep flying towards your velocity?

		camera faces velocity direction

		*/
		
		if (!playerManager.isFlying) // paste elsewhere ?
		{
			animatorManager.PlayTargetAnimation("Fly", false);
			animatorManager.animator.SetBool("isFlying",  true);
		}

		// air Timer and fake gravity
		inAirTimer = inAirTimer + Time.deltaTime;
		playerRigidbody.AddForce(-Vector3.up * fallingVelocity * inAirTimer * 4);

		if (playerRigidbody.useGravity == true)
			playerRigidbody.useGravity = false; // turn off gravity

		rigidVelocity = playerRigidbody.velocity.magnitude; // RigidBody velocity
		yVelocity = playerRigidbody.velocity.y; // y amount, YAmt
		velocityDirection = transform.forward;

		if (flyAdjustmentLerp < 1.1)
			flyAdjustmentLerp += Time.deltaTime * flyAdjustmentSpeed;

		if (inputManager.b_input) // shift input, possibly move to input manager script
		{
			// fall fast
		}

		#region Velocity gain/loss
		if (yVelocity < flyLowLimit)
		{
			if (flySpeed < flyMaxSpeed) // tilted DOWNWARDS
				flySpeed += Time.deltaTime * (flyVelocityGain * (velocityDirection.y * -8f));
		}
		else if (yVelocity > flyHighLimit)
		{
			if (flySpeed > flyMinSpeed) // tilted UPWARDS
				flySpeed -= Time.deltaTime * (flyVelocityLoss * (velocityDirection.y * 8f));
		}
		if (flySpeed > flyMinSpeed) // fake air drag
		{
			flySpeed -= flySpeed * 0.01f * Time.deltaTime;
		}
		flySpeed = Mathf.Clamp(flySpeed, -flyMinSpeed, flyMaxSpeed);
		#endregion

		#region W + S input
		if (inputManager.verticalInput > 0.1f)
		{
			inAirTimer = 0;// flap slows fall down a bit
			if (flySpeed < flyMaxSpeed)
			{
				flySpeed += flapStrength / 4;
			}
		}
		else if (inputManager.verticalInput < -0.1f)
		{
			inAirTimer = 0;// flap slows fall down a bit
			if (flySpeed > -flyMinSpeed)
			{
				flySpeed -= flapStrength / 4;
			}
		}
		#endregion

		// actual movement code
		HandleVelocity2(Time.deltaTime, flySpeed, flyAccel);
		HandleFlight2(Time.deltaTime, actualFlySpeed, inputManager.cameraInputX, inputManager.cameraInputY);
	}
	Vector3 HandleDirection(float delta, float inputHorizontal, float inputVertical)
	{
		Vector3 direction = cameraObject.forward;

		return direction;
	}
	 
	//rotate horizontal direction
	void Rotate(float delta, Vector3 Direction, float flyRotationSpeed)
	{
		// tilt towards horizontal mouse input
		#region tilt
		Vector3 tilt = Vector3.zero;
		if (Vector3.Dot(transform.right, cameraObject.forward) > 0)
		{
			tilt = Vector3.Lerp(cameraObject.up, cameraObject.right, Vector3.Dot(transform.right, cameraObject.forward) * delta * 100);
		}
		else if (Vector3.Dot(-transform.right, cameraObject.forward) > 0)
		{
			tilt = Vector3.Lerp(cameraObject.up, -cameraObject.right, Vector3.Dot(-transform.right, cameraObject.forward) * delta * 100);
		}
		else
		{
			tilt = cameraObject.up;
		}
		#endregion
		Quaternion SlerpRotation = Quaternion.LookRotation(Direction, tilt);
		transform.rotation = Quaternion.Slerp(transform.rotation, SlerpRotation, flyRotationSpeed * delta);
	}

	private void HandleVelocity2(float delta, float targetFlySpeed, float flyAccel) // set actualFlySpeed
	{
		//clamp speed
		targetFlySpeed = Mathf.Clamp(targetFlySpeed, -flyMinSpeed, flyMaxSpeed);
		//lerp speed
		actualFlySpeed = Mathf.Lerp(actualFlySpeed, targetFlySpeed, flyAccel * delta);
	}

	private void HandleFlight2(float delta, float Speed, float inputHorizontal, float inputVertical)
	{
		//input direction 
		float InvertX = -1;
		float InvertY = -1;

		inputHorizontal = inputHorizontal * InvertX; //horizontal inputs
		inputVertical = inputVertical * InvertY; //vertical inputs

		//get direction to move and rotate
		Vector3 direction = HandleDirection(delta, inputHorizontal, inputVertical);

		float flyLerpSpd = flyAdjustmentSpeed * flyAdjustmentLerp;

		//rotate code
		Rotate(delta, direction, flyRotationSpeed);

		
		Vector3 targetFlyVelocity = transform.forward * Speed;

		//fake gravity
		actualGravityAmount = Mathf.Lerp(actualGravityAmount, glideGravityAmount, flyGravityBuildSpeed * 0.5f * delta);

		targetFlyVelocity -= Vector3.up * actualGravityAmount;
		//lerp velocity
		Vector3 dir = Vector3.Lerp(playerRigidbody.velocity, targetFlyVelocity, delta * flyLerpSpd);
		playerRigidbody.velocity = dir;
	}
	#endregion
}
