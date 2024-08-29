using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CLoginScreen : MonoBehaviour
{
    public TMP_InputField nicknameInput;
    public TextMeshProUGUI connectionInfoText;
    public Button loginButton;
    public Button backButton;
    public Button quitButton;

    private void Awake()
    {
        loginButton.onClick.AddListener(OnLoginButtonClick);
    }

    private void Start()
    {
        nicknameInput.text = $"Player {Random.Range(100, 1000)}";
    }

    private void OnEnable()
    {

        nicknameInput.interactable = true;
        loginButton.interactable = true;
    }

    public void OnLoginButtonClick()
    {
        connectionInfoText.text = "마스터 서버에 접속 중";
        PhotonNetwork.LocalPlayer.NickName = nicknameInput.text;
        PhotonNetwork.ConnectUsingSettings();
    }


    public void OnQuitButtonClick()
    {
        // 게임 종료
        Application.Quit();
    }

    public void OnBackButtonClick()
    {
        // 더 구상해야함
        
    }



}
