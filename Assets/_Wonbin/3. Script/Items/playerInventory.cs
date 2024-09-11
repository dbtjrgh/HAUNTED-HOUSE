using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviourPun
{
    public List<GameObject> inventoryItems = new List<GameObject>();
    public Transform dropPoint;  // �������� ����� ��ġ (�÷��̾� ����)
    public Transform handPosition;  // �÷��̾ �������� �տ� �� �� ��ġ
    private int currentItemIndex = -1;  // ���� ��� �ִ� ������ �ε���

    private GameObject currentItem;

    public bool CanAddItem(GameObject item)
    {
        return inventoryItems.Count < 3;  // �κ��丮 ũ�� ����
    }

    public void AddToInventory(GameObject item)
    {
        // �κ��丮�� �߰�
        inventoryItems.Add(item);
        if (currentItemIndex == -1)
        {
            // ù ��° �������� �߰��� �� �ڵ� ����
            currentItemIndex = 0;
            EquipCurrentItem();
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            // �ǽð����� �������� �տ� ��� �ְ� �ϱ�
            if (currentItem != null)
            {
                currentItem.transform.position = handPosition.position;
                currentItem.transform.rotation = handPosition.rotation;

                // �ٸ� �÷��̾�Ե� �������� ��ġ�� ȸ���� ����ȭ
                photonView.RPC("UpdateItemPositionRotation", RpcTarget.Others, currentItem.GetComponent<PhotonView>().ViewID, handPosition.position, handPosition.rotation);

                // ������ ��� ó�� (���� �������� _EMF �Ǵ� flashLight ��ũ��Ʈ�� ������ �ִ��� Ȯ��)
                if (Input.GetButtonDown("Fire1"))
                {
                    var emfScript = currentItem.GetComponent<changwon._EMF>();
                    if (emfScript != null)
                    {
                        emfScript.EMFSwitching();  // EMF ������ ���
                    }
                }

                if (Input.GetKeyDown(KeyCode.R))
                {
                    var flashlightScript = currentItem.GetComponent<flashLight>();
                    if (flashlightScript != null)
                    {
                        flashlightScript.lightOnOFF();  // ������ ���
                    }
                }

            }

            // G Ű�� ������ ���
            if (Input.GetKeyDown(KeyCode.G))
            {
                DropItem();
            }

            // Q Ű�� ������ ����
            if (Input.GetKeyDown(KeyCode.Q))
            {
                SwitchItem();
            }

            if(inventoryItems == null)
            {
                delMissingItem(); // �κ��丮 ������ missing�� �������� ������ ���������� Ȯ��.
            }
        }
    }

    void SwitchItem()
    {
        if (inventoryItems.Count == 0) return;

        // ���� �������� ��Ȱ��ȭ
        if (currentItem != null)
        {
            currentItem.SetActive(false);
        }

        // ���� ���������� ����
        currentItemIndex = (currentItemIndex + 1) % inventoryItems.Count;
        EquipCurrentItem();
    }

    void EquipCurrentItem()
    {
        currentItem = inventoryItems[currentItemIndex];

        // �� ������ Ȱ��ȭ
        currentItem.SetActive(true);

        // �������� �� ��ġ�� ����
        currentItem.transform.SetParent(handPosition);  // �տ� �������� ��ġ
        currentItem.transform.localPosition = Vector3.zero;  // ��ġ �ʱ�ȭ
        currentItem.transform.localRotation = Quaternion.identity;  // ȸ�� �ʱ�ȭ

        // �ٸ� �÷��̾�Ե� �������� ��ġ�� ȸ���� ����ȭ
        photonView.RPC("UpdateItemPositionRotation", RpcTarget.Others, currentItem.GetComponent<PhotonView>().ViewID, handPosition.position, handPosition.rotation);
    }

    void DropItem()
    {
        if (currentItem != null)
        {
            inventoryItems.Remove(currentItem);

            PhotonView itemPhotonView = currentItem.GetComponent<PhotonView>();

            if (itemPhotonView != null)
            {
                // �������� �÷��̾��� �ڽĿ��� �и�
                currentItem.transform.SetParent(null);

                // ��� ��ġ ���� (�÷��̾� ����)
                Vector3 dropPosition = dropPoint.position;

                // ��� Ŭ���̾�Ʈ���� ��ӵ� �������� ���� ������ ����ȭ
                photonView.RPC("DropItemRPC", RpcTarget.AllBuffered, itemPhotonView.ViewID, dropPosition);

                // ������ ȿ�� �߰�
                Rigidbody itemRigidbody = currentItem.GetComponent<Rigidbody>();
                if (itemRigidbody != null)
                {
                    // Rigidbody�� ������ ��ȣ�ۿ��� Ȱ��ȭ
                    itemRigidbody.isKinematic = false;
                    itemRigidbody.AddForce(dropPoint.forward * 2.0f, ForceMode.Impulse);  // �������� ���� ���� �������� ����߸�
                }
            }

            // ���� ��� �ִ� �������� ����
            currentItem = null;
            currentItemIndex = -1;
        }
    }

    void delMissingItem() //������ �κ��丮 ����Ʈ�� ��ȸ�ؼ�, missing �� index�� �ִٸ�, �ڵ����� �������ִ� �Լ�.
    {
        foreach (GameObject item in inventoryItems)
        {
            if (item == null)
            {
                inventoryItems.Remove(item);
            }
        }
    }



        [PunRPC]
    void DropItemRPC(int itemViewID, Vector3 dropPosition)
    {
        PhotonView itemPhotonView = PhotonView.Find(itemViewID);

        if (itemPhotonView != null)
        {
            GameObject item = itemPhotonView.gameObject;

            // �������� ��� ��ġ�� �̵�
            item.transform.position = dropPosition;

            // Rigidbody�� ���� ȿ�� Ȱ��ȭ
            Rigidbody itemRigidbody = item.GetComponent<Rigidbody>();
            if (itemRigidbody != null)
            {
                itemRigidbody.isKinematic = false;
                itemRigidbody.AddForce(Vector3.forward * 2.0f, ForceMode.Impulse);  // �������� ������ ����
            }

            // PhotonRigidbodyView ����ȭ
            PhotonRigidbodyView rbView = item.GetComponent<PhotonRigidbodyView>();
            if (rbView != null)
            {
                rbView.enabled = true;  // ������ ����ȭ Ȱ��ȭ
            }
        }
        else
        {
            Debug.LogError("PhotonView not found for item with ID: " + itemViewID);
        }
    }


    IEnumerator ReenableTransformSync(GameObject item, float delay)
    {
        yield return new WaitForSeconds(delay);

        PhotonTransformView transformView = item.GetComponent<PhotonTransformView>();
        if (transformView != null)
        {
            transformView.enabled = true;
        }
    }




    [PunRPC]
    void UpdateItemPositionRotation(int itemViewID, Vector3 newPosition, Quaternion newRotation)
    {
        PhotonView itemPhotonView = PhotonView.Find(itemViewID);
        if (itemPhotonView != null)
        {
            GameObject item = itemPhotonView.gameObject;
            item.transform.position = newPosition;
            item.transform.rotation = newRotation;
        }
    }



}
