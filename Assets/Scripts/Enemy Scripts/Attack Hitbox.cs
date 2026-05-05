using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField] float damage;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Collider");
        if (other.CompareTag("Player"))
        {
            Debug.Log("Found Player");
            other.GetComponent<HealthBarController>().TakeDamage(damage);
        }
    }
}

