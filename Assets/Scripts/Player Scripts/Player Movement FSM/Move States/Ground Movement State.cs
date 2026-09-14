using UnityEngine;

public class GroundMovementState : MovementAbstractState
{
    public override void EnterState(MovementStateMachineManager state)
    {
        Debug.Log("Entering Ground Movement State");

    }

    public override void ExitState(MovementStateMachineManager state)
    {
        Debug.Log("Exiting Ground Movement State");

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

    // private void HandleMovement()
    // {
    //     // if (isGrounded())
    //     // {
    //         Quaternion yaw = Quaternion.Euler(0, lookDirection.eulerAngles.y, 0);
    //         Vector3 targetDirection = yaw * new Vector3(recievedMoveInput.x, _playerRb.linearVelocity.y, recievedMoveInput.y);

    //         Vector3 moveDirection = _playerRb.position + targetDirection * Time.fixedDeltaTime * speed;

    //         Vector3 axis = Vector3.up;
    //         float angle = sensitivity * Time.fixedDeltaTime * recievedLookInput;

    //         Quaternion targetTurn = Quaternion.AngleAxis(angle, axis);

    //         _playerRb.MoveRotation(_playerRb.rotation * targetTurn);

    //         Vector3 newPositon = targetTurn * (moveDirection - lookDirection.position) + lookDirection.position;

    //         _playerRb.MovePosition(newPositon);
    //     // }
    // }
}
