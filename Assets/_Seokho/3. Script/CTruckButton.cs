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

    private bool playerNearby = false;  // �÷��̾ �ݶ��̴� �ȿ� �ִ��� ����

    private void Update()
    {
        // �÷��̾ �ݶ��̴� �ȿ� �ְ� 'E' Ű�� ������ ���� �۵�
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            TruckOnClick();  // ��ư ����
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Player �±׸� ���� ������Ʈ�� ��ư �ݶ��̴� �ȿ� ���� ��
        if (other.CompareTag("Player"))
        {
            playerNearby = true;  // �÷��̾ ��ó�� ������ ����
            Debug.Log("Player is near the button.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Player �±׸� ���� ������Ʈ�� ��ư �ݶ��̴��� ���� ��
        if (other.CompareTag("Player"))
        {
            playerNearby = false;  // �÷��̾ ��ó���� �������� ����
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
