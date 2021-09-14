using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    

    public override void EnterState(StateManager stateManager)
    {
        Debug.Log("I am in IdleState");

    }

    public override void OnCollisionEnter(StateManager stateManager)
    {
       
    }

    public override void UpdateState(StateManager stateManager, InputManager inputManager)
    {
        if (inputManager.moveAmount > 0.1)
        {
            Debug.Log("I am switching to walkState");
            stateManager.SwitchState(stateManager.walkState);
        }
        else if (stateManager.GetInputManager().jump_input)
        {
            stateManager.SwitchState(stateManager.jumpState);
        }





    }
}
