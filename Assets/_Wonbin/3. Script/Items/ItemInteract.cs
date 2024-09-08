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

        // 레이캐스트를 시각적으로 표시 (디버그 용도)
        DebugRaycast();
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
                    photonView.RPC("PickupItem", RpcTarget.AllBuffered, itemPhotonView.ViewID, photonView.ViewID);
                }
                else
                {
                    Debug.LogError("아이템에 PhotonView가 없습니다: " + item.name);
                }
            }
        }
    }

    // 레이캐스트를 시각적으로 표시
    void DebugRaycast()
    {
        ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));  // 화면 중앙에서 발사
        Debug.DrawRay(ray.origin, ray.direction * rayCastDistance, Color.green);  // 레이의 경로를 그린다
    }

    [PunRPC]
    void PickupItem(int itemViewID, int playerViewID)
    {
        PhotonView itemPhotonView = PhotonView.Find(itemViewID);
        PhotonView playerPhotonView = PhotonView.Find(playerViewID);

        if (itemPhotonView == null || playerPhotonView == null)
        {
            Debug.LogError($"PhotonView를 찾을 수 없습니다. itemViewID: {itemViewID}, playerViewID: {playerViewID}");
            return;
        }

        GameObject item = itemPhotonView.gameObject;
        PlayerInventory playerInventory = playerPhotonView.GetComponent<PlayerInventory>();

        if (playerInventory.CanAddItem(item))
        {
            playerInventory.AddToInventory(item);
        }
        else
        {
            Debug.LogWarning("인벤토리가 가득 찼습니다.");
        }
    }
}
