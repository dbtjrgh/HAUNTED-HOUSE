using Photon.Pun;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class CPlayerMentalGaugeDisplay : MonoBehaviourPunCallbacks
{
    #region 변수
    public TextMeshPro player1Text;
    public TextMeshPro player2Text;
    public TextMeshPro player3Text;
    public TextMeshPro player4Text;
    public TextMeshPro diffText;

    private Dictionary<int, mentalGaugeManager> playerMentalGauges;
    #endregion

    private void Awake()
    {
        CRoomScreen roomScreen = FindObjectOfType<CRoomScreen>();
    }
    private void Start()
    {
        playerMentalGauges = new Dictionary<int, mentalGaugeManager>();
    }

    private void Update()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (!playerMentalGauges.ContainsKey(player.ActorNumber)) // 만약 아직 등록되지 않은 플레이어라면 추가
            {
                GameObject playerObject = GetPlayerObject(player);
                if (playerObject != null)
                {
                    mentalGaugeManager mentalGauge = playerObject.GetComponent<mentalGaugeManager>();
                    if (mentalGauge != null)
                    {
                        playerMentalGauges[player.ActorNumber] = mentalGauge;
                    }
                }
            }
        }

        UpdatePlayerTexts();
    }
    /// <summary>
    /// 바꾼 난이도에 따라 보여주는 텍스트 포톤 콜백 함수
    /// </summary>
    /// <param name="props"></param>
    public override void OnRoomPropertiesUpdate(PhotonHashtable props)
    {
        props = PhotonNetwork.CurrentRoom.CustomProperties;

        if (props.ContainsKey("Diff"))
        {
            string text = ((Difficulty)props["Diff"]).ToString();
            diffText.text = ($"난이도 : {text}");
        }
    }
    /// <summary>
    /// 플레이어의 포톤뷰를 이용해 플레이어 게임 오브젝트를 찾아 반환시켜주는 함수
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    private GameObject GetPlayerObject(Photon.Realtime.Player player)
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
        {
            PhotonView pv = obj.GetComponent<PhotonView>();
            if (pv != null && pv.Owner == player)
            {
                return obj;
            }
        }
        return null;
    }

    /// <summary>
    /// 항상 갱신시키는 함수로 디스플레이에 플레이어 이름과 현재 멘탈 게이지 수치를 보여줌
    /// </summary>
    private void UpdatePlayerTexts()
    {
        player1Text.text = player2Text.text = player3Text.text = player4Text.text = "";

        int index = 0;

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (index > 3)
            {
                break; // 최대 4명의 플레이어만 표시
            }

            string playerName = player.NickName;
            float mentalGauge = playerMentalGauges.ContainsKey(player.ActorNumber)
                ? playerMentalGauges[player.ActorNumber].MentalGauge : 0;

            // 소수점 한 자리까지만 표시
            string PlayerMentalText = playerName + "\nMental: " + mentalGauge.ToString("F1");

            switch (index)
            {
                case 0:
                    player1Text.text = PlayerMentalText;
                    break;
                case 1:
                    player2Text.text = PlayerMentalText;
                    break;
                case 2:
                    player3Text.text = PlayerMentalText;
                    break;
                case 3:
                    player4Text.text = PlayerMentalText;
                    break;
            }

            index++;
        }
    }
}
