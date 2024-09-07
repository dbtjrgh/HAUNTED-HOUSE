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
        public static bool isInItemSlot; // EMF�� ItemSlot�� �ִ��� ����
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

            // ItemSlot ������Ʈ�� ã��, ������ ���� �α� ���
            GameObject itemSlotObject = GameObject.Find("ItemSlot");
            if (itemSlotObject != null)
            {
                itemSlotTransform = itemSlotObject.transform;
            }
            else
            {
                Debug.LogError("ItemSlot ������Ʈ�� ã�� �� �����ϴ�!");
            }
        }

        private void Update()
        {
            // itemSlotTransform�� null���� Ȯ��
            if (itemSlotTransform != null)
            {
                isInItemSlot = transform.IsChildOf(itemSlotTransform); // EMF�� ���� ItemSlot�� �ִ��� Ȯ��

                if (isInItemSlot)
                {
                    EMFSwitching();
                }
                else
                {
                    // ������ ���Կ� ������ ��� ����Ʈ�� ����
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

                    isInItemSlot = true; // ItemSlot�� �߰��Ǿ����� ǥ��
                }
                else
                {
                    Debug.LogError("ItemSlot ������Ʈ�� ã�� �� �����ϴ�!");
                }
            }
            else
            {
                Debug.LogError("EMF ������Ʈ�� ã�� �� �����ϴ�!");
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
