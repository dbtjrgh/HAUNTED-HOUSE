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
    /// ��� UI�� ������ �� ���Ž����ִ� �Լ�
    /// ��ǥ�� ���� �ͽ��� ������� ���ο� �ͽ��� �̸� ����
    /// </summary>
    /// <param name="resultText"></param>
    /// <param name="ghostType"></param>
    public void SetGameResult(string resultText, string ghostType)
    {
        Debug.Log(resultText + ghostType);
        GameResultText.text = resultText;
        GhostText.text = $"�߰ߵ� �ͽ� : {ghostType}";
    }

    /// <summary>
    /// ���ư��� ��ư Ŭ���� ��Ƽ �κ������ ���ư��� ���ִ� �Լ�
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
