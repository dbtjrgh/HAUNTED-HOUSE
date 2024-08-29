using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static System.IO.Enumeration.FileSystemEnumerable<TResult>;

public class ItemInteract : MonoBehaviour
{
    private RaycastHit hit;
    private Ray ray;
    float rayCastGirth = 0.5f;
    float rayCastWidth = 2.3f;    
    playerInventory Inventory;

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
        ray = Camera.main.ViewportPointToRay(Vector3.one * rayCastGirth);


        // 레이어 마스크 설정 (예: "Items" 레이어만 탐지)
        int layerMask = LayerMask.GetMask("Interactable");

        // Flash 획득
        if (Physics.Raycast(ray, out hit, rayCastWidth, layerMask))
        {
            if (hit.collider.gameObject.CompareTag("Items"))
            {
                Debug.Log("ray가 아이템을 인식했습니다."); // 오브젝트가 아이템을 인식했을 경우

                flashLight.lightEquip(); // 아이템 획득 처리
            }
        }
    }

}