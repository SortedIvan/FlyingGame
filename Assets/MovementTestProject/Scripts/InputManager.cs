using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	PlayerControls playerControls;
	AnimatorManager animatorManager;

	public Vector2 movementInput;
	public Vector2 cameraInput;

	public float cameraInputX;
	public float cameraInputY;

	public float moveAmount;
	public float verticalInput;
	public float horizontalInput;

	public bool b_input;
	public bool jump_input;

	private void Awake() // get components
	{
		animatorManager = GetComponent<AnimatorManager>();
	}

	private void OnEnable() 
	{
		if (playerControls == null)
		{
			playerControls = new PlayerControls();

			playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
			playerControls.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

			playerControls.PlayerActions.B.performed += i => b_input = true;
			playerControls.PlayerActions.B.canceled += i => b_input = false;
			playerControls.PlayerActions.Jump.performed += i => jump_input = true;
		}

		playerControls.Enable();
	}

	private void OnDisable()
	{
		playerControls.Disable();
	}

	public void HandleAllInputs()
	{
		HandleMovementInput();

	}

	private void HandleMovementInput()
	{
		verticalInput = movementInput.y;
		horizontalInput = movementInput.x;

		cameraInputX = cameraInput.x;
		cameraInputY = cameraInput.y;

		moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
	}



}
