using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviourPun
{
    public List<GameObject> inventoryItems = new List<GameObject>();
    public Transform dropPoint;  // 아이템을 드랍할 위치 (플레이어 앞쪽)
    public Transform handPosition;  // 플레이어가 아이템을 손에 들 때 위치
    private int currentItemIndex = -1;  // 현재 들고 있는 아이템 인덱스

    private GameObject currentItem;

    public bool CanAddItem(GameObject item)
    {
        return inventoryItems.Count < 3;  // 인벤토리 크기 제한
    }

    public void AddToInventory(GameObject item)
    {
        // 인벤토리에 추가
        inventoryItems.Add(item);
        if (currentItemIndex == -1)
        {
            // 첫 번째 아이템을 추가할 때 자동 장착
            currentItemIndex = 0;
            EquipCurrentItem();
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            // 실시간으로 아이템을 손에 들고 있게 하기
            if (currentItem != null)
            {
                currentItem.transform.position = handPosition.position;
                currentItem.transform.rotation = handPosition.rotation;

                // 다른 플레이어에게도 아이템의 위치와 회전을 동기화
                photonView.RPC("UpdateItemPositionRotation", RpcTarget.Others, currentItem.GetComponent<PhotonView>().ViewID, handPosition.position, handPosition.rotation);

                // 아이템 사용 처리 (현재 아이템이 _EMF 또는 flashLight 스크립트를 가지고 있는지 확인)
                if (Input.GetButtonDown("Fire1"))
                {
                    var emfScript = currentItem.GetComponent<changwon._EMF>();
                    if (emfScript != null)
                    {
                        emfScript.EMFSwitching();  // EMF 아이템 사용
                    }
                }

                if (Input.GetKeyDown(KeyCode.R))
                {
                    var flashlightScript = currentItem.GetComponent<flashLight>();
                    if (flashlightScript != null)
                    {
                        flashlightScript.lightOnOFF();  // 손전등 사용
                    }
                }

            }

            // G 키로 아이템 드롭
            if (Input.GetKeyDown(KeyCode.G))
            {
                DropItem();
            }

            // Q 키로 아이템 변경
            if (Input.GetKeyDown(KeyCode.Q))
            {
                SwitchItem();
            }

            if(inventoryItems == null)
            {
                delMissingItem(); // 인벤토리 내에서 missing된 아이템이 없는지 지속적으로 확인.
            }
        }
    }

    void SwitchItem()
    {
        if (inventoryItems.Count == 0) return;

        // 현재 아이템을 비활성화
        if (currentItem != null)
        {
            currentItem.SetActive(false);
        }

        // 다음 아이템으로 변경
        currentItemIndex = (currentItemIndex + 1) % inventoryItems.Count;
        EquipCurrentItem();
    }

    void EquipCurrentItem()
    {
        currentItem = inventoryItems[currentItemIndex];

        // 새 아이템 활성화
        currentItem.SetActive(true);

        // 아이템을 손 위치로 설정
        currentItem.transform.SetParent(handPosition);  // 손에 아이템을 배치
        currentItem.transform.localPosition = Vector3.zero;  // 위치 초기화
        currentItem.transform.localRotation = Quaternion.identity;  // 회전 초기화

        // 다른 플레이어에게도 아이템의 위치와 회전을 동기화
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
                // 아이템을 플레이어의 자식에서 분리
                currentItem.transform.SetParent(null);

                // 드롭 위치 설정 (플레이어 앞쪽)
                Vector3 dropPosition = dropPoint.position;

                // 모든 클라이언트에게 드롭된 아이템의 물리 동작을 동기화
                photonView.RPC("DropItemRPC", RpcTarget.AllBuffered, itemPhotonView.ViewID, dropPosition);

                // 물리적 효과 추가
                Rigidbody itemRigidbody = currentItem.GetComponent<Rigidbody>();
                if (itemRigidbody != null)
                {
                    // Rigidbody의 물리적 상호작용을 활성화
                    itemRigidbody.isKinematic = false;
                    itemRigidbody.AddForce(dropPoint.forward * 2.0f, ForceMode.Impulse);  // 앞쪽으로 힘을 가해 아이템을 떨어뜨림
                }
            }

            // 현재 들고 있는 아이템을 없앰
            currentItem = null;
            currentItemIndex = -1;
        }
    }

    void delMissingItem() //아이템 인벤토리 리스트를 순회해서, missing 된 index가 있다면, 자동으로 제거해주는 함수.
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

            // 아이템을 드롭 위치로 이동
            item.transform.position = dropPosition;

            // Rigidbody의 물리 효과 활성화
            Rigidbody itemRigidbody = item.GetComponent<Rigidbody>();
            if (itemRigidbody != null)
            {
                itemRigidbody.isKinematic = false;
                itemRigidbody.AddForce(Vector3.forward * 2.0f, ForceMode.Impulse);  // 아이템을 앞으로 던짐
            }

            // PhotonRigidbodyView 동기화
            PhotonRigidbodyView rbView = item.GetComponent<PhotonRigidbodyView>();
            if (rbView != null)
            {
                rbView.enabled = true;  // 물리적 동기화 활성화
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
