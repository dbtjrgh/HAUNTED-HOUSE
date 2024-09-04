using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDontDestroySpawnPoint : MonoBehaviour
{
    public static CDontDestroySpawnPoint instance = null;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }
}
