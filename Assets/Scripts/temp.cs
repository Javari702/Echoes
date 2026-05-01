using UnityEngine;
using UnityEngine.InputSystem;

public class temp : MonoBehaviour
{
    public GameObject particlePrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnAttack(InputValue input)
    {
        if (input.isPressed)
        {
            GameObject particleInstance = Instantiate(particlePrefab, transform.position, transform.rotation);
            particleInstance.GetComponent<ParticleSystem>().Play();
        }
    }
}
