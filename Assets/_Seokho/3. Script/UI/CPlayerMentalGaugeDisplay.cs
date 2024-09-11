using Photon.Pun;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CPlayerMentalGaugeDisplay : MonoBehaviourPunCallbacks
{
    public TextMeshPro player1Text;
    public TextMeshPro player2Text;
    public TextMeshPro player3Text;
    public TextMeshPro player4Text;

    private Dictionary<int, mentalGaugeManager> playerMentalGauges;

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
                ? playerMentalGauges[player.ActorNumber].MentalGauge
                : 0;

            string PlayerMentalText = playerName + "\nMental: " + mentalGauge;

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
