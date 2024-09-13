using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CGameDefeatUI : MonoBehaviour
{
    #region 변수
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
    /// 실패 UI를 나타내고 유령의 이름을 알려주며 멀티 로비로 돌아가게 해주는 UI
    /// </summary>
    private void Awake()
    {
        backButton.onClick.AddListener(OnBackButtonClick);
        random = Random.Range(0, 3);
        setMsg($"유령의 정체는...?  {Ghost.instance.ghostType}");
    }

    /// <summary>
    /// 패배 UI가 떴을 때 랜덤으로 팁 문구가 뜨는 함수
    /// </summary>
    private void OnEnable()
    {
        switch (random)
        {
            case 0:
                tipText.text = "팁 : 헌팅중일땐 문이 잠기므로 버텨야합니다.";
                return;
            case 1:
                tipText.text = "팁 : 실내에 있을땐 정신력이 감소합니다.";
                return;
            case 2:
                tipText.text = "팁 : 동료들과 함께 다니세요.";
                return;
        }

    }

    /// <summary>
    /// 돌아가기 버튼
    /// 클릭시 멀티로비씬으로 돌아감
    /// </summary>
    public void OnBackButtonClick()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("MultiLobby");
    }

    /// <summary>
    /// 메세지 천천히 출력되게 나오는 함수들
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
