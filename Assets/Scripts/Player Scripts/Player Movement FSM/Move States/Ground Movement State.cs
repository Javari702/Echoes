using UnityEngine;

public class GroundMovementState : MovementAbstractState
{
    public override void EnterState(MovementStateMachineManager state)
    {
        Debug.Log("Entering Ground Movement State");

        state.OnSwingWebStart += HandleSwingPressed;
        state.OnMovementStop += HandleMovementStop;
    }

    public override void ExitState(MovementStateMachineManager state)
    {
        Debug.Log("Exiting Ground Movement State");

        state.OnSwingWebStart -= HandleSwingPressed;
        state.OnMovementStop -= HandleMovementStop;
    }

    public override void UpdateState(MovementStateMachineManager state)
    {
        
    }

    public override void FixedUpdate(MovementStateMachineManager state)
    {
        HandleMovement(state);
    }

    public override void OnCollisionEnter(MovementStateMachineManager state)
    {
        
    }

    private void HandleMovement(MovementStateMachineManager state)
    {
        // if (isGrounded())
        // {
            Quaternion yaw = Quaternion.Euler(0, state.lookDirection.eulerAngles.y, 0);
            Vector3 targetDirection = yaw * new Vector3(state.moveInputValue.x, 0f, state.moveInputValue.y);

            Vector3 moveDirection = state.playerRb.position + targetDirection * Time.fixedDeltaTime * state.speed;

            // Vector3 axis = Vector3.up;
            // float angle = state.sensitivity * Time.fixedDeltaTime * state.lookInputValue;

            // Quaternion targetTurn = Quaternion.AngleAxis(angle, axis);

            // state.playerRb.MoveRotation(state.playerRb.rotation * targetTurn);

            Vector3 newPositon = (moveDirection - state.lookDirection.position) + state.lookDirection.position;

            state.playerRb.MovePosition(newPositon);
        // }
    }
     
    // Transition Logic 
    private void HandleSwingPressed(MovementStateMachineManager state)
    {
        state.SwitchState(state.SwingingState);
    }

    private void HandleMovementStop(MovementStateMachineManager state)
    {
        state.SwitchState(state.IdleState);
    }
}
