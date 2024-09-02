using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CPlayerEntry : MonoBehaviour
{
    public Photon.Realtime.Player player;
    public TextMeshProUGUI playerNameText;
    public Toggle readyToggle;

    public bool IsMine => player == PhotonNetwork.LocalPlayer;

    private void Awake()
    {
        readyToggle.SetIsOnWithoutNotify(false);
    }

}
