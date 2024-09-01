using Photon.Pun;
using UnityEngine;

public class CGameManager : MonoBehaviourPunCallbacks
{
    private static GameObject localPlayerInstance;

    private void Start()
    {
        // 방에 이미 참가한 상태인 경우 플레이어 생성
        if (PhotonNetwork.InRoom)
        {
            CreatePlayer();
        }
    }

    // 방에 참가했을 때 호출되는 콜백
    public override void OnJoinedRoom()
    {
        // 방에 참가했을 때 플레이어 생성
        CreatePlayer();
    }

    private void CreatePlayer()
    {
        if (localPlayerInstance != null) return; // 이미 플레이어가 생성된 경우 중복 생성 방지

        GameObject playerPrefab = Resources.Load<GameObject>("PlayerPrefab");
        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab not found.");
            return;
        }

        Vector3 spawnPosition = new Vector3(11, 4, -7); 
        localPlayerInstance = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);

        PhotonNetwork.LocalPlayer.TagObject = localPlayerInstance;
    }
}
