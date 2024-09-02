using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using UnityEngine;

public class CGameManager : MonoBehaviourPunCallbacks
{
    #region ����
    public Transform startPositions;

    #endregion

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        if (PhotonNetwork.IsConnectedAndReady)
        {
            StartCoroutine(NormalStart());
        }


    }

    private IEnumerator NormalStart()
    {
        // PhotonNetwork�� ��� �÷��̾��� �ε� ���¸� �Ǵ��Ͽ� �ѹ����� �ؾ� �ϴµ�,
        // ���� �׷� ����� �����Ǿ����� �����Ƿ�, 1�� ��� �� ���� ���� ������ ����
        yield return new WaitUntil(() => PhotonNetwork.LocalPlayer.GetPlayerNumber() != -1);

        //GameObject playerPrefab = Resources.Load<GameObject>("Player");

        //Instantiate(playerPrefab, startPositions.GetChild(0).position,Quaternion.identity);

        // ���ӿ� ������ �濡�� �ο��� �� ��ȣ
        // Ȱ���ϱ� ���ؼ��� ���� ���� PlayerNumbering ������Ʈ�� �����ؾ���.
        int playerNumber = PhotonNetwork.LocalPlayer.GetPlayerNumber();

        Transform playerPos = startPositions.GetChild(playerNumber);

        GameObject playerObj = PhotonNetwork.Instantiate("PlayerPrefab", playerPos.position, playerPos.rotation);

        playerObj.name = $"Player {playerNumber}";
    }
}
