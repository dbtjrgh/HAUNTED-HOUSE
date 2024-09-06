using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public static CBoardManager instance = null;
    public Transform startPositions;
    public TextMeshProUGUI InfoText;
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

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
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
        InfoText.text = "���θ޴� ����";
        ScreenOpen("Menu");
    }

    /// <summary>
    /// ���濡 ������ ������ �� �ҷ����� �Լ�
    /// </summary>
    /// <param name="cause"></param>
    public override void OnDisconnected(DisconnectCause cause)
    {
        InfoText.text = "������ ������ϴ�.";
        ScreenOpen("Login");
    }

    /// <summary>
    /// ���� �κ� �������� �� �ҷ����� �Լ�
    /// </summary>
    public override void OnJoinedLobby()
    {
        InfoText.text = "�κ� ����";
        ScreenOpen("Find");
    }

    /// <summary>
    /// ���� �κ񿡼� ������ �� �ҷ����� �Լ�
    /// </summary>
    public override void OnLeftLobby()
    {
        InfoText.text = "���θ޴� ����";
        ScreenOpen("Menu");
    }

    /// <summary>
    /// ���� �濡 �������� �� �ҷ����� �Լ�
    /// </summary>
    public override void OnJoinedRoom()
    {
        InfoText.text = "�뿡 ����";
        ScreenOpen("Room");
        // �� �ε� �� �÷��̾� �����ϵ��� ����
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LoadLevel("MultiLobby");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MultiLobby")
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;

            GameObject startPositionsObject = GameObject.Find("PlayerStartPositions");
            if (startPositionsObject == null)
            {
                return;
            }

            startPositions = startPositionsObject.GetComponent<Transform>();
            if (startPositions == null)
            {
                return;
            }

            Vector3 pos = startPositions.position;
            Quaternion rot = startPositions.rotation;

            // �÷��̾� ������ �ν��Ͻ�ȭ
            GameObject playerPrefab = PhotonNetwork.Instantiate("MultiPlayer", pos, rot, 0);

            // ���� �÷��̾ �ƴ� ��� ���̾ "OtherPlayer"�� ����
            PhotonView photonView = playerPrefab.GetComponent<PhotonView>();
            if (photonView != null && !photonView.IsMine)
            {
                SetLayerRecursively(playerPrefab, LayerMask.NameToLayer("MultiPlayer"));
            }
        }
    }

    // �������� ���̾ ��������� �����ϴ� �Լ�
    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (child != null)
            {
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }
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
        InfoText.text = "�뿡 ����";
        base.OnCreatedRoom();
        
        ScreenOpen("Room");

    }

    /// <summary>
    /// ���� �濡�� ������ �� �ҷ����� �Լ�
    /// </summary>
    public override void OnLeftRoom()
    {

        InfoText.text = "���θ޴� ����";
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

        // ���ο� �÷��̾��� "Ready" ���¸� ����ȭ
        if (newPlayer.CustomProperties.ContainsKey("Ready"))
        {
            room.SetPlayerReady(newPlayer.ActorNumber, (bool)newPlayer.CustomProperties["Ready"]);
        }

        
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
    {        find.UpdateRoomList(roomList);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        ScreenOpen("Menu");
    }
}

