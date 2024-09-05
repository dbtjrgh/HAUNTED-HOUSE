using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CDontDestroyChat : MonoBehaviourPunCallbacks
{
    #region 변수
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
    /// 포톤 방에서 나갔을 때 불러오는 함수
    /// </summary>
    public override void OnLeftRoom()
    {
        // 씬 로드 후 싱글로비 씬이면 이 오브젝트 파괴
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
