using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public Button readyButton;
    public Button mapButton;
    public TMP_Dropdown diffDropdown;
    public TextMeshProUGUI diffText;
    public TextMeshProUGUI infoText; // �� ���� ��� �޽����� ���� �ؽ�Ʈ
    public Text mapText; // ��� �÷��̾ Ȯ���� �� �ִ� ���õ� �� ���� �ؽ�Ʈ
    public GameObject chooseMapScreen; // �� ���� ȭ�� (��Ȱ��ȭ�� ���·� �ΰ�, ��ư�� ���� Ȱ��ȭ)
    private string selectedMap = ""; // ���õ� �� ����


    // ������ ���, �÷��̾���� ready ���¸� ������ Dictionary
    public Dictionary<int, bool> playersReady;

    // �濡 ���� ��� �÷��̾���� ���θ� �˰� �ֵ��� ����� Dictionary
    public Dictionary<int, CPlayerEntry> playerEntries = new();

    #endregion

    private void Awake()
    {
        startButton.onClick.AddListener(StartButtonClick);
        exitButton.onClick.AddListener(QuitButtonClick);
        readyButton.onClick.AddListener(OnReadyButtonClick);
        diffDropdown.ClearOptions();
        mapButton.onClick.AddListener(OpenMapSelection);

        foreach (object diff in Enum.GetValues(typeof(Difficulty)))
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

        // ������ �ƴϸ� ���� ���� ��ư �� ���̵� ���� ��Ӵٿ� ��Ȱ��ȭ
        startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        diffDropdown.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        mapButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);


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

        if (PhotonNetwork.LocalPlayer.ActorNumber == newPlayer.ActorNumber)
        {
            // �� ��Ʈ���� ���
            toggle.onValueChanged.AddListener(ReadyToggleClick);
            toggle.interactable = true; // �� ����� ��ȣ�ۿ� ����
        }
        else
        {
            // ���� �ƴ� �ٸ� �÷��̾��� ��Ʈ��
            toggle.interactable = false; // �ٸ� �÷��̾��� ����� ��Ȱ��ȭ (������ ���̵��� ����)
        }

        playerEntries[newPlayer.ActorNumber] = playerEntry;

        // ������ ��� ���ο� �÷��̾��� �غ� ���� Off
        if (PhotonNetwork.IsMasterClient)
        {
            playersReady[newPlayer.ActorNumber] = false;
            CheckReady();
        }

        SortPlayers();
    }

    private void OnReadyButtonClick()
    {
        var toggle = playerEntries[PhotonNetwork.LocalPlayer.ActorNumber].readyToggle;

        bool newState = !toggle.isOn;

        toggle.SetIsOnWithoutNotify(newState);

        ReadyToggleClick(newState);
    }

    public void LeavePlayer(Photon.Realtime.Player leavePlayer)
    {
        if (playerEntries.ContainsKey(leavePlayer.ActorNumber))
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
    }

    /// <summary>
    /// ���� ��ġ�� �ִ� ������Ʈ���� ���
    /// transform.SetSiblingIndex : �ش� ������Ʈ�� ������ ����.
    /// </summary>
    public void SortPlayers()
    {
        var sortedEntries = playerEntries.Values
            .OrderBy(entry => entry.player.ActorNumber)
            .ToList();

        for (int i = 0; i < sortedEntries.Count; i++)
        {
            if (sortedEntries[i] != null)
            {
                sortedEntries[i].transform.SetSiblingIndex(i);
            }
        }
    }

    /// <summary>
    /// ���� ���� ��ư
    /// </summary>
    public void StartButtonClick()
    {
        if (string.IsNullOrEmpty(selectedMap))
        {
            infoText.text = "���� �������ּ���!";
            return;
        }

        PhotonNetwork.AutomaticallySyncScene = true;

        if (PhotonNetwork.IsMasterClient && AllPlayersReady())
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            if(selectedMap == "����")
            {
                PhotonNetwork.LoadLevel("Factory");
            }
            else if(selectedMap == "����")
            {
                PhotonNetwork.LoadLevel("Turkwood");
            }
            
        }
    }

    

    private bool AllPlayersReady()
    {
        foreach (var playerReady in playersReady.Values)
        {
            if (!playerReady) return false;
        }
        return true;
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
        Photon.Realtime.Player localPlayer = PhotonNetwork.LocalPlayer;

        // PhotonNetwork�� customProperties�� Hashtable ������ Ȱ��
        // �׷��� dotnet�� HashTable�� �ƴ� ����ȭ ������ Hashtable Ŭ������ ���� ����

        PhotonHashtable customProps = localPlayer.CustomProperties;
        customProps["Ready"] = isOn;

        localPlayer.SetCustomProperties(customProps);
    }

    // �ٸ� �÷��̾ ReadyToggle�� �������� ��� �� Ŭ���̾�Ʈ���� �ݿ�.
    public void SetPlayerReady(int actorNumber, bool isReady)
{
    if (playerEntries.ContainsKey(actorNumber))
    {
        var playerEntry = playerEntries[actorNumber];
        playerEntry.readyToggle.SetIsOnWithoutNotify(isReady); // ����� ���¸� ������Ʈ

        if (PhotonNetwork.IsMasterClient)
        {
            playersReady[actorNumber] = isReady;
            CheckReady();
        }

        // �ڽ��� ����� ��ȣ�ۿ� �����ϰ� ����, �ٸ� �÷��̾�� ��Ȱ��ȭ
        if (actorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            playerEntry.readyToggle.interactable = false;
        }
    }
}

    public void CheckReady()
    {
        bool allReady = playersReady.Values.All(x => x); // ��� �÷��̾ �غ� �Ǿ����� �� ��ư Ȱ��ȭ
        bool anyReady = playersReady.Values.Any(x => x); // �Ѹ��̶� �غ� �Ǿ����� �� ��ư Ȱ��ȭ

        startButton.interactable = allReady && !string.IsNullOrEmpty(selectedMap); // �� ���� ���� üũ
    }

    private void DifficultyValueChange(int value)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        var customProps = PhotonNetwork.CurrentRoom.CustomProperties;
        customProps["Diff"] = value;
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProps);
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, PhotonHashtable changedProps)
    {
        if (changedProps.ContainsKey("Ready"))
        {
            SetPlayerReady(targetPlayer.ActorNumber, (bool)changedProps["Ready"]);
        }
    }

    public override void OnRoomPropertiesUpdate(PhotonHashtable props)
    {
        props = PhotonNetwork.CurrentRoom.CustomProperties;

        if (props.ContainsKey("Diff"))
        {
            print($"�� ���̵� ����� : {props["Diff"]}");
            diffText.text = ((Difficulty)props["Diff"]).ToString();
        }

        if (props.ContainsKey("Map"))
        {
            selectedMap = (string)props["Map"];
            mapText.text = $"{selectedMap}";

            // ���� ���õǸ� ��� �޽��� �����
            infoText.text = "";
        }

    }

    // �� ���� ȭ���� ���� ���� �޼���
    public void OpenMapSelection()
    {
        this.gameObject.SetActive(false);
        chooseMapScreen.gameObject.SetActive(true); // �� ���� ȭ�� Ȱ��ȭ
    }

    // �� ���� �� ȣ���� �޼���
    public void ChooseMap(string mapName)
    {
        selectedMap = mapName;

        // �� ���� ������ ��� �÷��̾�� ����
        var customProps = PhotonNetwork.CurrentRoom.CustomProperties;
        customProps["Map"] = mapName;
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProps);

        // �� �ؽ�Ʈ�� ���õ� �� ǥ��
        mapText.text = mapName;
    }




    /// <summary>
    /// ������ ������ �� ȣ��Ǵ� �Լ���, �濡 �����Ǿ� �ִ� ���¿��� ������ ������
    /// ���� �� �� �ֵ��� ��ȿ�� �˻� �� �߰� ��ġ �䱸
    ///  Ex : playerReady�� Dictionary��ü ���� ���
    /// </summary>
    /// <param name="newMasterClient"></param>
    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        Debug.Log($"new master client : {newMasterClient.NickName}");
        // newmasterClient= ismasterclient?

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("I'm master client");
        }
            
        if (playersReady == null)
        {
            playersReady = new Dictionary<int, bool>();
        }
        foreach (Photon.Realtime.Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            if (!playersReady.ContainsKey(player.ActorNumber))
            {
                playersReady.Add(player.ActorNumber, false);
            }
        }

        CheckReady();
        startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        diffDropdown.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        mapButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        SortPlayers();
    }


}