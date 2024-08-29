using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerInventory : MonoBehaviour
{

    public GameObject itemSlot; // ItemSlot ������Ʈ
    public int maxInventorySize = 3; // �ִ� �κ��丮 ũ��

    [SerializeField]
    private List<GameObject> inventoryItems = new List<GameObject>(); // �κ��丮 ����Ʈ
    private int currentMainSlotIndex = 0; // ���� ���� ���� �ε���
    flashLight FlashLight;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwapItems();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            DropItem();
        }

        // �� �������� ItemSlot�� �߰��Ǿ����� üũ
        AddItems();
    }

    private void AddItems()
    {
        foreach (Transform child in itemSlot.transform)
        {
            if (!inventoryItems.Contains(child.gameObject))
            {
                if (inventoryItems.Count < maxInventorySize)
                {
                    inventoryItems.Add(child.gameObject);
                }
                else
                {
                    Debug.LogWarning("�κ��丮�� ���� á���ϴ�!");
                }
            }
        }
    }

    private void SwapItems()
    {
        if (inventoryItems.Count == 0)
        {
            Debug.LogWarning("�κ��丮�� �������� �����ϴ�!");
            return;
        }

        // ���� ���� ���� ������ ��Ȱ��ȭ
        if (inventoryItems.Count > 0)
        {
            GameObject currentMainItem = inventoryItems[currentMainSlotIndex];
            currentMainItem.SetActive(false);
        }

        // ���� ���� ������ Ȱ��ȭ
        currentMainSlotIndex = (currentMainSlotIndex + 1) % inventoryItems.Count;
        GameObject newMainItem = inventoryItems[currentMainSlotIndex];
        newMainItem.SetActive(true);
    }

    private void DropItem()
    {
        if (inventoryItems.Count == 0)
        {
            Debug.LogWarning("�κ��丮�� �������� �����ϴ�!");
            return;
        }

        // ���� ���� ���� �������� ã���ϴ�.
        GameObject currentMainItem = inventoryItems[currentMainSlotIndex];
        if (currentMainItem != null)
        {
            currentMainItem.transform.SetParent(null); // ItemSlot���� �и�
            currentMainItem.SetActive(true); // ������ Ȱ��ȭ
            currentMainItem.transform.position = transform.position + transform.forward * 2; // �� �տ� ��ġ
            currentMainItem.transform.rotation = Quaternion.identity;

            // �κ��丮���� ������ ����
            inventoryItems.RemoveAt(currentMainSlotIndex);

            // ���� �ε��� ����
            if (inventoryItems.Count == 0)
            {
                currentMainSlotIndex = 0;
            }
            else
            {
                currentMainSlotIndex = Mathf.Clamp(currentMainSlotIndex, 0, inventoryItems.Count - 1);
            }

            flashLight.isInItemSlot = false; // �������� ItemSlot�� ������ ǥ��
        }
    }

    //private void itemInSlot()
    //{
    //    if (inventoryItems.Count > 0)
    //    {
    //        foreach (GameObject item in inventoryItems)
    //        {
    //            GameObject.Find.tag("Items");
    //        }
    //    }
    //}
}

