using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLookControls : MonoBehaviour
{
    [SerializeField] InputActionProperty lookInput;
    [SerializeField] float sensitivity;

    private Rigidbody _playerRb;
    private float lookInputValue;

    void OnEnable()
    {
        lookInput.action.performed += OnLookStart;
        lookInput.action.canceled += OnLookStop;  
    } 
    
    void OnDisable()
    {
        lookInput.action.performed -= OnLookStart;
        lookInput.action.canceled -= OnLookStop;  
    } 

    void Awake()
    {
        _playerRb = GetComponentInChildren<Rigidbody>();
    }

    void Update()
    {
        HandleLooking();
    }

    void FixedUpdate()
    {
        
    }

    private void HandleLooking()
    {
        Vector3 axis = Vector3.up;
        float angle = sensitivity * Time.fixedDeltaTime * lookInputValue;

        Quaternion targetTurn = Quaternion.AngleAxis(angle, axis);

        _playerRb.MoveRotation(_playerRb.rotation * targetTurn);
    }

    public void OnLookStart(InputAction.CallbackContext input)
    {
        lookInputValue = input.ReadValue<Vector2>().x;
    }

    private void OnLookStop(InputAction.CallbackContext input)
    {
        lookInputValue = 0f;
    }
}
