using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class journalBook : MonoBehaviour
{
    //메뉴 버튼 3가지
    public Button homeMark;
    public Button evidenceMark;
    public Button ghostInfoMark;

    //페이지 3가지
    private GameObject homePage;
    private GameObject evidencePage;
    private GameObject ghostInfoPage;


    // 페이지 넘기기 위한 숫자
    private int currentPage = 0;
    public TextMeshProUGUI leftNumber; // 좌측하단 페이지 표기
    public TextMeshProUGUI rightNumber; // 우측하단 페이지 표기


    //멘탈 게이지 상속 후 UI에 표기
    public TextMeshProUGUI mentalGaugeText; //갱신 받아오는 게이지 값
    private mentalGaugeManager mental; //멘탈 게이지 매니저를 상속하기 위한 변수

    //증거 페이지 증거 체크 토글
    public Toggle[] evidenceItemCheck = new Toggle[3];
    public ToggleGroup ghostToggleGroup;

    // 고스트 정보 페이지 할당 변수
    public TextMeshProUGUI ghostName;
    public TextMeshProUGUI ghostDescription;
    public TextMeshProUGUI relatedItem;

    //UI 배치요소들 자동 할당 구성
    void Start()
    {

        assignComponent();

        //상단 버튼 기능 할당
        homeMark.onClick.AddListener(ShowHomePage);
        evidenceMark.onClick.AddListener(ShowEvidencePage);
        ghostInfoMark.onClick.AddListener(ShowGhostInfoPage);

    }

    // 오브젝트를 자동으로 할당해주는 함수
    void assignComponent()
    {
        // 'journal' 태그를 가진 오브젝트의 자식들을 찾아 할당
        GameObject playerBook = GameObject.FindGameObjectWithTag("journal");

        // 버튼들 할당
        homeMark = playerBook.transform.Find("HomeButton").GetComponent<Button>();
        evidenceMark = playerBook.transform.Find("EvidenceButton").GetComponent<Button>();
        ghostInfoMark = playerBook.transform.Find("GhostInfoButton").GetComponent<Button>();

        // 페이지 넘버 텍스트 할당
        leftNumber = playerBook.transform.Find("LeftNumberText").GetComponent<TextMeshProUGUI>();
        rightNumber = playerBook.transform.Find("RightNumberText").GetComponent<TextMeshProUGUI>();

        // 멘탈 게이지 텍스트 할당
        mentalGaugeText = playerBook.transform.Find("MentalGaugeText").GetComponent<TextMeshProUGUI>();

        // 증거 체크 토글 할당
        for (int i = 0; i < evidenceItemCheck.Length; i++)
        {
            evidenceItemCheck[i] = playerBook.transform.Find($"EvidenceItemToggle{i + 1}").GetComponent<Toggle>();
        }

        // 고스트 정보 페이지 텍스트 할당
        ghostName = playerBook.transform.Find("GhostNameText").GetComponent<TextMeshProUGUI>();
        ghostDescription = playerBook.transform.Find("GhostDescriptionText").GetComponent<TextMeshProUGUI>();
        relatedItem = playerBook.transform.Find("RelatedItemText").GetComponent<TextMeshProUGUI>();
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
    }

}
