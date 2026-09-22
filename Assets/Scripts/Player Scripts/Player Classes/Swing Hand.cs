using System;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class SwingHand
{
    // Controller Inputs 
    public InputActionProperty triggerInput;
    public InputActionProperty controllerDelta;

    // Climbing Components
    public GameObject climbHand;
    public Collider wallCollider;
    public bool onWall;

    // Swing Components
    public Transform hand;
    public Transform swingPointVisual;
    public LineRenderer line;
    public SpringJoint joint;
    public bool hasHit;
    public RaycastHit hit;
    public Vector3 anchorPoint;
    public float maxWebShootDistance;
    public float pointOffset;
    public float springStrength;
    public float jointDamper;
    public float jointMassScale;

    // Tracking Controller Position Components
    public GestureTracker trackerZ = new GestureTracker();
    public GestureTracker trackerY = new GestureTracker();
    public Vector3 controllerPosition;
    public float yankForce;

    // Web Visual Functions
    public void ShowAnchorPoint()
    {
        swingPointVisual.gameObject.SetActive(false);

        if (onWall)
        {
            swingPointVisual.gameObject.SetActive(false);
            return;
        }

        if (joint)
        {
            swingPointVisual.gameObject.SetActive(false);
            return;
        }

        hasHit = Physics.Raycast(
            hand.position,
            hand.forward,
            out hit,
            maxWebShootDistance,
            LayerMask.GetMask("Default")
        );

        Vector3 anchorOffset = hit.normal * pointOffset;

        if (hasHit)
        {
            anchorPoint = hit.point + anchorOffset;
            swingPointVisual.gameObject.SetActive(true);
            swingPointVisual.position = hit.point;
        }
    }

    public void DrawWeb()
    {
        line.enabled = true;
        line.positionCount = 2;
        line.SetPosition(0, hand.position);
        line.SetPosition(1, hit.point);
    }

    public void DrawWebStop()
    {
        if (line) line.enabled = false; 
    }

    // Wall Climb Functions
    public void FindWallAnchor(SwingHand hand, Action<SwingHand> WallClimb)
    { 
        if (hand.climbHand == null) return; 

        Vector3 position = hand.climbHand.transform.position;
        float radius = 0.3f;

        Collider[] hits = Physics.OverlapSphere(position, radius, LayerMask.GetMask("Default"));

        if (hits.Length == 0) return;

        hand.wallCollider = hits[0];
        
        WallClimb?.Invoke(hand);
    }

    public void OnWall()
    {
        Vector3 position = climbHand.transform.position;
        float radius = 0.3f;
        Collider[] hits = Physics.OverlapSphere(position, radius, LayerMask.GetMask("Default"));

        onWall = hits.Length != 0;
    }
}
