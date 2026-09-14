using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementStateMachineManager : MonoBehaviour
{
    // Input References
    [SerializeField] InputActionProperty moveInput;
    [SerializeField] InputActionProperty lookInput;


    // Force Multipliers
    [SerializeField] float speed;
    [SerializeField] float sensitivity;

    // Shared Components
    private Rigidbody _playerRb;
    private Transform lookDirection;
    private Vector2 moveInputValue;
    private float lookInputValue;

    // VR Rig Components
    [SerializeField] float _bodyHeightMin;
    [SerializeField] float _bodyHeightMax;
    [SerializeField] Transform _leftHand;
    [SerializeField] Transform _rightHand;
    [SerializeField] ConfigurableJoint _headjoint;
    [SerializeField] ConfigurableJoint _leftHandJoint;
    [SerializeField] ConfigurableJoint _rightHandJoint;
    private Transform _playerHead;
    private CapsuleCollider _playerCollider;

    // States
    public MovementAbstractState currentState;
    public IdleState IdleState = new IdleState();
    public GroundMovementState GroundMovementState = new GroundMovementState();
    public SwingingState SwingingState = new SwingingState();

    // Context Delegates
    public event Action<MovementStateMachineManager> OnSwingWebShot;

    // Input Events
    void OnEnable()
    {
        moveInput.action.performed += OnMove;
        lookInput.action.performed += OnLook;
    } 
    void OnDisable()
    {
        moveInput.action.performed -= OnMove;
        lookInput.action.performed -= OnLook;
    } 

    void Awake()
    {
        _playerHead = GetComponentInChildren<Camera>().transform;
        _playerCollider = GetComponentInChildren<CapsuleCollider>();
    }

    void Start()
    {
        currentState = IdleState;

        currentState.EnterState(this);
    }

    void Update()
    {
        currentState.UpdateState(this);
    }

    void FixedUpdate()
    {
        currentState.FixedUpdate(this);  

        HandleColliderChange();
    }
    
    public void SwitchState(MovementAbstractState state)
    {
        state.ExitState(this);
        currentState = state;
        state.EnterState(this);
    }

    // VR Rig Functions
    private void HandleColliderChange()
    {
        _playerCollider.height = Mathf.Clamp(_playerHead.localPosition.y, _bodyHeightMin, _bodyHeightMax);
        _playerCollider.center = new Vector3(_playerHead.localPosition.x, _playerCollider.height / 2, _playerHead.localPosition.z);

        _leftHandJoint.targetPosition = _leftHand.localPosition;
        _leftHandJoint.targetRotation = _leftHand.localRotation;

        _rightHandJoint.targetPosition = _rightHand.localPosition;
        _rightHandJoint.targetRotation = _rightHand.localRotation;

        _headjoint.targetPosition = _playerHead.localPosition;
    }

    // Unity Input Callback
    public void OnMove(InputAction.CallbackContext input)
    {
        moveInputValue = input.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext input)
    {
        lookInputValue = input.ReadValue<Vector2>().x;
    }
}