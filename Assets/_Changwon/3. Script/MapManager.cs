using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public Transform randomSpawn;
    
    public GameObject GhostPrefab;

    private void Start()
    {
        randomGhostSpawn();

    }

    public void randomGhostSpawn()
    {
        
        Instantiate(GhostPrefab, randomSpawn.position, Quaternion.identity);        //고스트 생성

    }
}
