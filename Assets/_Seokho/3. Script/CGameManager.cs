using Photon.Pun;
using UnityEngine;

public class CGameManager : MonoBehaviourPunCallbacks
{
    private static GameObject localPlayerInstance;

    private void Start()
    {
        // �濡 �̹� ������ ������ ��� �÷��̾� ����
        if (PhotonNetwork.InRoom)
        {
            CreatePlayer();
        }
    }

    // �濡 �������� �� ȣ��Ǵ� �ݹ�
    public override void OnJoinedRoom()
    {
        // �濡 �������� �� �÷��̾� ����
        CreatePlayer();
    }

    private void CreatePlayer()
    {
        if (localPlayerInstance != null) return; // �̹� �÷��̾ ������ ��� �ߺ� ���� ����

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
