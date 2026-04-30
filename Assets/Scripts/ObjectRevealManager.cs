using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRevealManager : MonoBehaviour
{
    public static ObjectRevealManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void RevealMaterial(Material mat, float duration, HashSet<Material> activeMaterials)
    {
        StartCoroutine(MakeObjectVisible(mat, duration, activeMaterials));
    }

    private IEnumerator MakeObjectVisible(Material mat, float duration, HashSet<Material> activeMaterials)
    {
        mat.SetFloat("_Fade", 1.0f);
        yield return new WaitForSecondsRealtime(duration);
        mat.SetFloat("_Fade", 0f);
        activeMaterials.Remove(mat);
    }
}