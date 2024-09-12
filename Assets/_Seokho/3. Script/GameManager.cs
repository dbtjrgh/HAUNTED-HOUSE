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
    public GameObject defeatUI; // ��� �÷��̾ ������� �� ǥ���� UI
    public GameObject resultUI; // ��ǥ�� �Ϸ����� �� ������ UI
    private CGameResultUI gameResultUI; // ���� ��� UI
    private CMultiPlayer[] players; // ��� �÷��̾� ����
    public CjournalBook journal;
    private CBoardManager boardManager;
    private CTruckButton truckButton; // CTruckButton ���� �߰�
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

        // CTruckButton ��ũ��Ʈ ã��
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

        // ���� ��� Ư�� ���ǿ��� Ʈ���� LevelEnd ����
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
    /// �÷��̾� ���� ���� ����
    /// </summary>
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

    /// <summary>
    /// ���� ���� ���� ����
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


    /// <summary>
    /// ���� ��� ���� ����
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
            return true; // �ͽ� ��� �� �ϳ��� ���� ������ true ��ȯ
        }
        return false; // �ͽ� ����� ���� ���� ������ false ��ȯ
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
