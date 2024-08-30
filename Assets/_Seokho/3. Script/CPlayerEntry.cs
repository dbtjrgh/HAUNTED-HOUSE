using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CPlayerEntry : MonoBehaviour
{
    public TextMeshProUGUI playerNameText;
    public Toggle readyToggle;

    public Photon.Realtime.Player player;

    public bool Ismine => player == PhotonNetwork.LocalPlayer;

    private void Awake()
    {
        //readyToggle.onValueChanged.AddListener(ReadyToggleClick);
        // readyToggle.isOn = false; => onValueChanged�� ȣ��
        readyToggle.SetIsOnWithoutNotify(false);
    }

    private void ReadyToggleClick(bool isOn)
    {
        // Ŀ���� ������Ƽ�� isOn�� �߰��ϴ� ������ �ۼ����� ���
    }

}
