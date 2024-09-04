using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDontDestroySetup : MonoBehaviour
{
    public static CDontDestroySetup instance = null;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if(instance != this)
        {
            Destroy(this.gameObject);
        }
    }
}
