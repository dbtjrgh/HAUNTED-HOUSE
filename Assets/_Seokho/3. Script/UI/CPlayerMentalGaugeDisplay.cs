using Photon.Pun;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class CPlayerMentalGaugeDisplay : MonoBehaviourPunCallbacks
{
    #region ����
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

        UpdatePlayerTexts();
    }
    /// <summary>
    /// �ٲ� ���̵��� ���� �����ִ� �ؽ�Ʈ ���� �ݹ� �Լ�
    /// </summary>
    /// <param name="props"></param>
    public override void OnRoomPropertiesUpdate(PhotonHashtable props)
    {
        props = PhotonNetwork.CurrentRoom.CustomProperties;

        if (props.ContainsKey("Diff"))
        {
            string text = ((Difficulty)props["Diff"]).ToString();
            diffText.text = ($"���̵� : {text}");
        }
    }
    /// <summary>
    /// �÷��̾��� ����並 �̿��� �÷��̾� ���� ������Ʈ�� ã�� ��ȯ�����ִ� �Լ�
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
    /// �׻� ���Ž�Ű�� �Լ��� ���÷��̿� �÷��̾� �̸��� ���� ��Ż ������ ��ġ�� ������
    /// </summary>
    private void UpdatePlayerTexts()
    {
        player1Text.text = player2Text.text = player3Text.text = player4Text.text = "";

        int index = 0;

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (index > 3)
            {
                break; // �ִ� 4���� �÷��̾ ǥ��
            }

            string playerName = player.NickName;
            float mentalGauge = playerMentalGauges.ContainsKey(player.ActorNumber)
                ? playerMentalGauges[player.ActorNumber].MentalGauge : 0;

            // �Ҽ��� �� �ڸ������� ǥ��
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
