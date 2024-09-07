using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public Transform[] Spawn;    
    public GameObject GhostPrefab;
    public GameObject GhostOrb;
    Ghost ghost;
    public Vector3 returnRandom;
    

    private void Awake()
    {
        GhostSpawn();
    }


    public void GhostSpawn()
    {
        int randomIndex = Random.Range(0, Spawn.Length);
        print(randomIndex);
        GameObject ghostInstance = Instantiate(GhostPrefab, Spawn[randomIndex].position, Quaternion.identity);
        ghost = ghostInstance.GetComponent<Ghost>();
        
        // ghost ��ü�� null�� �ƴ��� Ȯ���� �� GhostOrb�� ����
        if (ghost != null && (ghost.ghostType == GhostType.NIGHTMARE || ghost.ghostType == GhostType.BANSHEE))
        {
            Instantiate(GhostOrb, Spawn[randomIndex].position, Quaternion.identity);
        }
        else if (ghost == null)
        {
            Debug.LogError("Ghost component is missing on the instantiated GhostPrefab.");
        }
        returnRandom = new Vector3(Spawn[randomIndex].position.x, Spawn[randomIndex].position.y, Spawn[randomIndex].position.z);
    }

    
    
}
