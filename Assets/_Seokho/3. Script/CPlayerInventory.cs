using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CPlayerInventory : MonoBehaviourPun
{
    #region 변수
    public List<GameObject> inventoryItems;
    public Transform dropPoint;
    public Transform handPosition;
    private int currentItemIndex = -1;
    private GameObject currentItem;
    private RaycastHit hit;
    private Ray ray;
    private float rayCastDistance = 4.0f;
    private mentalGaugeManager playerMentalGauge;
    flashLight flashlight; // flashLight의 레이어를 할당하기 위해 선언.
    private Vector3 lastSentPosition;
    private Quaternion lastSentRotation;
    private float positionThreshold = 0.05f; // 이 값을 넘으면 업데이트 전송
    private float rotationThreshold = 1f;
    #endregion
    private void Awake()
    {
        if (inventoryItems == null)
        {
            inventoryItems = new List<GameObject>();
        }

        playerMentalGauge = GetComponent<mentalGaugeManager>();
        flashlight = GetComponent<flashLight>(); // flashLight의 레이어를 할당하기 위해 Awake에서 초기화.
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

            // 아이템이 존재할 때만 위치와 회전 동기화
            if (currentItem != null)
            {
                currentItem.transform.position = handPosition.position;
                currentItem.transform.rotation = handPosition.rotation;

                // 위치나 회전에 큰 변화가 있을 때만 RPC 전송
                if (Vector3.Distance(currentItem.transform.position, lastSentPosition) > positionThreshold ||
                    Quaternion.Angle(currentItem.transform.rotation, lastSentRotation) > rotationThreshold)
                {
                    lastSentPosition = currentItem.transform.position;
                    lastSentRotation = currentItem.transform.rotation;

                    // 위치와 회전 정보를 다른 클라이언트에 전송
                    photonView.RPC("UpdateItemPositionRotation", RpcTarget.Others,
                        currentItem.GetComponent<PhotonView>().ViewID, handPosition.position, handPosition.rotation);
                }
            }
            else
            {
                DelMissingItem(); // 인벤토리 내에서 null이 발생된 아이템이 있으면, 인벤토리 리스트를 순회후 제거
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
    /// <summary>
    /// 아이템 줍기 키를 눌렀을 시 호출되는 함수
    /// </summary>
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
    // 지워볼생각
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
    /// <summary>
    /// 최대 아이템 개수 제한 체크
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool CanAddItem(GameObject item)
    {
        return inventoryItems.Count < 3;  // 인벤토리 크기 제한
    }
    /// <summary>
    /// 인벤토리에 추가 후 비활성화시키는 함수
    /// </summary>
    /// <param name="item"></param>
    public void AddToInventory(GameObject item)
    {
        // 인벤토리에 추가 후 비활성화
        inventoryItems.Add(item);
        item.SetActive(false);  // 추가된 아이템은 숨김
        item.transform.SetParent(transform);  // 플레이어의 자식으로 설정
        if (currentItemIndex == -1)
        {
            // currentItemIndex = 0;  // 첫 번째 아이템 자동 선택
            EquipCurrentItem();
        }
    }
    /// <summary>
    /// 아이템을 바꿨을 때 호출되는 함수
    /// </summary>
    void SwitchItem()
    {
        if (inventoryItems.Count == 0)
        {
            return;
        }

        //// 현재 아이템이 손전등(flashLight)이면 mesh를 비활성화
        //if (currentItem == flashlight.gameObject)
        //{
        //    SetFlashlightMeshActive(false);
        //}
        if (currentItem != null)
        {
            currentItem.SetActive(false);  // 일반 아이템은 그냥 비활성화
        }

        // 다음 아이템으로 교체
        currentItemIndex = (currentItemIndex + 1) % inventoryItems.Count;

        EquipCurrentItem();  // 새 아이템 장착

        //// 교체한 아이템이 flashLight일 경우 mesh를 활성화
        //if (currentItem == flashlight.gameObject)
        //{
        //    SetFlashlightMeshActive(true);
        //}
    }

    //// 손전등의 MeshRenderer를 활성화 또는 비활성화하는 메서드
    //void SetFlashlightMeshActive(bool isActive)
    //{
    //    MeshRenderer flashlightRenderer = flashlight.GetComponent<MeshRenderer>();

    //    if (flashlightRenderer != null)
    //    {
    //        flashlightRenderer.enabled = isActive;  // MeshRenderer 활성화/비활성화
    //    }
    //}

    /// <summary>
    /// 아이템 인벤토리 리스트를 순회해서, missing 된 index가 있다면, 자동으로 제거해주는 함수
    /// </summary>
    void DelMissingItem()
    {
        for (int i = inventoryItems.Count - 1; i >= 0; i--)
        {
            if (inventoryItems[i] == null)
            {
                inventoryItems.RemoveAt(i);
            }
        }
    }
    /// <summary>
    /// 아이템 바꿨을때 같이 호출되는 함수
    /// </summary>
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
    /// <summary>
    /// 아이템을 떨궜을 때 호출되는 함수
    /// </summary>
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
    /// <summary>
    /// 아이템 떨궜을때 호출되는 함수(멀티) 수정 예정
    /// </summary>
    /// <param name="itemViewID"></param>
    /// <param name="dropPosition"></param>
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
    /// <summary>
    /// 아이템 위치가 갱신이 안돼서 만들어놓은 함수
    /// </summary>
    /// <param name="itemViewID"></param>
    /// <param name="newPosition"></param>
    /// <param name="newRotation"></param>
    [PunRPC]
    void UpdateItemPositionRotation(int itemViewID, Vector3 newPosition, Quaternion newRotation)
    {
        PhotonView itemPhotonView = PhotonView.Find(itemViewID);
        if (itemPhotonView != null)
        {
            GameObject item = itemPhotonView.gameObject;
            // 부드러운 이동과 회전을 위해 보간 처리
            StartCoroutine(SmoothMove(item.transform, newPosition, newRotation, 0.1f)); // 0.1초 동안 부드럽게 이동
        }
    }
    /// <summary>
    /// 아이템을 들고있을 때 자연스럽게 들고있게 만들어본 함수
    /// </summary>
    /// <param name="itemTransform"></param>
    /// <param name="targetPosition"></param>
    /// <param name="targetRotation"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
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

        // 최종적으로 정확한 위치와 회전 값 설정
        itemTransform.position = targetPosition;
        itemTransform.rotation = targetRotation;
    }
    /// <summary>
    /// 아이템을 쉽게 줍도록 디버깅하기위해 만들어놓은 함수
    /// </summary>
    void DebugRaycast()
    {
        ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Debug.DrawRay(ray.origin, ray.direction * rayCastDistance, Color.green);
    }
}
