using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CGameDefeatUI : MonoBehaviour
{
    public Button backButton;
    public Text tipText;
    private int random;
    private string targetMsg;
    private int index;
    private float interval;
    public float CharPerSeconds;
    public Text Correctanswer;
    private void Awake()
    {
        backButton.onClick.AddListener(OnBackButtonClick);
        tipText = GetComponent<Text>();
        random = Random.Range(0, 3);
        setMsg($"유령의 정체는...?  {Ghost.instance.ghostType}");
    }
    public void OnBackButtonClick()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("MultiLobby");
    }

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
        Correctanswer.text += targetMsg[index];
        index++;

        Invoke("Effecting", interval);
    }
}
