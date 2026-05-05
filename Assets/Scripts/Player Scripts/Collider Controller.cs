using UnityEngine;

public class ColliderController : MonoBehaviour
{
    private Transform _playerHead;
    private CapsuleCollider _playerCollider;

    [SerializeField] float _bodyHeightMin;
    [SerializeField] float _bodyHeightMax;
    [SerializeField] Transform _leftHand;
    [SerializeField] Transform _rightHand;
    [SerializeField] ConfigurableJoint _headjoint;
    [SerializeField] ConfigurableJoint _leftHandJoint;
    [SerializeField] ConfigurableJoint _rightHandJoint;

    void Awake()
    {
        _playerHead = GetComponentInChildren<Camera>().transform;
        _playerCollider = GetComponentInChildren<CapsuleCollider>();
    }

    void FixedUpdate()
    {
        HandleColliderChange();
    }

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
}
