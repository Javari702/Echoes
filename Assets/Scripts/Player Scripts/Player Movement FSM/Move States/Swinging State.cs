using UnityEngine;

public class SwingingState : MovementAbstractState
{
    private MovementStateMachineManager manager;
    private SwingHand _firstHand;
    private SwingHand _secondHand;

    public override void EnterState(MovementStateMachineManager state)
    {
        manager = state;
        Debug.Log("Entering Swinging State");

        StartSwing(state, state.pendingHand);
        _firstHand = state.pendingHand;

        state.OnTriggerStart += HandleSecondTriggerPessed;
        state.OnTriggerStop += HandleSwingReleased;
    }

    public override void ExitState(MovementStateMachineManager state)
    {
        Debug.Log("Exiting Swinging State");

        state.OnTriggerStart -= HandleSecondTriggerPessed;
        state.OnTriggerStop -= HandleSwingReleased;
    }

    public override void UpdateState(MovementStateMachineManager state)
    {
        if (_firstHand != null) _firstHand.DrawWeb();
        if (_secondHand != null) _secondHand.DrawWeb();
    }

    public override void FixedUpdate(MovementStateMachineManager state)
    {
        HandleControllerInput(state);
    }

    public override void OnCollisionEnter(MovementStateMachineManager state)
    {
        
    }

    private void StartSwing(MovementStateMachineManager state, SwingHand hand)
    {
        if (!hand.hasHit) return;

        hand.joint = state.playerRb.gameObject.AddComponent<SpringJoint>();
        hand.joint.autoConfigureConnectedAnchor = false;
        hand.joint.connectedAnchor = hand.anchorPoint;

        float maxDistance = Vector3.Distance(state.playerRb.position, hand.anchorPoint);
        hand.joint.minDistance = 0;
        hand.joint.maxDistance = maxDistance * 0.6f;

        hand.joint.spring = hand.springStrength;
        hand.joint.damper = hand.jointDamper;
        hand.joint.massScale = hand.jointMassScale;
    }

    private void StopSwing(SwingHand hand)
    {
        hand.isSwinging = false;
        if (hand.joint != null) Object.Destroy(hand.joint);

        hand.DrawWebStop();

        if (hand == _firstHand) _firstHand = null;
        if (hand == _secondHand) _secondHand = null;
    }

    private void HandleControllerInput(MovementStateMachineManager state)
    {
        if (_firstHand != null) 
        {
            _firstHand.trackerZ.TrackGesture(_firstHand.controllerPosition, v => v.z, state.pullThreshold, _firstHand, WebYankForward);
            _firstHand.trackerY.TrackGesture(_firstHand.controllerPosition, v => v.y, state.pullThreshold, _firstHand, WebYankUp);        
        }

        if (_secondHand != null) 
        {
            _secondHand.trackerZ.TrackGesture(_secondHand.controllerPosition, v => v.z, state.pullThreshold, _secondHand, WebYankForward);
            _secondHand.trackerY.TrackGesture(_secondHand.controllerPosition, v => v.y, state.pullThreshold, _secondHand, WebYankUp);
        }
    }

    private void WebYankForward(SwingHand hand)
    {
        if (!hand.joint) return;

        Vector3 direction = (hand.anchorPoint - manager.playerRb.position).normalized;

        StopSwing(hand); 

        manager.playerRb.linearVelocity = Vector3.zero;
        manager.playerRb.AddForce(direction * hand.yankForce, ForceMode.VelocityChange);
    }

    private void WebYankUp(SwingHand hand)
    {
        if (!hand.joint) return;

        StopSwing(hand); 

        manager.playerRb.linearVelocity = Vector3.zero;
        manager.playerRb.AddForce(Vector3.up * hand.yankForce, ForceMode.VelocityChange);
    }

    private void HandleSecondTriggerPessed(MovementStateMachineManager state, SwingHand hand)
    {
        if (hand.onWall)
        {
            if (_firstHand != null) 
            {
                StopSwing(_firstHand);
            }
            else
            {
                StopSwing(_secondHand);
            }

            state.pendingHand = hand;
            hand.isSwinging = false;
            state.SwitchState(state.ClimbingState);
            return;
        }

        if (_firstHand == null)
        {
            _firstHand = hand;
            StartSwing(state, _firstHand);
        }
        else
        {
            _secondHand = hand;  
            StartSwing(state, _secondHand);
        }
    }

    // Transition Logic 
    private void HandleSwingReleased(MovementStateMachineManager state, SwingHand hand)
    {
        StopSwing(hand);

        if (_firstHand == null && _secondHand == null && state.isInAir) state.SwitchState(state.AirState);

        if (_firstHand == null && _secondHand == null && state.isGrounded) state.SwitchState(state.IdleState);
    }


}