using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class CGameResultUI : MonoBehaviour
{
    #region
    public Button backButton;
    public Text GameResultText;
    public Text GhostText;
    #endregion

    private void Awake()
    {
        backButton.onClick.AddListener(OnBackButtonClick);
    }
    /// <summary>
    /// 결과 UI가 나왔을 시 갱신시켜주는 함수
    /// 투표를 통해 귀신을 맞췄는지 여부와 귀신의 이름 공개
    /// </summary>
    /// <param name="resultText"></param>
    /// <param name="ghostType"></param>
    public void SetGameResult(string resultText, string ghostType)
    {
        Debug.Log(resultText + ghostType);
        GameResultText.text = resultText;
        GhostText.text = $"발견된 귀신 : {ghostType}";
    }

    /// <summary>
    /// 돌아가기 버튼 클릭시 멀티 로비씬으로 돌아가게 해주는 함수
    /// </summary>
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
