using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public enum Difficulty
{
    Easy,
    Normal,
    Hard
}

public class CRoomScreen : MonoBehaviourPunCallbacks
{
    #region ����
    public TextMeshProUGUI roomTitleText;
    public RectTransform playerList;
    public GameObject playerTextPrefab;

    public Button startButton;
    public Button exitButton;
    public TMP_Dropdown diffDropdown;
    public TextMeshProUGUI diffText;

    // ������ ���, �÷��̾���� ready ���¸� ������ Dictionary
    private Dictionary<int, bool> playersReady;

    // �濡 ���� ��� �÷��̾���� ���θ� �˰� �ֵ��� ����� Dictionary
    public Dictionary<int, CPlayerEntry> playerEntries = new();

    #endregion

    private void Awake()
    {
        startButton.onClick.AddListener(StartButtonClick);
        exitButton.onClick.AddListener(QuitButtonClick);
        diffDropdown.ClearOptions();

        foreach(object diff in Enum.GetValues(typeof(Difficulty)))
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(diff.ToString());
            diffDropdown.options.Add(option);
        }

        diffDropdown.onValueChanged.AddListener(DifficultyValueChange);
    }

    public override void OnDisable()
    {
        base.OnDisable();

        foreach(Transform child in playerList)
        {
            Destroy(child.gameObject);
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();

        roomTitleText.text = PhotonNetwork.CurrentRoom.Name;

        // ���� �����̶��
        if(PhotonNetwork.IsMasterClient)
        {
            playersReady = new Dictionary<int, bool>();
        }
        else
        {
            // ������ �ƴ� ����
        }
            // ������ �ƴϸ� ���� ���� ��ư �� ���̵� ���� ��Ӵٿ� ��Ȱ��ȭ
            startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
            diffDropdown.gameObject.SetActive(PhotonNetwork.IsMasterClient);
            diffText.gameObject.SetActive(!PhotonNetwork.IsMasterClient);

        foreach (Photon.Realtime.Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            // �÷��̾� ��Ͽ� �÷��̾� �̸�ǥ �ϳ��� ����
            JoinPlayer(player);
            if (player.CustomProperties.ContainsKey("Ready"))
            {
                SetPlayerReady(player.ActorNumber, (bool)player.CustomProperties["Ready"]);
            }
        }
        // �濡 ���� ���� ��, ������ �� �ε� ���ο� ���� �Բ� �� �ε�
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void JoinPlayer(Photon.Realtime.Player newPlayer)
    {
        var playerEntry = Instantiate(playerTextPrefab, playerList, false).GetComponent<CPlayerEntry>();
        playerEntry.player = newPlayer;
        playerEntry.playerNameText.text = newPlayer.NickName;

        var toggle = playerEntry.readyToggle;

        if(PhotonNetwork.LocalPlayer.ActorNumber == newPlayer.ActorNumber)
        {
            // �� ��Ʈ���� ���
            toggle.onValueChanged.AddListener(ReadyToggleClick);
        }
        else
        {
            // ���� �ƴ� �ٸ� �÷��̾��� ��Ʈ��
            toggle.gameObject.SetActive(false);
        }

        playerEntries[newPlayer.ActorNumber] = playerEntry;

        // ������ ��� ���ο� �÷��̾��� �غ� ���� Off
        if(PhotonNetwork.IsMasterClient)
        {
            playersReady[newPlayer.ActorNumber] = false;
            CheckReady();
        }

        SortPlayers();

    }

    public void LeavePlayer(Photon.Realtime.Player leavePlayer)
    {
        GameObject leaveTarget = playerEntries[leavePlayer.ActorNumber].gameObject;
        playerEntries.Remove(leavePlayer.ActorNumber);
        Destroy(leaveTarget);

        if (PhotonNetwork.IsMasterClient)
        {
            playersReady.Remove(leavePlayer.ActorNumber);
            CheckReady();
        }

        SortPlayers();
    }

    /// <summary>
    /// ���� ��ġ�� �ִ� ������Ʈ���� ���
    /// transform.SetSiblingIndex : �ش� ������Ʈ�� ������ ����.
    /// </summary>
    public void SortPlayers()
    {
        foreach(int actorNumber in playerEntries.Keys)
        {
            playerEntries[actorNumber].transform.SetSiblingIndex(actorNumber);
        }
    }

    /// <summary>
    /// ���� ���� ��ư
    /// </summary>
    public void StartButtonClick()
    {
        // Photon�� ���� �÷��̾��� ���� ����ȭ�Ͽ� �ε�
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("MultiLobby");
        }
    }

    /// <summary>
    /// ���� ������ ��ư
    /// </summary>
    public void QuitButtonClick()
    {
        PhotonNetwork.LeaveRoom();
        // �ð� �������� ���� ���� �����µ� ������ ���� �ݿ� ���� ���� �Ѿ�� ���� ����
        PhotonNetwork.AutomaticallySyncScene = false;
    }

    public void ReadyToggleClick(bool isOn)
    {

    }

    public void SetPlayerReady(int actorNum, bool isReady)
    {

    }

    public void CheckReady()
    {

    }

    private void DifficultyValueChange(int value)
    {

    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, PhotonHashtable changedProps)
    {

    }

    public override void OnRoomPropertiesUpdate(PhotonHashtable props)
    {
        if(props.ContainsKey("Diff"))
        {
            print($"room difficulty changed: {props["Diff"]}");
            diffText.text = ((Difficulty)props["Diff"]).ToString();
        }
    }

    public override void OnJoinedRoom()
    {
        
    }

    /// <summary>
    /// ������ ������ �� ȣ��Ǵ� �Լ���, �濡 �����Ǿ� �ִ� ���¿��� ������ ������
    /// ���� �� �� �ֵ��� ��ȿ�� �˻� �� �߰� ��ġ �䱸
    ///  Ex : playerReady�� Dictionary��ü ���� ���
    /// </summary>
    /// <param name="newMasterClient"></param>
    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {

    }


}
