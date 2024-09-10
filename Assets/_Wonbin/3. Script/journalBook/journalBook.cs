using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class journalBook : MonoBehaviour
{


    // 상단 메뉴 버튼 3가지
    public Button homeMark;
    public Button evidenceMark;
    public Button ghostInfoMark;

    // 페이지 할당
    private GameObject homePage;
    private GameObject evidencePage;
    private GameObject ghostInfoPage;

    // 페이지 넘기기 위한 기능
    private int currentPage = 0;
    public TextMeshProUGUI leftNumber; // 좌측하단 페이지 표기
    public TextMeshProUGUI rightNumber; // 우측하단 페이지 표기
    public Button leftButton;
    public Button rightButton;

    // 멘탈 게이지 상속 후 UI에 표기
    public TextMeshProUGUI mentalGaugeText; // 갱신 받아오는 게이지 값

    // 증거 페이지 증거 체크 토글
    evidenceEnum evidence; // 증거 아이템 받아오는 값
    public Toggle[] evidenceItemCheck = new Toggle[3]; // 3개의 증거 토글
    public ToggleGroup ghostToggleGroup; // 귀신 토글 그룹

    // 고스트 정보 페이지 할당 변수
    [SerializeField]
    private ghostObject[] ghostObjects = new ghostObject[3]; // 고스트 object

    public TextMeshProUGUI ghostName;
    public TextMeshProUGUI ghostDescription;
    public TextMeshProUGUI relatedItem;
    public Image ghostImage;

    // UI 배치요소들 자동 할당 구성
    void Start()
    {
        // 페이지 초기화
        homePage = transform.GetChild(1).gameObject;
        evidencePage = transform.GetChild(2).gameObject;
        ghostInfoPage = transform.GetChild(3).gameObject;

        // 상단 버튼 기능 할당
        homeMark.onClick.AddListener(ShowHomePage);
        evidenceMark.onClick.AddListener(ShowEvidencePage);
        ghostInfoMark.onClick.AddListener(ShowGhostInfoPage);

        // 페이지 전환 버튼 기능 할당
        if (leftButton != null)
        {
            leftButton.onClick.AddListener(ShowPreviousPage);
            Debug.Log("Left Button listener added.");
        }
        else
        {
            Debug.LogError("Left Button is not assigned.");
        }
        rightButton.onClick.AddListener(ShowNextPage);


        foreach (Toggle toggle in evidenceItemCheck)
        {
            toggle.onValueChanged.AddListener(OnEvidenceToggleChanged); // 토글 값이 변경될 때마다 조건 확인
        }

        gameObject.SetActive(false); // 초기에는 비활성화

    }

    void Update()
    {
        Debug.Log("Update is called");
        showMentalGauge(); // 플레이어의 멘탈 게이지 상시 갱신.

        // 고스트 정보 페이지가 활성화되어 있을 때만 정보 업데이트
        if (ghostInfoPage.activeSelf)
        {
            UpdateGhostInfo();
        }

        // 증거 토글 대조 후 고스트 토글 상태 업데이트
        CheckGhostToggles();
    }

    public void OnEvidenceToggleChanged(bool isOn)
    {
        foreach (Toggle toggle in evidenceItemCheck)
        {
            // Toggle의 자식 중 "Checkmark" 오브젝트를 찾음
            Transform checkmarkTransform = toggle.transform.Find("Background/Checkmark"); //경로 설정 중요함.

            if (checkmarkTransform != null)
            {
                GameObject checkmark = checkmarkTransform.gameObject;

                // Toggle의 isOn 상태 확인 후 Checkmark 상태 업데이트
                if (toggle.isOn)
                {
                    checkmark.SetActive(true);  // 체크되었을 때 Checkmark 활성화
                }
                else
                {
                    checkmark.SetActive(false); // 체크 해제 시 Checkmark 비활성화
                }
            }
            else
            {
                Debug.LogError($"{toggle.name}에 'Checkmark' 오브젝트가 없습니다.");
            }
        }
    }


    public void CheckGhostToggles()
    {
        int selectedEvidenceCount = 0;
        List<string> selectedEvidence = new List<string>();

        // 선택된 증거를 확인
        foreach (Toggle toggle in evidenceItemCheck)
        {
            if (toggle.isOn)
            {
                selectedEvidenceCount++;
                selectedEvidence.Add(toggle.name); // 선택된 증거 이름을 리스트에 추가
                Debug.Log($"Selected Evidence: {toggle.name}"); // 선택된 증거 확인
            }
        }

        // 두 개의 증거가 선택되었을 때만 확인
        if (selectedEvidenceCount == 2)
        {
            // Banshee와 증거 비교
            if (selectedEvidence.Contains(evidenceEnum.BANSHEE.UVLight.ToString()) &&
                selectedEvidence.Contains(evidenceEnum.BANSHEE.Camcoder.ToString()))
            {
                SetGhostToggle(0); // Banshee 토글 켜기
            }
            // Demon과 증거 비교
            else if (selectedEvidence.Contains(evidenceEnum.DEMON.EMF.ToString()) &&
                     selectedEvidence.Contains(evidenceEnum.DEMON.UVLight.ToString()))
            {
                SetGhostToggle(1); // Demon 토글 켜기
            }
            // Nightmare와 증거 비교
            else if (selectedEvidence.Contains(evidenceEnum.NIGHTMARE.Camcoder.ToString()) &&
                     selectedEvidence.Contains(evidenceEnum.NIGHTMARE.EMF.ToString()))
            {
                SetGhostToggle(2); // Nightmare 토글 켜기
                
            }
            else
            {
                // 일치하는 귀신이 없으면 모든 토글 끄기
                ghostToggleGroup.SetAllTogglesOff();
            }
        }
        else
        {
            Debug.Log("증거가 충분하지 않습니다.");
        }
    }

    void SetGhostToggle(int index)
    {
        Toggle[] toggles = ghostToggleGroup.GetComponentsInChildren<Toggle>();
        for (int i = 0; i < toggles.Length; i++)
        {
            toggles[i].isOn = (i == index); // index에 해당하는 Toggle만 활성화
            Transform ghostTransform = toggles[i].transform.Find("CheckImage");
            ghostTransform.gameObject.SetActive(i == index); // 해당하는 귀신 토글만 활성화
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

    void showMentalGauge()
    {
        mentalGaugeManager mentalGaugeManager = GameObject.FindWithTag("Player").GetComponent<mentalGaugeManager>();
        mentalGaugeText.text = mentalGaugeManager.MentalGauge.ToString(); // 멘탈 게이지 값 갱신
    }

    void UpdateGhostInfo()
    {
        if (ghostObjects.Length > 0)
        {
            ghostObject currentGhostObjects = ghostObjects[currentPage];
            ghostName.text = currentGhostObjects.ghostName;
            ghostDescription.text = currentGhostObjects.description;
            relatedItem.text = currentGhostObjects.relatedItem;
            ghostImage.sprite = currentGhostObjects.ghostImage;

            // 페이지 번호 업데이트
            leftNumber.text = (currentPage + 1).ToString();
            rightNumber.text = ghostObjects.Length.ToString();
        }
    }

    void ShowPreviousPage()
    {
        if (ghostObjects.Length > 0)
        {

            currentPage = (currentPage - 1) % ghostObjects.Length;
            UpdateGhostInfo();
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