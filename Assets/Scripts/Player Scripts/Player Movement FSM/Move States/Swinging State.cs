using UnityEngine;

public class SwingingState : MovementAbstractState
{
    public override void EnterState(MovementStateMachineManager state)
    {
        Debug.Log("Entering Swinging State");

    }

    public override void ExitState(MovementStateMachineManager state)
    {
        Debug.Log("Exiting Swinging State");

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
}
