using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CGameDefeatUI : MonoBehaviour
{
    #region ����
    public Button backButton;
    public Text tipText;
    public Text Correctanswer;
    private int random;
    private string targetMsg;
    private int index;
    private float interval;
    public float CharPerSeconds;
    #endregion
    /// <summary>
    /// ���� UI�� ��Ÿ���� ������ �̸��� �˷��ָ� ��Ƽ �κ�� ���ư��� ���ִ� UI
    /// </summary>
    private void Awake()
    {
        backButton.onClick.AddListener(OnBackButtonClick);
        random = Random.Range(0, 3);
        setMsg($"������ ��ü��...?  {Ghost.instance.ghostType}");
    }

    /// <summary>
    /// �й� UI�� ���� �� �������� �� ������ �ߴ� �Լ�
    /// </summary>
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

    /// <summary>
    /// ���ư��� ��ư
    /// Ŭ���� ��Ƽ�κ������ ���ư�
    /// </summary>
    public void OnBackButtonClick()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("MultiLobby");
    }

    /// <summary>
    /// �޼��� õõ�� ��µǰ� ������ �Լ���
    /// </summary>
    /// <param name="msg"></param>
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
        if (index < targetMsg.Length)
        {
            Correctanswer.text += targetMsg[index];
            index++;
            Invoke("Effecting", interval);
        }
        else
        {
            CancelInvoke("Effecting");
        }
    }

}
