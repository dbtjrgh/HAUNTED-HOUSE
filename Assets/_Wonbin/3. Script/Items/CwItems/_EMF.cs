using ExitGames.Client.Photon;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace changwon
{
    public class _EMF : MonoBehaviour
    {
        public Collider interaction;
        public Light[] lights;
        Ghost ghost;

        static bool getEMF;
        public static bool isInItemSlot; // EMF가 ItemSlot에 있는지 여부
        private Transform itemSlotTransform;
        playerInventory Inventory;
        public bool EMFOnOff = false;

        private void Awake()
        {
            ghost = GetComponent<Ghost>();
        }

        private void Start()
        {
            getEMF = false;
            isInItemSlot = false;

            // ItemSlot 오브젝트를 찾고, 없으면 에러 로그 출력
            GameObject itemSlotObject = GameObject.Find("ItemSlot");
            if (itemSlotObject != null)
            {
                itemSlotTransform = itemSlotObject.transform;
            }
            else
            {
                Debug.LogError("ItemSlot 오브젝트를 찾을 수 없습니다!");
            }
        }

        private void Update()
        {
            // itemSlotTransform이 null인지 확인
            if (itemSlotTransform != null)
            {
                isInItemSlot = transform.IsChildOf(itemSlotTransform); // EMF가 현재 ItemSlot에 있는지 확인

                if (isInItemSlot)
                {
                    EMFSwitching();
                }
                else
                {
                    // 아이템 슬롯에 없으면 모든 라이트를 끄기
                    for (int i = 0; i < lights.Length; i++)
                    {
                        lights[i].gameObject.SetActive(false);
                    }
                }
            }
        }

        static internal void EMFEquip()
        {
            getEMF = true;
            GameObject EMFObject = GameObject.FindGameObjectWithTag("Items");

            if (EMFObject != null)
            {
                GameObject itemSlot = GameObject.Find("ItemSlot");
                if (itemSlot != null)
                {
                    EMFObject.transform.SetParent(itemSlot.transform);
                    EMFObject.transform.localPosition = Vector3.zero;
                    EMFObject.transform.localRotation = Quaternion.identity;

                    isInItemSlot = true; // ItemSlot에 추가되었음을 표시
                }
                else
                {
                    Debug.LogError("ItemSlot 오브젝트를 찾을 수 없습니다!");
                }
            }
            else
            {
                Debug.LogError("EMF 오브젝트를 찾을 수 없습니다!");
            }
        }

        public void EMFSwitching()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (EMFOnOff == false)
                {
                    EMFOnOff = true;
                    lights[0].gameObject.SetActive(true);
                }
                else
                {
                    EMFOnOff = false;
                    for (int i = 0; i < lights.Length; i++)
                        lights[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
