using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
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

        List<RoomInfo> destroyCandidate = currentRoomList.FindAll((x) => false == roomList.Contains(x));
        foreach (RoomInfo rooninfo in roomList)
        {
            if(currentRoomList.Contains(rooninfo))
            {
                continue;
            }
            AddRoomButton(rooninfo);
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
        joinButton.GetComponentInChildren<Text>().text = roomInfo.Name;
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
