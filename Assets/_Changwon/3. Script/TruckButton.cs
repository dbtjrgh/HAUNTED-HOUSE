using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckButton : MonoBehaviour
{
    public Animator anim;

    private bool TruckDoorOpen = false;
    private bool canOpen = true;
    private float delay = 3f;
    private float closingTime = 4f;

    public void TruckOnClick()
    {
        if(canOpen&&!TruckDoorOpen&&anim.GetCurrentAnimatorStateInfo(0).IsName("ClosingDoors"))
        {
            anim.SetTrigger("OpenDoors");
            StopCoroutine("LevelEnd");
            TruckDoorOpen = true;
        }

        else if(TruckDoorOpen&&anim.GetCurrentAnimatorStateInfo(0).IsName("OpeningDoors"))
        {
            anim.SetTrigger("CloseDoors");
            StartCoroutine("LevelEnd");
            TruckDoorOpen = false;
        }
    }

    IEnumerator LevelEnd()
    {
        yield return new WaitForSeconds(delay + closingTime);
        canOpen = false;
        
    }


}
