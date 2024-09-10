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

        public bool EMFOnOff = false;

        private void Start()
        {
            getEMF = false;
            isInItemSlot = false;
            itemSlotTransform = GameObject.Find("ItemSlot")?.transform;
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
            EMFOnOff = !EMFOnOff;
            if (photonView.IsMine)
            {
                photonView.RPC("SyncEMFState", RpcTarget.All, EMFOnOff);
            }
        }

        [PunRPC]
        void SyncEMFState(bool state)
        {
            EMFOnOff = state;
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

        private void OnTriggerEnter(Collider other)
        {
            if (EMFOnOff && other.CompareTag("Ghost")) // 귀신에 닿았을 때
            {
                ghost = other.GetComponent<Ghost>(); // 귀신의 Ghost 스크립트 가져옴
                if (ghost != null)
                {
                    HandleGhostDetection();
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (EMFOnOff && other.CompareTag("Ghost"))
            {
                if (ghost == null) // 충돌 시 이미 스크립트를 가져오지 못한 경우 다시 가져옴
                {
                    ghost = other.GetComponent<Ghost>();
                }

                if (ghost != null)
                {
                    HandleGhostDetection();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Ghost"))
            {
                lights[0].gameObject.SetActive(false); // 귀신이 사라지면 EMF 끔
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
                        // BANSHEE 관련 처리
                        Debug.Log("밴시라 패스");
                        break;

                    case GhostType.NIGHTMARE:
                        for (int i = 0; i < 3; i++)
                        {
                            Debug.Log("나이트메어 인식");
                            lights[i].gameObject.SetActive(true);
                        }
                        break;

                    case GhostType.DEMON:
                        for (int i = 0; i < lights.Length; i++)
                        {
                            Debug.Log("데몬 인식");
                            lights[i].gameObject.SetActive(true);
                        }
                        break;
                }
            }
        }
    }
}
