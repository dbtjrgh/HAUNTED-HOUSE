using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CBoardManager : MonoBehaviourPunCallbacks
{

    // �̱��� ���� CBoardManager �ν��Ͻ� ���������� ���� ����
    public static CBoardManager Instance { get; private set; }

    #region ����
    [Header("Screen")]
    public CLoginScreen login; // �α��� ��ũ��
    public CMenuScreen menu;   // �޴� ��ũ��
    public CFindRoom find;     // ��ã�� ��ũ��
    public CRoomScreen room;   // �� ��ũ��
    #endregion

    // ��ũ���� �̸����� ����
    private Dictionary<string, GameObject> screens;


    private void Awake()
    {
        // �̱��� �ν��Ͻ� ����
        Instance = this;

        // ��ųʸ��� �ʱ�ȭ ��ũ�� �̸��� Ű��, �Ҵ��� ������Ʈ�� ������ �߰�
        screens = new Dictionary<string, GameObject>
        {
            {"Login", login.gameObject },
            {"Menu",menu.gameObject},
            {"Find",find.gameObject},
            {"Room", room.gameObject }
        };
        // ���� ���۵� �� �α��� ȭ������ ����
        ScreenOpen("Login");

        // ��Ʈ��ũ �̺�Ʈ�� ���� ������ �ݹ� Ÿ������ ���
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    /// <summary>
    /// Ư�� ��ũ���� �̸����� ���� ���� �Լ�
    /// </summary>
    /// <param name="screenName"></param>
    public void ScreenOpen(string screenName)
    {
        // ��� ��ũ���� ��ȸ�� ��ũ�� �̸��� ��ġ�ϸ� Ȱ��ȭ ���� ����
        foreach(var row in screens)
        {
            row.Value.SetActive(row.Key.Equals(screenName));
        }
    }

    /// <summary>
    /// ���濡 ������� �� �ҷ����� �Լ�
    /// </summary>
    public override void OnConnected()
    {
        print("���θ޴� ����");
        ScreenOpen("Menu");
    }

    /// <summary>
    /// ���濡 ������ ������ �� �ҷ����� �Լ�
    /// </summary>
    /// <param name="cause"></param>
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"disconnected cause : {cause}");
        ScreenOpen("Login");
    }

    /// <summary>
    /// ���� �κ� �������� �� �ҷ����� �Լ�
    /// </summary>
    public override void OnJoinedLobby()
    {
        print("�κ� ����");
        ScreenOpen("Find");
        // PhotonNetwork.LoadLevel("Lobby");
    }

    /// <summary>
    /// ���� �κ񿡼� ������ �� �ҷ����� �Լ�
    /// </summary>
    public override void OnLeftLobby()
    {
        print("���θ޴� ����");
        ScreenOpen("Menu");
    }

    /// <summary>
    /// ���� �濡 �������� �� �ҷ����� �Լ�
    /// </summary>
    public override void OnJoinedRoom()
    {
        print("�뿡 ����");
        ScreenOpen("Room");
        SceneManager.LoadScene("Multi Lobby");
        
    }

    /// <summary>
    /// ���� �� ������ �������� �� �ҷ����� �Լ�
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("���� ������ ���� �����Ƿ� �� ����");
        RoomOptions option = new()
        {
            MaxPlayers = 4
        };
        string roomName = $"Random Room {Random.Range(100, 300)}";
        PhotonNetwork.CreateRoom(roomName: roomName, roomOptions: option);
    }

    /// <summary>
    /// ���� ���� �������� �� �ҷ����� �Լ�
    /// </summary>
    public override void OnCreatedRoom()
    {
        print("�뿡 ����");
        base.OnCreatedRoom();
        
        ScreenOpen("Room");

    }

    /// <summary>
    /// ���� �濡�� ������ �� �ҷ����� �Լ�
    /// </summary>
    public override void OnLeftRoom()
    {
        print("���θ޴� ����");
        ScreenOpen("Menu");
    }

    /// <summary>
    /// �ٸ� �÷��̾ ���� �濡 �������� �� �ҷ����� �Լ�
    /// </summary>
    /// <param name="newPlayer"></param>
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        room.JoinPlayer(newPlayer);
        // ���ο� �÷��̾� 3D ������Ʈ ����
        Vector3 spawnPosition = new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
        GameObject playerObject = PhotonNetwork.Instantiate("PlayerPrefab", spawnPosition, Quaternion.identity);
    }

    /// <summary>
    /// �ٸ� �÷��̾ ���� �濡 ������ �� �ҷ����� �Լ�
    /// </summary>
    /// <param name="otherPlayer"></param>
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        room.LeavePlayer(otherPlayer);
        // ���� �÷��̾��� 3D ������Ʈ ����
        PhotonNetwork.DestroyPlayerObjects(otherPlayer);
    }

    /// <summary>
    /// �κ񿡼� ��� ������ �� ����� ������Ʈ ���� �� �ҷ����� �Լ�
    /// </summary>
    /// <param name="roomList"></param>
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        find.UpdateRoomList(roomList);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        ScreenOpen("Menu");
    }
}

