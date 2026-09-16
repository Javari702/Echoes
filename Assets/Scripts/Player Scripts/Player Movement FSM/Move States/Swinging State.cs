using Oculus.Interaction.Input;
using Unity.VisualScripting;
using UnityEngine;

public class SwingingState : MovementAbstractState
{
    private SwingHand _firstHand;
    private SwingHand _secondHand;

    public override void EnterState(MovementStateMachineManager state)
    {
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
        if (_firstHand != null) _firstHand.DrawWeb();
        if (_secondHand != null) _secondHand.DrawWeb();
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
    }

    // Transition Logic 
    private void HandleSwingReleased(MovementStateMachineManager state, SwingHand hand)
    {
        StopSwing(hand);

        if (hand == _firstHand) _firstHand = null;
        if (hand == _secondHand) _secondHand = null;

        if (_firstHand == null && _secondHand == null) state.SwitchState(state.IdleState);
    }
}


