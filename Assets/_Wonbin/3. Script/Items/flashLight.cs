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
        rb = GetComponent<Rigidbody>(); // Rigidbody ������Ʈ ��������
        if (myLight != null)
        {
            myLight.intensity = 0; // ������ �� ������ ���� ����
        }

        // Rigidbody ����: ó������ ������ ��ȣ�ۿ� ��Ȱ��ȭ
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

        // ������ ���¸� ���
        isLightOn = !isLightOn;
        myLight.intensity = isLightOn ? 10 : 0;

        // ��� Ŭ���̾�Ʈ���� �������� Ȱ��ȭ�ǵ��� ����
        gameObject.SetActive(true);
    }

    // �������� ����� �� Rigidbody ���� Ȱ��ȭ
    public void DropItem(Vector3 dropPosition)
    {
        // PhotonRigidbodyView�� ����ȭ�Ǵ� ���� ���� Ȱ��ȭ
        rb.isKinematic = false;
        rb.AddForce(dropPosition * 2.0f, ForceMode.Impulse); // �������� ��� ��ġ�� �о
    }
}
