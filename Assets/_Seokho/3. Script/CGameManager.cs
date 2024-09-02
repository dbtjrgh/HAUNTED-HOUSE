using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using UnityEngine;

public class CGameManager : MonoBehaviourPunCallbacks
{
    #region 변수
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
        // PhotonNetwork가 모든 플레이어의 로드 상태를 판단하여 넘버링을 해야 하는데,
        // 현재 그런 모듈이 구현되어있지 않으므로, 1초 대기 후 게임 시작 절차를 수행
        yield return new WaitUntil(() => PhotonNetwork.LocalPlayer.GetPlayerNumber() != -1);

        //GameObject playerPrefab = Resources.Load<GameObject>("Player");

        //Instantiate(playerPrefab, startPositions.GetChild(0).position,Quaternion.identity);

        // 게임에 참여한 방에서 부여된 내 번호
        // 활용하기 위해서는 게임 씬에 PlayerNumbering 컴포넌트가 존재해야함.
        int playerNumber = PhotonNetwork.LocalPlayer.GetPlayerNumber();

        Transform playerPos = startPositions.GetChild(playerNumber);

        GameObject playerObj = PhotonNetwork.Instantiate("PlayerPrefab", playerPos.position, playerPos.rotation);

        playerObj.name = $"Player {playerNumber}";
    }
}
