using UnityEngine;

public class SwingingState : MovementAbstractState
{
    private MovementStateMachineManager manager;
    private SwingHand _firstHand;
    private SwingHand _secondHand;
    private GestureTracker tracker = new GestureTracker();

    public override void EnterState(MovementStateMachineManager state)
    {
        manager = state;
        Debug.Log("Entering Swinging State");

        StartSwing(state, state.pendingHand);
        _firstHand = state.pendingHand;

        state.OnSwingWebStart += HandleSecondHand;
        state.OnSwingWebStop += HandleSwingReleased;
    }

    public override void ExitState(MovementStateMachineManager state)
    {
        Debug.Log("Exiting Swinging State");

        state.OnSwingWebStart -= HandleSecondHand;
        state.OnSwingWebStop -= HandleSwingReleased;
    }

    public override void UpdateState(MovementStateMachineManager state)
    {

        if (_firstHand != null) _firstHand.DrawWeb(_firstHand);
        if (_secondHand != null) _secondHand.DrawWeb(_secondHand);

        if (_firstHand != null) {
            tracker.TrackGesture(tracker, _firstHand.controllerPosition, v => v.z, state.pullThreshold, _firstHand, WebYankForward);
            tracker.TrackGesture(tracker, _firstHand.controllerPosition, v => v.y, state.pullThreshold, _firstHand, WebYankUp);
        }
 
        if (_secondHand != null) {
            tracker.TrackGesture(tracker, _secondHand.controllerPosition, v => v.z, state.pullThreshold, _secondHand, WebYankForward);
            tracker.TrackGesture(tracker, _secondHand.controllerPosition, v => v.y, state.pullThreshold, _secondHand, WebYankUp);
        }
    }

    public override void FixedUpdate(MovementStateMachineManager state)
    {
        
    }

    public override void OnCollisionEnter(MovementStateMachineManager state)
    {
        
    }

    private void HandleSecondHand(MovementStateMachineManager state, SwingHand hand)
    {
        _secondHand = hand;
        StartSwing(state, hand);
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
        if (hand.joint != null) Object.Destroy(hand.joint);

        hand.DrawWebStop();

        if (hand == _firstHand) _firstHand = null;
        if (hand == _secondHand) _secondHand = null;
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


    // Transition Logic 
    private void HandleSwingReleased(MovementStateMachineManager state, SwingHand hand)
    {
        StopSwing(hand);

        if (_firstHand == null && _secondHand == null) state.SwitchState(state.IdleState);
    }
}


