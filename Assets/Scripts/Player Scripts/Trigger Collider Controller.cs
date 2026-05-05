using UnityEngine;

public class TriggerColliderController : MonoBehaviour
{
    private Transform _playerHead;

    [SerializeField] CapsuleCollider _playerTriggerCollider;
    [SerializeField] float _bodyHeightMin;
    [SerializeField] float _bodyHeightMax;

    void Awake()
    {
        _playerHead = GetComponentInChildren<Camera>().transform;
    }

    void FixedUpdate()
    {
        HandleColliderChange();
    }

    private void HandleColliderChange()
    {
        _playerTriggerCollider.height = Mathf.Clamp(_playerHead.localPosition.y, _bodyHeightMin, _bodyHeightMax);
        _playerTriggerCollider.center = new Vector3(_playerHead.localPosition.x, _playerTriggerCollider.height / 2, _playerHead.localPosition.z);
    }
}
