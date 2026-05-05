using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class WebShooter : MonoBehaviour
{
    [SerializeField] GameObject web;
    [SerializeField] float cooldownTimer;
    [SerializeField] InputActionProperty shootAction;

    private float cooldown;

    void Start()
    {
        cooldown = cooldownTimer;
    }

    void Update()
    {
        ShootWeb();
    }

    private void ShootWeb()
    {
        cooldown -= Time.deltaTime;

        if (shootAction.action.WasPressedThisFrame() && cooldown <= 0f)
        {
            Instantiate(web, transform.position, transform.rotation);
            cooldown = cooldownTimer;
        }
    }
}
