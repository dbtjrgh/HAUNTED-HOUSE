using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    #region 변수
    public Transform startPositions;
    public GameObject deathUI; // 모든 플레이어가 사망했을 때 표시할 UI
    public Button backButton;
    private CMultiPlayer[] players; // 모든 플레이어 추적
    private CBoardManager CBoardManager;
    #endregion

    private void Awake()
    {
        backButton.onClick.AddListener(OnBackButtonClick);
        CBoardManager = FindAnyObjectByType<CBoardManager>();
        SceneManager.sceneLoaded += CBoardManager.OnSceneLoaded;
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
    }

    private void Start()
    {
        deathUI.SetActive(false); // 처음에는 사망 UI 비활성화
    }

    private void Update()
    {
        CheckAllPlayersDead();
    }

    // 모든 플레이어가 사망했는지 체크하는 함수
    public void CheckAllPlayersDead()
    {
        // 현재 씬에 있는 모든 플레이어 객체 찾기
        players = FindObjectsOfType<CMultiPlayer>();

        bool allDead = true; // 모든 플레이어가 사망했다고 가정

        // 모든 플레이어의 상태를 확인
        foreach (CMultiPlayer player in players)
        {
            if (!player.isDead)
            {
                allDead = false; // 살아있는 플레이어가 있으면 false로 설정
                break;
            }
        }

        // 모든 플레이어가 사망했다면 UI 표시
        if (allDead)
        {
            StartCoroutine(ShowDeathUIAfterDelay(5f)); // 5초 지연 후 UI 표시
        }
    }

    // 5초 후에 사망 UI를 표시하는 코루틴 함수
    private IEnumerator ShowDeathUIAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 5초 대기
        ShowDeathUI(); // 5초 후 UI 표시
        Cursor.lockState = CursorLockMode.Confined;
    }

    // 사망 UI를 표시하는 함수
    private void ShowDeathUI()
    {
        deathUI.SetActive(true); // 사망 UI 활성화
    }

    public void OnBackButtonClick()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("MultiLobby");
    }
}
