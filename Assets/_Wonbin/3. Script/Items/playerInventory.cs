using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class playerInventory : MonoBehaviour
{

    public GameObject itemSlot; // ItemSlot ������Ʈ
    public int maxInventorySize = 3; // �ִ� �κ��丮 ũ��
    public static bool isInItemSlot; // �������� ItemSlot�� �ִ��� ���θ� Ȯ��


    
    private Rigidbody itemDrop; // ������ ��� ��, addforce�� �����ϱ� ���� ����.
    private Transform itemTransform; // �������� Transform�� �������� ���� ����.


    [SerializeField]
    private List<GameObject> inventoryItems = new List<GameObject>(); // �κ��丮 ����Ʈ
    private int currentMainSlotIndex = 0; // ���� ���� ���� �ε���
    private int newItemsIndex = 0; // ���� �÷��̾ �տ� ��� �ִ� �����۰� ���� �߰��Ǵ� �������� �ε����� ������ üũ�ϱ� ���� �ε���.




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
        checkCurrentSlot();
        pullUseSlot();
    }


    public void AddToInventory(GameObject item)
    {
        if (inventoryItems.Count < maxInventorySize)
        {
            if (itemSlot != null)
            {
                item.transform.SetParent(itemSlot.transform); // �������� ItemSlot�� �ڽ����� ����
                item.transform.localPosition = Vector3.zero; // ��ġ �ʱ�ȭ
                item.transform.localRotation = Quaternion.identity; // ȸ�� �ʱ�ȭ

                Debug.Log("�������� �κ��丮�� �߰��Ǿ����ϴ�: " + item.name);

            }
        }
        else
        {
            Debug.LogWarning("�κ��丮�� ���� á���ϴ�!");
        }
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

    public void SwapItems()
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

    private void checkCurrentSlot()
    {
        if (newItemsIndex >= inventoryItems.Count)
        {
            newItemsIndex = 0; // �ε����� ����Ʈ ũ�⸦ �ʰ��ϸ� �ʱ�ȭ
            return;
        }

        // ���� ������ �����۰� ���� �߰��� �������� ������ ���
        if (inventoryItems[currentMainSlotIndex] == inventoryItems[newItemsIndex])
        {
            newItemsIndex++; // �ε��� ����
        }
        else
        {
            // �� �������� ���� ������ �����۰� �ٸ���, �������� �߰��ϰ� �� ������ ��Ȱ��ȭ
            GameObject AddNewItem = inventoryItems[newItemsIndex];
            AddNewItem.SetActive(false);

            // ���� ������ �񱳸� ���� �ε��� ����
            newItemsIndex++;
        }
    }


   
private void pullUseSlot()
    {
        // ����Ʈ�� ��� ���� �ʰ�, currentMainSlotIndex�� ��ȿ���� Ȯ��
        if (inventoryItems.Count > 0 && currentMainSlotIndex < inventoryItems.Count)
        {
            // ���� ���� ������ �������� null�̸� ����Ʈ���� ����
            if (inventoryItems[currentMainSlotIndex] == null)
            {
                inventoryItems.RemoveAt(currentMainSlotIndex);

                // �ε��� ����
                if (currentMainSlotIndex >= inventoryItems.Count)
                {
                    currentMainSlotIndex = Mathf.Clamp(currentMainSlotIndex - 1, 0, inventoryItems.Count - 1);
                }
            }
        }
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

            itemDrop = currentMainItem.GetComponent<Rigidbody>(); //���� �������� Rigidbody�� ������.
            itemTransform = currentMainItem.transform; // ���� �������� Transform�� ������.


            if (itemDrop != null)
            {
                itemDrop.isKinematic = false; // isKinematic�� false�� �����Ͽ� ���� ������ ������ �޵��� ��
                Vector3 throwDirection = itemTransform.forward; // ���� ���� ����
                itemDrop.AddForce(throwDirection * 1, ForceMode.Impulse); // ������ ���� ����.
            }


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

        }
    }

}

