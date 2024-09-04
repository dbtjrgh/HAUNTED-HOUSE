using UnityEngine;
using System.Collections;
using changwon;


public class UVFlashlight : MonoBehaviour
{
    [SerializeField]
    private UVLight _uvLight; //핸드 프린트 상호작용

    [SerializeField]
    private Light myLight; // Light 컴포넌트 관리


    [SerializeField]
    public Material _revealableMaterial;

    //private AudioSource _audioSource;

    bool playerGetLight; // 플레이어가 손전등을 on한 상태인지 확인
    static bool getLight; // 손전등 획득 여부 확인


    public static bool isInItemSlot; // 손전등이 ItemSlot에 있는지 여부를 확인
    private Transform itemSlotTransform;

    playerInventory Inventory;

    private void Start()
    {
        //_audioSource = GetComponent<AudioSource>();
        playerGetLight = false;
        getLight = false;
        isInItemSlot = false; // 초기 상태는 ItemSlot에 없음

        if (myLight == null)
        {
            myLight = GetComponent<Light>(); // 필요한 경우 다시 할당
        }


        itemSlotTransform = GameObject.Find("ItemSlot")?.transform;
    }

    private void Update()
    {
        if (itemSlotTransform == null)
        {
            // ItemSlot이 존재하지 않을 때
            myLight.enabled = false;
            return;
        }

        // 손전등이 ItemSlot의 자식인지 확인
        bool isInItemSlot = transform.IsChildOf(itemSlotTransform);
       

        if (isInItemSlot)
        {
            Debug.Log("손전등이 ItemSlot에 있음");
            lightOnOFF(); // 손전등이 ItemSlot에 있을 때만 호출
        }
        else
        {
            myLight.enabled = false; // 손전등이 ItemSlot에 없을 때 비활성화
        }

    }

    static internal void lightEquip()
    {
        getLight = true;
        GameObject flashLightObject = GameObject.FindGameObjectWithTag("Items");

        if (flashLightObject != null)
        {
            GameObject itemSlot = GameObject.Find("ItemSlot");
            if (itemSlot != null)
            {
                flashLightObject.transform.SetParent(itemSlot.transform);
                flashLightObject.transform.localPosition = Vector3.zero; // 위치 초기화
                flashLightObject.transform.localRotation = Quaternion.identity; // 회전 초기화

                isInItemSlot = true; // ItemSlot에 추가되었음을 표시

            }
        }
    }

    void lightOnOFF()
    {
        Debug.Log("사용 로직 호출됨.");
        getLight = true;
        if (getLight)
        {
            Debug.Log(getLight);
            if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("손전등 사용");
                playerGetLight = !playerGetLight; // 손전등 on/off
                myLight.intensity = playerGetLight ? 10 : 0; // 손전등 밝기 조정
                myLight.enabled = playerGetLight; // 손전등 활성화/비활성화
            }
        }
    }




}