using UnityEngine;

public class IdleState : MovementAbstractState
{
    public override void EnterState(MovementStateMachineManager state)
    {
        Debug.Log("Entering Idle State"); 
        
        state.OnSwingWebStart += HandleSwingPressed;
        state.OnMovementStart += HandleMovementStart;
    }

    public override void ExitState(MovementStateMachineManager state)
    {
        Debug.Log("Exiting Idle State");

        state.OnSwingWebStart -= HandleSwingPressed;
        state.OnMovementStart -= HandleMovementStart;
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

    // Transition Logic
    private void HandleSwingPressed(MovementStateMachineManager state, SwingHand hand)
    {
        state.pendingHand = hand;
        state.SwitchState(state.SwingingState);
    }

    private void HandleMovementStart(MovementStateMachineManager state)
    {
        state.SwitchState(state.GroundMovementState);
    }
}
