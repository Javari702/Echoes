using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectReveal : MonoBehaviour
{
    [SerializeField] float visibilityDuration;

    private Collider[] _hitColliders;
    private int _maxColliders = 65;
    private float _particleRadius = 0f;
    private ParticleSystem _particle; 
    private HashSet<Material> _activeMaterials; 
    private Dictionary<Collider, Material> _materialCache; 

    void Awake()
    {
        _hitColliders = new Collider[_maxColliders];
        _particle = GetComponent<ParticleSystem>();
        _activeMaterials = new HashSet<Material>();
        _materialCache = new Dictionary<Collider, Material>();
    }

    void Update()
    {
        GetObjects();   
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _particleRadius);
    }

    private Material GetMaterial(Collider col)
    {
        if (!_materialCache.TryGetValue(col, out Material mat))
        {
            mat = col.GetComponent<Renderer>().material; 
            _materialCache[col] = mat;
        }
        return mat;
    }

    private void GetObjects()
    {
        System.Array.Clear(_hitColliders, 0, _hitColliders.Length);

        if (_particleRadius >= _particle.main.startSize.constant / 2) {
            return;
        }
        
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, _particleRadius, _hitColliders, LayerMask.GetMask("RadarDetection"));
        _particleRadius += _particle.main.startSize.constant / _particle.main.startLifetime.constant / 2 * Time.deltaTime;

        for (int i = 0; i < numColliders; i++)
        {
            Material _sonarSenseShader = GetMaterial(_hitColliders[i]); 

            //if (_hitColliders[i].CompareTag("Floor")) _sonarSenseShader.SetFloat("_Moving", 0);

            if (!_activeMaterials.Contains(_sonarSenseShader))
            {
                _activeMaterials.Add(_sonarSenseShader);
                ObjectRevealManager.Instance.RevealMaterial(_sonarSenseShader, visibilityDuration, _activeMaterials);
            }
        }   
    }
}

