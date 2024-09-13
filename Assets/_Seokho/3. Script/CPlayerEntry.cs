using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CPlayerEntry : MonoBehaviour
{
    #region º¯¼ö
    public Photon.Realtime.Player player;
    public TextMeshProUGUI playerNameText;
    public Toggle readyToggle;
    #endregion
    public bool IsMine => player == PhotonNetwork.LocalPlayer;

    private void Awake()
    {
        readyToggle.SetIsOnWithoutNotify(false);
    }

}
