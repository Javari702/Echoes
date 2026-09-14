using UnityEngine;
using UnityEngine.InputSystem;


public class WebPull : MonoBehaviour
{
    [SerializeField] InputActionProperty pullWebAction;
    [SerializeField] float pullThreshold;
    [SerializeField] float pullForce;
    
    private Rigidbody playerRb;
    private float pullDistance;
    private bool gestureStarted;
    private Vector3 lastPosition;
    private Vector3 gestureStartPosition;

    void Awake()
    {
        playerRb = GetComponentInParent<Rigidbody>();
    }

    void Update()
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

    private void WebYankForward()
    {
        // playerRb.linearVelocity = Vector3.zero;
        // Vector3 direction = (SwingProtocol.anchorPoint - playerRb.position).normalized;
        // playerRb.AddForce(direction * pullForce, ForceMode.VelocityChange);
    }
}

/*

*/



