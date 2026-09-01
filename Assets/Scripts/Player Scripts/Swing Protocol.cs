using UnityEngine;
using UnityEngine.InputSystem;

public class SwingProtocol : MonoBehaviour
{
    [SerializeField] Transform swingHand;
    [SerializeField] Rigidbody _playerRb;
    [SerializeField] float maxWebShootDistance;
    [SerializeField] float springStrength;
    [SerializeField] float jointDamper;
    [SerializeField] float jointMassScale;
    [SerializeField] float _pointOffset;
    [SerializeField] Transform swingPointVisual;
    [SerializeField] InputActionProperty swingAction;
    [SerializeField] InputActionProperty pullAction;
    [SerializeField] float pullStrength;
    [SerializeField] LineRenderer line; 

    private Vector3 anchorPoint; 
    private SpringJoint joint;
    private bool hasHit;
    private RaycastHit hit;


    void Update()
    {
        ShowAnchorPoint();

        HandleSwing();

        DrawWeb();

        PullWeb();
    }

    private void HandleSwing()
    {
        if (swingAction.action.WasPressedThisFrame()) StartSwing();

        if (swingAction.action.WasReleasedThisFrame()) StopSwing();
    }

    private void ShowAnchorPoint()
    {
        swingPointVisual.gameObject.SetActive(false);

        if (joint) {
            swingPointVisual.gameObject.SetActive(false);
            return;
        }

        hasHit = Physics.Raycast(swingHand.position, swingHand.forward, out hit, maxWebShootDistance, LayerMask.GetMask("Default"));
        Vector3 anchorOffset = hit.normal * _pointOffset;

        if (hasHit)
        {
            anchorPoint = hit.point + anchorOffset;
            swingPointVisual.gameObject.SetActive(true);
            swingPointVisual.position = hit.point;
        }
    }


    private void StartSwing()
    {
        if (!hasHit) return;

        joint = _playerRb.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = anchorPoint;

        float maxDistance = Vector3.Distance(_playerRb.position, anchorPoint);
        joint.minDistance = 0;
        joint.maxDistance = maxDistance * 0.8f;

        joint.spring = springStrength;
        joint.damper = jointDamper;
        joint.massScale = jointMassScale;
    }

    private void StopSwing()
    {
        if (joint != null) Destroy(joint);
    }

    private void DrawWeb()
    {
        if (!joint)
        {
            line.enabled = false;
            return;
        } 

        line.enabled = true;
        line.positionCount = 2;
        line.SetPosition(0, swingHand.position);
        line.SetPosition(1, hit.point);
    }

    private void PullWeb()
    {
        if (pullAction.action.IsPressed())
        {
            if (!joint) return;

            Vector3 direction = (anchorPoint - swingHand.position).normalized;
            _playerRb.AddForce(direction * pullStrength * Time.deltaTime);

            float maxDistance = Vector3.Distance(_playerRb.position, anchorPoint);
            joint.minDistance = 0;
            joint.maxDistance = maxDistance * 0.8f;
        }
    }
}
