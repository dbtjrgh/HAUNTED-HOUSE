using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashSpawn : MonoBehaviour
{
    public GameObject Flash;

    private void Awake()
    {
        Spawn();
    }


    void Spawn()
    {
        Instantiate(Flash, transform.position, transform.rotation);
    }    


}
