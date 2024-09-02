using ExitGames.Client.Photon;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace changwon
{
    public class _EMF : MonoBehaviour
    {
        public Collider interaction;
        
        public Light[] lights;

        Ghost ghost;

        private void Start()
        {
            getEMF = false;
            isInItemSlot = false;
            itemSlotTransform = GameObject.Find("ItemSlot")?.transform;
        }


        static bool getEMF;

        public static bool isInItemSlot; // EMF가 ItemSlot에 있는지 여부를 확인
        private Transform itemSlotTransform;
        playerInventory Inventory;


        private bool EMFOnOff = false;


        private void Awake()
        {
            ghost = GetComponent<Ghost>();
                     
        }

        private void Update()
        {


            bool isInItemSlot = transform.IsChildOf(itemSlotTransform); //Emf가 현재 아이템 슬롯에 들어가 있는지 확인.

            if (isInItemSlot)
            {
                EMFSwitching();
            }

            else
            {
                for (int i = 0; i < lights.Length; i++)
                {
                    lights[i].gameObject.SetActive(false);
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

                    isInItemSlot = true; //ItemSlot에 추가되었음을 표시
                }
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
                    lights[0].gameObject.SetActive(false);
                }
            }
        }



        private void OnTriggerStay(Collider other)
        {
            other=interaction;
            if (EMFOnOff == true)
            {
                if (other.tag == "Ghost")
                {
                    if (ghost.state == GhostState.HUNTTING)
                    {
                        for (int i = 0; i < lights.Length; i++)
                        {
                            lights[i].gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        switch (ghost.ghostType)
                        {
                            case GhostType.BANSHEE:

                                
                                break;

                            case GhostType.NIGHTMARE:
                                for (int i = 0; i < 3; i++)
                                {
                                    lights[i].gameObject.SetActive(true);
                                }
                                break;

                            case GhostType.DEMON:
                                for (int i = 0; i < lights.Length; i++)
                                {
                                    lights[i].gameObject.SetActive(true);
                                }
                                break;
                        }
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Ghost")
            {
                lights[0].gameObject.SetActive(true);
            }
        }
    }



}