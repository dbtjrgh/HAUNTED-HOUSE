using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviourPunCallbacks
{
    #region º¯¼ö
    public static GameManager instance = null;
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
    }

    private void Start()
    {
        Vector3 pos = startPositions.position;
        Quaternion rot = startPositions.rotation;

        PhotonNetwork.Instantiate("MultiPlayer", pos, rot, 0);
    }


}