using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flashLight : MonoBehaviour
{
    bool playerGetLight; // true�� ���, �÷��̾ �������� on�� ����.
    static bool getLight; // ������ ȹ�� ���� Ȯ��.
    Light myLight; // light ������Ʈ ����.

    private void Start()
    {
        playerGetLight = false;
        getLight = false;
        myLight = this.GetComponent<Light>(); //flashLight�� Light ������Ʈ�� ������.
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
                playerGetLight = playerGetLight ? false : true; // ������ on/off
            }

        }
    }

}
