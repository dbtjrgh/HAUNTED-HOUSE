using Photon.Pun;
using System.Collections;
using UI.Journal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    public Transform startPositions;
    public GameObject DefeatUI; // 모든 플레이어가 사망했을 때 표시할 UI
    public GameObject ResultUI; // 투표를 완료했을 때 나오는 UI
    private CMultiPlayer[] players; // 모든 플레이어 추적
    private bool onOff = false;
    public CjournalBook journal;
    private CBoardManager CBoardManager;
    private CTruckButton truckButton; // CTruckButton 참조 추가

    private void Awake()
    {
        CBoardManager = FindAnyObjectByType<CBoardManager>();
        SceneManager.sceneLoaded += CBoardManager.OnSceneLoaded;

        DefeatUI.SetActive(false);
        ResultUI.SetActive(false);

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
            if (truckButton != null)
            {
                StartCoroutine(truckButton.LevelEnd());
            }
        }
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

    public bool CheckAllPlayersSelectedGhost()
    {
        // CjournalBook에서 ghostToggleGroup에 있는 토글 상태 확인
        Toggle[] ghostToggles = journal.ghostToggleGroup.GetComponentsInChildren<Toggle>();

        foreach (Toggle toggle in ghostToggles)
        {
            if (toggle.isOn)
            {
                return true; // 귀신 토글 중 하나라도 켜져 있으면 true 반환
            }
        }

        return false; // 귀신 토글이 켜져 있지 않으면 false 반환
    }

    public void ShowResultUI()
    {
        ResultUI.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
    }

    private IEnumerator ShowDeathUIAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowDeathUI();
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void ShowDeathUI()
    {
        DefeatUI.SetActive(true);
    }
}
