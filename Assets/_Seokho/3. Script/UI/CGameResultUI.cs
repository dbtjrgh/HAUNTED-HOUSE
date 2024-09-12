using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class CGameResultUI : MonoBehaviour
{
    public Button backButton;
    public Text GameResultText;
    public Text GhostText;

    private void Awake()
    {
        backButton.onClick.AddListener(OnBackButtonClick);
    }

    public void SetGameResult(string resultText, string ghostType)
    {
        Debug.Log(resultText + ghostType);
        GameResultText.text = resultText;
        GhostText.text = $"발견된 귀신 : {ghostType}";
    }

    void Update()
    {
        // 필요 시 자동 초기화 로직 추가
    }

    public void OnBackButtonClick()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("MultiLobby");
        SoundManager.instance.musicSource.Stop();
        SoundManager.instance.musicSource.clip = null;


    }
}
