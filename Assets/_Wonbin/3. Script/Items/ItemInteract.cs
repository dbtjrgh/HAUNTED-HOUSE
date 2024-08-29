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
        // Ray�� ���� ��ġ�� ī�޶��� �߾ӿ��� �ణ �Ʒ��� ����
        ray = Camera.main.ViewportPointToRay(Vector3.one * rayCastGirth);


        // ���̾� ����ũ ���� (��: "Items" ���̾ Ž��)
        int layerMask = LayerMask.GetMask("Interactable");

        // Flash ȹ��
        if (Physics.Raycast(ray, out hit, rayCastWidth, layerMask))
        {
            if (hit.collider.gameObject.CompareTag("Items"))
            {
                Debug.Log("ray�� �������� �ν��߽��ϴ�."); // ������Ʈ�� �������� �ν����� ���

                flashLight.lightEquip(); // ������ ȹ�� ó��
            }
        }
    }

}