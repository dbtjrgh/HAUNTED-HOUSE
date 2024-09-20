using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonRigidbodyView))]
public class flashLight : MonoBehaviourPun
{
    private Light myLight;
    public bool isLightOn = false;
    private Rigidbody rb;
    public MeshRenderer lightMesh;
    CPlayerInventory playerInventory; // mesh���� ���� ��ȭ�� ���� playerInventory ��ũ��Ʈ�� ������
    private GameObject Flashlight;

    private void Start()
    {
        myLight = GetComponentInChildren<Light>();
        rb = GetComponent<Rigidbody>(); // Rigidbody ������Ʈ ��������
        lightMesh = GetComponentInChildren<MeshRenderer>();
        Flashlight = this.gameObject; // Flashlight�� ���� �Ҵ�.

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
        SoundManager.instance.FlashLightSound();
    }

    // �������� ����� �� Rigidbody ���� Ȱ��ȭ
    public void DropItem(Vector3 dropPosition)
    {
        // PhotonRigidbodyView�� ����ȭ�Ǵ� ���� ���� Ȱ��ȭ
        rb.isKinematic = false;
        rb.AddForce(dropPosition * 2.0f, ForceMode.Impulse); // �������� ��� ��ġ�� �о
    }


    public void meshHandle(bool isActive)
    {

        Debug.Log("isActive: " + isActive); // ȣ��ɶ����� isActive �Ҵ� Ȯ��.
        if (lightMesh != null)
        {
            lightMesh.gameObject.SetActive(isActive); // �Ž��� Ȱ��ȭ ���� ����
        }

    }


    public void swapLight(bool isActive)
    {
        if (Flashlight != null)
        {
            Flashlight.SetActive(true); // ������ ������Ʈ�� �׻� Ȱ��ȭ ���� ����
        }
    }
}