using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHandsInput : MonoBehaviour
{
    [SerializeField] InputActionProperty _triggerValue;
    [SerializeField] InputActionProperty _gripValue; 
    [SerializeField] Animator handAnimator;

    void Update()
    {
        HandleAnimation();
    }

    private void HandleAnimation()
    {
        float trigger = _triggerValue.action.ReadValue<float>();
        float grip = _gripValue.action.ReadValue<float>();

        handAnimator.SetFloat("Trigger", trigger);
        handAnimator.SetFloat("Grip", grip);
    }
}
