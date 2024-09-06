using Photon.Pun;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    #region ����
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

        // �÷��̾� ������ �ν��Ͻ�ȭ
        GameObject playerPrefab = PhotonNetwork.Instantiate("MultiPlayer", pos, rot, 0);

        // ���� �÷��̾ �ƴ� ��� ���̾ "MultiPlayer"�� ����
        PhotonView photonView = playerPrefab.GetComponent<PhotonView>();
        if (photonView != null && !photonView.IsMine)
        {
            SetLayerRecursively(playerPrefab, LayerMask.NameToLayer("OtherPlayer"));
        }
    }

    // �������� ���̾ ��������� �����ϴ� �Լ�
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