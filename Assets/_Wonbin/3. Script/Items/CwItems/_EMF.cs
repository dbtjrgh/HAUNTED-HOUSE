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
        private Ghost ghost; // �ͽ��� ��ũ��Ʈ�� �������� ������

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
            if (EMFOnOff && other.CompareTag("Ghost")) // �ͽſ� ����� ��
            {
                ghost = other.GetComponent<Ghost>(); // �ͽ��� Ghost ��ũ��Ʈ ������
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
                if (ghost == null) // �浹 �� �̹� ��ũ��Ʈ�� �������� ���� ��� �ٽ� ������
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
                lights[0].gameObject.SetActive(false); // �ͽ��� ������� EMF ��
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
                        // BANSHEE ���� ó��
                        Debug.Log("��ö� �н�");
                        break;

                    case GhostType.NIGHTMARE:
                        for (int i = 0; i < 3; i++)
                        {
                            Debug.Log("����Ʈ�޾� �ν�");
                            lights[i].gameObject.SetActive(true);
                        }
                        break;

                    case GhostType.DEMON:
                        for (int i = 0; i < lights.Length; i++)
                        {
                            Debug.Log("���� �ν�");
                            lights[i].gameObject.SetActive(true);
                        }
                        break;
                }
            }
        }
    }
}
