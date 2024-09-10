using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace changwon
{
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(PhotonTransformView))]
    [RequireComponent(typeof(PhotonRigidbodyView))]
    public class _EMF : MonoBehaviourPun
    {
        public Collider interaction;
        public Light[] lights;
        private Ghost ghost; // 귀신의 스크립트를 동적으로 가져옴

        static bool getEMF;
        public static bool isInItemSlot;
        private Transform itemSlotTransform;
        PlayerInventory Inventory;

        public bool EMFOn = false;

        private void Start()
        {
            getEMF = false;
            isInItemSlot = false;
            itemSlotTransform = GameObject.Find("ItemSlot")?.transform;

            // 자식 오브젝트의 InteractionTrigger에 이 스크립트를 연결
            if (interaction != null)
            {
                CEMFTrigger interactionTrigger = interaction.GetComponent<CEMFTrigger>();
                if (interactionTrigger != null)
                {
                    interactionTrigger.parentEMF = this;
                }
            }
        }

        private void Update()
        {
            bool isInItemSlot = transform.IsChildOf(itemSlotTransform);

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

        public void ToggleEMFState()
        {
            EMFOn = !EMFOn;
            if (photonView.IsMine)
            {
                photonView.RPC("SyncEMFState", RpcTarget.All, EMFOn);
            }
        }

        [PunRPC]
        void SyncEMFState(bool state)
        {
            EMFOn = state;
            foreach (var light in lights)
            {
                light.gameObject.SetActive(state);
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
                    isInItemSlot = true;
                }
            }
        }

        public void EMFSwitching()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (EMFOn == false)
                {
                    EMFOn = true;
                    lights[0].gameObject.SetActive(true);
                }
                else if (EMFOn == true)
                {
                    EMFOn = false;
                    lights[0].gameObject.SetActive(false);
                }
            }
        }

        // 자식 오브젝트에서 트리거 이벤트를 전달받아 처리
        public void OnChildTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ghost"))
            {
                ghost = other.GetComponent<Ghost>();
                if (ghost != null && EMFOn)
                {
                    HandleGhostDetection();
                }
            }
        }

        public void OnChildTriggerStay(Collider other)
        {
            if (other.CompareTag("Ghost"))
            {
                ghost = other.GetComponent<Ghost>();
                if (ghost != null && EMFOn)
                {
                    HandleGhostDetection();
                }
            }
        }

        public void OnChildTriggerExit(Collider other)
        {
            if (other.CompareTag("Ghost"))
            {
                // 귀신이 나가면 EMF 꺼짐
                for (int i = 1; i < lights.Length; i++)
                {
                    lights[i].gameObject.SetActive(false);
                }
            }
        }

        private void HandleGhostDetection()
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
                        Debug.Log("밴시 감지됨");
                        break;

                    case GhostType.NIGHTMARE:
                        for (int i = 0; i < 3; i++)
                        {
                            Debug.Log("나이트메어 감지됨");
                            lights[i].gameObject.SetActive(true);
                        }
                        break;

                    case GhostType.DEMON:
                        for (int i = 0; i < lights.Length; i++)
                        {
                            Debug.Log("데몬 감지됨");
                            lights[i].gameObject.SetActive(true);
                        }
                        break;
                }
            }
        }
    }
}
