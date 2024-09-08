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
                    // �������� �ݱ� ���� PhotonView�� ��ȿ���� Ȯ���մϴ�.
                    photonView.RPC("PickupItem", RpcTarget.AllBuffered, itemPhotonView.ViewID);
                }
                else
                {
                    Debug.LogError("�����ۿ� PhotonView�� �����ϴ�: " + item.name);
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
            Debug.LogError($"PhotonView�� ã�� �� �����ϴ�. itemViewID: {itemViewID}");
            return;
        }

        GameObject item = itemPhotonView.gameObject;

        // �κ��丮�� �߰��� �� �ִ��� Ȯ�� �� �߰�
        if (inventory.CanAddItem(item))
        {
            inventory.AddToInventory(item);
            item.SetActive(false);  // �������� �ݰ� ���� ��Ȱ��ȭ
        }
        else
        {
            Debug.LogWarning("�κ��丮�� ���� á���ϴ�.");
        }
    }

}
