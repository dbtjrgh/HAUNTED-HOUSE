using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UI.Journal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    #region
    public Transform startPositions;
    public GameObject defeatUI; // 모든 플레이어가 사망했을 때 표시할 UI
    public GameObject resultUI; // 투표를 완료했을 때 나오는 UI
    private CGameResultUI gameResultUI; // 게임 결과 UI
    private CMultiPlayer[] players; // 모든 플레이어 추적
    public CjournalBook journal;
    private CBoardManager boardManager;
    private CTruckButton truckButton; // CTruckButton 참조 추가
    private bool onOff = false;

    private Dictionary<int, mentalGaugeManager> playerMentalGaues;
    #endregion

    private void Awake()
    {
        boardManager = FindAnyObjectByType<CBoardManager>();
        SceneManager.sceneLoaded += boardManager.OnSceneLoaded;
        gameResultUI = resultUI.GetComponent<CGameResultUI>();

        defeatUI.SetActive(false);
        resultUI.SetActive(false);

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

        PhotonNetwork.Instantiate("MultiPlayer", pos, rot, 0);

        // CTruckButton 스크립트 찾기
        truckButton = FindObjectOfType<CTruckButton>();
        playerMentalGaues = new Dictionary<int, mentalGaugeManager>();
    }

    private void Update()
    {
        CheckAllPlayersDead();

        if (Input.GetKeyDown(KeyCode.J))
        {
            ToggleJournal();
        }

        // 예를 들어 특정 조건에서 트럭의 LevelEnd 실행
        if (CheckAllPlayersSelectedGhost())
        {
            if (!truckButton.TruckDoorOpen)
            {
                StartCoroutine(ShowResultUI());
                StartCoroutine(BGMClearSoundDelay());
            }
        }
    }

    /// <summary>
    /// 플레이어 저널 관련 로직
    /// </summary>
    private void ToggleJournal()
    {
        if (journal != null)
        {
            onOff = !onOff; // 상태 토글

            if (onOff)
            {
                // Journal이 활성화될 때
                Cursor.visible = true; // 커서 보이게 설정
                Cursor.lockState = CursorLockMode.None; // 커서 이동 가능
            }
            else
            {
                // Journal이 비활성화될 때
                Cursor.visible = false; // 커서 숨기기
                Cursor.lockState = CursorLockMode.Locked; // 커서 고정
            }

            journal.gameObject.SetActive(onOff); // journalBook의 활성화 상태 설정
        }
    }

    /// <summary>
    /// 게임 실패 관련 로직
    /// </summary>
    private void ShowDeathUI()
    {

        defeatUI.SetActive(true);

        SoundManager.instance.StopGameSceneMusic();
        SoundManager.instance.PlayFailSceneMusic();

    }
    private IEnumerator ShowDeathUIAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Cursor.lockState = CursorLockMode.Confined;
        ShowDeathUI();
    }
    // 모든 플레이어가 사망했는지 체크하는 함수
    public void CheckAllPlayersDead()
    {
        players = FindObjectsOfType<CMultiPlayer>();

        bool allDead = true; // 모든 플레이어가 사망했다고 가정

        foreach (CMultiPlayer player in players)
        {
            if (!player.isDead)
            {
                allDead = false; // 살아있는 플레이어가 있으면 false로 설정
                break;
            }
        }

        if (allDead)
        {
            StartCoroutine(ShowDeathUIAfterDelay(5f)); // 5초 지연 후 UI 표시
        }
    }


    /// <summary>
    /// 게임 결과 관련 로직
    /// </summary>
    public IEnumerator ShowResultUI()
    {
        yield return new WaitForSeconds(5f);
        resultUI.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
    }
    public bool CheckAllPlayersSelectedGhost()
    {
        if (journal.ghostSelected)
        {
            return true; // 귀신 토글 중 하나라도 켜져 있으면 true 반환
        }
        return false; // 귀신 토글이 켜져 있지 않으면 false 반환
    }

    IEnumerator BGMClearSoundDelay()
    {
        yield return new WaitForSeconds(5f);
        SoundManager.instance.PlayClearSceneMusic();
    }

    IEnumerator BGMDeathSoundDelay()
    {
        yield return new WaitForSeconds(5f);
        SoundManager.instance.PlayFailSceneMusic();
    }


}
