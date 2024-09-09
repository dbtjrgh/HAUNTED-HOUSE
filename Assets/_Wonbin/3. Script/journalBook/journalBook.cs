using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class journalBook : MonoBehaviour
{
    //��� �޴� ��ư 3����
    public Button homeMark;
    public Button evidenceMark;
    public Button ghostInfoMark;

    //������ �Ҵ�
    private GameObject homePage;
    private GameObject evidencePage;
    private GameObject ghostInfoPage;

    // ������ �ѱ�� ���� ���
    private int currentPage = 0;
    public TextMeshProUGUI leftNumber; // �����ϴ� ������ ǥ��
    public TextMeshProUGUI rightNumber; // �����ϴ� ������ ǥ��
    public Button leftButton;
    public Button rightButton;
   

    //��Ż ������ ��� �� UI�� ǥ��
    public TextMeshProUGUI mentalGaugeText; //���� �޾ƿ��� ������ ��

    //���� ������ ���� üũ ���
    public Toggle[] evidenceItemCheck = new Toggle[3];
    public ToggleGroup ghostToggleGroup;

    // ��Ʈ ���� ������ �Ҵ� ����
    [SerializeField]
    private ghostObject[] ghostObjects = new ghostObject[3]; //��Ʈ object

    public TextMeshProUGUI ghostName;
    public TextMeshProUGUI ghostDescription;
    public TextMeshProUGUI relatedItem;

    //UI ��ġ��ҵ� �ڵ� �Ҵ� ����
    void Start()
    {
        //0���� remainUI��.
        homePage = transform.GetChild(1).gameObject;
        evidencePage = transform.GetChild(2).gameObject;
        ghostInfoPage = transform.GetChild(3).gameObject;

        //��� ��ư ��� �Ҵ�
        homeMark.onClick.AddListener(ShowHomePage);
        evidenceMark.onClick.AddListener(ShowEvidencePage);
        ghostInfoMark.onClick.AddListener(ShowGhostInfoPage);

        // ������ ��ȯ ��ư ��� �Ҵ�

        leftButton.onClick.AddListener(ShowPreviousPage);
        rightButton.onClick.AddListener(ShowNextPage);

        gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            // ���� ��ũ��Ʈ�� �پ� �ִ� ������Ʈ�� Ȱ��ȭ ���¸� ����
            gameObject.SetActive(!gameObject.activeSelf);
        }

        showMentalGauge(); // �÷��̾��� ��Ż�������� ��ȭ�� ������ ��� ����.

        // ��Ʈ ���� �������� Ȱ��ȭ�Ǿ� ���� ���� ���� ������Ʈ
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
        UpdateGhostInfo(); // ��Ʈ ���� ������Ʈ
    }


    //��Ż �������� ǥ�����ִ� �޼���
    void showMentalGauge()
    {
        mentalGaugeManager mentalGaugeManager = GameObject.FindWithTag("Player").GetComponent<mentalGaugeManager>();
        mentalGaugeText.text = mentalGaugeManager.MentalGauge.ToString(); // ��Ż������ �ؽ�Ʈ�� �÷��̾��� MentalGauge���� ��ӹ޾� ǥ��.
    }

    void UpdateGhostInfo()
    {
        if (ghostObjects.Length > 0)
        {
            ghostObject currentGhostObjects = ghostObjects[currentPage];
            ghostName.text = currentGhostObjects.ghostName;
            ghostDescription.text = currentGhostObjects.description;
            relatedItem.text = currentGhostObjects.relatedItem;

            // ������ ��ȣ ������Ʈ
            leftNumber.text = (currentPage + 1).ToString();
            rightNumber.text = ghostObjects.Length.ToString();
        }
    }

    void ShowPreviousPage()
    {
        Debug.Log("��ư ȣ�� �Ϸ�");
        if (ghostObjects.Length > 0)
        {
            currentPage = (currentPage - 1 + ghostObjects.Length) % ghostObjects.Length;
            UpdateGhostInfo();
            Debug.Log("������ �ѱ� �Ϸ�");
        }

        else
        {
            Debug.Log("������ �ѱ� ����");
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