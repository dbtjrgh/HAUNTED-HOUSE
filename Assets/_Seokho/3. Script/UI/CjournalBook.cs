using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CjournalBook : MonoBehaviour
{
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

    public bool ghostSelected = false;

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

    public void SetGhostSelected(bool selected)
    {
        ghostSelected = selected;
        CheckAllPlayersGhostSelected();
    }

    private void CheckAllPlayersGhostSelected()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.CheckAllPlayersSelectedGhost();
        }
    }

    public void OnEvidenceToggleChanged(bool isOn)
    {
        foreach (Toggle toggle in evidenceItemCheck)
        {
            Transform checkmarkTransform = toggle.transform.Find("Background/Checkmark");

            if (checkmarkTransform != null)
            {
                GameObject checkmark = checkmarkTransform.gameObject;
                checkmark.SetActive(toggle.isOn);
            }
        }
    }

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
            if (selectedEvidence.Contains(evidenceEnum.BANSHEE.UVLight.ToString()) &&
                selectedEvidence.Contains(evidenceEnum.BANSHEE.Camcoder.ToString()))
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
            else if (selectedEvidence.Contains(evidenceEnum.NIGHTMARE.Camcoder.ToString()) &&
                     selectedEvidence.Contains(evidenceEnum.NIGHTMARE.EMF.ToString()))
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
        UpdateGhostInfo();
    }

    void showMentalGauge()
    {
        mentalGaugeManager mentalGaugeManager = GameObject.FindWithTag("Player").GetComponent<mentalGaugeManager>();
        mentalGaugeText.text = mentalGaugeManager.MentalGauge.ToString();
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

            int leftPageNumber = currentPage * 2 + 1;
            int rightPageNumber = leftPageNumber + 1;

            leftNumber.text = leftPageNumber.ToString();
            rightNumber.text = (rightPageNumber <= ghostObjects.Length * 2) ? rightPageNumber.ToString() : "";
        }
    }

    void ShowPreviousPage()
    {
        if (ghostObjects.Length > 0)
        {
            currentPage = (currentPage == 0) ? ghostObjects.Length - 1 : currentPage - 1;
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
