using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementStateMachineManager : MonoBehaviour
{
    // Input References
    [SerializeField] InputActionProperty moveInput;

    // Swinging Componenets
    public SwingHand leftHand;
    public SwingHand rightHand;
    public SwingHand pendingHand;
    public float pullThreshold;
    

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

    // Shared Components
    public Rigidbody playerRb;
    public Transform lookDirection;
    public Vector2 moveInputValue;

    // Multipliers
    public float speed;

    // Private Global Variables
    private bool _wasMoving;

    // States
    public MovementAbstractState CurrentState;
    public IdleState IdleState = new IdleState();
    public GroundMovementState GroundMovementState = new GroundMovementState();
    public SwingingState SwingingState = new SwingingState();

    // Context Delegates
    public event Action<MovementStateMachineManager> OnMovementStart;
    public event Action<MovementStateMachineManager> OnMovementStop; 
    public event Action<MovementStateMachineManager, SwingHand> OnSwingWebStart;
    public event Action<MovementStateMachineManager, SwingHand> OnSwingWebStop;

    // Input Events
    void OnEnable()
    {
        moveInput.action.performed += OnMovePerfomred;
        moveInput.action.canceled += OnMoveCanceled;

        leftHand.swingWebInput.action.performed += ctx => OnSwingPerformed(ctx, leftHand);
        rightHand.swingWebInput.action.performed += ctx => OnSwingPerformed(ctx, rightHand);
        leftHand.swingWebInput.action.canceled += ctx => OnSwingPerformed(ctx, leftHand);
        rightHand.swingWebInput.action.canceled  += ctx => OnSwingPerformed(ctx, rightHand);
        
        leftHand.controllerDelta.action.performed += ctx => OnMovementDetected(ctx, leftHand);
        rightHand.controllerDelta.action.performed += ctx => OnMovementDetected(ctx, rightHand);
    } 
    void OnDisable()
    {
        moveInput.action.performed -= OnMovePerfomred;
        moveInput.action.canceled -= OnMoveCanceled;

        leftHand.swingWebInput.action.performed -= ctx => OnSwingPerformed(ctx, leftHand);
        rightHand.swingWebInput.action.performed -= ctx => OnSwingPerformed(ctx, rightHand);
        leftHand.swingWebInput.action.canceled -= ctx => OnSwingPerformed(ctx, leftHand);
        rightHand.swingWebInput.action.canceled  -= ctx => OnSwingPerformed(ctx, rightHand);

        leftHand.controllerDelta.action.performed -= ctx => OnMovementDetected(ctx, leftHand);
        rightHand.controllerDelta.action.performed -= ctx => OnMovementDetected(ctx, rightHand);
    } 

    // Unity Event Functions
    void Awake()
    {
        _playerHead = GetComponentInChildren<Camera>().transform;
        _playerCollider = GetComponentInChildren<CapsuleCollider>();
        lookDirection = GetComponentInChildren<Camera>().transform;
        _playerCollider = GetComponentInChildren<CapsuleCollider>();
        playerRb = GetComponentInChildren<Rigidbody>();
    }

    void Start()
    {
        CurrentState = IdleState;

        CurrentState.EnterState(this);
    }

    void Update()
    {
        CurrentState.UpdateState(this);

        ConditonUpdates();

        leftHand.ShowAnchorPoint(leftHand);
        rightHand.ShowAnchorPoint(rightHand);
    }

    void FixedUpdate()
    {
        CurrentState.FixedUpdate(this);  

        HandleColliderChange();
    }
    
    // FSM Functions
    public void SwitchState(MovementAbstractState state)
    {
        CurrentState.ExitState(this);
        CurrentState = state;
        state.EnterState(this);
    }

    private void ConditonUpdates()
    {
        bool isMoving = moveInputValue.magnitude > 0.01f; 

        if (_wasMoving && !isMoving) OnMovementStop?.Invoke(this);
        if (!_wasMoving && isMoving) OnMovementStart?.Invoke(this);

        _wasMoving = isMoving;
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
    private void OnMovePerfomred(InputAction.CallbackContext input)
    {
        moveInputValue = input.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext input)
    {
        moveInputValue = Vector2.zero;
    } 

    private void OnSwingPerformed(InputAction.CallbackContext input, SwingHand hand)
    {
        if (input.ReadValueAsButton()) {
            hand.isSwinging = true;
            OnSwingWebStart?.Invoke(this, hand);
        }
        else
        {
            hand.isSwinging = false;
            OnSwingWebStop?.Invoke(this, hand);
        }    
    }

    private void OnMovementDetected(InputAction.CallbackContext input, SwingHand hand)
    {
        hand.controllerPosition = input.ReadValue<Vector3>();
    }
}