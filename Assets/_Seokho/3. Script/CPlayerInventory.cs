using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;

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

    private void Awake()
    {
        if (inventoryItems == null)
        {
            inventoryItems = new List<GameObject>();
        }

        playerMentalGauge = GetComponent<mentalGaugeManager>();
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            // E 키를 눌러 아이템을 집기 시도
            if (Input.GetKeyDown(KeyCode.E))
            {
                TryPickupItem();
            }

            // 현재 손에 든 아이템을 손 위치에 맞춰 움직이기
            if (currentItem != null)
            {
                currentItem.transform.position = handPosition.position;
                currentItem.transform.rotation = handPosition.rotation;

                // 다른 플레이어에게도 아이템 위치와 회전 동기화
                photonView.RPC("UpdateItemPositionRotation", RpcTarget.Others, currentItem.GetComponent<PhotonView>().ViewID, handPosition.position, handPosition.rotation);

                // R 키를 눌러 손전등 또는 정신력 회복 아이템 사용
                if (Input.GetKeyDown(KeyCode.R))
                {
                    // 손전등 사용
                    var flashlightScript = currentItem.GetComponent<flashLight>();
                    if (flashlightScript != null)
                    {
                        flashlightScript.lightOnOFF();
                    }

                    // 정신력 회복 아이템 사용
                    var gaugeFillScript = currentItem.GetComponent<gaugeFill>();
                    if (gaugeFillScript != null && playerMentalGauge.MentalGauge < playerMentalGauge.maxMentalGauge)
                    {
                        gaugeFillScript.fillUse();
                    }
                    else if (gaugeFillScript != null && playerMentalGauge.MentalGauge >= playerMentalGauge.maxMentalGauge)
                    {
                        Debug.Log("정신력 게이지가 최대치입니다.");
                    }
                }
            }

            // G 키를 눌러 아이템 드롭
            if (Input.GetKeyDown(KeyCode.G))
            {
                DropCurrentItem();
            }

            // Q 키를 눌러 아이템 교체
            if (Input.GetKeyDown(KeyCode.Q))
            {
                SwitchItem();
            }
        }

        // 디버그용 레이캐스트 시각화
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
                    // 아이템의 소유권을 해당 플레이어로 변경
                    itemPhotonView.RequestOwnership();
                    photonView.RPC("PickupItem", RpcTarget.AllBuffered, itemPhotonView.ViewID, photonView.ViewID);
                }
                else
                {
                    Debug.LogError("아이템에 PhotonView가 없습니다: " + item.name);
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
            Debug.LogError($"PhotonView를 찾을 수 없습니다. itemViewID: {itemViewID}, playerViewID: {playerViewID}");
            return;
        }

        GameObject item = itemPhotonView.gameObject;
        CPlayerInventory playerInventory = playerPhotonView.GetComponent<CPlayerInventory>();

        // 아이템이 인벤토리에 들어갈 수 있는지 확인
        if (playerInventory.CanAddItem(item))
        {
            playerInventory.AddToInventory(item);  // 인벤토리에 아이템 추가
        }
        else
        {
            Debug.LogWarning("인벤토리가 가득 찼습니다.");
        }
    }

    public bool CanAddItem(GameObject item)
    {
        return inventoryItems.Count < 3;  // 인벤토리 크기 제한
    }

    public void AddToInventory(GameObject item)
    {
        // 인벤토리에 추가 후 비활성화
        inventoryItems.Add(item);
        item.SetActive(false);  // 추가된 아이템은 숨김
        item.transform.SetParent(transform);  // 플레이어의 자식으로 설정
        if (currentItemIndex == -1)
        {
            currentItemIndex = 0;  // 첫 번째 아이템 자동 선택
            EquipCurrentItem();
        }
    }

    void SwitchItem()
    {
        if (inventoryItems.Count == 0) return;

        // 현재 아이템 숨기기
        if (currentItem != null)
        {
            currentItem.SetActive(false);
        }

        // 다음 아이템으로 교체
        currentItemIndex = (currentItemIndex + 1) % inventoryItems.Count;
        EquipCurrentItem();
    }

    void EquipCurrentItem()
    {
        currentItem = inventoryItems[currentItemIndex];
        currentItem.SetActive(true);  // 현재 아이템 활성화
        currentItem.transform.SetParent(handPosition);  // 손 위치에 배치
        currentItem.transform.localPosition = Vector3.zero;
        currentItem.transform.localRotation = Quaternion.identity;

        // 다른 플레이어에게도 동기화
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
                currentItem.transform.SetParent(null);  // 플레이어에서 분리
                Vector3 dropPosition = dropPoint.position;

                // 아이템을 드롭한 후 소유권을 원래 상태로 전환
                itemPhotonView.TransferOwnership(PhotonNetwork.MasterClient);

                // 모든 클라이언트에게 드롭된 아이템의 동작을 동기화
                photonView.RPC("DropItemRPC", RpcTarget.AllBuffered, itemPhotonView.ViewID, dropPosition);

                Rigidbody itemRigidbody = currentItem.GetComponent<Rigidbody>();
                if (itemRigidbody != null)
                {
                    itemRigidbody.isKinematic = false;
                    itemRigidbody.AddForce(dropPoint.forward * 2.0f, ForceMode.Impulse);
                }
            }

            currentItem = null;
            currentItemIndex = -1;  // 아이템이 없을 경우 인덱스를 초기화
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

            // 아이템이 드롭된 후 부모 관계 해제
            if (item.transform.parent != null)
            {
                item.transform.SetParent(null);  // 아이템을 부모에서 분리
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
            item.transform.position = newPosition;
            item.transform.rotation = newRotation;
        }
    }

    void DebugRaycast()
    {
        ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Debug.DrawRay(ray.origin, ray.direction * rayCastDistance, Color.green);
    }
}
