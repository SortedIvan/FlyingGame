using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{

    PlayerBaseState currentPlayerState; //Holds an instance of what state a player is currently in.


    public InputManager inputManager;
    public AnimatorManager animatorManager;
    Vector3 moveDirection;
    public Transform cameraObject;
    public Rigidbody playerRigidbody;

    public PlayerIdleState playerIdleState = new PlayerIdleState();
    public WalkState walkState = new WalkState();
    public RunState runState = new RunState();
    public JumpState jumpState = new JumpState();
    public FallState fallState = new FallState();
    public FlyingState flyingState = new FlyingState();
        





    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        inputManager = GetComponent<InputManager>();
        playerRigidbody = GetComponent<Rigidbody>();
        cameraObject = Camera.main.transform;
    }


    void Start()
    {



        currentPlayerState = playerIdleState; //The state that the player is currently in (and also upon launching the game/starting);

        currentPlayerState.EnterState(this); //This references to the statemanager itself (the context is specified THIS)
    }

    void Update()
    {
        inputManager.HandleAllInputs();
        currentPlayerState.UpdateState(this, inputManager);
        
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

    public InputManager GetInputManager()
    {
        return this.inputManager;
    }

    public AnimatorManager GetAnimatorManager()
    {
        return this.animatorManager;
    }

    public Transform GetCameraObject()
    {
        return this.cameraObject;
    }

    public Rigidbody GetPlayerRigidBody()
    {
        return this.playerRigidbody;
    }

}
