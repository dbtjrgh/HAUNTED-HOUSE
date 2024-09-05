using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CChatManager : MonoBehaviourPunCallbacks
{
    #region ����
    public Button sendBtn; //ä�� �Է¹�ư
    public TextMeshProUGUI chatLog; //ä�� ����
    public TMP_InputField inputField; //ä���Է� ��ǲ�ʵ�
    public TextMeshProUGUI playerList; //������ ���
    string players; //�����ڵ�

    public static CChatManager instance = null;
    ScrollRect scroll_rect = null; //ä���� ���� ���� ��� ��ũ�ѹ��� ��ġ�� �Ʒ��� �����ϱ� ����
    #endregion
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }
    private void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;
        scroll_rect = GameObject.FindObjectOfType<ScrollRect>();
    }

    /// <summary>
    /// chatterUpdate(); �޼ҵ�� �ֱ������� �÷��̾� ����Ʈ�� ������Ʈ�ϸ�
    /// input�� ��Ŀ���� �������ְ� ����Ű�� �������� ��쿡�� SendButtonOnClicked(); �޼ҵ带 ����.
    /// </summary>
    void Update()
    {
        ChatterUpdate();
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && !inputField.isFocused)
        {
            SendButtonOnClicked();
        }
    }

    /// <summary>
    /// ���� ��ư�� ������ ����� �޼ҵ�. �޼��� ������ �����.
    /// input�� ��������� �ƹ��͵� �������� �ʰ�, ������� �ʴٸ�
    /// "[ID] �޼���"�� �������� �޼����� ������.
    /// �޼��� ������ photonView.RPC �޼ҵ带 �̿��� �� �����鿡�� ReceiveMsg �޼ҵ带 �����ϰ� ��.
    /// �ڱ� �ڽſ��Ե� �޼����� ����� �ϹǷ� ReceiveMsg(msg);�� ������.
    /// input.ActivateInputField();�� �޼��� ���� �� �ٷ� �޼����� �Է��� �� �ְ� ��Ŀ���� Input Field�� �ű� (���� ���)
    /// �� �� input.text�� �� ĭ���� ����
    /// </summary>
    public void SendButtonOnClicked()
    {
        if (inputField.text.Equals(""))
        {
            Debug.Log("Empty");
            return;
        }
        string msg = string.Format("[{0}] {1}", PhotonNetwork.LocalPlayer.NickName, inputField.text);
        photonView.RPC("ReceiveMsg", RpcTarget.OthersBuffered, msg);
        ReceiveMsg(msg);
        inputField.ActivateInputField(); // �޼��� ���� �� �ٷ� �޼����� �Է��� �� �ְ� ��Ŀ���� Input Field�� �ű�� ���� ���
        inputField.text = "";
    }

    /// <summary>
    /// ä�� ������ ����� ������Ʈ �ϴ� �Լ�.
    /// '������ ���' �ؽ�Ʈ �Ʒ��� �÷��̾���� ID�� �����ִ� ������ �۵��ϸ�,
    /// �ǽð����� �����ϴ� �������� ID�� �ݿ���.
    /// </summary>
    void ChatterUpdate()
    {
        players = "������ ���\n";
        foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
        {
            players += p.NickName + "\n";
        }
        playerList.text = players;
    }


    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        string msg = string.Format("<color=#00ff00>[{0}]���� �����ϼ̽��ϴ�.</color>", newPlayer.NickName);
        ReceiveMsg(msg);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        string msg = string.Format("<color=#ff0000>[{0}]���� �����ϼ̽��ϴ�.</color>", otherPlayer.NickName);
        ReceiveMsg(msg);
    }

    [PunRPC]
    public void ReceiveMsg(string msg)
    {
        chatLog.text += "\n" + msg;
        StartCoroutine(ScrollUpdate());
    }


    IEnumerator ScrollUpdate()
    {
        yield return null;
        scroll_rect.verticalNormalizedPosition = 0.0f;
    }
}
