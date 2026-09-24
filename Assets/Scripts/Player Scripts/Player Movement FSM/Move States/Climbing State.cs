using Unity.VisualScripting;
using UnityEngine;

public class ClimbingState : MovementAbstractState
{
    private SwingHand _firstHand;
    private SwingHand _secondHand;
    private MovementStateMachineManager _manager;

    public override void EnterState(MovementStateMachineManager state)
    {
        Debug.Log("Entering Climbing State"); 

        _manager = state;

        // state.OnSwingWebStart += HandleSwingPressed;
        // state.OnMovementStop += HandleMovementStop;
        state.OnTriggerStart += HandleSecondTiggerPressed;
        state.OnTriggerStop += HandleTriggerStop;

        _firstHand = state.pendingHand;

        if (_firstHand != null) _firstHand.FindWallAnchor(_firstHand, StartClimb);
    }

    public override void ExitState(MovementStateMachineManager state)
    {
        Debug.Log("Exiting Climbing State");

        // state.OnSwingWebStart -= HandleSwingPressed;
        // state.OnMovementStop -= HandleMovementStop;
        state.OnTriggerStart -= HandleSecondTiggerPressed;
        state.OnTriggerStop -= HandleTriggerStop;
    }

    public override void UpdateState(MovementStateMachineManager state)
    {
        // Debug.DrawRay();
    }

    public override void FixedUpdate(MovementStateMachineManager state)
    {
        HandleVaulting(state);
    }

    public override void OnCollisionEnter(MovementStateMachineManager state)
    {
        
    }

    private void StartClimb(SwingHand hand)
    {
        if (hand.wallCollider == null) return;

        hand.joint = hand.climbHand.AddComponent<SpringJoint>();
        hand.joint.autoConfigureConnectedAnchor = false;

        hand.joint.connectedAnchor = hand.wallCollider.ClosestPoint(hand.climbHand.transform.position);

        hand.joint.minDistance = 0f;
        hand.joint.maxDistance = 0f;

        hand.joint.spring = 1000f;
        hand.joint.damper = 500f;
    }

    private void StopClimb(SwingHand hand) 
    {
        Object.Destroy(hand.joint);

        if (hand == _firstHand) _firstHand = null;
        if (hand == _secondHand) _secondHand = null;
    }

    private void VaultOver(SwingHand hand)
    {
        Debug.Log("Function Called");

        StopClimb(hand);

        _manager.playerRb.AddForce(Vector3.up * 20f, ForceMode.VelocityChange);

        if (_firstHand == null && _secondHand == null) _manager.SwitchState(_manager.IdleState);
    }

    private void HandleVaulting(MovementStateMachineManager state)
    {
        if (_firstHand == null || _secondHand == null) return; 

        float firstHandToRoofDistance = _firstHand.wallCollider.bounds.max.y - _firstHand.wallCollider.ClosestPoint(_firstHand.climbHand.transform.position).y;
        float secondHandToRoofDistance = _secondHand.wallCollider.bounds.max.y - _secondHand.wallCollider.ClosestPoint(_secondHand.climbHand.transform.position).y;   

        if (_firstHand.wallCollider != _secondHand.wallCollider) return;

        float wallPeak = _firstHand.wallCollider.bounds.max.y;
        float wallBottom = _firstHand.wallCollider.bounds.min.y;
        float leftHandPosition = Mathf.InverseLerp(wallBottom, wallPeak, _firstHand.climbHand.GetComponentInChildren<Collider>().bounds.min.y);
        float rightHandPosition = Mathf.InverseLerp(wallBottom, wallPeak, _secondHand.climbHand.GetComponentInChildren<Collider>().bounds.min.y);

        if (leftHandPosition > 0.94f && rightHandPosition > 0.94f) 
        {
            _firstHand.trackerY.TrackGesture(_firstHand.controllerPosition, v => v.y, 0.4f, _firstHand, VaultOver);        
            _secondHand.trackerY.TrackGesture(_secondHand.controllerPosition, v => v.y, 0.4f, _secondHand, VaultOver);
        }
    }

    private void HandleSecondTiggerPressed(MovementStateMachineManager state, SwingHand hand)
    {
        if (!hand.onWall)
        {
            if (_firstHand != null) 
            {
                StopClimb(_firstHand);
            }
            else
            {
                StopClimb(_secondHand);
            }

            state.pendingHand = hand;
            hand.isSwinging = true;
            state.SwitchState(state.SwingingState);
            return;
        }

        if (_firstHand == null)
        {
            _firstHand = hand;
            _firstHand.FindWallAnchor(_firstHand, StartClimb);
        }
        else
        {
            _secondHand = hand;  
            _secondHand.FindWallAnchor(_secondHand, StartClimb);          
        }
    }

    // Transition Logic
    private void HandleTriggerStop(MovementStateMachineManager state, SwingHand hand)
    {
        StopClimb(hand);

        if (_firstHand == null && _secondHand == null && state.isGrounded) state.SwitchState(state.IdleState);

        if (_firstHand == null && _secondHand == null && state.isInAir) state.SwitchState(state.AirState);
    }
}
