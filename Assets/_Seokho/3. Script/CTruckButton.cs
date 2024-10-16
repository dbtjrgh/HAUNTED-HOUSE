using Photon.Pun;
using System.Collections;
using UnityEngine;

public class CTruckButton : MonoBehaviourPun
{
    #region 변수
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
            photonView.RPC("RPCSetTrigger", RpcTarget.All, "OpenDoors"); // 모든 클라이언트에 트리거 전달
            SoundManager.instance.TruckButtonSound();
            StartCoroutine(WaitForAnimation());
            TruckDoorOpen = true;
        }
        else if (TruckDoorOpen)
        {
            photonView.RPC("RPCSetTrigger", RpcTarget.All, "CloseDoors"); // 모든 클라이언트에 트리거 전달
            SoundManager.instance.TruckButtonSound();
            StartCoroutine(WaitForAnimation());
            TruckDoorOpen = false;
        }
    }

    [PunRPC]
    public void RPCSetTrigger(string triggerName)
    {
        anim.SetTrigger(triggerName);  // 애니메이션 트리거 실행
    }

    IEnumerator WaitForAnimation()
    {
        isAnimating = true;
        yield return new WaitForSeconds(delay + closingTime);
        isAnimating = false;
    }
}
