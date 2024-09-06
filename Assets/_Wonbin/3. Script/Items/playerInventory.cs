using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class playerInventory : MonoBehaviour
{

    public GameObject itemSlot; // ItemSlot 오브젝트
    public int maxInventorySize = 3; // 최대 인벤토리 크기
    public static bool isInItemSlot; // 아이템이 ItemSlot에 있는지 여부를 확인


    
    private Rigidbody itemDrop; // 아이템 드랍 시, addforce를 관리하기 위한 변수.
    private Transform itemTransform; // 아이템의 Transform을 가져오기 위한 변수.


    [SerializeField]
    private List<GameObject> inventoryItems = new List<GameObject>(); // 인벤토리 리스트
    private int currentMainSlotIndex = 0; // 현재 메인 슬롯 인덱스
    private int newItemsIndex = 0; // 현재 플레이어가 손에 들고 있는 아이템과 새로 추가되는 아이템의 인덱스가 같은지 체크하기 위한 인덱스.




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

        // 새 아이템이 ItemSlot에 추가되었는지 체크
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
                item.transform.SetParent(itemSlot.transform); // 아이템을 ItemSlot의 자식으로 설정
                item.transform.localPosition = Vector3.zero; // 위치 초기화
                item.transform.localRotation = Quaternion.identity; // 회전 초기화

                Debug.Log("아이템이 인벤토리에 추가되었습니다: " + item.name);

            }
        }
        else
        {
            Debug.LogWarning("인벤토리가 가득 찼습니다!");
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
                    Debug.LogWarning("인벤토리가 가득 찼습니다!");
                }
            }
        }
    }

    public void SwapItems()
    {
        if (inventoryItems.Count == 0)
        {
            Debug.LogWarning("인벤토리에 아이템이 없습니다!");
            return;
        }

        // 현재 메인 슬롯 아이템 비활성화
        if (inventoryItems.Count > 0)
        {
            GameObject currentMainItem = inventoryItems[currentMainSlotIndex];
            currentMainItem.SetActive(false);
        }

        // 다음 슬롯 아이템 활성화
        currentMainSlotIndex = (currentMainSlotIndex + 1) % inventoryItems.Count;
        GameObject newMainItem = inventoryItems[currentMainSlotIndex];
        newMainItem.SetActive(true);
    }

    private void checkCurrentSlot()
    {
        if (newItemsIndex >= inventoryItems.Count)
        {
            newItemsIndex = 0; // 인덱스가 리스트 크기를 초과하면 초기화
            return;
        }

        // 현재 슬롯의 아이템과 새로 추가된 아이템이 동일한 경우
        if (inventoryItems[currentMainSlotIndex] == inventoryItems[newItemsIndex])
        {
            newItemsIndex++; // 인덱스 증가
        }
        else
        {
            // 새 아이템이 현재 슬롯의 아이템과 다르면, 아이템을 추가하고 새 아이템 비활성화
            GameObject AddNewItem = inventoryItems[newItemsIndex];
            AddNewItem.SetActive(false);

            // 다음 아이템 비교를 위해 인덱스 증가
            newItemsIndex++;
        }
    }


   
private void pullUseSlot()
    {
        // 리스트가 비어 있지 않고, currentMainSlotIndex가 유효한지 확인
        if (inventoryItems.Count > 0 && currentMainSlotIndex < inventoryItems.Count)
        {
            // 만약 현재 슬롯의 아이템이 null이면 리스트에서 제거
            if (inventoryItems[currentMainSlotIndex] == null)
            {
                inventoryItems.RemoveAt(currentMainSlotIndex);

                // 인덱스 조정
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
            Debug.LogWarning("인벤토리에 아이템이 없습니다!");
            return;
        }

        // 현재 메인 슬롯 아이템을 찾습니다.
        GameObject currentMainItem = inventoryItems[currentMainSlotIndex];
        if (currentMainItem != null)
        {
            currentMainItem.transform.SetParent(null); // ItemSlot에서 분리
            currentMainItem.SetActive(true); // 아이템 활성화
            currentMainItem.transform.position = transform.position + transform.forward * 2; // 손 앞에 위치
            currentMainItem.transform.rotation = Quaternion.identity;

            itemDrop = currentMainItem.GetComponent<Rigidbody>(); //현재 아이템의 Rigidbody를 가져옴.
            itemTransform = currentMainItem.transform; // 현재 아이템의 Transform을 가져옴.


            if (itemDrop != null)
            {
                itemDrop.isKinematic = false; // isKinematic을 false로 설정하여 물리 엔진의 영향을 받도록 함
                Vector3 throwDirection = itemTransform.forward; // 던질 방향 설정
                itemDrop.AddForce(throwDirection * 1, ForceMode.Impulse); // 던지는 힘을 가함.
            }


            // 인벤토리에서 아이템 제거
            inventoryItems.RemoveAt(currentMainSlotIndex);


            // 현재 인덱스 조정
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

