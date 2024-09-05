using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CDontDestroyChat : MonoBehaviourPunCallbacks
{
    #region ����
    public static CDontDestroyChat instance = null;
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
    }

    /// <summary>
    /// ���� �濡�� ������ �� �ҷ����� �Լ�
    /// </summary>
    public override void OnLeftRoom()
    {
        // �� �ε� �� �̱۷κ� ���̸� �� ������Ʈ �ı�
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SingleLobby")
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(this.gameObject);
        }
    }
}
