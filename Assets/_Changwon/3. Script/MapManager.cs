using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public Transform Spawn;
    
    public GameObject GhostPrefab;

    public GameObject Ghost;

    public GameObject GhostOrb;
    Ghost ghost;


    private void Start()
    {
        GhostSpawn();
        ghost=GetComponent<Ghost>();

    }

    public void GhostSpawn()
    {
        
        Instantiate(GhostPrefab, Spawn.position, Quaternion.identity);        //고스트 생성
        if (ghost.ghostType==GhostType.NIGHTMARE||ghost.ghostType==GhostType.BANSHEE)
        {
            Instantiate(GhostOrb, Spawn.position, Quaternion.identity);        //고스트 생성
        }

    }

    public void ghostblink()
    {
        //고스트 깜빡임
        //고스트가 플레이어를 향해 이동
        //고스트가 플레이어에게 접근
        //고스트가 플레이어에게 접근하면 플레이어가 죽음
        GhostPrefab.SetActive(false);
        
    }
}
