using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CjournalBook : MonoBehaviour
{
    #region 변수
    public Button homeMark;
    public Button evidenceMark;
    public Button ghostInfoMark;

    private GameObject homePage;
    private GameObject evidencePage;
    private GameObject ghostInfoPage;

    private int currentPage = 0;
    public TextMeshProUGUI leftNumber;
    public TextMeshProUGUI rightNumber;
    public Button leftButton;
    public Button rightButton;

    public TextMeshProUGUI mentalGaugeText;

    public Toggle[] evidenceItemCheck = new Toggle[3];
    public ToggleGroup ghostToggleGroup;

    [SerializeField]
    private ghostObject[] ghostObjects = new ghostObject[3];

    public TextMeshProUGUI ghostName;
    public TextMeshProUGUI ghostDescription;
    public TextMeshProUGUI relatedItem;
    public Image ghostImage;

    public Toggle resultNightMare;
    public Toggle resultDemon;
    public Toggle resultBanshee;

    // 결과를 UI에 반영
    public CGameResultUI resultUI;

    public bool ghostSelected = false;
    #endregion

    void Start()
    {
        homePage = transform.GetChild(1).gameObject;
        evidencePage = transform.GetChild(2).gameObject;
        ghostInfoPage = transform.GetChild(3).gameObject;

        homeMark.onClick.AddListener(ShowHomePage);
        evidenceMark.onClick.AddListener(ShowEvidencePage);
        ghostInfoMark.onClick.AddListener(ShowGhostInfoPage);

        leftButton.onClick.AddListener(ShowPreviousPage);
        rightButton.onClick.AddListener(ShowNextPage);

        foreach (Toggle toggle in evidenceItemCheck)
        {
            toggle.onValueChanged.AddListener(OnEvidenceToggleChanged);
        }

        setupToggles();

    }

    void Update()
    {
        showMentalGauge();

        if (ghostInfoPage.activeSelf)
        {
            UpdateGhostInfo();
        }

        CheckGhostToggles();
    }

    /// <summary>
    /// 고스트가 선택됐다면 bool값으로 처리
    /// </summary>
    /// <param name="selected"></param>
    public void SetGhostSelected(bool selected)
    {
        ghostSelected = selected;
    }

    /// <summary>
    /// 변경된 토글 하나만의 Checkmark 상태를 업데이트해주는 함수
    /// </summary>
    /// <param name="isOn"></param>
    public void OnEvidenceToggleChanged(bool isOn)
    {
        for (int i = 0; i < evidenceItemCheck.Length; i++)
        {
            Transform checkmarkTransform = evidenceItemCheck[i].transform.Find("Background/Checkmark");

            if (checkmarkTransform != null)
            {
                GameObject checkmark = checkmarkTransform.gameObject;
                checkmark.SetActive(evidenceItemCheck[i].isOn);
            }
        }
    }

    /// <summary>
    /// // 처음 1회, 증거들의 toggle.ison을 비활성화시켜주는 함수
    /// </summary>
    public void setupToggles()
    {
        foreach(Toggle toggle in evidenceItemCheck)
        {
            toggle.isOn = false;
        }
    }

    /// <summary>
    /// 증거 토글 선택에 따라 고스트의 이름 토글이 자동으로 체크시켜주는 함수
    /// </summary>
    public void CheckGhostToggles()
    {
        int selectedEvidenceCount = 0;
        List<string> selectedEvidence = new List<string>();

        foreach (Toggle toggle in evidenceItemCheck)
        {
            if (toggle.isOn)
            {
                selectedEvidenceCount++;
                selectedEvidence.Add(toggle.name);
            }
        }

        if (selectedEvidenceCount == 2)
        {
            // 정확한 인덱스와 토글을 일치시킵니다.
            if (selectedEvidence.Contains(evidenceEnum.NIGHTMARE.Camcoder.ToString()) &&
                selectedEvidence.Contains(evidenceEnum.NIGHTMARE.EMF.ToString()))
            {
                SetGhostToggle(0);
                SetGhostSelected(true);
            }
            else if (selectedEvidence.Contains(evidenceEnum.DEMON.EMF.ToString()) &&
                     selectedEvidence.Contains(evidenceEnum.DEMON.UVLight.ToString()))
            {
                SetGhostToggle(1);
                SetGhostSelected(true);
            }
            else if (selectedEvidence.Contains(evidenceEnum.BANSHEE.UVLight.ToString()) &&
                     selectedEvidence.Contains(evidenceEnum.BANSHEE.Camcoder.ToString()))
            {
                SetGhostToggle(2);
                SetGhostSelected(true);
            }
            else
            {
                ghostToggleGroup.SetAllTogglesOff();
                SetGhostSelected(false);
            }
        }
        else
        {
            ghostToggleGroup.SetAllTogglesOff(); // 두 개 이상의 토글이 선택되지 않은 경우 모든 토글을 비활성화
            SetGhostSelected(false);
        }
    }

    void SetGhostToggle(int index)
    {
        Toggle[] toggles = ghostToggleGroup.GetComponentsInChildren<Toggle>();


        for (int i = 0; i < toggles.Length; i++)
        {
            toggles[i].isOn = (i == index);
            Transform ghostTransform = toggles[i].transform.Find("CheckImage");
            ghostTransform.gameObject.SetActive(i == index);
        }
        CheckGhostMatchWithToggle();
    }
    /// <summary>
    /// 선택한 고스트와 씬에 있는 고스트가 같은지 여부에 따라
    /// 게임 결과 UI에 나타나는 텍스트가 결정되게 하는 함수
    /// </summary>
    public void CheckGhostMatchWithToggle()
    {
        Ghost ghost = FindObjectOfType<Ghost>();  // Ghost 클래스 인스턴스를 가져옴
        if (ghost == null) return;

        string resultText = "";
        string ghostTypeText = ghost.ghostType.ToString();

        if ((ghostTypeText == "NIGHTMARE" && resultNightMare.isOn) ||
            (ghostTypeText == "DEMON" && resultDemon.isOn) ||
            (ghostTypeText == "BANSHEE" && resultBanshee.isOn))
        {
            resultText = "귀신 유형이 일치합니다!";
        }
        else
        {
            resultText = "귀신 유형이 일치하지 않습니다.";
        }


        if (resultUI != null)
        {
            if (ghostTypeText == "NIGHTMARE")
            {
                ghostTypeText = "나이트메어";
            }
            else if (ghostTypeText == "DEMON")
            {
                ghostTypeText = "데몬";
            }
            else if (ghostTypeText == "BANSHEE")
            {
                ghostTypeText = "밴시";
            }
            resultUI.SetGameResult(resultText, ghostTypeText);
        }
    }

    /// <summary>
    /// 저널 메인 화면 탭 열기 함수
    /// </summary>
    void ShowHomePage()
    {
        homePage.SetActive(true);
        evidencePage.SetActive(false);
        ghostInfoPage.SetActive(false);
    }
    /// <summary>
    /// 저널 증거 및 투표 화면 탭 열기 함수
    /// </summary>
    void ShowEvidencePage()
    {
        homePage.SetActive(false);
        evidencePage.SetActive(true);
        ghostInfoPage.SetActive(false);
    }
    /// <summary>
    /// 저널 귀신 정보 화면 탭 열기 함수
    /// </summary>
    void ShowGhostInfoPage()
    {
        homePage.SetActive(false);
        evidencePage.SetActive(false);
        ghostInfoPage.SetActive(true);
        UpdateGhostInfo();
    }
    /// <summary>
    /// 해당 플레이어의 멘탈 게이지를 가져와서 표시해주는 함수
    /// </summary>
    void showMentalGauge()
    {
        mentalGaugeManager mentalGaugeManager = GameObject.FindWithTag("Player").GetComponent<mentalGaugeManager>();
        mentalGaugeText.text = mentalGaugeManager.MentalGauge.ToString("F1");
    }

    /// <summary>
    /// 귀신 정보탭 내 파일을 할당해 나타내는 함수
    /// </summary>
    void UpdateGhostInfo()
    {
        if (ghostObjects.Length > 0)
        {
            ghostObject currentGhostObjects = ghostObjects[currentPage];
            ghostName.text = currentGhostObjects.ghostName;
            ghostDescription.text = currentGhostObjects.description;
            relatedItem.text = currentGhostObjects.relatedItem;
            ghostImage.sprite = currentGhostObjects.ghostImage;

            int leftPageNumber = currentPage * 2 + 1;
            int rightPageNumber = leftPageNumber + 1;

            leftNumber.text = leftPageNumber.ToString();
            rightNumber.text = (rightPageNumber <= ghostObjects.Length * 2) ? rightPageNumber.ToString() : "";
        }
    }

    /// <summary>
    /// 귀신 정보탭 내 이전 페이지로 넘기는 함수
    /// </summary>
    void ShowPreviousPage()
    {
        if (ghostObjects.Length > 0)
        {
            currentPage = (currentPage == 0) ? ghostObjects.Length - 1 : currentPage - 1;
            UpdateGhostInfo();
        }
    }

    /// <summary>
    /// 귀신 정보탭 내 다음 페이지로 넘기는 함수
    /// </summary>
    void ShowNextPage()
    {
        if (ghostObjects.Length > 0)
        {
            currentPage = (currentPage + 1) % ghostObjects.Length;
            UpdateGhostInfo();
        }
    }
}
