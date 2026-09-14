using UnityEngine;

public class IdleState : MovementAbstractState
{
    public override void EnterState(MovementStateMachineManager state)
    {
        Debug.Log("Entering Idle State"); 
        
        state.OnSwingWebShot += HandleSwingPressed;
    }

    public override void ExitState(MovementStateMachineManager state)
    {
        Debug.Log("Exiting Idle State");

        state.OnSwingWebShot -= HandleSwingPressed;
    }

    public override void UpdateState(MovementStateMachineManager state)
    {
        
    }

    public override void FixedUpdate(MovementStateMachineManager state)
    {
        
    }

    public override void OnCollisionEnter(MovementStateMachineManager state)
    {
        
    }

    private void HandleSwingPressed(MovementStateMachineManager state)
    {
        state.SwitchState(state.SwingingState);
    }
}
