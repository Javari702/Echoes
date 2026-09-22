using Unity.VisualScripting;
using UnityEngine;

public class IdleState : MovementAbstractState
{
    public override void EnterState(MovementStateMachineManager state)
    {
        Debug.Log("Entering Idle State"); 
        
        state.OnTriggerStart += HandleTriggerPressed;
        state.OnMovementStart += HandleMovementStart;
    }

    public override void ExitState(MovementStateMachineManager state)
    {
        Debug.Log("Exiting Idle State");

        state.OnTriggerStart -= HandleTriggerPressed;
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
    private void HandleTriggerPressed(MovementStateMachineManager state, SwingHand hand)
    {
        state.pendingHand = hand;

        if (!hand.onWall)
        {
            state.SwitchState(state.SwingingState);
            return;            
        }

        state.SwitchState(state.ClimbingState);
    }

    private void HandleMovementStart(MovementStateMachineManager state)
    {
        state.SwitchState(state.GroundMovementState);
    }
}
