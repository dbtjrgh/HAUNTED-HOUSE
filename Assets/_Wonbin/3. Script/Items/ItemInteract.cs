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

        // ����ĳ��Ʈ�� �ð������� ǥ�� (����� �뵵)
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
                    Debug.LogError("�����ۿ� PhotonView�� �����ϴ�: " + item.name);
                }
            }
        }
    }

    // ����ĳ��Ʈ�� �ð������� ǥ��
    void DebugRaycast()
    {
        ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));  // ȭ�� �߾ӿ��� �߻�
        Debug.DrawRay(ray.origin, ray.direction * rayCastDistance, Color.green);  // ������ ��θ� �׸���
    }

    [PunRPC]
    void PickupItem(int itemViewID, int playerViewID)
    {
        PhotonView itemPhotonView = PhotonView.Find(itemViewID);
        PhotonView playerPhotonView = PhotonView.Find(playerViewID);

        if (itemPhotonView == null || playerPhotonView == null)
        {
            Debug.LogError($"PhotonView�� ã�� �� �����ϴ�. itemViewID: {itemViewID}, playerViewID: {playerViewID}");
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
            Debug.LogWarning("�κ��丮�� ���� á���ϴ�.");
        }
    }
}
