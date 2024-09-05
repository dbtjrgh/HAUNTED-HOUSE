using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CChatManager : MonoBehaviourPunCallbacks
{
    #region 변수
    public Button sendBtn; //채팅 입력버튼
    public TextMeshProUGUI chatLog; //채팅 내역
    public TMP_InputField inputField; //채팅입력 인풋필드
    public TextMeshProUGUI playerList; //참가자 목록
    string players; //참가자들

    public static CChatManager instance = null;
    ScrollRect scroll_rect = null; //채팅이 많이 쌓일 경우 스크롤바의 위치를 아래로 고정하기 위함
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
    }

    /// <summary>
    /// chatterUpdate(); 메소드로 주기적으로 플레이어 리스트를 업데이트하며
    /// input에 포커스가 맞춰져있고 엔터키가 눌려졌을 경우에도 SendButtonOnClicked(); 메소드를 실행.
    /// </summary>
    void Update()
    {
        ChatterUpdate();
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && !inputField.isFocused)
        {
            SendButtonOnClicked();
        }
    }

    /// <summary>
    /// 전송 버튼이 눌리면 실행될 메소드. 메세지 전송을 담당함.
    /// input이 비어있으면 아무것도 전송하지 않고, 비어있지 않다면
    /// "[ID] 메세지"의 형식으로 메세지를 전송함.
    /// 메세지 전송은 photonView.RPC 메소드를 이용해 각 유저들에게 ReceiveMsg 메소드를 실행하게 함.
    /// 자기 자신에게도 메세지를 띄워야 하므로 ReceiveMsg(msg);를 실행함.
    /// input.ActivateInputField();는 메세지 전송 후 바로 메세지를 입력할 수 있게 포커스를 Input Field로 옮김 (편의 기능)
    /// 그 후 input.text를 빈 칸으로 만듦
    /// </summary>
    public void SendButtonOnClicked()
    {
        if (inputField.text.Equals(""))
        {
            Debug.Log("Empty");
            return;
        }
        string msg = string.Format("[{0}] {1}", PhotonNetwork.LocalPlayer.NickName, inputField.text);
        photonView.RPC("ReceiveMsg", RpcTarget.OthersBuffered, msg);
        ReceiveMsg(msg);
        inputField.ActivateInputField(); // 메세지 전송 후 바로 메세지를 입력할 수 있게 포커스를 Input Field로 옮기는 편의 기능
        inputField.text = "";
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
