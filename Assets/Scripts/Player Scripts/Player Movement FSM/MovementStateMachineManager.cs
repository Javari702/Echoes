using System;
using System.Collections;
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
    public float groundSpeed;
    public float airSpeed;
    public float vaultForce;

    // Condition Flags
    public bool isGrounded;
    public bool isInAir;

    // Private Global Variables
    private bool _wasMoving;

    // States
    public MovementAbstractState CurrentState;
    public IdleState IdleState = new IdleState();
    public GroundMovementState GroundMovementState = new GroundMovementState();
    public SwingingState SwingingState = new SwingingState();
    public ClimbingState ClimbingState = new ClimbingState();
    public AirState AirState = new AirState();

    // Context Delegates
    public event Action<MovementStateMachineManager> OnMovementStart;
    public event Action<MovementStateMachineManager> OnMovementStop; 
    public event Action<MovementStateMachineManager, SwingHand> OnTriggerStart;
    public event Action<MovementStateMachineManager, SwingHand> OnTriggerStop;

    // Input Events
    private Action<InputAction.CallbackContext> _leftControllerMovement;
    private Action<InputAction.CallbackContext> _rightControllerMovement;

    private Action<InputAction.CallbackContext> _leftTriggerInput;
    private Action<InputAction.CallbackContext> _rightTriggerInput;

    void OnEnable()
    {
        moveInput.action.performed += OnMovePerfomred;
        moveInput.action.canceled += OnMoveCanceled;

        _leftControllerMovement = ctx => OnControllerMovement(ctx, leftHand);
        _rightControllerMovement = ctx => OnControllerMovement(ctx, rightHand);
        leftHand.controllerDelta.action.performed += _leftControllerMovement;
        rightHand.controllerDelta.action.performed += _rightControllerMovement;

        _leftTriggerInput = ctx => OnTriggerPerformed(ctx, leftHand);
        _rightTriggerInput = ctx => OnTriggerPerformed(ctx, rightHand);
        leftHand.triggerInput.action.performed += _leftTriggerInput;
        rightHand.triggerInput.action.performed += _rightTriggerInput;
        leftHand.triggerInput.action.canceled += _leftTriggerInput;
        rightHand.triggerInput.action.canceled  += _rightTriggerInput;
    } 
    void OnDisable()
    {
        moveInput.action.performed -= OnMovePerfomred;
        moveInput.action.canceled -= OnMoveCanceled;

        leftHand.controllerDelta.action.performed -= _leftControllerMovement;
        rightHand.controllerDelta.action.performed -= _rightControllerMovement;

        leftHand.triggerInput.action.performed -= _leftTriggerInput;
        rightHand.triggerInput.action.performed -= _rightTriggerInput;
        leftHand.triggerInput.action.canceled -= _leftTriggerInput;
        rightHand.triggerInput.action.canceled  -= _rightTriggerInput;
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

        leftHand.ShowAnchorPoint();
        rightHand.ShowAnchorPoint();
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
        IsMoving();

        IsGround();

        IsAir(leftHand, rightHand);

        leftHand.ShowAnchorPoint();
        rightHand.ShowAnchorPoint();

        leftHand.OnWall();
        rightHand.OnWall();
    }

    private void IsMoving()
    {
        bool isMoving = moveInputValue.magnitude > 0.01f; 

        if (_wasMoving && !isMoving) OnMovementStop?.Invoke(this);
        if (!_wasMoving && isMoving) OnMovementStart?.Invoke(this);

        _wasMoving = isMoving;
    }

    private void IsGround()
    {
        Vector3 position = playerRb.transform.position;
        float radius = 0.5f;
        
        isGrounded = Physics.CheckSphere(position, radius);
    }

    private void IsAir(SwingHand handOne, SwingHand handTwo)
    {
        isInAir = !isGrounded && !handOne.onWall && !handTwo.onWall && !handOne.isSwinging && !handTwo.isSwinging; 
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

    private void OnTriggerPerformed(InputAction.CallbackContext input, SwingHand hand)
    {
        if (input.ReadValueAsButton()) 
        {
            OnTriggerStart?.Invoke(this, hand);
        }
        else
        {
            OnTriggerStop?.Invoke(this, hand);
        }    
    }

    private void OnControllerMovement(InputAction.CallbackContext input, SwingHand hand)
    {
        hand.controllerPosition = input.ReadValue<Vector3>();
    }
}