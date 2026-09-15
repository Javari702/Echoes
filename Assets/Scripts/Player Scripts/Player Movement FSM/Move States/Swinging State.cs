using UnityEngine;

public class SwingingState : MovementAbstractState
{
    private float pullDistance;
    private float pullDistanceY;
    private bool gestureStarted;
    private bool gestureStartedY;
    private Vector3 lastPosition;
    private Vector3 lastPositionY;
    private Vector3 gestureStartPosition; 
    private Vector3 gestureStartPositionY;
    private Vector3 anchorPoint; 
    private RaycastHit hit;

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
