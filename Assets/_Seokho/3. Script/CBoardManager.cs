using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBoardManager : MonoBehaviourPunCallbacks
{
    public static CBoardManager Instance { get; private set; }

    public CLoginScreen login;
    public CMenuScreen Menu;
    public CFindRoom find;
    public CRoomScreen room;

    private Dictionary<string, GameObject> screens;


    private void Awake()
    {
        Instance = this;
        screens = new Dictionary<string, GameObject>
        {
            {"Login", login.gameObject },
            {"Menu",Menu.gameObject},
            {"Find",find.gameObject},
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
        print("���θ޴� ����");
        ScreenOpen("Menu");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"disconnected cause : {cause}");
        ScreenOpen("Login");
    }

    public override void OnJoinedLobby()
    {
        print("�κ� ����");
        ScreenOpen("Find");
    }

    public override void OnLeftLobby()
    {
        print("���θ޴� ����");
        ScreenOpen("Menu");
    }

    public override void OnJoinedRoom()
    {
        print("�뿡 ����");
        ScreenOpen("Room");

    }
    public override void OnCreatedRoom()
    {
        print("�뿡 ����");
        ScreenOpen("Room");
    }

    public override void OnLeftRoom()
    {
        print("���θ޴� ����");
        ScreenOpen("Menu");
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        // �� �����ؾ��� room.JoinPlayer(newPlayer);
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

