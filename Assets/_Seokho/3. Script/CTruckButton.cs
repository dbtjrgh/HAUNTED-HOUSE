using Photon.Pun;
using System.Collections;
using UnityEngine;

public class CTruckButton : MonoBehaviourPun
{
    #region ����
    public Animator anim;

    public bool TruckDoorOpen = false; // Ʈ�� �� ���� ����
    private float delay = 3f;
    private float closingTime = 4f;

    private bool playerNearby = false;
    private bool isAnimating = false;
    #endregion

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
            photonView.RPC("RPCSetTrigger", RpcTarget.All, "OpenDoors"); // ��� Ŭ���̾�Ʈ�� Ʈ���� ����
            SoundManager.instance.TruckButtonSound();
            StartCoroutine(WaitForAnimation());

            photonView.RPC("SetTruckDoorState", RpcTarget.All, true);  // ��� Ŭ���̾�Ʈ�� �� ���� ���� ����ȭ
        }
        else if (TruckDoorOpen)
        {
            photonView.RPC("RPCSetTrigger", RpcTarget.All, "CloseDoors"); // ��� Ŭ���̾�Ʈ�� Ʈ���� ����
            SoundManager.instance.TruckButtonSound();
            StartCoroutine(WaitForAnimation());

            photonView.RPC("SetTruckDoorState", RpcTarget.All, false); // ��� Ŭ���̾�Ʈ�� �� ���� ���� ����ȭ
        }
    }

    [PunRPC]
    public void RPCSetTrigger(string triggerName)
    {
        anim.SetTrigger(triggerName);  // �ִϸ��̼� Ʈ���� ����
    }

    [PunRPC]
    public void SetTruckDoorState(bool isOpen)
    {
        TruckDoorOpen = isOpen;  // ��� Ŭ���̾�Ʈ���� TruckDoorOpen ���� ����ȭ
    }

    IEnumerator WaitForAnimation()
    {
        isAnimating = true;
        yield return new WaitForSeconds(delay + closingTime);
        isAnimating = false;
    }
}
