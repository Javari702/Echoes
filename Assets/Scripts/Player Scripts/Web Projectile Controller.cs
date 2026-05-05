using UnityEngine;

public class WebProjectileController : MonoBehaviour
{
    [SerializeField] float webSpeed;

    private Rigidbody webRb;

    void Awake()
    {
        webRb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        webRb.AddForce(transform.forward * webSpeed, ForceMode.Impulse);
        Destroy(gameObject, 3f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyHealth>().TakeDamage(25);
            Destroy(gameObject);
        }

    }
}
