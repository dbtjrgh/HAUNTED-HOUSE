using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviourPunCallbacks
{
    #region ����
    public static GameManager instance = null;
    public bool isConnect = false;
    public Transform startPositions;
    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }

        // �� �ε� �̺�Ʈ ����
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // �� �ε� �̺�Ʈ ���� ����
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ���� �ε�� ��, �÷��̾ �����ϴ� �ڷ�ƾ ����
        if (isConnect)
        {
            StartCoroutine(CreatePlayer());
        }
    }

    private IEnumerator CreatePlayer()
    {
        startPositions = GameObject.Find("Initial").GetComponentInChildren<Transform>();
        if (startPositions == null)
        {
            Debug.LogError("Start positions not found in the scene.");
            yield break;
        }

        Vector3 pos = startPositions.position;
        Quaternion rot = startPositions.rotation;

        GameObject playerTemp = PhotonNetwork.Instantiate("PlayerPrefab", pos, rot, 0);
        if (playerTemp != null)
        {
            Debug.Log("Player instantiated successfully.");
        }
        else
        {
            Debug.LogError("Failed to instantiate player.");
        }
    }
}