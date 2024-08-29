using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CLoginScreen : MonoBehaviour
{
    #region ���� 
    public TMP_InputField nicknameInput; // �÷��̾� �г����� �Է��� Input
    public TextMeshProUGUI connectionInfoText; // ���� ���¸� ǥ���� LogText
    public Button loginButton; // �α��� ��ư
    public Button backButton; // �ڷ� ���� ��ư
    public Button quitButton; // ���� ��ư

    private CLookBoard lookBoard;
    #endregion

    private void Awake()
    {
        // ��ư Ŭ�� �� ȣ��� �޼����
        loginButton.onClick.AddListener(OnLoginButtonClick);
        quitButton.onClick.AddListener(OnQuitButtonClick);
        backButton.onClick.AddListener(OnBackButtonClick);
    }

    private void Start()
    {
        // �г��� Input�� �⺻ �� ����
        nicknameInput.text = $"Player {Random.Range(100, 1000)}";
        lookBoard = FindObjectOfType<CLookBoard>();
    }

    private void OnEnable()
    {
        // �α��� �� �г��� Input Ȱ��ȭ
        nicknameInput.interactable = true;
        loginButton.interactable = true;
    }

    /// <summary>
    /// �α��� ��ư Ŭ���� ���� ���� �õ�
    /// </summary>
    public void OnLoginButtonClick()
    {
        Debug.Log("�� �ȵ�");
        connectionInfoText.text = "������ ������ ���� ��";
        PhotonNetwork.LocalPlayer.NickName = nicknameInput.text;
        PhotonNetwork.ConnectUsingSettings();
    }

    /// <summary>
    /// ���� ��ư Ŭ���� ���� ����
    /// </summary>
    public void OnQuitButtonClick()
    {
        // ���� ����
        Application.Quit();
    }

    /// <summary>
    /// �ڷΰ��� ��ư Ŭ���� CLookBoard ��ũ��Ʈ�� 
    /// ReturnToPlayerCamera �Լ��� ȣ���� �÷��̾� ī�޶�� ����
    /// </summary>
    public void OnBackButtonClick()
    {
        if (lookBoard != null)
        {
            lookBoard.ReturnToPlayerCamera();
        }
        else
        {
            Debug.Log("CLookBoard instance not found.");
        }
    }
}
