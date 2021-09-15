using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : PlayerBaseState
{
    InputManager inputManager;
    AnimatorManager animatorManager;
    Rigidbody playerRigidbody;

    public override void EnterState(StateManager stateManager)
    {
        stateManager.currentStateVisual = "jump";

        inputManager = stateManager.inputManager;
        animatorManager = stateManager.animatorManager;
        playerRigidbody = stateManager.playerRigidbody;

        animatorManager.animator.SetBool("isJumping", true);
        animatorManager.PlayTargetAnimation("Jump");
        inputManager.jump_input = false;

        float jumpingVelocity = Mathf.Sqrt(-2 * -15 * 2); // (-2 * gravity *  jump height)
        Vector3 playerVelocity = playerRigidbody.velocity;
        playerVelocity.y = jumpingVelocity;
        playerRigidbody.velocity = playerVelocity;
    }

    public override void OnCollisionEnter(StateManager stateManager)
    {
        
    }

    public override void UpdateState(StateManager stateManager)
    {
        if (!stateManager.isJumping)
        {
            stateManager.SwitchState(stateManager.fallState);
        }
    }
}
