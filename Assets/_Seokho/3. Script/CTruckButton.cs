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

        // Ʈ�� ���� ���� ������ �� ���� �Ŵ����� CheckAllPlayersSelectedGhost ȣ��
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null && gameManager.CheckAllPlayersSelectedGhost())
        {
            gameManager.ShowResultUI(); // ���ο��� �ͽ��� ���õ� ��� ResultUI ǥ��
        }
    }

    public bool IsTruckDoorClosed()
    {
        return !TruckDoorOpen;
    }
}
