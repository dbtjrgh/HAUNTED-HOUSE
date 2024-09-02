using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class _Camcorder : MonoBehaviour
{
    public RawImage camcorderDisplay; // UI의 Raw Image를 연결
    
    private bool isCamcorderActive = false;

    /*void Update()
    {
        if (*//*캠코더키*//*)
        {
            isCamcorderActive = !isCamcorderActive;
            camcorderDisplay.gameObject.SetActive(isCamcorderActive);
        }
    }*/
}
