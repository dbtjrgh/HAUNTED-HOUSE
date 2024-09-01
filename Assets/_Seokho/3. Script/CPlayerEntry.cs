using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CPlayerEntry : MonoBehaviour
{
    public Photon.Realtime.Player player;
    public Text playerNameText;
    public Toggle readyToggle;
    public Image readyStatusImage; // �غ� ���¸� �ð������� ǥ���ϴ� �̹���

    private void Start()
    {
        if (player.CustomProperties.ContainsKey("Ready"))
        {
            bool isReady = (bool)player.CustomProperties["Ready"];
            readyToggle.isOn = isReady;
            readyStatusImage.color = isReady ? Color.green : Color.red;
        }
    }

    public void UpdateReadyStatus(bool isReady)
    {
        readyToggle.isOn = isReady;
        readyStatusImage.color = isReady ? Color.green : Color.red;
    }

}
