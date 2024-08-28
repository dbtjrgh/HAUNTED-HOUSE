using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBoardManager : MonoBehaviourPunCallbacks
{
    public static CBoardManager Instance { get; private set; }

    public CLoginScreen login;
    public CMainMenuScreen mainMenu;
    public CLobbyScreen lobby;
    public CRoomScreen room;

    private Dictionary<string, GameObject> screens;

    private void Awake()
    {
        Instance = this;
        screens = new Dictionary<string, GameObject>
        {
            {"Login", login.gameObject },
            {"MainMenu",mainMenu.gameObject},
            {"Lobby",lobby.gameObject},
            {"Room", room.gameObject }
        };
        ScreenOpen("Login");
        PhotonNetwork.AddCallbackTarget(this);
    }


    public void ScreenOpen(string screenName)
    {
        foreach(var row in screens)
        {
            row.Value.SetActive(row.Key.Equals(screenName));
        }
    }

    public override void OnConnected()
    {
        print("메인메뉴 입장");
        ScreenOpen("MainMenu");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"disconnected cause : {cause}");
        ScreenOpen("Login");
    }

    public override void OnJoinedLobby()
    {
        print("로비 입장");
        ScreenOpen("Lobby");
    }

    public override void OnLeftLobby()
    {
        print("메인메뉴 입장");
        ScreenOpen("MainMenu");
    }

    public override void OnJoinedRoom()
    {
        print("룸에 입장");
        ScreenOpen("Room");

    }
    public override void OnCreatedRoom()
    {
        print("룸에 입장");
        ScreenOpen("Room");
    }

    public override void OnLeftRoom()
    {
        print("메인메뉴 입장");
        ScreenOpen("MainMenu");
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        // 잘 따라해야함 room.JoinPlayer(newPlayer);
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        // room.LeavePlayer(otherPlayer);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // lobby.UpdateRoomList(roomList);
    }
}

