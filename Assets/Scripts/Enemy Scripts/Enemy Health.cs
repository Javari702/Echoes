using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float health;

    void Update()
    {
        Kill();
    }

    private void Kill()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage)
    { 
        health -= damage;
    }
}
