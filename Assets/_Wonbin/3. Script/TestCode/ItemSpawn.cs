using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawn : MonoBehaviour
{
    public GameObject Item;

    private void Awake()
    {
        Spawn();
    }


    void Spawn()
    {
        Instantiate(Item, transform.position, transform.rotation);
    }    


}
