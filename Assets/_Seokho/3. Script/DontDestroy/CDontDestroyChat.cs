using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDontDestroyChat : MonoBehaviour
{
    #region º¯¼ö
    public static CDontDestroyChat instance = null;
    #endregion

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
