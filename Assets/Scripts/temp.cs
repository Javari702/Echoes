using UnityEngine;

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject particleInstance = Instantiate(particlePrefab, transform.position, transform.rotation);
            particleInstance.GetComponent<ParticleSystem>().Play();
        }
    }
}
