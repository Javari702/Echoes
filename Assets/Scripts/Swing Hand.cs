using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class SwingHand
{
    public InputActionProperty swingWebInput;
    public Transform hand;
    public Transform swingPointVisual;
    public SpringJoint joint;
    public bool hasHit;
    public RaycastHit hit;
    public Vector3 anchorPoint;
    public float maxWebShootDistance;
    public float pointOffset;

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
}
