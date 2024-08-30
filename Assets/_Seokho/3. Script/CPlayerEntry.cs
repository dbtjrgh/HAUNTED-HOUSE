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
        // readyToggle.isOn = false; => onValueChanged가 호출
        readyToggle.SetIsOnWithoutNotify(false);
    }

    private void ReadyToggleClick(bool isOn)
    {
        // 커스텀 프로퍼티에 isOn을 추가하는 로직을 작성했을 경우
    }

}
