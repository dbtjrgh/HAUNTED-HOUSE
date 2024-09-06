using Photon.Pun;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    #region 변수
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

        // 플레이어 프리팹 인스턴스화
        GameObject playerPrefab = PhotonNetwork.Instantiate("MultiPlayer", pos, rot, 0);

        // 로컬 플레이어가 아닌 경우 레이어를 "MultiPlayer"로 변경
        PhotonView photonView = playerPrefab.GetComponent<PhotonView>();
        if (photonView != null && !photonView.IsMine)
        {
            SetLayerRecursively(playerPrefab, LayerMask.NameToLayer("OtherPlayer"));
        }
    }

    // 프리팹의 레이어를 재귀적으로 변경하는 함수
    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (child != null)
            {
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }
    }


}