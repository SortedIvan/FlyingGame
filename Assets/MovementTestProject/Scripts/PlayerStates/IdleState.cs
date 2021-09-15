using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : PlayerBaseState
{
    AnimatorManager animatorManager;
    InputManager inputManager;

    public override void EnterState(StateManager stateManager)
    {
        stateManager.currentStateVisual = "idle";

        animatorManager = stateManager.animatorManager;
        inputManager = stateManager.inputManager;
    }

    public override void OnCollisionEnter(StateManager stateManager)
    {
       
    }

    public override void UpdateState(StateManager stateManager)
    {
        animatorManager.UpdateAnimatorValues(0, 0, false); // updates animator boolean, false for no sprinting
        stateManager.transform.position = stateManager.targetPosition;
  

        #region State switch
        if (inputManager.moveAmount > 0.05)
        {
            Debug.Log("I am switching to walkState");
            stateManager.SwitchState(stateManager.walkState);
            return;
        }
        
        if (inputManager.jump_input)
        {
            stateManager.SwitchState(stateManager.jumpState);
            return;
        }
		#endregion
	}
}
