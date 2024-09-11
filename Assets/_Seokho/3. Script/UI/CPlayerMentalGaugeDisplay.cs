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
        // Find and update all players and their mental gauge managers dynamically
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (!playerMentalGauges.ContainsKey(player.ActorNumber)) // ���� ���� ��ϵ��� ���� �÷��̾��� �߰�
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

        // Update the player UI texts with the latest mental gauge values
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
        // Clear previous values
        player1Text.text = player2Text.text = player3Text.text = player4Text.text = "";

        int index = 0;

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (index > 3) break; // �ִ� 4���� �÷��̾ ǥ��

            string playerName = player.NickName;
            float mentalGauge = playerMentalGauges.ContainsKey(player.ActorNumber)
                ? playerMentalGauges[player.ActorNumber].MentalGauge
                : 0;

            string combinedText = playerName + "\nMental: " + mentalGauge;

            // Update the correct text field
            switch (index)
            {
                case 0:
                    player1Text.text = combinedText;
                    break;
                case 1:
                    player2Text.text = combinedText;
                    break;
                case 2:
                    player3Text.text = combinedText;
                    break;
                case 3:
                    player4Text.text = combinedText;
                    break;
            }

            index++;
        }
    }
}
