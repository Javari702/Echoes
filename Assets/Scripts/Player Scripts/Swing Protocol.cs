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
    [SerializeField] InputActionProperty pullWebAction;
    [SerializeField] float pullThreshold;
    [SerializeField] float pullForce;
    [SerializeField] float pullUpForce;
    
    private float pullDistance;
    private float pullDistanceY;
    private bool gestureStarted;
    private bool gestureStartedY;
    private Vector3 lastPosition;
    private Vector3 lastPositionY;
    private Vector3 gestureStartPosition; 
    private Vector3 gestureStartPositionY;
    private Vector3 anchorPoint; 
    private SpringJoint joint;
    private bool hasHit;
    private RaycastHit hit;

    void Update()
    {
        ShowAnchorPoint();

        HandleSwing();

        DrawWeb();

        TrackGesture();

        TrackGestureY();

        // PullWeb();
    }

    private void HandleSwing()
    {
        if (swingAction.action.WasPressedThisFrame()) { 
            StartSwing();
        }

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
        joint.maxDistance = maxDistance * 0.6f;

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

    private void WebYankForward()
    {
        if (!joint) return;

        Vector3 direction = (anchorPoint - _playerRb.position).normalized;

        StopSwing(); 

        _playerRb.linearVelocity = Vector3.zero;
        _playerRb.AddForce(direction * pullForce, ForceMode.VelocityChange);
    }

    private void WebYankUp()
    {
        if (!joint) return;

        StopSwing(); 

        _playerRb.linearVelocity = Vector3.zero;
        _playerRb.AddForce(Vector3.up * pullUpForce, ForceMode.VelocityChange);
    }

    // private void PullWeb()
    // {
    //     if (pullAction.action.IsPressed())
    //     {
    //         if (!joint) return;

    //         Vector3 direction = (anchorPoint - swingHand.position).normalized;
    //         _playerRb.AddForce(direction * pullStrength * Time.deltaTime);

    //         float maxDistance = Vector3.Distance(_playerRb.position, anchorPoint);
    //         joint.minDistance = 0;
    //         joint.maxDistance = maxDistance * 0.8f;
    //     }
    // }

    private void TrackGesture()
    {
        Vector3 currentPosition = pullWebAction.action.ReadValue<Vector3>();
        Vector3 positionDelta = currentPosition - lastPosition;

        if (!gestureStarted && positionDelta.z < -0.01f)
        {
            gestureStarted = true;
            gestureStartPosition = currentPosition;
        }


        if (gestureStarted)
        {
            pullDistance = gestureStartPosition.z - currentPosition.z;

            if (pullDistance > pullThreshold)
            {
                WebYankForward();
                gestureStarted = false;
                lastPosition = Vector3.zero;
                return;
            }
            
            if (positionDelta.z > 0.01f)
            {
                gestureStarted = false;
                pullDistance = 0f;
            }
        }

        lastPosition = currentPosition;
    }

    private void TrackGestureY()
    {
        Vector3 currentPosition = pullWebAction.action.ReadValue<Vector3>();
        Vector3 positionDelta = currentPosition - lastPositionY;

        if (!gestureStartedY && positionDelta.y < -0.01f)
        {
            gestureStartedY = true;
            gestureStartPositionY = currentPosition;
        }


        if (gestureStartedY)
        {
            pullDistanceY = gestureStartPositionY.y - currentPosition.y;

            if (pullDistanceY > pullThreshold)
            {
                WebYankUp();
                gestureStartedY = false;
                lastPositionY = Vector3.zero;
                return;
            }
            
            if (positionDelta.y > 0.01f)
            {
                gestureStartedY = false;
                pullDistanceY = 0f;
            }
        }

        lastPositionY = currentPosition;
    }
}
