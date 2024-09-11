using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CLoginScreen : MonoBehaviour
{
    #region 변수 
    public TMP_InputField nicknameInput; // 플레이어 닉네임을 입력할 Input
    public Button loginButton; // 로그인 버튼
    public Button backButton; // 뒤로 가기 버튼
    public Button quitButton; // 종료 버튼
    public TextMeshProUGUI InfoText; // 연결 상태를 표시할 LogText

    private CLookBoard lookBoard;
    #endregion

    private void Awake()
    {
        // 버튼 클릭 시 호출될 메서드들
        loginButton.onClick.AddListener(OnLoginButtonClick);
        quitButton.onClick.AddListener(OnQuitButtonClick);
        backButton.onClick.AddListener(OnBackButtonClick);

        InfoText.text = "닉네임을 입력하세요.";
    }

    private void Start()
    {
        // 닉네임 Input에 기본 값 설정
        nicknameInput.text = $"Player {Random.Range(100, 1000)}";
        lookBoard = FindObjectOfType<CLookBoard>();
    }

    private void OnEnable()
    {
        // 로그인 및 닉네임 Input 활성화
        nicknameInput.interactable = true;
        loginButton.interactable = true;
    }

    /// <summary>
    /// 로그인 버튼 클릭시 포톤 접속 시도
    /// </summary>
    public void OnLoginButtonClick()
    {
        string nickname = nicknameInput.text.Trim();
        

        PhotonNetwork.NickName = nickname;
        InfoText.text = "마스터 서버에 접속 중...";
        PhotonNetwork.ConnectUsingSettings();
    }

    /// <summary>
    /// 종료 버튼 클릭시 게임 종료
    /// </summary>
    public void OnQuitButtonClick()
    {
        Application.Quit();
    }

    /// <summary>
    /// 뒤로가기 버튼 클릭시 CLookBoard 스크립트의 
    /// ReturnToPlayerCamera 함수를 호출해 플레이어 카메라로 복귀
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
