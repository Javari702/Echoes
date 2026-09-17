using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class SwingHand
{
    public InputActionProperty swingWebInput;
    public InputActionProperty controllerDelta;
    public Vector3 controllerPosition;
    public Transform hand;
    public Transform swingPointVisual;
    public SpringJoint joint;
    public bool hasHit;
    public bool isSwinging;
    public RaycastHit hit;
    public Vector3 anchorPoint;
    public float maxWebShootDistance;
    public float pointOffset;
    public LineRenderer line;
    public float springStrength;
    public float jointDamper;
    public float jointMassScale;
    public float yankForce;

    public void ShowAnchorPoint(SwingHand swingHand)
    {
        swingHand.swingPointVisual.gameObject.SetActive(false);

        if (swingHand.joint)
        {
            swingHand.swingPointVisual.gameObject.SetActive(false);
            return;
        }

        swingHand.hasHit = Physics.Raycast(
            swingHand.hand.position,
            swingHand.hand.forward,
            out swingHand.hit,
            maxWebShootDistance,
            LayerMask.GetMask("Default")
        );

        Vector3 anchorOffset = swingHand.hit.normal * pointOffset;

        if (swingHand.hasHit)
        {
            swingHand.anchorPoint = swingHand.hit.point + anchorOffset;
            swingHand.swingPointVisual.gameObject.SetActive(true);
            swingHand.swingPointVisual.position = swingHand.hit.point;
        }
    }

    public void DrawWeb(SwingHand swingHand)
    {
        line.enabled = true;
        line.positionCount = 2;
        line.SetPosition(0, swingHand.hand.position);
        line.SetPosition(1, hit.point);
    }

    public void DrawWebStop()
    {
        if (line) line.enabled = false; 
    }
}
