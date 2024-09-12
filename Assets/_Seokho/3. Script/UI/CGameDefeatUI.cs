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
    private string targetMsg;
    private int index;
    private float interval;
    public float CharPerSeconds;
    public Text Correctanswer;
    private void Awake()
    {
        backButton.onClick.AddListener(OnBackButtonClick);
        tipText = GetComponent<Text>();
        random = Random.Range(0, 3);
        setMsg($"������ ��ü��...?  {Ghost.instance.ghostType}");
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

    public void setMsg(string msg)
    {
        targetMsg = msg;
        EffectStart();

    }
    void EffectStart()
    {
        Correctanswer.text = "";
        index = 0;

        interval = 1.0f / CharPerSeconds;

        Invoke("Effecting", interval);
    }

    void Effecting()
    {
        Correctanswer.text += targetMsg[index];
        index++;

        Invoke("Effecting", interval);
    }
}
