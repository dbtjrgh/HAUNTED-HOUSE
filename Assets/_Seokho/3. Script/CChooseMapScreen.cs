using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CChooseMapScreen : MonoBehaviour
{
    public Button factoryButton; // Factory �� ���� ��ư
    public Button turkwoodButton; // Turkwood �� ���� ��ư
    public Button backButton; // �ڷ� ���� ��ư

    private string selectedMap = ""; // ���õ� �� �̸�

    public GameObject roomScreen;
    public GameObject ChooseMapScreen;

    private void Start()
    {

        // ��ư�� ������ ���
        factoryButton.onClick.AddListener(() => SelectMap("Factory"));
        turkwoodButton.onClick.AddListener(() => SelectMap("Turkwood"));
        backButton.onClick.AddListener(BackToRoom);
    }

    // �� ���� �޼��� (�� ���� �ϳ��� ����)
    private void SelectMap(string mapName)
    {
        selectedMap = mapName;

        // �� ��ư�� ���¸� ���� (���õ� ��ư�� ��ȣ�ۿ� �Ұ�)
        factoryButton.interactable = mapName != "Factory";
        turkwoodButton.interactable = mapName != "Turkwood";

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
