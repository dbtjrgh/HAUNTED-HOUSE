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
        // Ray�� ���� ��ġ�� ī�޶��� �߾ӿ��� �ణ �Ʒ��� ����
        ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.45f, 0));


        // ���̾� ����ũ ���� (��: "Items" ���̾ Ž��)
        int layerMask = LayerMask.GetMask("Interactable");

        // Flash ȹ��
        if (Physics.Raycast(ray, out hit, rayCastDistance, layerMask))
        {
            if (hit.collider.gameObject.CompareTag("Items"))
            {
                
                Debug.Log("������ ����: " + hit.collider.gameObject.name);

                // ������ �������� �κ��丮�� �߰�
                
                Inventory.AddToInventory(hit.collider.gameObject);
            }
        }
    }

}