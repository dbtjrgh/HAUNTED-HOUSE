using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
[RequireComponent(typeof(PhotonRigidbodyView))]
public class gaugeFill : MonoBehaviourPun
{
    mentalGaugeManager playerMentalGauge;

    [SerializeField]
    private GameObject _Player;

    private float ToAdd = 30;
    static bool getFill;
    public static bool isInItemSlot;
    private Transform itemSlotTransform;
    static bool useFill; //��� ������ ���¸� �ǹ��ϴ� ����


    private void Start()
    {
        playerMentalGauge = GameObject.FindGameObjectWithTag("Player").GetComponent<mentalGaugeManager>();
        getFill = false;
        isInItemSlot = false;
        useFill = true;
        itemSlotTransform = GameObject.Find("ItemSlot")?.transform;
    }
    public void fillUse() // ��Ż ������ �� ���
    {
        playerMentalGauge.AddMentalGauge(ToAdd);
        photonView.RPC("DestroyGauge", RpcTarget.All);
    }

    [PunRPC]
    public void DestroyGauge()
    {
        PhotonNetwork.Destroy(gameObject);
    }

}
