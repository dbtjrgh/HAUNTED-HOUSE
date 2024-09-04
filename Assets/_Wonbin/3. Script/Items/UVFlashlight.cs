using UnityEngine;
using System.Collections;
using changwon;


public class UVFlashlight : MonoBehaviour
{
    [SerializeField]
    private UVLight _uvLight; //�ڵ� ����Ʈ ��ȣ�ۿ�

    [SerializeField]
    private Light myLight; // Light ������Ʈ ����


    [SerializeField]
    public Material _revealableMaterial;

    //private AudioSource _audioSource;

    bool playerGetLight; // �÷��̾ �������� on�� �������� Ȯ��
    static bool getLight; // ������ ȹ�� ���� Ȯ��


    public static bool isInItemSlot; // �������� ItemSlot�� �ִ��� ���θ� Ȯ��
    private Transform itemSlotTransform;

    playerInventory Inventory;

    private void Start()
    {
        //_audioSource = GetComponent<AudioSource>();
        playerGetLight = false;
        getLight = false;
        isInItemSlot = false; // �ʱ� ���´� ItemSlot�� ����

        if (myLight == null)
        {
            myLight = GetComponent<Light>(); // �ʿ��� ��� �ٽ� �Ҵ�
        }


        itemSlotTransform = GameObject.Find("ItemSlot")?.transform;
    }

    private void Update()
    {
        if (itemSlotTransform == null)
        {
            // ItemSlot�� �������� ���� ��
            myLight.enabled = false;
            return;
        }

        // �������� ItemSlot�� �ڽ����� Ȯ��
        bool isInItemSlot = transform.IsChildOf(itemSlotTransform);
       

        if (isInItemSlot)
        {
            Debug.Log("�������� ItemSlot�� ����");
            lightOnOFF(); // �������� ItemSlot�� ���� ���� ȣ��
        }
        else
        {
            myLight.enabled = false; // �������� ItemSlot�� ���� �� ��Ȱ��ȭ
        }

    }

    static internal void lightEquip()
    {
        getLight = true;
        GameObject flashLightObject = GameObject.FindGameObjectWithTag("Items");

        if (flashLightObject != null)
        {
            GameObject itemSlot = GameObject.Find("ItemSlot");
            if (itemSlot != null)
            {
                flashLightObject.transform.SetParent(itemSlot.transform);
                flashLightObject.transform.localPosition = Vector3.zero; // ��ġ �ʱ�ȭ
                flashLightObject.transform.localRotation = Quaternion.identity; // ȸ�� �ʱ�ȭ

                isInItemSlot = true; // ItemSlot�� �߰��Ǿ����� ǥ��

            }
        }
    }

    void lightOnOFF()
    {
        Debug.Log("��� ���� ȣ���.");
        getLight = true;
        if (getLight)
        {
            Debug.Log(getLight);
            if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("������ ���");
                playerGetLight = !playerGetLight; // ������ on/off
                myLight.intensity = playerGetLight ? 10 : 0; // ������ ��� ����
                myLight.enabled = playerGetLight; // ������ Ȱ��ȭ/��Ȱ��ȭ
            }
        }
    }




}