using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CLobbyManager : MonoBehaviourPunCallbacks
{
    #region ����

    private string gameVersion = "1.0";


    //  ��Ʈ��ũ ������ ǥ���� �ؽ�Ʈ
    public Text connectionInfoText;
    // �� ���ӹ�ư
    public Button joinButton;

    #endregion

    private void Start()
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();

        joinButton.interactable = false;

        connectionInfoText.text = "������ ������ ���� ��...";
    }

    public override void OnConnectedToMaster()
    {
        joinButton.interactable = true;

        connectionInfoText.text = "�¶��� : ������ ������ �����";
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        joinButton.interactable = false;

        connectionInfoText.text = "�������� : ������ ������ ������� ����\n���� ��õ� ��...";

        PhotonNetwork.ConnectUsingSettings();
    }

    public void Connect()
    {
        joinButton.interactable = false;

        if(PhotonNetwork.IsConnected)
        {
            connectionInfoText.text = "�뿡 ������...";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            connectionInfoText.text = "�������� : ������ ������ ������� ����\n���� ��õ� ��...";

            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        connectionInfoText.text = "�� ���� ����, ���ο� �� ����...";

        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });
    }

    public override void OnJoinedRoom()
    {
        connectionInfoText.text = "�� ���� ����";

        PhotonNetwork.LoadLevel("Main");
    }

    public void ConnectButtonClick()
    {

    }
}
