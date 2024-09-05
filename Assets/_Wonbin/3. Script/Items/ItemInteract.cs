using Player.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static System.IO.Enumeration.FileSystemEnumerable<TResult>;

public class ItemInteract : MonoBehaviour
{
    private RaycastHit hit;
    private Ray ray;
    float rayCastDistance = 3.0f;
    playerInventory Inventory;


    private void Start()
    {
        Inventory = GetComponent<playerInventory>();
    }



    private void Update()
    {

        

        if (Input.GetKeyDown(KeyCode.E))
        {
            pickupItem();
        }


    }




    void pickupItem()
    {
        // Ray의 시작 위치를 카메라의 중앙에서 약간 아래로 조정
        ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.45f, 0));


        // 레이어 마스크 설정 (예: "Items" 레이어만 탐지)
        int layerMask = LayerMask.GetMask("Interactable");

        // Flash 획득
        if (Physics.Raycast(ray, out hit, rayCastDistance, layerMask))
        {
            if (hit.collider.gameObject.CompareTag("Items"))
            {
                
                Debug.Log("아이템 감지: " + hit.collider.gameObject.name);

                // 감지된 아이템을 인벤토리에 추가
                
                Inventory.AddToInventory(hit.collider.gameObject);
            }
        }
    }

}