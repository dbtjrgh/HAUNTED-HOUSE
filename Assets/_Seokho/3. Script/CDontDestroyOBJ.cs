using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDontDestroyOBJ : MonoBehaviour
{
    public List<GameObject> dontDestroyObjects;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        for(int i = 0; i < dontDestroyObjects.Count; i++)
        {
            DontDestroyOnLoad(dontDestroyObjects[i]);
        }
    }
}
