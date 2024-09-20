using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonRigidbodyView))]
public class flashLight : MonoBehaviourPun
{
    private Light myLight;
    public bool isLightOn = false;
    private Rigidbody rb;
    public MeshRenderer lightMesh;
    CPlayerInventory playerInventory; // mesh값의 상태 변화를 위해 playerInventory 스크립트를 가져옴
    private GameObject Flashlight;

    private void Start()
    {
        myLight = GetComponentInChildren<Light>();
        rb = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트 가져오기
        lightMesh = GetComponentInChildren<MeshRenderer>();
        Flashlight = this.gameObject; // Flashlight에 본인 할당.

        if (myLight != null)
        {
            myLight.intensity = 0; // 시작할 때 손전등 꺼짐 상태
        }

        // Rigidbody 설정: 처음에는 물리적 상호작용 비활성화
        rb.isKinematic = true;
    }



    public void lightOnOFF()
    {
        photonView.RPC("SyncLightState", RpcTarget.All);
    }

    [PunRPC]
    public void SyncLightState()
    {
        if (myLight == null) return;

        // 손전등 상태를 토글
        isLightOn = !isLightOn;
        myLight.intensity = isLightOn ? 10 : 0;

        // 모든 클라이언트에서 손전등이 활성화되도록 보장
        gameObject.SetActive(true);
        SoundManager.instance.FlashLightSound();
    }

    // 아이템을 드롭할 때 Rigidbody 물리 활성화
    public void DropItem(Vector3 dropPosition)
    {
        // PhotonRigidbodyView로 동기화되는 동안 물리 활성화
        rb.isKinematic = false;
        rb.AddForce(dropPosition * 2.0f, ForceMode.Impulse); // 아이템을 드롭 위치로 밀어냄
    }


    public void meshHandle(bool isActive)
    {

        Debug.Log("isActive: " + isActive); // 호출될때마다 isActive 할당 확인.
        if (lightMesh != null)
        {
            lightMesh.gameObject.SetActive(isActive); // 매쉬의 활성화 상태 변경
        }

    }


    public void swapLight(bool isActive)
    {
        if (Flashlight != null)
        {
            Flashlight.SetActive(true); // 손전등 오브젝트는 항상 활성화 상태 유지
        }
    }
}