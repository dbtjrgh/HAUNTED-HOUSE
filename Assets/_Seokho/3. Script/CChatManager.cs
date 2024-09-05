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
    public TextMeshProUGUI chatLog; // ä�� ����
    public InputField inputField; // ä���Է� ��ǲ�ʵ�
    public TextMeshProUGUI playerList; //������ ���
    public Canvas ChatCanvas; // ä�� ĵ����
    /// </summary>
    string players; // �����ڵ�

    public static CChatManager instance = null;
    ScrollRect scroll_rect = null; // ä���� ���� ���� ��� ��ũ�ѹ��� ��ġ�� �Ʒ��� �����ϱ� ����
    private bool hasSentMessage = false; // �޽����� ���۵Ǿ����� ���θ� �����ϴ� �÷���

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
        sendBtn.onClick.AddListener(SendButtonOnClicked);
    }

    /// <summary>
    /// chatterUpdate(); �޼ҵ�� �ֱ������� �÷��̾� ����Ʈ�� ������Ʈ�ϸ�
    /// input�� ��Ŀ���� �������ְ� ����Ű�� �������� ��쿡�� SendButtonOnClicked(); �޼ҵ带 ����.
    /// </summary>
    void Update()
    {
        ChatterUpdate();

        // ä��â ���� �ݱ�, �޽��� ���� ���� �Լ��� ó��
        HandleChatInput();

        // esc Ű�� ������ Ŀ���� Ȱ��ȭ
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    Cursor.visible = true;
        //}
    }

    /// <summary>
    /// ����Ű �Է��� ó���ϴ� �Լ�.
    /// ä��â�� ���� �ݰų� �޽����� �����ϴ� ����� ó����.
    /// </summary>
    void HandleChatInput()
    {
        // ���͸� ������ �� �� ������Ʈ�� �����ְ�, ��ǲ �ʵ尡 ����ִٸ� ä��â�� ��Ȱ��ȭ
        if (ChatCanvas.enabled && Input.GetKeyUp(KeyCode.Return) && string.IsNullOrEmpty(inputField.text) && !hasSentMessage)
        {
            ChatCanvas.enabled = false;
            Cursor.visible = false;
        }
        // ä��â�� �����ְ�, ����Ű�� �����ٸ� ä��â�� Ȱ��ȭ�ϰ� ��ǲ �ʵ忡 ��Ŀ���� ����
        else if (!ChatCanvas.enabled && Input.GetKeyUp(KeyCode.Return))
        {
            ChatCanvas.enabled = true;
            Cursor.visible = true;
            inputField.ActivateInputField();
            hasSentMessage = false; // ���ο� �Է��� �ޱ� ���� �÷��� �ʱ�ȭ
        }
        // ä��â�� �����ְ�, ��ǲ �ʵ尡 ������� �ʴٸ� ä���� �����ϰ� ��ǲ �ʵ带 ���
        else if (ChatCanvas.enabled && Input.GetKeyUp(KeyCode.Return) && !string.IsNullOrEmpty(inputField.text))
        {
            SendButtonOnClicked(); // ä�� ���� �Լ� ȣ��
            hasSentMessage = true; // �޽����� ���۵� ������ ����
        }
    }

    /// <summary>
    /// ���� ��ư�� ������ ����� �޼ҵ�. �޼��� ������ �����.
    /// </summary>
    public void SendButtonOnClicked()
    {
        if (inputField.text.Equals(""))
        {
            Debug.Log("Empty");
            return;
        }
        string msg = string.Format("{0} :{1}", PhotonNetwork.LocalPlayer.NickName, inputField.text);
        photonView.RPC("ReceiveMsg", RpcTarget.OthersBuffered, msg);
        ReceiveMsg(msg);
        inputField.text = ""; // ��ǲ �ʵ� �ʱ�ȭ
        inputField.ActivateInputField(); // �޼��� ���� �� �ٷ� �޼����� �Է��� �� �ְ� ��Ŀ���� Input Field�� �ű�� ���� ���
        hasSentMessage = true; // �޽����� ���۵��� ǥ��
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
    /// ���� �濡�� ������ �� �ҷ����� �Լ�
    /// </summary>
    public override void OnLeftRoom()
    {
        // �� �ε� �� �̱۷κ� ���̸� �� ������Ʈ �ı�
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "SingleLobby")
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(this.gameObject);
        }
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
