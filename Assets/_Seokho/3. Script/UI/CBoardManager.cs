using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CBoardManager : MonoBehaviourPunCallbacks
{

    // 싱글톤 패턴 CBoardManager 인스턴스 전역적으로 접근 가능
    public static CBoardManager Instance { get; private set; }

    #region 변수
    [Header("Screen")]
    public GameObject BoardCanvas; 
    public CLoginScreen login; // 로그인 스크린
    public CMenuScreen menu;   // 메뉴 스크린
    public CFindRoom find;     // 방찾기 스크린
    public CRoomScreen room;   // 방 스크린
    public static CBoardManager instance = null;
    public Transform startPositions;
    public TextMeshProUGUI InfoText;
    #endregion

    // 스크린을 이름으로 관리
    private Dictionary<string, GameObject> screens;


    private void Awake()
    {

        // 싱글톤 인스턴스 설정
        Instance = this;

        // 딕셔너리를 초기화 스크린 이름을 키로, 할당한 오브젝트를 값으로 추가
        screens = new Dictionary<string, GameObject>
        {
            {"Login", login.gameObject },
            {"Menu",menu.gameObject},
            {"Find",find.gameObject},
            {"Room", room.gameObject }
        };
        // 게임 시작될 때 로그인 화면으로 시작
        ScreenOpen("Login");

        // 네트워크 이벤트를 위한 포톤의 콜백 타겟으로 등록
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
    /// 특정 스크린을 이름으로 열기 위한 함수
    /// </summary>
    /// <param name="screenName"></param>
    public void ScreenOpen(string screenName)
    {
        // 모든 스크린을 순회해 스크린 이름이 일치하면 활성화 상태 설정
        foreach(var row in screens)
        {
            row.Value.SetActive(row.Key.Equals(screenName));
        }
    }

    /// <summary>
    /// 포톤에 연결됐을 때 불러오는 함수
    /// </summary>
    public override void OnConnected()
    {
        InfoText.text = "메인메뉴 입장";
        ScreenOpen("Menu");
    }

    /// <summary>
    /// 포톤에 연결이 끊겼을 때 불러오는 함수
    /// </summary>
    /// <param name="cause"></param>
    public override void OnDisconnected(DisconnectCause cause)
    {
        InfoText.text = "연결이 끊겼습니다.";
        ScreenOpen("Login");
    }

    /// <summary>
    /// 포톤 로비에 입장했을 때 불러오는 함수
    /// </summary>
    public override void OnJoinedLobby()
    {
        InfoText.text = "로비 입장";
        ScreenOpen("Find");
    }

    /// <summary>
    /// 포톤 로비에서 나갔을 때 불러오는 함수
    /// </summary>
    public override void OnLeftLobby()
    {
        InfoText.text = "메인메뉴 입장";
        ScreenOpen("Menu");
    }

    /// <summary>
    /// 포톤 방에 입장했을 때 불러오는 함수
    /// </summary>
    public override void OnJoinedRoom()
    {
        InfoText.text = "룸에 입장";
        ScreenOpen("Room");
        // 씬 로드 후 플레이어 생성하도록 변경
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LoadLevel("MultiLobby");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    /// <summary>
    /// 씬 로드가 되면 불러오는 함수
    /// 맵 선택한 것에 따라 불러오는 씬이 다름
    /// 씬 불러올 때 플레이어 형성
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 이름이 SingleLobby일 경우 BoardCanvas 비활성화
        if (scene.name == "Turkwood" || scene.name == "Factory")
        {
            BoardCanvas.gameObject.SetActive(false);
            return; // 다른 작업을 하지 않도록 리턴
        }

        if (scene.name == "MultiLobby")
        {
            BoardCanvas.gameObject.SetActive(true);
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SoundManager.instance.musicSource.Stop();

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

            // 플레이어 프리팹 인스턴스화
            GameObject playerPrefab = PhotonNetwork.Instantiate("MultiPlayer", pos, rot, 0);

        }
    }

    /// <summary>
    /// 랜덤 방 참가에 실패했을 때 불러오는 함수
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnJoinRandomFailed(short returnCode, string message)
    {

        RoomOptions option = new()
        {
            MaxPlayers = 4
        };
        string roomName = $"Random Room {Random.Range(100, 300)}";
        PhotonNetwork.CreateRoom(roomName: roomName, roomOptions: option);
    }

    /// <summary>
    /// 포톤 방을 생성했을 때 불러오는 함수
    /// </summary>
    public override void OnCreatedRoom()
    {
        InfoText.text = "룸에 입장";
        base.OnCreatedRoom();
        
        ScreenOpen("Room");

    }

    /// <summary>
    /// 포톤 방에서 나갔을 때 불러오는 함수
    /// </summary>
    public override void OnLeftRoom()
    {

        InfoText.text = "메인메뉴 입장";
        ScreenOpen("Menu");
    }

    /// <summary>
    /// 다른 플레이어가 현재 방에 입장했을 때 불러오는 함수
    /// </summary>
    /// <param name="newPlayer"></param>
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        room.JoinPlayer(newPlayer);

        // 새로운 플레이어의 "Ready" 상태를 동기화
        if (newPlayer.CustomProperties.ContainsKey("Ready"))
        {
            room.SetPlayerReady(newPlayer.ActorNumber, (bool)newPlayer.CustomProperties["Ready"]);
        }

        
    }
    /// <summary>
    /// 다른 플레이어가 현재 방에 나갔을 때 불러오는 함수
    /// </summary>
    /// <param name="otherPlayer"></param>
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        room.LeavePlayer(otherPlayer);
        // 떠난 플레이어의 3D 오브젝트 제거
        PhotonNetwork.DestroyPlayerObjects(otherPlayer);
    }

    /// <summary>
    /// 로비에서 사용 가능한 방 목록이 업데이트 됐을 때 불러오는 함수
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

