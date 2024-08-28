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
        randomSpawn.position = new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
        Instantiate(GhostPrefab, randomSpawn.position, Quaternion.identity);

    }
}
