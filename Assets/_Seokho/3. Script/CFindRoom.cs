using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CFindRoom : MonoBehaviour
{
    public RectTransform roomListRect;
    private List<RoomInfo> currentRoomList = new List<RoomInfo>();
    public Button roomButtonPrefab;
    public Button backButton;

    private void Awake()
    {
        backButton.onClick.AddListener(BackButtonClick);
    }

    private void OnDisable()
    {
        foreach(Transform child in  roomListRect)
        {
            Destroy(child.gameObject);
        }
    }

    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        // �ı��� �ĺ�
        List<RoomInfo> destroyCandidate =
            currentRoomList.FindAll((x) => false == roomList.Contains(x));

        foreach (RoomInfo roomInfo in roomList)
        {
            if (currentRoomList.Contains(roomInfo))
            {
                continue;
            }
            AddRoomButton(roomInfo);
        }

        foreach (Transform child in roomListRect)
        {
            if (destroyCandidate.Exists((x) => x.Name == child.name))
            {
                Destroy(child.gameObject);
            }
        }

        currentRoomList = roomList; 
    }

    /// <summary>
    /// Roo1mInfoList�� ���� ���������� �� ���� �� ���� ��ư �����ϴ� �Լ�
    /// </summary>
    /// <param name="roomInfo"></param>
    public void AddRoomButton(RoomInfo roomInfo)
    {
        Button joinButton = Instantiate(roomButtonPrefab, roomListRect, false);
        joinButton.gameObject.name = roomInfo.Name;
        joinButton.onClick.AddListener(() => PhotonNetwork.JoinRoom(roomInfo.Name));

        // �� �̸��� �߰� ������ ǥ��
        string roomInfoText = $"{roomInfo.Name} ({roomInfo.PlayerCount}/{roomInfo.MaxPlayers}) ";
        joinButton.GetComponentInChildren<TextMeshProUGUI>().text = roomInfoText;
    }

    /// <summary>
    /// ���� �����ϰ� ���� ��ư�� ���� ���� �ʹٸ� �Ҵ�
    /// </summary>
    /// <param name="roomName"></param>
    private void JoinButtonClick(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    /// <summary>
    /// CBoardManager��ũ��Ʈ�� OnLeftLobby �Լ��� �ҷ����� LeaveLobby �Լ� ȣ��
    /// </summary>
    private void BackButtonClick()
    {
        PhotonNetwork.LeaveLobby();
    }
}
