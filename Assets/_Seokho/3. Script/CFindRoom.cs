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

        foreach (Transform child in roomListRect)
        {
            Destroy(child.gameObject);
        }

        foreach (RoomInfo roomInfo in roomList)
        {
            if (!roomInfo.RemovedFromList)
            {
                AddRoomButton(roomInfo);
            }
        }
    }

    /// <summary>
    /// Roo1mInfoList를 통해 순차적으로 한 개씩 방 입장 버튼 생성하는 함수
    /// </summary>
    /// <param name="roomInfo"></param>
    public void AddRoomButton(RoomInfo roomInfo)
    {
        Button joinButton = Instantiate(roomButtonPrefab, roomListRect, false);
        joinButton.gameObject.name = roomInfo.Name;
        joinButton.onClick.AddListener(() => PhotonNetwork.JoinRoom(roomInfo.Name));

        // 방 이름과 추가 정보를 표시
        string roomInfoText = $"{roomInfo.Name} ({roomInfo.PlayerCount}/{roomInfo.MaxPlayers}) ";
        joinButton.GetComponentInChildren<Text>().text = roomInfoText;
    }

    /// <summary>
    /// 방을 선택하고 입장 버튼을 눌러 들어가고 싶다면 할당
    /// </summary>
    /// <param name="roomName"></param>
    private void JoinButtonClick(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    /// <summary>
    /// CBoardManager스크립트의 OnLeftLobby 함수를 불러오는 LeaveLobby 함수 호출
    /// </summary>
    private void BackButtonClick()
    {
        PhotonNetwork.LeaveLobby();
    }
}
