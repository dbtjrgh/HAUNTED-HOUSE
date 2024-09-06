using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CChooseMapScreen : MonoBehaviour
{
    public Button factoryButton; // Factory 맵 선택 버튼
    public Button turkwoodButton; // Turkwood 맵 선택 버튼
    public Button backButton; // 뒤로 가기 버튼

    private string selectedMap = ""; // 선택된 맵 이름

    public GameObject roomScreen;
    public GameObject ChooseMapScreen;

    private void Start()
    {

        // 버튼에 리스너 등록
        factoryButton.onClick.AddListener(() => SelectMap("Factory"));
        turkwoodButton.onClick.AddListener(() => SelectMap("Turkwood"));
        backButton.onClick.AddListener(BackToRoom);
    }

    // 맵 선택 메서드 (한 번에 하나만 선택)
    private void SelectMap(string mapName)
    {
        selectedMap = mapName;

        // 두 버튼의 상태를 설정 (선택된 버튼은 상호작용 불가)
        factoryButton.interactable = mapName != "Factory";
        turkwoodButton.interactable = mapName != "Turkwood";

        // 선택된 맵 정보를 RoomScreen으로 전달
        roomScreen.GetComponent<CRoomScreen>().ChooseMap(mapName);
    }

    // 뒤로 가기 버튼 메서드
    private void BackToRoom()
    {
        // RoomScreen 화면 활성화
        roomScreen.gameObject.SetActive(true);
        // 맵 선택 화면 비활성화
        ChooseMapScreen.gameObject.SetActive(false); // 현재 오브젝트 비활성화
    }
}
