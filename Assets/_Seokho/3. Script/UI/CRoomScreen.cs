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
    #region 변수
    public TextMeshProUGUI roomTitleText;
    public RectTransform playerList;
    public GameObject playerTextPrefab;

    public Button startButton;
    public Button exitButton;
    public Button readyButton;
    public Button mapButton;
    public TMP_Dropdown diffDropdown;
    public TextMeshProUGUI diffText;
    public TextMeshProUGUI infoText; // 맵 선택 경고 메시지를 위한 텍스트
    public Text mapText; // 모든 플레이어가 확인할 수 있는 선택된 맵 정보 텍스트
    public GameObject chooseMapScreen; // 맵 선택 화면 (비활성화된 상태로 두고, 버튼을 통해 활성화)
    private string selectedMap = ""; // 선택된 맵 정보


    // 방장일 경우, 플레이어들의 ready 상태를 저장할 Dictionary
    public Dictionary<int, bool> playersReady;

    // 방에 들어온 모든 플레이어들이 서로를 알고 있도록 사용할 Dictionary
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

        // 처음에 난이도를 Easy로 설정 (첫 번째 인덱스)
        diffDropdown.value = (int)Difficulty.Easy;
        DifficultyValueChange((int)Difficulty.Easy); // 난이도 변경 메서드 호출
    }
    /// <summary>
    /// 이 오브젝트가 비활성화일 때 플레이어 리스트 파괴
    /// </summary>
    public override void OnDisable()
    {
        base.OnDisable();

        foreach (Transform child in playerList)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// 이 오브젝트가 활성화일 때 플레이어 리스트 생성
    /// </summary>
    public override void OnEnable()
    {
        base.OnEnable();

        roomTitleText.text = PhotonNetwork.CurrentRoom.Name;

        // 만약 방장이라면
        if (PhotonNetwork.IsMasterClient)
        {
            playersReady = new Dictionary<int, bool>();
        }

        // 방장이 아니면 게임 시작 버튼 및 난이도 조절 드롭다운 비활성화
        startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        diffDropdown.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        mapButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);



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
    /// <summary>
    /// 플레이어가 방에 입장했을 때 불러오는 함수로
    /// 미리 프리팹으로 형성해놓은 플레이어 텍스트 문구를 가져와서
    /// 리스트로 나열
    /// </summary>
    /// <param name="newPlayer"></param>
    public void JoinPlayer(Photon.Realtime.Player newPlayer)
    {
        var playerEntry = Instantiate(playerTextPrefab, playerList, false).GetComponent<CPlayerEntry>();
        playerEntry.player = newPlayer;
        playerEntry.playerNameText.text = newPlayer.NickName;

        var toggle = playerEntry.readyToggle;

        if (PhotonNetwork.LocalPlayer.ActorNumber == newPlayer.ActorNumber)
        {
            // 내 엔트리일 경우
            toggle.onValueChanged.AddListener(ReadyToggleClick);
            toggle.interactable = true; // 내 토글은 상호작용 가능
        }
        else
        {
            // 내가 아닌 다른 플레이어의 엔트리
            toggle.interactable = false; // 다른 플레이어의 토글은 비활성화 (하지만 보이도록 유지)
        }

        playerEntries[newPlayer.ActorNumber] = playerEntry;

        // 방장일 경우 새로운 플레이어의 준비 상태 Off
        if (PhotonNetwork.IsMasterClient)
        {
            playersReady[newPlayer.ActorNumber] = false;
            CheckReady();
        }

        SortPlayers();
    }
    /// <summary>
    /// 게임 준비 버튼으로 클릭하면 모두에게 공유되며
    /// 준비가 된 것을 마스터 클라이언트에게 전달한다.
    /// </summary>
    private void OnReadyButtonClick()
    {
        var toggle = playerEntries[PhotonNetwork.LocalPlayer.ActorNumber].readyToggle;

        bool newState = !toggle.isOn;

        toggle.SetIsOnWithoutNotify(newState);

        ReadyToggleClick(newState);
    }
    /// <summary>
    /// 플레이어가 방을 떠났을 때 불러오는 함수
    /// 프리팹으로 불러온 플레이어 텍스트를 파괴시킨다.
    /// </summary>
    /// <param name="leavePlayer"></param>
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
    /// 같은 위치에 있는 오브젝트들을 대상
    /// transform.SetSiblingIndex : 해당 오브젝트의 순위를 얻어옴.
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
    /// 게임 시작 버튼으로 모든 플레이어가 준비가 되고 맵이 선택돼 있어야 버튼이 활성화된다. 
    /// 맵 선택에 따라 로드되는 씬이 나뉘어진다.
    /// </summary>
    public void StartButtonClick()
    {
        if (string.IsNullOrEmpty(selectedMap))
        {
            infoText.text = "맵을 선택해주세요!";
            return;
        }

        PhotonNetwork.AutomaticallySyncScene = true;

        if (PhotonNetwork.IsMasterClient && AllPlayersReady())
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            if (selectedMap == "공장")
            {
                PhotonNetwork.LoadLevel("Factory");
                SoundManager.instance.PlayGameSceneMusic();
            }
            else if (selectedMap == "폐허")
            {
                PhotonNetwork.LoadLevel("Turkwood");
                SoundManager.instance.PlayGameSceneMusic();
            }

        }
    }
    /// <summary>
    /// 모든 플레이어가 준비를 했는지 체크하는 함수
    /// </summary>
    /// <returns></returns>
    private bool AllPlayersReady()
    {
        foreach (var playerReady in playersReady.Values)
        {
            if (!playerReady)
            {
                return false;
            }
        }
        return true;
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
    /// <summary>
    /// 해당 플레이어 닉네임 옆에 토글버튼으로 누르면 준비 버튼과 동일한 함수를 호출한다.
    /// </summary>
    /// <param name="isOn"></param>
    public void ReadyToggleClick(bool isOn)
    {
        Photon.Realtime.Player localPlayer = PhotonNetwork.LocalPlayer;

        // PhotonNetwork의 customProperties는 Hashtable 구조를 활용
        // 그러나 dotnet의 HashTable이 아닌 간소화 형태의 Hashtable 클래스를 직접 제공

        PhotonHashtable customProps = localPlayer.CustomProperties;
        customProps["Ready"] = isOn;

        localPlayer.SetCustomProperties(customProps);
    }

    /// <summary>
    /// 다른 플레이어가 ReadyToggle을 변경했을 경우 내 클라이언트에도 반영
    /// </summary>
    /// <param name="actorNumber"></param>
    /// <param name="isReady"></param>
    public void SetPlayerReady(int actorNumber, bool isReady)
    {
        if (playerEntries.ContainsKey(actorNumber))
        {
            var playerEntry = playerEntries[actorNumber];
            playerEntry.readyToggle.SetIsOnWithoutNotify(isReady); // 토글의 상태를 업데이트

            if (PhotonNetwork.IsMasterClient)
            {
                playersReady[actorNumber] = isReady;
                CheckReady();
            }

            // 자신의 토글은 상호작용 가능하게 유지, 다른 플레이어는 비활성화
            if (actorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
            {
                playerEntry.readyToggle.interactable = false;
            }
        }
    }

    /// <summary>
    /// 모든 플레이어가 준비됐는지 체크하는 함수
    /// </summary>
    public void CheckReady()
    {
        bool allReady = playersReady.Values.All(x => x); // 모든 플레이어가 준비가 되어있을 때 버튼 활성화
        bool anyReady = playersReady.Values.Any(x => x); // 한명이라도 준비가 되어있을 때 버튼 활성화

        startButton.interactable = allReady && !string.IsNullOrEmpty(selectedMap); // 맵 선택 여부 체크
    }
    /// <summary>
    /// 난이도 변경될 때 호출되는 함수
    /// </summary>
    /// <param name="value"></param>
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
    /// <summary>
    /// 플레이어가 준비할 때 호출되는 함수
    /// </summary>
    /// <param name="targetPlayer"></param>
    /// <param name="changedProps"></param>
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, PhotonHashtable changedProps)
    {
        if (changedProps.ContainsKey("Ready"))
        {
            SetPlayerReady(targetPlayer.ActorNumber, (bool)changedProps["Ready"]);
        }
    }
    /// <summary>
    /// 게임 난이도와 맵 선택 할 때마다 호출되는 함수
    /// </summary>
    /// <param name="props"></param>
    public override void OnRoomPropertiesUpdate(PhotonHashtable props)
    {
        props = PhotonNetwork.CurrentRoom.CustomProperties;

        if (props.ContainsKey("Diff"))
        {
            diffText.text = ((Difficulty)props["Diff"]).ToString();
        }

        if (props.ContainsKey("Map"))
        {
            selectedMap = (string)props["Map"];
            mapText.text = $"{selectedMap}";

            // 맵이 선택되면 경고 메시지 지우기
            infoText.text = "";
        }

    }

    /// <summary>
    /// 맵 선택 화면을 열기 위한 함수
    /// </summary>
    public void OpenMapSelection()
    {
        this.gameObject.SetActive(false);
        chooseMapScreen.gameObject.SetActive(true); // 맵 선택 화면 활성화
    }

    /// <summary>
    /// 맵 선택 후 호출할 함수
    /// </summary>
    /// <param name="mapName"></param>
    public void ChooseMap(string mapName)
    {
        selectedMap = mapName;

        // 맵 선택 정보를 모든 플레이어에게 공유
        var customProps = PhotonNetwork.CurrentRoom.CustomProperties;
        customProps["Map"] = mapName;
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProps);

        // 맵 텍스트에 선택된 맵 표시
        mapText.text = mapName;
    }
    /// <summary>
    /// 방장이 나갔을 때 호출되는 함수로, 방에 참가되어 있는 상태에서 방장의 역할을
    /// 수행 할 수 있도록 유효성 검사 및 추가 조치 요구
    ///  Ex : playerReady에 Dictionary객체 생성 등등
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
