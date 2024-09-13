using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CChatManager : MonoBehaviourPunCallbacks
{
    #region ����
    public Button sendBtn; // ä�� �Է¹�ư
    public Text chatLog; // ä�� ����
    public InputField inputField; // ä���Է� ��ǲ�ʵ�
    public TextMeshProUGUI playerList; //������ ���
    public Canvas ChatCanvas; // ä�� ĵ����
    /// </summary>
    string players; // �����ڵ�

    public static CChatManager instance = null;
    ScrollRect scroll_rect = null; // ä���� ���� ���� ��� ��ũ�ѹ��� ��ġ�� �Ʒ��� �����ϱ� ����

    #endregion

    /// <summary>
    /// ��Ƽ �κ���������� ����
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject); // ������Ʈ�� �� �ε� �� �ı����� �ʵ��� ����
        }
        else if (instance != this)
        {
            Destroy(this.gameObject); // �ߺ��� ������Ʈ�� �ı�
        }
    }

    private void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;
        scroll_rect = GameObject.FindObjectOfType<ScrollRect>();

        if (scroll_rect == null)
        {
            Debug.LogError("ScrollRect�� ã�� �� �����ϴ�.");
        }

        sendBtn.onClick.AddListener(SendButtonOnClicked);
        ChatCanvas.gameObject.SetActive(false); // ä�� ĵ������ ó���� ��Ȱ��ȭ
    }

    /// <summary>
    /// chatterUpdate(); �޼ҵ�� �ֱ������� �÷��̾� ����Ʈ�� ������Ʈ�ϸ�
    /// input�� ��Ŀ���� �������ְ� ����Ű�� �������� ��쿡�� SendButtonOnClicked(); �޼ҵ带 ����.
    /// </summary>
    void Update()
    {
        if (!PhotonNetwork.IsConnected)
        {
            return;
        }
        ChatterUpdate();

        // O Ű�� ���� ä�� ĵ������ ��/����
        if (Input.GetKeyDown(KeyCode.O))
        {
            ToggleChatCanvas();
        }

        // ����Ű�� Ű�е� ����Ű�� �޽����� ����
        if (Input.GetKeyDown(KeyCode.Return) && ChatCanvas.gameObject.activeSelf)
        {
            SendButtonOnClicked();
        }
        // ��Ȱ��ȭ �� �� ����Ű�� ������ Ȱ��ȭ
        else if (Input.GetKeyDown(KeyCode.Return) && !ChatCanvas.gameObject.activeSelf)
        {
            ChatCanvas.gameObject.SetActive(true);
            inputField.ActivateInputField();
        }
    }

    /// <summary>
    /// ä�� ĵ���� ������Ʈ ��ü�� ��/���� �ϴ� �Լ�
    /// </summary>
    void ToggleChatCanvas()
    {
        // ä�� ĵ���� ������Ʈ�� Ȱ��ȭ�Ǿ� ������ ��Ȱ��ȭ, ��Ȱ��ȭ�Ǿ� ������ Ȱ��ȭ
        ChatCanvas.gameObject.SetActive(!ChatCanvas.gameObject.activeSelf);

        // ä�� ĵ������ Ȱ��ȭ�� ��� �Է� �ʵ忡 ��Ŀ���� ����
        if (ChatCanvas.gameObject.activeSelf)
        {
            inputField.ActivateInputField();
        }
        
    }

    /// <summary>
    /// ���� ��ư�� ������ ����� �޼ҵ�. �޽��� ������ �����.
    /// </summary>
    public void SendButtonOnClicked()
    {
        // �Էµ� �޽������� ���� ���� ����
        string sanitizedMessage = inputField.text.Replace("\n", "").Replace("\r", "");

        // �Է� �ʵ尡 ��� ������ ä�� ������Ʈ�� �ڵ����� ��Ȱ��ȭ
        if (string.IsNullOrWhiteSpace(sanitizedMessage))
        {
            Debug.Log("Empty message, closing chat.");
            ChatCanvas.gameObject.SetActive(false); // �Է� �ʵ尡 ��� ������ ������Ʈ ��Ȱ��ȭ
            return;
        }

        // �޽��� ����
        string msg = string.Format("{0} : {1}", PhotonNetwork.LocalPlayer.NickName, sanitizedMessage);
        photonView.RPC("ReceiveMsg", RpcTarget.OthersBuffered, msg);
        ReceiveMsg(msg);
        inputField.text = ""; // ��ǲ �ʵ� �ʱ�ȭ
        inputField.ActivateInputField(); // ��Ŀ���� �����Ͽ� ��� �Է��� �� �ְ� ����
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

    /// <summary>
    /// �÷��̾ �������� �� �ҷ����� �Լ�
    /// </summary>
    /// <param name="newPlayer"></param>
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        string msg = string.Format("<color=#00ff00>[{0}]���� �����ϼ̽��ϴ�.</color>", newPlayer.NickName);
        ReceiveMsg(msg);
    }
    /// <summary>
    /// �÷��̾ �������� �� �ҷ����� �Լ�
    /// </summary>
    /// <param name="otherPlayer"></param>
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        string msg = string.Format("<color=#ff0000>[{0}]���� �����ϼ̽��ϴ�.</color>", otherPlayer.NickName);
        ReceiveMsg(msg);
    }
    /// <summary>
    /// ä���� ġ���� ä�� �Է�â ����ó��
    /// </summary>
    /// <param name="msg"></param>
    [PunRPC]
    public void ReceiveMsg(string msg)
    {
        chatLog.text += "\n" + msg;
        StartCoroutine(ScrollUpdate());
    }

    /// <summary>
    /// ���� ���� ��ũ�� �ʱ�ȭ
    /// </summary>
    /// <returns></returns>
    IEnumerator ScrollUpdate()
    {
        yield return null;
        scroll_rect.verticalNormalizedPosition = 0.0f;
    }
}
