using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float sensitivity;
    // [SerializeField] Transform playerHead;
    [SerializeField] InputActionProperty moveInput;
    [SerializeField] InputActionProperty lookInput;

    private Rigidbody _playerRb;
    private Transform lookDirection;
    private Vector2 recievedMoveInput;
    private float recievedLookInput;
    private CapsuleCollider _playerCollider;

    void Awake()
    {
        _playerRb = GetComponentInChildren<Rigidbody>();
        lookDirection = GetComponentInChildren<Camera>().transform;
        _playerCollider = GetComponentInChildren<CapsuleCollider>();
    }

    void Update()
    {
        recievedMoveInput = moveInput.action.ReadValue<Vector2>();
        recievedLookInput = lookInput.action.ReadValue<Vector2>().x;
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        // if (isGrounded())
        // {
            Quaternion yaw = Quaternion.Euler(0, lookDirection.eulerAngles.y, 0);
            Vector3 targetDirection = yaw * new Vector3(recievedMoveInput.x, _playerRb.linearVelocity.y, recievedMoveInput.y);

            Vector3 moveDirection = _playerRb.position + targetDirection * Time.fixedDeltaTime * speed;

            Vector3 axis = Vector3.up;
            float angle = sensitivity * Time.fixedDeltaTime * recievedLookInput;

            Quaternion targetTurn = Quaternion.AngleAxis(angle, axis);

            _playerRb.MoveRotation(_playerRb.rotation * targetTurn);

            Vector3 newPositon = targetTurn * (moveDirection - lookDirection.position) + lookDirection.position;

            _playerRb.MovePosition(newPositon);
        // }
    }

    private bool isGrounded()
    {
        RaycastHit hitInfo;
        Vector3 origin = _playerCollider.transform.TransformPoint(_playerCollider.center);
        float radius = _playerCollider.radius;
        float maxDistance = _playerCollider.height / 2 - _playerCollider.radius + 0.5f;

        return Physics.SphereCast(origin, radius, Vector3.down, out hitInfo, maxDistance, LayerMask.GetMask("Ground"));
    }
}
