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
    public Text chatLog; // 채팅 내역
    public InputField inputField; // 채팅입력 인풋필드
    public TextMeshProUGUI playerList; //참가자 목록
    public Canvas ChatCanvas; // 채팅 캔버스
    /// </summary>
    string players; // 참가자들

    public static CChatManager instance = null;
    ScrollRect scroll_rect = null; // 채팅이 많이 쌓일 경우 스크롤바의 위치를 아래로 고정하기 위함

    #endregion

    /// <summary>
    /// 멀티 로비씬에서부터 시작
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject); // 오브젝트가 씬 로드 시 파괴되지 않도록 설정
        }
        else if (instance != this)
        {
            Destroy(this.gameObject); // 중복된 오브젝트는 파괴
        }
    }

    private void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;
        scroll_rect = GameObject.FindObjectOfType<ScrollRect>();

        if (scroll_rect == null)
        {
            Debug.LogError("ScrollRect를 찾을 수 없습니다.");
        }

        sendBtn.onClick.AddListener(SendButtonOnClicked);
        ChatCanvas.gameObject.SetActive(false); // 채팅 캔버스를 처음엔 비활성화
    }

    /// <summary>
    /// chatterUpdate(); 메소드로 주기적으로 플레이어 리스트를 업데이트하며
    /// input에 포커스가 맞춰져있고 엔터키가 눌려졌을 경우에도 SendButtonOnClicked(); 메소드를 실행.
    /// </summary>
    void Update()
    {
        if (!PhotonNetwork.IsConnected)
        {
            return;
        }
        ChatterUpdate();

        // O 키를 눌러 채팅 캔버스를 온/오프
        if (Input.GetKeyDown(KeyCode.O))
        {
            ToggleChatCanvas();
        }

        // 엔터키나 키패드 엔터키로 메시지를 전송
        if (Input.GetKeyDown(KeyCode.Return) && ChatCanvas.gameObject.activeSelf)
        {
            SendButtonOnClicked();
        }
        // 비활성화 일 때 엔터키를 누르면 활성화
        else if (Input.GetKeyDown(KeyCode.Return) && !ChatCanvas.gameObject.activeSelf)
        {
            ChatCanvas.gameObject.SetActive(true);
            inputField.ActivateInputField();
        }
    }

    /// <summary>
    /// 채팅 캔버스 오브젝트 자체를 온/오프 하는 함수
    /// </summary>
    void ToggleChatCanvas()
    {
        // 채팅 캔버스 오브젝트가 활성화되어 있으면 비활성화, 비활성화되어 있으면 활성화
        ChatCanvas.gameObject.SetActive(!ChatCanvas.gameObject.activeSelf);

        // 채팅 캔버스가 활성화된 경우 입력 필드에 포커스를 맞춤
        if (ChatCanvas.gameObject.activeSelf)
        {
            inputField.ActivateInputField();
        }
        
    }

    /// <summary>
    /// 전송 버튼이 눌리면 실행될 메소드. 메시지 전송을 담당함.
    /// </summary>
    public void SendButtonOnClicked()
    {
        // 입력된 메시지에서 개행 문자 제거
        string sanitizedMessage = inputField.text.Replace("\n", "").Replace("\r", "");

        // 입력 필드가 비어 있으면 채팅 오브젝트를 자동으로 비활성화
        if (string.IsNullOrWhiteSpace(sanitizedMessage))
        {
            Debug.Log("Empty message, closing chat.");
            ChatCanvas.gameObject.SetActive(false); // 입력 필드가 비어 있으면 오브젝트 비활성화
            return;
        }

        // 메시지 전송
        string msg = string.Format("{0} : {1}", PhotonNetwork.LocalPlayer.NickName, sanitizedMessage);
        photonView.RPC("ReceiveMsg", RpcTarget.OthersBuffered, msg);
        ReceiveMsg(msg);
        inputField.text = ""; // 인풋 필드 초기화
        inputField.ActivateInputField(); // 포커스를 유지하여 계속 입력할 수 있게 설정
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
    /// 플레이어가 입장헀을 때 불러오는 함수
    /// </summary>
    /// <param name="newPlayer"></param>
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        string msg = string.Format("<color=#00ff00>[{0}]님이 입장하셨습니다.</color>", newPlayer.NickName);
        ReceiveMsg(msg);
    }
    /// <summary>
    /// 플레이어가 퇴장헀을 때 불러오는 함수
    /// </summary>
    /// <param name="otherPlayer"></param>
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        string msg = string.Format("<color=#ff0000>[{0}]님이 퇴장하셨습니다.</color>", otherPlayer.NickName);
        ReceiveMsg(msg);
    }
    /// <summary>
    /// 채팅을 치고나서 채팅 입력창 공백처리
    /// </summary>
    /// <param name="msg"></param>
    [PunRPC]
    public void ReceiveMsg(string msg)
    {
        chatLog.text += "\n" + msg;
        StartCoroutine(ScrollUpdate());
    }

    /// <summary>
    /// 공백 이후 스크롤 초기화
    /// </summary>
    /// <returns></returns>
    IEnumerator ScrollUpdate()
    {
        yield return null;
        scroll_rect.verticalNormalizedPosition = 0.0f;
    }
}
