using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CChooseMapScreen : MonoBehaviour
{
    #region 변수
    public Button factoryButton; // Factory 맵 선택 버튼
    public Button turkwoodButton; // Turkwood 맵 선택 버튼
    public Button backButton; // 뒤로 가기 버튼
    public Image factoryImage;
    public Image turkwoodImage;

    private string selectedMap = ""; // 선택된 맵 이름

    public GameObject roomScreen;
    public GameObject ChooseMapScreen;
    #endregion

    private void Start()
    {

        // 버튼에 리스너 등록
        factoryButton.onClick.AddListener(() => SelectMap("공장"));
        turkwoodButton.onClick.AddListener(() => SelectMap("폐허"));
        backButton.onClick.AddListener(BackToRoom);
    }

    /// <summary>
    /// 맵 선택 함수 (한 번에 하나만 선택)
    /// </summary>
    /// <param name="mapName"></param>
    private void SelectMap(string mapName)
    {
        selectedMap = mapName;

        // 두 버튼의 상태를 설정 (선택된 버튼은 상호작용 불가)
        factoryButton.interactable = mapName != "공장";
        turkwoodButton.interactable = mapName != "폐허";

        if ( mapName == "공장")
        {
            factoryImage.enabled = true;
            turkwoodImage.enabled = false;
        }
        else if( mapName == "폐허")
        {
            factoryImage.enabled = false;
            turkwoodImage.enabled = true;
        }

        // 선택된 맵 정보를 RoomScreen으로 전달
        roomScreen.GetComponent<CRoomScreen>().ChooseMap(mapName);
    }

    /// <summary>
    /// 뒤로 가기 버튼 함수
    /// </summary>
    private void BackToRoom()
    {
        // RoomScreen 화면 활성화
        roomScreen.gameObject.SetActive(true);
        // 맵 선택 화면 비활성화
        ChooseMapScreen.gameObject.SetActive(false); // 현재 오브젝트 비활성화
    }
}
