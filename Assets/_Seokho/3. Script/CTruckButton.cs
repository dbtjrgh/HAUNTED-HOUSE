using Photon.Pun;
using System.Collections;
using UnityEngine;

public class CTruckButton : MonoBehaviour
{
    public Animator anim;

    private bool TruckDoorOpen = false;
    private bool canOpen = true;
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
        if (canOpen && !TruckDoorOpen)
        {
            anim.SetTrigger("OpenDoors");
            TruckDoorOpen = true;
            StartCoroutine(WaitForAnimation());
        }
        else if (TruckDoorOpen)
        {
            anim.SetTrigger("CloseDoors");
            TruckDoorOpen = false;
            StartCoroutine(WaitForAnimation());
        }
    }

    IEnumerator WaitForAnimation()
    {
        isAnimating = true;
        yield return new WaitForSeconds(delay + closingTime);
        isAnimating = false;
    }

    public IEnumerator LevelEnd()
    {
        yield return new WaitForSeconds(delay + closingTime);
        canOpen = false;

        // 트럭 문이 닫힌 상태일 때 게임 매니저의 CheckAllPlayersSelectedGhost 호출
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null && gameManager.CheckAllPlayersSelectedGhost())
        {
            gameManager.ShowResultUI(); // 저널에서 귀신이 선택된 경우 ResultUI 표시
        }
    }

    public bool IsTruckDoorClosed()
    {
        return !TruckDoorOpen;
    }
}
