using Photon.Pun;
using System.Collections;
using UnityEngine;

public class CTruckButton : MonoBehaviourPun
{
    #region ����
    public Animator anim;

    public bool TruckDoorOpen = false;
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
            TruckDoorOpen = true;
        }
        else if (TruckDoorOpen)
        {
            photonView.RPC("RPCSetTrigger", RpcTarget.All, "CloseDoors"); // ��� Ŭ���̾�Ʈ�� Ʈ���� ����
            SoundManager.instance.TruckButtonSound();
            StartCoroutine(WaitForAnimation());
            TruckDoorOpen = false;
        }
    }

    [PunRPC]
    public void RPCSetTrigger(string triggerName)
    {
        anim.SetTrigger(triggerName);  // �ִϸ��̼� Ʈ���� ����
    }

    IEnumerator WaitForAnimation()
    {
        isAnimating = true;
        yield return new WaitForSeconds(delay + closingTime);
        isAnimating = false;
    }
}
