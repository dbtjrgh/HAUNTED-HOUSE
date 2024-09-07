using Photon.Pun.UtilityScripts;
using System.Collections;
using UnityEngine;

public class CTruckButton : MonoBehaviour
{
    public Animator anim;

    private bool TruckDoorOpen = false;
    private bool canOpen = true;
    private float delay = 3f;
    private float closingTime = 4f;

    private bool playerNearby = false;  // 플레이어가 콜라이더 안에 있는지 여부

    private void Update()
    {
        // 플레이어가 콜라이더 안에 있고 'E' 키를 눌렀을 때만 작동
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            TruckOnClick();  // 버튼 동작
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Player 태그를 가진 오브젝트가 버튼 콜라이더 안에 들어올 때
        if (other.CompareTag("Player"))
        {
            playerNearby = true;  // 플레이어가 근처에 있음을 설정
            Debug.Log("Player is near the button.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Player 태그를 가진 오브젝트가 버튼 콜라이더를 나갈 때
        if (other.CompareTag("Player"))
        {
            playerNearby = false;  // 플레이어가 근처에서 나갔음을 설정
            Debug.Log("Player left the button area.");
        }
    }

    private void TruckOnClick()
    {
        if (canOpen && !TruckDoorOpen)
        {
            anim.SetTrigger("OpenDoors");
            StopCoroutine("LevelEnd");
            TruckDoorOpen = true;
        }
        else if (TruckDoorOpen)
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
