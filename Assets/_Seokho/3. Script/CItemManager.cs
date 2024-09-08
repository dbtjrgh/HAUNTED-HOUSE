using Photon.Pun;
using UnityEngine;

public class CItemManager : MonoBehaviourPun
{
    public GameObject VideoCamera;      // ���� ī�޶�
    public GameObject UVflashlight;     // UV ������
    public GameObject EMF;              // EMF ��ġ
    public GameObject Flashlight;       // ������
    public GameObject SanityPill;       // ��Ż ȸ����

    public Transform[] VideoCameraSpawnPoints;     // ���� ī�޶� ���� ����Ʈ
    public Transform[] UVflashlightSpawnPoints;    // UV ������ ���� ����Ʈ
    public Transform[] EMFSpawnPoints;             // EMF ��ġ ���� ����Ʈ
    public Transform[] FlashlightSpawnPoints;      // ������ ���� ����Ʈ
    public Transform[] SanityPillSpawnPoints;      // ��Ż ȸ���� ���� ����Ʈ

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnItems();
        }
    }

    void SpawnItems()
    {
        // �� ������ Ÿ�Ժ��� ������ ����
        SpawnItem(VideoCamera, VideoCameraSpawnPoints);
        SpawnItem(UVflashlight, UVflashlightSpawnPoints);
        SpawnItem(EMF, EMFSpawnPoints);
        SpawnItem(Flashlight, FlashlightSpawnPoints);
        SpawnItem(SanityPill, SanityPillSpawnPoints);
    }

    // �� �������� �ش� ���� ����Ʈ���� �����ϴ� �Լ�
    void SpawnItem(GameObject itemPrefab, Transform[] spawnPoints)
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            PhotonNetwork.Instantiate(itemPrefab.name, spawnPoints[i].position, spawnPoints[i].rotation);
        }
    }
}
