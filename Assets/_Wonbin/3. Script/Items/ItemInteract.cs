using Photon.Pun;
using UnityEngine;

public class ItemInteract : MonoBehaviourPun
{
    private RaycastHit hit;
    private Ray ray;
    private float rayCastDistance = 4.0f;
    private PlayerInventory inventory;

    private void Start()
    {
        inventory = GetComponent<PlayerInventory>();
    }

    private void Update()
    {
        if (photonView.IsMine && Input.GetKeyDown(KeyCode.E))
        {
            TryPickupItem();
        }
    }

    void TryPickupItem()
    {
        ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        int layerMask = LayerMask.GetMask("Interactable");

        if (Physics.Raycast(ray, out hit, rayCastDistance, layerMask))
        {
            if (hit.collider.CompareTag("Items"))
            {
                GameObject item = hit.collider.gameObject;
                PhotonView itemPhotonView = item.GetComponent<PhotonView>();

                if (itemPhotonView != null)
                {
                    // 아이템을 줍기 전에 PhotonView가 유효한지 확인합니다.
                    photonView.RPC("PickupItem", RpcTarget.AllBuffered, itemPhotonView.ViewID);
                }
                else
                {
                    Debug.LogError("아이템에 PhotonView가 없습니다: " + item.name);
                }
            }
        }
    }


    [PunRPC]
    void PickupItem(int itemViewID)
    {
        PhotonView itemPhotonView = PhotonView.Find(itemViewID);

        if (itemPhotonView == null)
        {
            Debug.LogError($"PhotonView를 찾을 수 없습니다. itemViewID: {itemViewID}");
            return;
        }

        GameObject item = itemPhotonView.gameObject;

        // 인벤토리에 추가할 수 있는지 확인 후 추가
        if (inventory.CanAddItem(item))
        {
            inventory.AddToInventory(item);
            item.SetActive(false);  // 아이템을 줍고 나면 비활성화
        }
        else
        {
            Debug.LogWarning("인벤토리가 가득 찼습니다.");
        }
    }

}
