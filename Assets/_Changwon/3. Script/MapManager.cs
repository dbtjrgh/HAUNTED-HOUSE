using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public Transform Spawn;    
    public GameObject GhostPrefab;
    public Camera mainCamera;
    public GameObject GhostOrb;
    Ghost ghost;


    private void Awake()
    {
        GhostSpawn();
    }

    

    

    public void GhostSpawn()
    {
        GameObject ghostInstance = Instantiate(GhostPrefab, Spawn.position, Quaternion.identity);
        ghost = ghostInstance.GetComponent<Ghost>();
        
        // ghost 객체가 null이 아닌지 확인한 후 GhostOrb를 생성
        if (ghost != null && (ghost.ghostType == GhostType.NIGHTMARE || ghost.ghostType == GhostType.BANSHEE))
        {
            Instantiate(GhostOrb, Spawn.position, Quaternion.identity);
        }
        else if (ghost == null)
        {
            Debug.LogError("Ghost component is missing on the instantiated GhostPrefab.");
        }

    }

    
}
