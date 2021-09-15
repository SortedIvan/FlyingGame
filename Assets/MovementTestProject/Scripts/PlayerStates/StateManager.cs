using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    #region Stats
    [Header("Stats")]
    public string currentState;

    [Header("Booleans")]
    public bool isGrounded;
    public bool isJumping;
    public bool isFlying;

    [Header("Other")]
    public LayerMask groundCheckLayer;
    #endregion

    #region Components
    [Header("Components")]
    // managers
	public InputManager inputManager;
    public AnimatorManager animatorManager;

    // camera
    public CameraManager cameraManager;
    public Transform cameraObject;
    public Transform cameraManagerObject;

    // rigidbody
    public Rigidbody playerRigidbody;

	#endregion

	#region States
	PlayerBaseState currentPlayerState; // Holds an instance of what state a player is currently in.
    public IdleState idleState = new IdleState();
    public WalkState walkState = new WalkState();
    public RunState runState = new RunState();
    public JumpState jumpState = new JumpState();
    public FallState fallState = new FallState();
    public LandState landState = new LandState();

	#endregion

	private void Awake() // get components on awake
    {
        animatorManager = GetComponent<AnimatorManager>();
        inputManager = GetComponent<InputManager>();
        cameraManager = GameObject.Find("Camera Manager").GetComponent<CameraManager>();
        playerRigidbody = GetComponent<Rigidbody>();
        cameraObject = Camera.main.transform;
        cameraManagerObject = GameObject.Find("Camera Manager").transform;
    }


    void Start()
    {
        currentPlayerState = idleState; // The state that the player is currently in (and also upon launching the game/starting);

        currentPlayerState.EnterState(this); // (this) references to the statemanager itself (the context is specified THIS)
    }

    void Update() // input code in uptade
    {
        inputManager.HandleAllInputs();
    }

	private void FixedUpdate() // movement code in fixed update
	{
        currentPlayerState.UpdateState(this);
    }

	private void LateUpdate() // camera movement, rotation
	{
        cameraManager.HandleALlCameraMovement();

        isJumping = animatorManager.animator.GetBool("isJumping");
    }

	public void SwitchState(PlayerBaseState playerState)
    {
        currentPlayerState = playerState;
        playerState.EnterState(this);
    }

    public void OnCollisionEnter(Collision collision)
    {
        currentPlayerState.OnCollisionEnter(this);
    }

}
