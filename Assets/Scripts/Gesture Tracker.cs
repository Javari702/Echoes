using UnityEngine;
using System;

public class GestureTracker
{
    private bool _gestureStarted;
    private float _pullDistance;
    private Vector3 _gestureStartPosition;
    private Vector3 _lastPosition;

    public void TrackGesture(GestureTracker tracker, Vector3 controllerPosition, Func<Vector3, float> axisSelector, float pullThreshold, SwingHand hand, Action<SwingHand> PullWeb)
    {
        Vector3 currentPosition = controllerPosition;
        Vector3 positionDelta = currentPosition - _lastPosition;

        if (!_gestureStarted && axisSelector(positionDelta) < -0.01f)
        {
            _gestureStarted = true;
            _gestureStartPosition = currentPosition;
        }


        if (_gestureStarted)
        {
            _pullDistance = axisSelector(_gestureStartPosition) - axisSelector(currentPosition);

            if (_pullDistance > pullThreshold)
            {
                PullWeb?.Invoke(hand);
                _gestureStarted = false;
                _lastPosition = Vector3.zero;
                return;
            }
            
            if (axisSelector(positionDelta) > 0.01f)
            {
                _gestureStarted = false;
                _pullDistance = 0f;
            }
        }

        _lastPosition = currentPosition;
    }
}
