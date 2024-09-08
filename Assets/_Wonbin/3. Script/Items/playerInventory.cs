using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviourPun
{
    public List<GameObject> inventoryItems = new List<GameObject>();
    public Transform dropPoint;  // �������� ����� ��ġ (�÷��̾� ����)

    public bool CanAddItem(GameObject item)
    {
        return inventoryItems.Count < 3;  // �κ��丮 ũ�� ����
    }

    public void AddToInventory(GameObject item)
    {
        inventoryItems.Add(item);
    }

    private void Update()
    {
        if (photonView.IsMine && Input.GetKeyDown(KeyCode.G))
        {
            DropItem();
        }
    }

    void DropItem()
    {
        if (inventoryItems.Count > 0)
        {
            // �κ��丮���� �������� �����ϴ�.
            GameObject itemToDrop = inventoryItems[0];
            inventoryItems.RemoveAt(0);

            // �������� PhotonView�� �����ɴϴ�.
            PhotonView itemPhotonView = itemToDrop.GetComponent<PhotonView>();

            if (itemPhotonView != null)
            {
                // �������� ����� ��ġ�� ����մϴ�.
                Vector3 dropPosition = dropPoint.position;

                // ��� Ŭ���̾�Ʈ�� ������ ����� ����ȭ�ϴ� RPC ȣ��
                photonView.RPC("DropItemRPC", RpcTarget.AllBuffered, itemPhotonView.ViewID, dropPosition);
            }
            else
            {
                Debug.LogError("����Ϸ��� �����ۿ� PhotonView�� �����ϴ�.");
            }
        }
    }

    [PunRPC]
    void DropItemRPC(int itemViewID, Vector3 dropPosition)
    {
        PhotonView itemPhotonView = PhotonView.Find(itemViewID);

        if (itemPhotonView != null)
        {
            // �������� Ȱ��ȭ�ϰ� ����� ��ġ�� �̵���ŵ�ϴ�.
            GameObject item = itemPhotonView.gameObject;
            item.transform.position = dropPosition;
            item.SetActive(true);

            // ���� ������ ��ȣ�ۿ��� �� �ֵ��� ����
            Rigidbody itemRigidbody = item.GetComponent<Rigidbody>();
            if (itemRigidbody != null)
            {
                itemRigidbody.isKinematic = false;
                // �ʿ��� ��� ���� �߰��Ͽ� ���� �� �ֽ��ϴ�.
                itemRigidbody.AddForce(dropPoint.forward * 2.0f, ForceMode.Impulse);
            }
        }
        else
        {
            Debug.LogError("PhotonView not found for item with ID: " + itemViewID);
        }
    }
}
