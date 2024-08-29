using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerInventory : MonoBehaviour
{

    public GameObject itemSlot; // ItemSlot 오브젝트
    public int maxInventorySize = 3; // 최대 인벤토리 크기

    [SerializeField]
    private List<GameObject> inventoryItems = new List<GameObject>(); // 인벤토리 리스트
    private int currentMainSlotIndex = 0; // 현재 메인 슬롯 인덱스
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

        // 새 아이템이 ItemSlot에 추가되었는지 체크
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
                    Debug.LogWarning("인벤토리가 가득 찼습니다!");
                }
            }
        }
    }

    private void SwapItems()
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

            flashLight.isInItemSlot = false; // 손전등이 ItemSlot에 없음을 표시
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

