using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	Animator animator;
	InputManager inputManager;
	CameraManager cameraManager;
	PlayerLocomotion playerLocomotion;

	public bool isInteracting;
	public bool isFlying;

	private void Awake() // get components
	{
		animator = GetComponent<Animator>();
		inputManager = GetComponent<InputManager>();
		cameraManager = FindObjectOfType<CameraManager>();
		playerLocomotion = GetComponent<PlayerLocomotion>();
	}

	private void Update()
	{
		inputManager.HandleAllInputs();
	}

	private void FixedUpdate()
	{
		playerLocomotion.HandleAllMovement();
	}

	private void LateUpdate() // important which bool is where and where to set it
	{
		cameraManager.HandleALlCameraMovement();

		isFlying = animator.GetBool("isFlying");
		isInteracting = animator.GetBool("isInteracting");
		playerLocomotion.isJumping = animator.GetBool("isJumping");
		animator.SetBool("isGrounded", playerLocomotion.isGrounded);
	}
}
