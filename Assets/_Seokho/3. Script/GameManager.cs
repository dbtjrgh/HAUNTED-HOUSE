using Photon.Pun;
using System.Collections;
using UI.Journal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    public Transform startPositions;
    public GameObject DefeatUI; // ��� �÷��̾ ������� �� ǥ���� UI
    public GameObject ResultUI; // ��ǥ�� �Ϸ����� �� ������ UI
    private CMultiPlayer[] players; // ��� �÷��̾� ����
    private bool onOff = false;
    public CjournalBook journal;
    private CBoardManager CBoardManager;
    private CTruckButton truckButton; // CTruckButton ���� �߰�

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

        // CTruckButton ��ũ��Ʈ ã��
        truckButton = FindObjectOfType<CTruckButton>();
    }

    private void Update()
    {
        CheckAllPlayersDead();

        if (Input.GetKeyDown(KeyCode.J))
        {
            ToggleJournal();
        }

        // ���� ��� Ư�� ���ǿ��� Ʈ���� LevelEnd ����
        if (CheckAllPlayersSelectedGhost())
        {
            if (truckButton != null)
            {
                StartCoroutine(truckButton.LevelEnd());
            }
        }
    }

    // ��� �÷��̾ ����ߴ��� üũ�ϴ� �Լ�
    public void CheckAllPlayersDead()
    {
        players = FindObjectsOfType<CMultiPlayer>();

        bool allDead = true; // ��� �÷��̾ ����ߴٰ� ����

        foreach (CMultiPlayer player in players)
        {
            if (!player.isDead)
            {
                allDead = false; // ����ִ� �÷��̾ ������ false�� ����
                break;
            }
        }

        if (allDead)
        {
            StartCoroutine(ShowDeathUIAfterDelay(5f)); // 5�� ���� �� UI ǥ��
        }
    }

    private void ToggleJournal()
    {
        if (journal != null)
        {
            onOff = !onOff; // ���� ���

            if (onOff)
            {
                // Journal�� Ȱ��ȭ�� ��
                Cursor.visible = true; // Ŀ�� ���̰� ����
                Cursor.lockState = CursorLockMode.None; // Ŀ�� �̵� ����
            }
            else
            {
                // Journal�� ��Ȱ��ȭ�� ��
                Cursor.visible = false; // Ŀ�� �����
                Cursor.lockState = CursorLockMode.Locked; // Ŀ�� ����
            }

            journal.gameObject.SetActive(onOff); // journalBook�� Ȱ��ȭ ���� ����
        }
    }

    public bool CheckAllPlayersSelectedGhost()
    {
        // CjournalBook���� ghostToggleGroup�� �ִ� ��� ���� Ȯ��
        Toggle[] ghostToggles = journal.ghostToggleGroup.GetComponentsInChildren<Toggle>();

        foreach (Toggle toggle in ghostToggles)
        {
            if (toggle.isOn)
            {
                return true; // �ͽ� ��� �� �ϳ��� ���� ������ true ��ȯ
            }
        }

        return false; // �ͽ� ����� ���� ���� ������ false ��ȯ
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
