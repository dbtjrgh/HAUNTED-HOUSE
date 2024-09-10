using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    #region ����
    public Transform startPositions;
    public GameObject deathUI; // ��� �÷��̾ ������� �� ǥ���� UI
    public Button backButton;
    private CMultiPlayer[] players; // ��� �÷��̾� ����
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
        deathUI.SetActive(false); // ó������ ��� UI ��Ȱ��ȭ
    }

    private void Update()
    {
        CheckAllPlayersDead();
    }

    // ��� �÷��̾ ����ߴ��� üũ�ϴ� �Լ�
    public void CheckAllPlayersDead()
    {
        // ���� ���� �ִ� ��� �÷��̾� ��ü ã��
        players = FindObjectsOfType<CMultiPlayer>();

        bool allDead = true; // ��� �÷��̾ ����ߴٰ� ����

        // ��� �÷��̾��� ���¸� Ȯ��
        foreach (CMultiPlayer player in players)
        {
            if (!player.isDead)
            {
                allDead = false; // ����ִ� �÷��̾ ������ false�� ����
                break;
            }
        }

        // ��� �÷��̾ ����ߴٸ� UI ǥ��
        if (allDead)
        {
            StartCoroutine(ShowDeathUIAfterDelay(5f)); // 5�� ���� �� UI ǥ��
        }
    }

    // 5�� �Ŀ� ��� UI�� ǥ���ϴ� �ڷ�ƾ �Լ�
    private IEnumerator ShowDeathUIAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 5�� ���
        ShowDeathUI(); // 5�� �� UI ǥ��
        Cursor.lockState = CursorLockMode.Confined;
    }

    // ��� UI�� ǥ���ϴ� �Լ�
    private void ShowDeathUI()
    {
        deathUI.SetActive(true); // ��� UI Ȱ��ȭ
    }

    public void OnBackButtonClick()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("MultiLobby");
    }
}
