using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CMenuScreen : MonoBehaviour
{
    public TextMeshProUGUI InfoText;

    [Header("Main Menu")]
    public RectTransform mainMenuScreen;
    public TextMeshProUGUI playerName;
    public TMP_InputField playerNameInput;
    public Button changeNickNameButton;
    public Button createRoomScreenButton;
    public Button findRoomButton;
    public Button joinRandomRoomButton;
    public Button quitButton;
    public Button MenubackButton;
   
    [Header("Create Room Menu")]
    public RectTransform createRoomScreen;
    public TMP_InputField roomNameInput;
    public TMP_InputField playerNumInput;
    public Button createRoomButton;
    public Button createRoombackButton;

    private void Awake()
    {
        // 메인메뉴 관련
        changeNickNameButton.onClick.AddListener(PlayerNameChangeButtonClick);
        createRoomScreenButton.onClick.AddListener(CreateRoomScreenButtonClick);
        findRoomButton.onClick.AddListener(FindRoomButtonClick);
        joinRandomRoomButton.onClick.AddListener(JoinRandomRoomButtonClick);
        quitButton.onClick.AddListener(QuitButtonClick);
        MenubackButton.onClick.AddListener(OnBackButtonClick);

        // 방만들기 관련
        createRoomButton.onClick.AddListener(CreateRoomButtonClick);
        createRoombackButton.onClick.AddListener(OnBackButtonClick);
    }

    private void OnEnable()
    {
        playerName.text = PhotonNetwork.LocalPlayer.NickName;
        mainMenuScreen.gameObject.SetActive(true);
        createRoomScreen.gameObject.SetActive(false);
    }

    #region 메인메뉴 스크린 함수

    /// <summary>
    /// 메인 메뉴 스크린의 취소 버튼 함수 : 포톤 로그아웃
    /// 방 생성 스크린의 취소 버튼 함수 : 메인 메뉴로 이동
    /// </summary>
    public void OnBackButtonClick()
    {
        // 메인 메뉴 스크린의 취소 버튼
        if(mainMenuScreen.gameObject.activeSelf)
        {
            mainMenuScreen.gameObject.SetActive(false);
            PhotonNetwork.Disconnect();
            
        }

        // 방 생성 스크린의 취소 버튼
        else if(createRoomScreen.gameObject.activeSelf)
        {          
            mainMenuScreen.gameObject.SetActive(true);
            createRoomScreen.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// createRoomScreen으로 이동하는 버튼 함수
    /// </summary>
    public void CreateRoomScreenButtonClick()
    {
        InfoText.text = "방 만들기 입장";
        mainMenuScreen.gameObject.SetActive(false);
        createRoomScreen.gameObject.SetActive(true);
    }

    /// <summary>
    /// 닉네임 바꾸는 버튼 함수
    /// </summary>
    public void PlayerNameChangeButtonClick()
    {
        InfoText.text = "닉네임 변경 완료";
        PhotonNetwork.NickName = playerNameInput.text;
        PhotonNetwork.ConnectUsingSettings();
        playerName.text = PhotonNetwork.LocalPlayer.NickName;
    }

    /// <summary>
    /// FindRoomScreen으로 이동하는 버튼 함수
    /// </summary>
    public void FindRoomButtonClick()
    {
        PhotonNetwork.JoinLobby();
    }

    /// <summary>
    /// 랜덤 참가 버튼 함수
    /// 방이 없다면 CBoardManager에서 OnJoinRandomFailed함수를 실행해 자동으로 방을 생성
    /// </summary>
    public void JoinRandomRoomButtonClick()
    {
        if(PhotonNetwork.IsConnected)
        {
            InfoText.text = "룸에 접속 시도";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            InfoText.text = "마스터 서버와 연결되지 않음.\n접속 재시동 중";
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    /// <summary>
    /// 종료 버튼 클릭시 게임 종료
    /// </summary>
    public void QuitButtonClick()
    {
        Application.Quit();
    }

    #endregion



    #region 방만들기 스크린 함수
    /// <summary>
    /// 포톤을 이용한 방 생성 함수
    /// 방 제목과 최대 인원 수를 받고 호출
    /// </summary>
    public void CreateRoomButtonClick()
    {
        string roomName = roomNameInput.text.Trim();
        int maxPlayer;

        if (string.IsNullOrEmpty(roomName))
        {
            roomName = $"Room {Random.Range(1000, 9999)}";
        }

        if (!int.TryParse(playerNumInput.text, out maxPlayer) || maxPlayer <= 0 || maxPlayer > 4)
        {
            maxPlayer = 4; // 기본 최대 플레이어 수 설정
        }

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = (byte)maxPlayer,
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable { { "Diff", 0 }, { "Character", "Default" } },
            CustomRoomPropertiesForLobby = new string[] { "Diff", "Character" }
        };

        PhotonNetwork.CreateRoom(roomName, roomOptions);// 최대인원이 0명이하거나 4명 초과할 때 권장인원 4명으로 변환.

    }
    #endregion

}
