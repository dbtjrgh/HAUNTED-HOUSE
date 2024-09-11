using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CChooseMapScreen : MonoBehaviour
{
    public Button factoryButton; // Factory �� ���� ��ư
    public Button turkwoodButton; // Turkwood �� ���� ��ư
    public Button backButton; // �ڷ� ���� ��ư
    public Image factoryImage;
    public Image turkwoodImage;

    private string selectedMap = ""; // ���õ� �� �̸�

    public GameObject roomScreen;
    public GameObject ChooseMapScreen;

    private void Start()
    {

        // ��ư�� ������ ���
        factoryButton.onClick.AddListener(() => SelectMap("����"));
        turkwoodButton.onClick.AddListener(() => SelectMap("����"));
        backButton.onClick.AddListener(BackToRoom);
    }

    // �� ���� �޼��� (�� ���� �ϳ��� ����)
    private void SelectMap(string mapName)
    {
        selectedMap = mapName;

        // �� ��ư�� ���¸� ���� (���õ� ��ư�� ��ȣ�ۿ� �Ұ�)
        factoryButton.interactable = mapName != "����";
        turkwoodButton.interactable = mapName != "����";

        if ( mapName == "����")
        {
            factoryImage.enabled = true;
            turkwoodImage.enabled = false;
        }
        else if( mapName == "����")
        {
            factoryImage.enabled = false;
            turkwoodImage.enabled = true;
        }

        // ���õ� �� ������ RoomScreen���� ����
        roomScreen.GetComponent<CRoomScreen>().ChooseMap(mapName);
    }

    // �ڷ� ���� ��ư �޼���
    private void BackToRoom()
    {
        // RoomScreen ȭ�� Ȱ��ȭ
        roomScreen.gameObject.SetActive(true);
        // �� ���� ȭ�� ��Ȱ��ȭ
        ChooseMapScreen.gameObject.SetActive(false); // ���� ������Ʈ ��Ȱ��ȭ
    }
}