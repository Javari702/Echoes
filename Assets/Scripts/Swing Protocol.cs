using Oculus.Platform;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwingProtocol : MonoBehaviour
{
    [SerializeField] Transform swingHand;
    [SerializeField] Rigidbody _playerRb;
    [SerializeField] float maxWebShootDistance;
    [SerializeField] float jointSpring;
    [SerializeField] float jointDamper;
    [SerializeField] float jointMassScale;
    [SerializeField] Transform swingPointVisual;
    [SerializeField] InputActionProperty swingAction;
    [SerializeField] InputActionProperty pullAction;
    [SerializeField] float pullStrength;
    [SerializeField] LineRenderer line; 

    private Vector3 swingPoint; 
    private SpringJoint joint;
    private bool hasHit;


    void Update()
    {
        ShowSwingPoint();

        HandleSwing();

        DrawWeb();

        PullWeb();
    }

    private void HandleSwing()
    {
        if (swingAction.action.WasPressedThisFrame()) StartSwing();

        if (swingAction.action.WasReleasedThisFrame()) StopSwing();
    }

    private void ShowSwingPoint()
    {
        swingPointVisual.gameObject.SetActive(false);

        if (joint) {
            swingPointVisual.gameObject.SetActive(false);
            return;
        }

        RaycastHit hit;
        hasHit = Physics.Raycast(swingHand.position, swingHand.forward, out hit, maxWebShootDistance, LayerMask.GetMask("Default"));

        if (hasHit)
        {
            swingPoint = hit.point;
            swingPointVisual.gameObject.SetActive(true);
            swingPointVisual.position = swingPoint;
        }
    }

    private void StartSwing()
    {
        if (!hasHit) return;

        joint = _playerRb.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = swingPoint;

        float maxDistance = Vector3.Distance(_playerRb.position, swingPoint);
        joint.minDistance = 0;
        joint.maxDistance = maxDistance * 0.8f;

        joint.spring = jointSpring;
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
        line.SetPosition(1, swingPoint);
    }

    private void PullWeb()
    {
        if (pullAction.action.IsPressed())
        {
            if (!joint) return;

            Vector3 direction = (swingPoint - swingHand.position).normalized;
            _playerRb.AddForce(direction * pullStrength * Time.deltaTime);

            float maxDistance = Vector3.Distance(_playerRb.position, swingPoint);
            joint.minDistance = 0;
            joint.maxDistance = maxDistance * 0.8f;
        }
    }
}
