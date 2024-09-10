using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CGameResultUI : MonoBehaviour
{
    public Button backButton;
    public Text GameResultText;
    public Text GhostText;
    private void Awake()
    {
        backButton.onClick.AddListener(OnBackButtonClick);
    }

    void Update()
    {
        
    }

    public void OnBackButtonClick()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("MultiLobby");
    }
}
