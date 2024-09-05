using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CChatManager : MonoBehaviourPunCallbacks
{
    #region 변수
    public Button sendBtn; // 채팅 입력버튼
    public TextMeshProUGUI chatLog; // 채팅 내역
    public InputField inputField; // 채팅입력 인풋필드
    public TextMeshProUGUI playerList; //참가자 목록
    public Canvas ChatCanvas; // 채팅 캔버스
    /// </summary>
    string players; // 참가자들

    public static CChatManager instance = null;
    ScrollRect scroll_rect = null; // 채팅이 많이 쌓일 경우 스크롤바의 위치를 아래로 고정하기 위함
    private bool hasSentMessage = false; // 메시지가 전송되었는지 여부를 추적하는 플래그

    #endregion
    private void Awake()
    {
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
    private void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;
        scroll_rect = GameObject.FindObjectOfType<ScrollRect>();
        sendBtn.onClick.AddListener(SendButtonOnClicked);
    }

    /// <summary>
    /// chatterUpdate(); 메소드로 주기적으로 플레이어 리스트를 업데이트하며
    /// input에 포커스가 맞춰져있고 엔터키가 눌려졌을 경우에도 SendButtonOnClicked(); 메소드를 실행.
    /// </summary>
    void Update()
    {
        ChatterUpdate();

        // 채팅창 열고 닫기, 메시지 전송 관련 함수를 처리
        HandleChatInput();

        // esc 키를 누르면 커서를 활성화
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    Cursor.visible = true;
        //}
    }

    /// <summary>
    /// 엔터키 입력을 처리하는 함수.
    /// 채팅창을 열고 닫거나 메시지를 전송하는 기능을 처리함.
    /// </summary>
    void HandleChatInput()
    {
        // 엔터를 눌렀을 때 이 오브젝트가 켜져있고, 인풋 필드가 비어있다면 채팅창을 비활성화
        if (ChatCanvas.enabled && Input.GetKeyUp(KeyCode.Return) && string.IsNullOrEmpty(inputField.text) && !hasSentMessage)
        {
            ChatCanvas.enabled = false;
            Cursor.visible = false;
        }
        // 채팅창이 꺼져있고, 엔터키를 눌렀다면 채팅창을 활성화하고 인풋 필드에 포커스를 맞춤
        else if (!ChatCanvas.enabled && Input.GetKeyUp(KeyCode.Return))
        {
            ChatCanvas.enabled = true;
            Cursor.visible = true;
            inputField.ActivateInputField();
            hasSentMessage = false; // 새로운 입력을 받기 위해 플래그 초기화
        }
        // 채팅창이 켜져있고, 인풋 필드가 비어있지 않다면 채팅을 전송하고 인풋 필드를 비움
        else if (ChatCanvas.enabled && Input.GetKeyUp(KeyCode.Return) && !string.IsNullOrEmpty(inputField.text))
        {
            SendButtonOnClicked(); // 채팅 전송 함수 호출
            hasSentMessage = true; // 메시지가 전송된 것으로 설정
        }
    }

    /// <summary>
    /// 전송 버튼이 눌리면 실행될 메소드. 메세지 전송을 담당함.
    /// </summary>
    public void SendButtonOnClicked()
    {
        if (inputField.text.Equals(""))
        {
            Debug.Log("Empty");
            return;
        }
        string msg = string.Format("{0} :{1}", PhotonNetwork.LocalPlayer.NickName, inputField.text);
        photonView.RPC("ReceiveMsg", RpcTarget.OthersBuffered, msg);
        ReceiveMsg(msg);
        inputField.text = ""; // 인풋 필드 초기화
        inputField.ActivateInputField(); // 메세지 전송 후 바로 메세지를 입력할 수 있게 포커스를 Input Field로 옮기는 편의 기능
        hasSentMessage = true; // 메시지가 전송됨을 표시
    }

    /// <summary>
    /// 채팅 참가자 목록을 업데이트 하는 함수.
    /// '참가자 목록' 텍스트 아래에 플레이어들의 ID를 더해주는 식으로 작동하며,
    /// 실시간으로 출입하는 유저들의 ID를 반영함.
    /// </summary>
    void ChatterUpdate()
    {
        players = "참가자 목록\n";
        foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
        {
            players += p.NickName + "\n";
        }
        playerList.text = players;
        
    }

    /// <summary>
    /// 포톤 방에서 나갔을 때 불러오는 함수
    /// </summary>
    public override void OnLeftRoom()
    {
        // 씬 로드 후 싱글로비 씬이면 이 오브젝트 파괴
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "SingleLobby")
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(this.gameObject);
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        string msg = string.Format("<color=#00ff00>[{0}]님이 입장하셨습니다.</color>", newPlayer.NickName);
        ReceiveMsg(msg);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        string msg = string.Format("<color=#ff0000>[{0}]님이 퇴장하셨습니다.</color>", otherPlayer.NickName);
        ReceiveMsg(msg);
    }

    [PunRPC]
    public void ReceiveMsg(string msg)
    {
        chatLog.text += "\n" + msg;
        StartCoroutine(ScrollUpdate());
    }


    IEnumerator ScrollUpdate()
    {
        yield return null;
        scroll_rect.verticalNormalizedPosition = 0.0f;
    }
}
