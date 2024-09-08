using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviourPun
{
    public List<GameObject> inventoryItems = new List<GameObject>();
    public Transform dropPoint;  // 아이템을 드랍할 위치 (플레이어 앞쪽)

    public bool CanAddItem(GameObject item)
    {
        return inventoryItems.Count < 3;  // 인벤토리 크기 제한
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
            // 인벤토리에서 아이템을 꺼냅니다.
            GameObject itemToDrop = inventoryItems[0];
            inventoryItems.RemoveAt(0);

            // 아이템의 PhotonView를 가져옵니다.
            PhotonView itemPhotonView = itemToDrop.GetComponent<PhotonView>();

            if (itemPhotonView != null)
            {
                // 아이템을 드롭할 위치를 계산합니다.
                Vector3 dropPosition = dropPoint.position;

                // 모든 클라이언트에 아이템 드롭을 동기화하는 RPC 호출
                photonView.RPC("DropItemRPC", RpcTarget.AllBuffered, itemPhotonView.ViewID, dropPosition);
            }
            else
            {
                Debug.LogError("드롭하려는 아이템에 PhotonView가 없습니다.");
            }
        }
    }

    [PunRPC]
    void DropItemRPC(int itemViewID, Vector3 dropPosition)
    {
        PhotonView itemPhotonView = PhotonView.Find(itemViewID);

        if (itemPhotonView != null)
        {
            // 아이템을 활성화하고 드롭할 위치로 이동시킵니다.
            GameObject item = itemPhotonView.gameObject;
            item.transform.position = dropPosition;
            item.SetActive(true);

            // 물리 엔진과 상호작용할 수 있도록 설정
            Rigidbody itemRigidbody = item.GetComponent<Rigidbody>();
            if (itemRigidbody != null)
            {
                itemRigidbody.isKinematic = false;
                // 필요한 경우 힘을 추가하여 던질 수 있습니다.
                itemRigidbody.AddForce(dropPoint.forward * 2.0f, ForceMode.Impulse);
            }
        }
        else
        {
            Debug.LogError("PhotonView not found for item with ID: " + itemViewID);
        }
    }
}
