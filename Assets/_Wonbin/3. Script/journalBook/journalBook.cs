using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class journalBook : MonoBehaviour
{


    // ��� �޴� ��ư 3����
    public Button homeMark;
    public Button evidenceMark;
    public Button ghostInfoMark;

    // ������ �Ҵ�
    private GameObject homePage;
    private GameObject evidencePage;
    private GameObject ghostInfoPage;

    // ������ �ѱ�� ���� ���
    private int currentPage = 0;
    public TextMeshProUGUI leftNumber; // �����ϴ� ������ ǥ��
    public TextMeshProUGUI rightNumber; // �����ϴ� ������ ǥ��
    public Button leftButton;
    public Button rightButton;

    // ��Ż ������ ��� �� UI�� ǥ��
    public TextMeshProUGUI mentalGaugeText; // ���� �޾ƿ��� ������ ��

    // ���� ������ ���� üũ ���
    evidenceEnum evidence; // ���� ������ �޾ƿ��� ��
    public Toggle[] evidenceItemCheck = new Toggle[3]; // 3���� ���� ���
    public ToggleGroup ghostToggleGroup; // �ͽ� ��� �׷�

    // ��Ʈ ���� ������ �Ҵ� ����
    [SerializeField]
    private ghostObject[] ghostObjects = new ghostObject[3]; // ��Ʈ object

    public TextMeshProUGUI ghostName;
    public TextMeshProUGUI ghostDescription;
    public TextMeshProUGUI relatedItem;
    public Image ghostImage;

    // UI ��ġ��ҵ� �ڵ� �Ҵ� ����
    void Start()
    {
        // ������ �ʱ�ȭ
        homePage = transform.GetChild(1).gameObject;
        evidencePage = transform.GetChild(2).gameObject;
        ghostInfoPage = transform.GetChild(3).gameObject;

        // ��� ��ư ��� �Ҵ�
        homeMark.onClick.AddListener(ShowHomePage);
        evidenceMark.onClick.AddListener(ShowEvidencePage);
        ghostInfoMark.onClick.AddListener(ShowGhostInfoPage);

        // ������ ��ȯ ��ư ��� �Ҵ�
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
            toggle.onValueChanged.AddListener(OnEvidenceToggleChanged); // ��� ���� ����� ������ ���� Ȯ��
        }

        gameObject.SetActive(false); // �ʱ⿡�� ��Ȱ��ȭ

    }

    void Update()
    {
        Debug.Log("Update is called");
        showMentalGauge(); // �÷��̾��� ��Ż ������ ��� ����.

        // ��Ʈ ���� �������� Ȱ��ȭ�Ǿ� ���� ���� ���� ������Ʈ
        if (ghostInfoPage.activeSelf)
        {
            UpdateGhostInfo();
        }

        // ���� ��� ���� �� ��Ʈ ��� ���� ������Ʈ
        CheckGhostToggles();
    }

    public void OnEvidenceToggleChanged(bool isOn)
    {
        foreach (Toggle toggle in evidenceItemCheck)
        {
            // Toggle�� �ڽ� �� "Checkmark" ������Ʈ�� ã��
            Transform checkmarkTransform = toggle.transform.Find("Background/Checkmark"); //��� ���� �߿���.

            if (checkmarkTransform != null)
            {
                GameObject checkmark = checkmarkTransform.gameObject;

                // Toggle�� isOn ���� Ȯ�� �� Checkmark ���� ������Ʈ
                if (toggle.isOn)
                {
                    checkmark.SetActive(true);  // üũ�Ǿ��� �� Checkmark Ȱ��ȭ
                }
                else
                {
                    checkmark.SetActive(false); // üũ ���� �� Checkmark ��Ȱ��ȭ
                }
            }
            else
            {
                Debug.LogError($"{toggle.name}�� 'Checkmark' ������Ʈ�� �����ϴ�.");
            }
        }
    }


    public void CheckGhostToggles()
    {
        int selectedEvidenceCount = 0;
        List<string> selectedEvidence = new List<string>();

        // ���õ� ���Ÿ� Ȯ��
        foreach (Toggle toggle in evidenceItemCheck)
        {
            if (toggle.isOn)
            {
                selectedEvidenceCount++;
                selectedEvidence.Add(toggle.name); // ���õ� ���� �̸��� ����Ʈ�� �߰�
                Debug.Log($"Selected Evidence: {toggle.name}"); // ���õ� ���� Ȯ��
            }
        }

        // �� ���� ���Ű� ���õǾ��� ���� Ȯ��
        if (selectedEvidenceCount == 2)
        {
            // Banshee�� ���� ��
            if (selectedEvidence.Contains(evidenceEnum.BANSHEE.UVLight.ToString()) &&
                selectedEvidence.Contains(evidenceEnum.BANSHEE.Camcoder.ToString()))
            {
                SetGhostToggle(0); // Banshee ��� �ѱ�
            }
            // Demon�� ���� ��
            else if (selectedEvidence.Contains(evidenceEnum.DEMON.EMF.ToString()) &&
                     selectedEvidence.Contains(evidenceEnum.DEMON.UVLight.ToString()))
            {
                SetGhostToggle(1); // Demon ��� �ѱ�
            }
            // Nightmare�� ���� ��
            else if (selectedEvidence.Contains(evidenceEnum.NIGHTMARE.Camcoder.ToString()) &&
                     selectedEvidence.Contains(evidenceEnum.NIGHTMARE.EMF.ToString()))
            {
                SetGhostToggle(2); // Nightmare ��� �ѱ�
                
            }
            else
            {
                // ��ġ�ϴ� �ͽ��� ������ ��� ��� ����
                ghostToggleGroup.SetAllTogglesOff();
            }
        }
        else
        {
            Debug.Log("���Ű� ������� �ʽ��ϴ�.");
        }
    }

    void SetGhostToggle(int index)
    {
        Toggle[] toggles = ghostToggleGroup.GetComponentsInChildren<Toggle>();
        for (int i = 0; i < toggles.Length; i++)
        {
            toggles[i].isOn = (i == index); // index�� �ش��ϴ� Toggle�� Ȱ��ȭ
            Transform ghostTransform = toggles[i].transform.Find("CheckImage");
            ghostTransform.gameObject.SetActive(i == index); // �ش��ϴ� �ͽ� ��۸� Ȱ��ȭ
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
        UpdateGhostInfo(); // ��Ʈ ���� ������Ʈ
    }

    void showMentalGauge()
    {
        mentalGaugeManager mentalGaugeManager = GameObject.FindWithTag("Player").GetComponent<mentalGaugeManager>();
        mentalGaugeText.text = mentalGaugeManager.MentalGauge.ToString(); // ��Ż ������ �� ����
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

            // ������ ��ȣ ������Ʈ
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