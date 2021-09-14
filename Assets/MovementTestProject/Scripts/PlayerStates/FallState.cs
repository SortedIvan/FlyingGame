using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : PlayerBaseState
{
    public override void EnterState(StateManager stateManager)
    {
        Debug.Log("I am definately working");
    }

    public override void OnCollisionEnter(StateManager stateManager)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState(StateManager stateManager, InputManager inputManager)
    {
        throw new System.NotImplementedException();
    }
}
