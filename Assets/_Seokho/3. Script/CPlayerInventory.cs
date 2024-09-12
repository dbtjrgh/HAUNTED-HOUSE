using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CPlayerInventory : MonoBehaviourPun
{
    public List<GameObject> inventoryItems;
    public Transform dropPoint;
    public Transform handPosition;
    private int currentItemIndex = -1;
    private GameObject currentItem;
    private RaycastHit hit;
    private Ray ray;
    private float rayCastDistance = 4.0f;
    private mentalGaugeManager playerMentalGauge;
    flashLight flashlight; // flashLight�� ���̾ �Ҵ��ϱ� ���� ����.
    private Vector3 lastSentPosition;
    private Quaternion lastSentRotation;
    private float positionThreshold = 0.05f; // �� ���� ������ ������Ʈ ����
    private float rotationThreshold = 1f;

    private void Awake()
    {
        if (inventoryItems == null)
        {
            inventoryItems = new List<GameObject>();
        }

        playerMentalGauge = GetComponent<mentalGaugeManager>();
        flashlight = GetComponent<flashLight>(); // flashLight�� ���̾ �Ҵ��ϱ� ���� Awake���� �ʱ�ȭ.
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            // E Ű�� ���� �������� ���� �õ�
            if (Input.GetKeyDown(KeyCode.E))
            {
                TryPickupItem();
            }

            // �������� ������ ���� ��ġ�� ȸ�� ����ȭ
            if (currentItem != null)
            {
                currentItem.transform.position = handPosition.position;
                currentItem.transform.rotation = handPosition.rotation;

                // ��ġ�� ȸ���� ū ��ȭ�� ���� ���� RPC ����
                if (Vector3.Distance(currentItem.transform.position, lastSentPosition) > positionThreshold ||
                    Quaternion.Angle(currentItem.transform.rotation, lastSentRotation) > rotationThreshold)
                {
                    lastSentPosition = currentItem.transform.position;
                    lastSentRotation = currentItem.transform.rotation;

                    // ��ġ�� ȸ�� ������ �ٸ� Ŭ���̾�Ʈ�� ����
                    photonView.RPC("UpdateItemPositionRotation", RpcTarget.Others,
                        currentItem.GetComponent<PhotonView>().ViewID, handPosition.position, handPosition.rotation);
                }
            }

            else
            {
                DelMissingItem(); // �κ��丮 ������ null�� �߻��� �������� ������, �κ��丮 ����Ʈ�� ��ȸ�� ����
            }

            // G Ű�� ���� ������ ���
            if (Input.GetKeyDown(KeyCode.G))
            {
                DropCurrentItem();
            }

            // Q Ű�� ���� ������ ��ü
            if (Input.GetKeyDown(KeyCode.Q))
            {
                SwitchItem();
            }
        }

        // ����׿� ����ĳ��Ʈ �ð�ȭ
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
                    // �������� �������� �ش� �÷��̾�� ����
                    itemPhotonView.RequestOwnership();
                    photonView.RPC("PickupItem", RpcTarget.AllBuffered, itemPhotonView.ViewID, photonView.ViewID);
                }
                else
                {
                    Debug.LogError("�����ۿ� PhotonView�� �����ϴ�: " + item.name);
                }
            }
        }
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
        CPlayerInventory playerInventory = playerPhotonView.GetComponent<CPlayerInventory>();

        // �������� �κ��丮�� �� �� �ִ��� Ȯ��
        if (playerInventory.CanAddItem(item))
        {
            playerInventory.AddToInventory(item);  // �κ��丮�� ������ �߰�
        }
        else
        {
            Debug.LogWarning("�κ��丮�� ���� á���ϴ�.");
        }
    }

    public bool CanAddItem(GameObject item)
    {
        return inventoryItems.Count < 3;  // �κ��丮 ũ�� ����
    }

    public void AddToInventory(GameObject item)
    {
        // �κ��丮�� �߰� �� ��Ȱ��ȭ
        inventoryItems.Add(item);
        item.SetActive(false);  // �߰��� �������� ����
        item.transform.SetParent(transform);  // �÷��̾��� �ڽ����� ����
        if (currentItemIndex == -1)
        {
            currentItemIndex = 0;  // ù ��° ������ �ڵ� ����
            EquipCurrentItem();
        }
    }
    void SwitchItem()
    {
        if (inventoryItems.Count == 0)
        {
            return;
        }

        //// ���� �������� ������(flashLight)�̸� mesh�� ��Ȱ��ȭ
        //if (currentItem == flashlight.gameObject)
        //{
        //    SetFlashlightMeshActive(false);
        //}
         if (currentItem != null)
        {
            currentItem.SetActive(false);  // �Ϲ� �������� �׳� ��Ȱ��ȭ
        }

        // ���� ���������� ��ü
        currentItemIndex = (currentItemIndex + 1) % inventoryItems.Count;

        EquipCurrentItem();  // �� ������ ����

        //// ��ü�� �������� flashLight�� ��� mesh�� Ȱ��ȭ
        //if (currentItem == flashlight.gameObject)
        //{
        //    SetFlashlightMeshActive(true);
        //}
    }

    //// �������� MeshRenderer�� Ȱ��ȭ �Ǵ� ��Ȱ��ȭ�ϴ� �޼���
    //void SetFlashlightMeshActive(bool isActive)
    //{
    //    MeshRenderer flashlightRenderer = flashlight.GetComponent<MeshRenderer>();

    //    if (flashlightRenderer != null)
    //    {
    //        flashlightRenderer.enabled = isActive;  // MeshRenderer Ȱ��ȭ/��Ȱ��ȭ
    //    }
    //}



    void DelMissingItem() //������ �κ��丮 ����Ʈ�� ��ȸ�ؼ�, missing �� index�� �ִٸ�, �ڵ����� �������ִ� �Լ�.
    {   
        for (int i = inventoryItems.Count - 1; i >= 0; i--)
        {
            if (inventoryItems[i] == null)
            {
                inventoryItems.RemoveAt(i);
            }
        }
    }

 

    void EquipCurrentItem()
    {
        currentItem = inventoryItems[currentItemIndex];
        currentItem.SetActive(true);  // ���� ������ Ȱ��ȭ
        currentItem.transform.SetParent(handPosition);  // �� ��ġ�� ��ġ
        currentItem.transform.localPosition = Vector3.zero;
        currentItem.transform.localRotation = Quaternion.identity;

        // �ٸ� �÷��̾�Ե� ����ȭ
        photonView.RPC("UpdateItemPositionRotation", RpcTarget.Others, currentItem.GetComponent<PhotonView>().ViewID, handPosition.position, handPosition.rotation);
    }

    public void DropCurrentItem()
    {
        if (currentItem != null)
        {
            inventoryItems.Remove(currentItem);

            PhotonView itemPhotonView = currentItem.GetComponent<PhotonView>();
            if (itemPhotonView != null)
            {
                currentItem.transform.SetParent(null);  // �÷��̾�� �и�
                Vector3 dropPosition = dropPoint.position;

                // �������� ����� �� �������� ���� ���·� ��ȯ
                itemPhotonView.TransferOwnership(PhotonNetwork.MasterClient);

                // ��� Ŭ���̾�Ʈ���� ��ӵ� �������� ������ ����ȭ
                photonView.RPC("DropItemRPC", RpcTarget.AllBuffered, itemPhotonView.ViewID, dropPosition);

                Rigidbody itemRigidbody = currentItem.GetComponent<Rigidbody>();
                if (itemRigidbody != null)
                {
                    itemRigidbody.isKinematic = false;
                    itemRigidbody.AddForce(dropPoint.forward * 2.0f, ForceMode.Impulse);
                }
            }

            currentItem = null;
            currentItemIndex = -1;  // �������� ���� ��� �ε����� �ʱ�ȭ
        }
    }

    [PunRPC]
    void DropItemRPC(int itemViewID, Vector3 dropPosition)
    {
        PhotonView itemPhotonView = PhotonView.Find(itemViewID);
        if (itemPhotonView != null)
        {
            GameObject item = itemPhotonView.gameObject;
            item.transform.position = dropPosition;

            // �������� ��ӵ� �� �θ� ���� ����
            if (item.transform.parent != null)
            {
                item.transform.SetParent(null);  // �������� �θ𿡼� �и�
            }

            Rigidbody itemRigidbody = item.GetComponent<Rigidbody>();
            if (itemRigidbody != null)
            {
                itemRigidbody.isKinematic = false;
                itemRigidbody.AddForce(Vector3.forward * 2.0f, ForceMode.Impulse);
            }

            PhotonRigidbodyView rbView = item.GetComponent<PhotonRigidbodyView>();
            if (rbView != null)
            {
                rbView.enabled = true;
            }
        }
    }

    [PunRPC]
    void UpdateItemPositionRotation(int itemViewID, Vector3 newPosition, Quaternion newRotation)
    {
        PhotonView itemPhotonView = PhotonView.Find(itemViewID);
        if (itemPhotonView != null)
        {
            GameObject item = itemPhotonView.gameObject;
            // �ε巯�� �̵��� ȸ���� ���� ���� ó��
            StartCoroutine(SmoothMove(item.transform, newPosition, newRotation, 0.1f)); // 0.1�� ���� �ε巴�� �̵�
        }
    }

    private IEnumerator SmoothMove(Transform itemTransform, Vector3 targetPosition, Quaternion targetRotation, float duration)
    {
        Vector3 startPosition = itemTransform.position;
        Quaternion startRotation = itemTransform.rotation;
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            itemTransform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / duration);
            itemTransform.rotation = Quaternion.Lerp(startRotation, targetRotation, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // ���������� ��Ȯ�� ��ġ�� ȸ�� �� ����
        itemTransform.position = targetPosition;
        itemTransform.rotation = targetRotation;
    }

    void DebugRaycast()
    {
        ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Debug.DrawRay(ray.origin, ray.direction * rayCastDistance, Color.green);
    }
}
