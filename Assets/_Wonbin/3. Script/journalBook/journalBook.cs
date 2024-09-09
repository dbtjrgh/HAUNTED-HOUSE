using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class journalBook : MonoBehaviour
{
    //상단 메뉴 버튼 3가지
    public Button homeMark;
    public Button evidenceMark;
    public Button ghostInfoMark;

    //페이지 할당
    private GameObject homePage;
    private GameObject evidencePage;
    private GameObject ghostInfoPage;

    // 페이지 넘기기 위한 기능
    private int currentPage = 0;
    public TextMeshProUGUI leftNumber; // 좌측하단 페이지 표기
    public TextMeshProUGUI rightNumber; // 우측하단 페이지 표기
    public Button leftButton;
    public Button rightButton;
   

    //멘탈 게이지 상속 후 UI에 표기
    public TextMeshProUGUI mentalGaugeText; //갱신 받아오는 게이지 값

    //증거 페이지 증거 체크 토글
    public Toggle[] evidenceItemCheck = new Toggle[3];
    public ToggleGroup ghostToggleGroup;

    // 고스트 정보 페이지 할당 변수
    [SerializeField]
    private ghostObject[] ghostObjects = new ghostObject[3]; //고스트 object

    public TextMeshProUGUI ghostName;
    public TextMeshProUGUI ghostDescription;
    public TextMeshProUGUI relatedItem;

    //UI 배치요소들 자동 할당 구성
    void Start()
    {
        //0번은 remainUI임.
        homePage = transform.GetChild(1).gameObject;
        evidencePage = transform.GetChild(2).gameObject;
        ghostInfoPage = transform.GetChild(3).gameObject;

        //상단 버튼 기능 할당
        homeMark.onClick.AddListener(ShowHomePage);
        evidenceMark.onClick.AddListener(ShowEvidencePage);
        ghostInfoMark.onClick.AddListener(ShowGhostInfoPage);

        // 페이지 전환 버튼 기능 할당

        leftButton.onClick.AddListener(ShowPreviousPage);
        rightButton.onClick.AddListener(ShowNextPage);

        gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            // 현재 스크립트가 붙어 있는 오브젝트의 활성화 상태를 반전
            gameObject.SetActive(!gameObject.activeSelf);
        }

        showMentalGauge(); // 플레이어의 멘탈게이지가 변화할 때마다 상시 갱신.

        // 고스트 정보 페이지가 활성화되어 있을 때만 정보 업데이트
        if (ghostInfoPage.activeSelf)
        {
            UpdateGhostInfo();
        }
    }

    void ShowHomePage()
    {
        homePage.SetActive(true);
        evidencePage.SetActive(false);
        ghostInfoPage.SetActive(false);
    }

    void ShowEvidencePage()
    {
        homePage.SetActive(false);
        evidencePage.SetActive(true);
        ghostInfoPage.SetActive(false);
    }

    void ShowGhostInfoPage()
    {
        homePage.SetActive(false);
        evidencePage.SetActive(false);
        ghostInfoPage.SetActive(true);
        UpdateGhostInfo(); // 고스트 정보 업데이트
    }


    //멘탈 게이지를 표기해주는 메서드
    void showMentalGauge()
    {
        mentalGaugeManager mentalGaugeManager = GameObject.FindWithTag("Player").GetComponent<mentalGaugeManager>();
        mentalGaugeText.text = mentalGaugeManager.MentalGauge.ToString(); // 멘탈게이지 텍스트를 플레이어의 MentalGauge에서 상속받아 표기.
    }

    void UpdateGhostInfo()
    {
        if (ghostObjects.Length > 0)
        {
            ghostObject currentGhostObjects = ghostObjects[currentPage];
            ghostName.text = currentGhostObjects.ghostName;
            ghostDescription.text = currentGhostObjects.description;
            relatedItem.text = currentGhostObjects.relatedItem;

            // 페이지 번호 업데이트
            leftNumber.text = (currentPage + 1).ToString();
            rightNumber.text = ghostObjects.Length.ToString();
        }
    }

    void ShowPreviousPage()
    {
        Debug.Log("버튼 호출 완료");
        if (ghostObjects.Length > 0)
        {
            currentPage = (currentPage - 1 + ghostObjects.Length) % ghostObjects.Length;
            UpdateGhostInfo();
            Debug.Log("페이지 넘김 완료");
        }

        else
        {
            Debug.Log("페이지 넘김 실패");
        }
    }

    void ShowNextPage()
    {
        if (ghostObjects.Length > 0)
        {
            currentPage = (currentPage + 1) % ghostObjects.Length;
            UpdateGhostInfo();
        }
    }

}