using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class journalBook : MonoBehaviour
{
    //�޴� ��ư 3����
    public Button homeMark;
    public Button evidenceMark;
    public Button ghostInfoMark;

    //������ 3����
    private GameObject homePage;
    private GameObject evidencePage;
    private GameObject ghostInfoPage;


    // ������ �ѱ�� ���� ����
    private int currentPage = 0;
    public TextMeshProUGUI leftNumber; // �����ϴ� ������ ǥ��
    public TextMeshProUGUI rightNumber; // �����ϴ� ������ ǥ��


    //��Ż ������ ��� �� UI�� ǥ��
    public TextMeshProUGUI mentalGaugeText; //���� �޾ƿ��� ������ ��
    private mentalGaugeManager mental; //��Ż ������ �Ŵ����� ����ϱ� ���� ����

    //���� ������ ���� üũ ���
    public Toggle[] evidenceItemCheck = new Toggle[3];
    public ToggleGroup ghostToggleGroup;

    // ��Ʈ ���� ������ �Ҵ� ����
    public TextMeshProUGUI ghostName;
    public TextMeshProUGUI ghostDescription;
    public TextMeshProUGUI relatedItem;

    //UI ��ġ��ҵ� �ڵ� �Ҵ� ����
    void Start()
    {

        assignComponent();

        //��� ��ư ��� �Ҵ�
        homeMark.onClick.AddListener(ShowHomePage);
        evidenceMark.onClick.AddListener(ShowEvidencePage);
        ghostInfoMark.onClick.AddListener(ShowGhostInfoPage);

    }

    // ������Ʈ�� �ڵ����� �Ҵ����ִ� �Լ�
    void assignComponent()
    {
        // 'journal' �±׸� ���� ������Ʈ�� �ڽĵ��� ã�� �Ҵ�
        GameObject playerBook = GameObject.FindGameObjectWithTag("journal");

        // ��ư�� �Ҵ�
        homeMark = playerBook.transform.Find("HomeButton").GetComponent<Button>();
        evidenceMark = playerBook.transform.Find("EvidenceButton").GetComponent<Button>();
        ghostInfoMark = playerBook.transform.Find("GhostInfoButton").GetComponent<Button>();

        // ������ �ѹ� �ؽ�Ʈ �Ҵ�
        leftNumber = playerBook.transform.Find("LeftNumberText").GetComponent<TextMeshProUGUI>();
        rightNumber = playerBook.transform.Find("RightNumberText").GetComponent<TextMeshProUGUI>();

        // ��Ż ������ �ؽ�Ʈ �Ҵ�
        mentalGaugeText = playerBook.transform.Find("MentalGaugeText").GetComponent<TextMeshProUGUI>();

        // ���� üũ ��� �Ҵ�
        for (int i = 0; i < evidenceItemCheck.Length; i++)
        {
            evidenceItemCheck[i] = playerBook.transform.Find($"EvidenceItemToggle{i + 1}").GetComponent<Toggle>();
        }

        // ��Ʈ ���� ������ �ؽ�Ʈ �Ҵ�
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
