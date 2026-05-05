using UnityEngine;

public class MissionZone : MonoBehaviour
{
    [SerializeField] GameObject boundary;
    [SerializeField] Transform enemiesParentObject;

    private bool _missionComplete;

    void Update()
    {
        if (AllDead()) CompleteMission();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if(other.CompareTag("Player")) StartMission();
        if (_missionComplete) return;
    }

    private void StartMission()
    {
        boundary.SetActive(true); 
    }

    private bool AllDead()
    {
        return enemiesParentObject.childCount == 0;
    }

    public void CompleteMission()
    {
        _missionComplete = true;
        boundary.SetActive(false);

        
    }
}
