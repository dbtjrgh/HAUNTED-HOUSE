using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CGameDefeatUI : MonoBehaviour
{
    public Button backButton;
    public Text tipText;
    private int random;
    private void Awake()
    {
        backButton.onClick.AddListener(OnBackButtonClick);
        tipText = GetComponent<Text>();
        random = Random.Range(0, 3);
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

    private void OnEnable()
    {
        switch (random)
        {
            case 0:
                tipText.text = "�� : �������϶� ���� ���Ƿ� ���߾��մϴ�.";
                return;
            case 1:
                tipText.text = "�� : �ǳ��� ������ ���ŷ��� �����մϴ�.";
                return;
            case 2:
                tipText.text = "�� : ������ �Բ� �ٴϼ���.";
                return;
        }

    }
}
