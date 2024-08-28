using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flashLight : MonoBehaviour
{
    bool playerGetLight; // true일 경우, 플레이어가 손전등을 on한 상태.
    static bool getLight; // 손전등 획득 여부 확인.
    Light myLight; // light 컴포넌트 관리.

    private void Start()
    {
        playerGetLight = false;
        getLight = false;
        myLight = this.GetComponent<Light>(); //flashLight의 Light 컴포넌트를 가져옴.
    }

    private void Update()
    {
        lightOnOFF();
        if(playerGetLight == false)
        {
            myLight.intensity = 0;
        }

        else if(playerGetLight == true)
        {
            myLight.intensity = 10;
        }
    }

    static internal void lightCheck()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            getLight = true;
            Destroy(GameObject.FindGameObjectWithTag("FlashLight"));
        }
    }

    void lightOnOFF()
    {
        if(getLight == true)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                playerGetLight = playerGetLight ? false : true; // 손전등 on/off
            }

        }
    }

}
