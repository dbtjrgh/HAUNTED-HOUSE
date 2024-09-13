using Photon.Pun;
using UnityEngine;

public class CItemManager : MonoBehaviourPun
{
    #region 변수
    public GameObject VideoCamera;      // 비디오 카메라
    public GameObject UVflashlight;     // UV 손전등
    public GameObject EMF;              // EMF 장치
    public GameObject Flashlight;       // 손전등
    public GameObject SanityPill;       // 멘탈 회복제

    public Transform[] VideoCameraSpawnPoints;     // 비디오 카메라 스폰 포인트
    public Transform[] UVflashlightSpawnPoints;    // UV 손전등 스폰 포인트
    public Transform[] EMFSpawnPoints;             // EMF 장치 스폰 포인트
    public Transform[] FlashlightSpawnPoints;      // 손전등 스폰 포인트
    public Transform[] SanityPillSpawnPoints;      // 멘탈 회복제 스폰 포인트
    #endregion

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnItems();
        }
    }
    /// <summary>
    /// 무슨 프리팹을 소환할지, 어디에 소환할지 할당시키면 자동으로 그곳에 형성시켜주는 함수
    /// </summary>
    void SpawnItems()
    {
        // 각 아이템 타입별로 아이템 생성
        SpawnItem(VideoCamera, VideoCameraSpawnPoints);
        SpawnItem(UVflashlight, UVflashlightSpawnPoints);
        SpawnItem(EMF, EMFSpawnPoints);
        SpawnItem(Flashlight, FlashlightSpawnPoints);
        SpawnItem(SanityPill, SanityPillSpawnPoints);
    }

    /// <summary>
    /// 각 아이템을 해당 스폰 포인트에서 생성하는 함수
    /// </summary>
    /// <param name="itemPrefab"></param>
    /// <param name="spawnPoints"></param>
    void SpawnItem(GameObject itemPrefab, Transform[] spawnPoints)
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            PhotonNetwork.Instantiate(itemPrefab.name, spawnPoints[i].position, spawnPoints[i].rotation);
        }
    }
}
