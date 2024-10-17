using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonRigidbodyView))]
public class flashLight : MonoBehaviourPun
{
    private Light myLight;
    public bool isLightOn = false;
    private Rigidbody rb;

    private void Start()
    {
        myLight = GetComponentInChildren<Light>();
        rb = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트 가져오기
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
}
