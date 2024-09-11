using Photon.Pun;
using System.Collections;
using UnityEngine;

public class CTruckButton : MonoBehaviour
{
    public Animator anim;

    public bool TruckDoorOpen = false;
    private float delay = 3f;
    private float closingTime = 4f;

    private bool playerNearby = false;
    private bool isAnimating = false;

    private void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E) && !isAnimating)
        {
            TruckOnClick();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }

    private void TruckOnClick()
    {
        if (!TruckDoorOpen)
        {
            anim.SetTrigger("OpenDoors");
            SoundManager.instance.TruckButtonSound();
            StartCoroutine(WaitForAnimation());
            TruckDoorOpen = true;
        }
        else if (TruckDoorOpen)
        {
            anim.SetTrigger("CloseDoors");
            SoundManager.instance.TruckButtonSound();
            StartCoroutine(WaitForAnimation());
            TruckDoorOpen = false;
        }
    }

    IEnumerator WaitForAnimation()
    {
        isAnimating = true;
        yield return new WaitForSeconds(delay + closingTime);
        isAnimating = false;
        
    }

}
