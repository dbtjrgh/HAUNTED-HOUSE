using Player.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // Photon ���� ���ӽ����̽� �߰�

public class ItemInteract : MonoBehaviourPun // MonoBehaviour���� MonoBehaviourPun���� ����
{
    private RaycastHit hit;
    private Ray ray;
    float rayCastDistance = 4.0f;
    playerInventory Inventory;

    private void Start()
    {
        Inventory = GetComponent<playerInventory>();
    }

    private void Update()
    {
        // ���� �÷��̾ ��ȣ�ۿ��� �� �ֵ��� ���� �߰�
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                pickupItem();
            }
        }
    }

    void pickupItem()
    {
        // Ray�� ���� ��ġ�� ī�޶��� �߾ӿ��� �ణ �Ʒ��� ����
        ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.45f, 0));

        // ���̾� ����ũ ����(Interactable ���̾ ����)
        int layerMask = LayerMask.GetMask("Interactable");

        if (Physics.Raycast(ray, out hit, rayCastDistance, layerMask))
        {
            if (hit.collider.gameObject.CompareTag("Items"))
            {
                GameObject item = hit.collider.gameObject;
                Debug.Log("������ ����: " + item.name);

                // �������� Rigidbody ��������
                Rigidbody isHolding = item.GetComponent<Rigidbody>();

                if (isHolding != null)
                {
                    // isKinematic�� true�� �����Ͽ� ���� ������ ������ ���� �ʵ��� ��
                    isHolding.isKinematic = true;
                }

                // ������ �������� �κ��丮�� �߰�
                Inventory.AddToInventory(hit.collider.gameObject);
            }
        }
    }
}
