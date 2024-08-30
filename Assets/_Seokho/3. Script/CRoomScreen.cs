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
    #region 변수
    public TextMeshProUGUI roomTitleText;
    public RectTransform playerList;
    public GameObject playerTextPrefab;

    public Button startButton;
    public Button exitButton;
    public TMP_Dropdown diffDropdown;
    public TextMeshProUGUI diffText;

    // 방장일 경우, 플레이어들의 ready 상태를 저장할 Dictionary
    private Dictionary<int, bool> playersReady;

    // 방에 들어온 모든 플레이어들이 서로를 알고 있도록 사용할 Dictionary
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

        // 만약 방장이라면
        if(PhotonNetwork.IsMasterClient)
        {
            playersReady = new Dictionary<int, bool>();
        }
        else
        {
            // 방장이 아닌 상태
        }
            // 방장이 아니면 게임 시작 버튼 및 난이도 조절 드롭다운 비활성화
            startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
            diffDropdown.gameObject.SetActive(PhotonNetwork.IsMasterClient);
            diffText.gameObject.SetActive(!PhotonNetwork.IsMasterClient);

        foreach (Photon.Realtime.Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            // 플레이어 목록에 플레이어 이름표 하나씩 생성
            JoinPlayer(player);
            if (player.CustomProperties.ContainsKey("Ready"))
            {
                SetPlayerReady(player.ActorNumber, (bool)player.CustomProperties["Ready"]);
            }
        }
        // 방에 입장 했을 때, 방장의 씬 로드 여부에 따라 함께 씬 로드
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
            // 내 엔트리일 경우
            toggle.onValueChanged.AddListener(ReadyToggleClick);
        }
        else
        {
            // 내가 아닌 다른 플레이어의 엔트리
            toggle.gameObject.SetActive(false);
        }

        playerEntries[newPlayer.ActorNumber] = playerEntry;

        // 방장일 경우 새로운 플레이어의 준비 상태 Off
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
    /// 같은 위치에 있는 오브젝트들을 대상
    /// transform.SetSiblingIndex : 해당 오브젝트의 순위를 얻어옴.
    /// </summary>
    public void SortPlayers()
    {
        foreach(int actorNumber in playerEntries.Keys)
        {
            playerEntries[actorNumber].transform.SetSiblingIndex(actorNumber);
        }
    }

    /// <summary>
    /// 게임 시작 버튼
    /// </summary>
    public void StartButtonClick()
    {
        // Photon을 통해 플레이어들과 씬을 동기화하여 로드
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("MultiLobby");
        }
    }

    /// <summary>
    /// 게임 나가기 버튼
    /// </summary>
    public void QuitButtonClick()
    {
        PhotonNetwork.LeaveRoom();
        // 시간 지연으로 인해 방을 나갔는데 방장의 시작 콜에 의해 씬이 넘어가는 것을 방지
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
    /// 방장이 나갔을 때 호출되는 함수로, 방에 참가되어 있는 상태에서 방장의 역할을
    /// 수행 할 수 있도록 유효성 검사 및 추가 조치 요구
    ///  Ex : playerReady에 Dictionary객체 생성 등등
    /// </summary>
    /// <param name="newMasterClient"></param>
    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {

    }


}
