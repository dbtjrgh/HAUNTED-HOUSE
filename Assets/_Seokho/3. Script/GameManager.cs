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

        }
    }
    
}