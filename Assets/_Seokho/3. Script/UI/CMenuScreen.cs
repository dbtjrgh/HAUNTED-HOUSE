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
        // ���θ޴� ����
        changeNickNameButton.onClick.AddListener(PlayerNameChangeButtonClick);
        createRoomScreenButton.onClick.AddListener(CreateRoomScreenButtonClick);
        findRoomButton.onClick.AddListener(FindRoomButtonClick);
        joinRandomRoomButton.onClick.AddListener(JoinRandomRoomButtonClick);
        quitButton.onClick.AddListener(QuitButtonClick);
        MenubackButton.onClick.AddListener(OnBackButtonClick);

        // �游��� ����
        createRoomButton.onClick.AddListener(CreateRoomButtonClick);
        createRoombackButton.onClick.AddListener(OnBackButtonClick);
    }

    private void OnEnable()
    {
        playerName.text = PhotonNetwork.LocalPlayer.NickName;
        mainMenuScreen.gameObject.SetActive(true);
        createRoomScreen.gameObject.SetActive(false);
    }

    #region ���θ޴� ��ũ�� �Լ�

    /// <summary>
    /// ���� �޴� ��ũ���� ��� ��ư �Լ� : ���� �α׾ƿ�
    /// �� ���� ��ũ���� ��� ��ư �Լ� : ���� �޴��� �̵�
    /// </summary>
    public void OnBackButtonClick()
    {
        // ���� �޴� ��ũ���� ��� ��ư
        if(mainMenuScreen.gameObject.activeSelf)
        {
            mainMenuScreen.gameObject.SetActive(false);
            PhotonNetwork.Disconnect();
            
        }

        // �� ���� ��ũ���� ��� ��ư
        else if(createRoomScreen.gameObject.activeSelf)
        {          
            mainMenuScreen.gameObject.SetActive(true);
            createRoomScreen.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// createRoomScreen���� �̵��ϴ� ��ư �Լ�
    /// </summary>
    public void CreateRoomScreenButtonClick()
    {
        InfoText.text = "�� ����� ����";
        mainMenuScreen.gameObject.SetActive(false);
        createRoomScreen.gameObject.SetActive(true);
    }

    /// <summary>
    /// �г��� �ٲٴ� ��ư �Լ�
    /// </summary>
    public void PlayerNameChangeButtonClick()
    {
        InfoText.text = "�г��� ���� �Ϸ�";
        PhotonNetwork.NickName = playerNameInput.text;
        PhotonNetwork.ConnectUsingSettings();
        playerName.text = PhotonNetwork.LocalPlayer.NickName;
    }

    /// <summary>
    /// FindRoomScreen���� �̵��ϴ� ��ư �Լ�
    /// </summary>
    public void FindRoomButtonClick()
    {
        PhotonNetwork.JoinLobby();
    }

    /// <summary>
    /// ���� ���� ��ư �Լ�
    /// ���� ���ٸ� CBoardManager���� OnJoinRandomFailed�Լ��� ������ �ڵ����� ���� ����
    /// </summary>
    public void JoinRandomRoomButtonClick()
    {
        if(PhotonNetwork.IsConnected)
        {
            InfoText.text = "�뿡 ���� �õ�";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            InfoText.text = "������ ������ ������� ����.\n���� ��õ� ��";
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    /// <summary>
    /// ���� ��ư Ŭ���� ���� ����
    /// </summary>
    public void QuitButtonClick()
    {
        Application.Quit();
    }

    #endregion



    #region �游��� ��ũ�� �Լ�
    /// <summary>
    /// ������ �̿��� �� ���� �Լ�
    /// �� ����� �ִ� �ο� ���� �ް� ȣ��
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
            maxPlayer = 4; // �⺻ �ִ� �÷��̾� �� ����
        }

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = (byte)maxPlayer,
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable { { "Diff", 0 }, { "Character", "Default" } },
            CustomRoomPropertiesForLobby = new string[] { "Diff", "Character" }
        };

        PhotonNetwork.CreateRoom(roomName, roomOptions);// �ִ��ο��� 0�����ϰų� 4�� �ʰ��� �� �����ο� 4������ ��ȯ.

    }
    #endregion

}
