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
        public Light[] lights;
        private Ghost ghost; // ±Í½ÅÀÇ ½ºÅ©¸³Æ®¸¦ µ¿ÀûÀ¸·Î °¡Á®¿È

        public static bool isInItemSlot;
        private Transform itemSlotTransform;

        public bool EMFOn = false;

        private void Start()
        {
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
                    if (Ghost.instance.state != GhostState.HUNTTING)
                    {
                        SoundManager.instance.EMFNormalSound();

                    }

                }
                else if (EMFOn == true)
                {

                    EMFOn = false;
                    // ±Í½ÅÀÌ ³ª°¡¸é EMF ²¨Áü
                    for (int i = 0; i < lights.Length; i++)
                    {
                        lights[i].gameObject.SetActive(false);
                    }
                    SoundManager.instance.NormalEMFStop();
                    SoundManager.instance.StopEMFHighSound();



                }
            }
        }

        // ÀÚ½Ä ¿ÀºêÁ§Æ®¿¡¼­ Æ®¸®°Å ÀÌº¥Æ®¸¦ Àü´Þ¹Þ¾Æ Ã³¸®
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
                // ±Í½ÅÀÌ ³ª°¡¸é EMF ²¨Áü
                for (int i = 1; i < lights.Length; i++)
                {
                    lights[i].gameObject.SetActive(false);
                    SoundManager.instance.StopEMFHighSound();
                    SoundManager.instance.EMFNormalSound();

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
                    SoundManager.instance.NormalEMFStop();
                    SoundManager.instance.EMFHighSound();
                }
            }
            else
            {
                switch (ghost.ghostType)
                {
                    case GhostType.BANSHEE:
                        Debug.Log("¹ê½Ã °¨ÁöµÊ");
                        SoundManager.instance.StopEMFHighSound();
                        SoundManager.instance.EMFNormalSound();
                        break;

                    case GhostType.NIGHTMARE:
                        for (int i = 0; i < lights.Length; i++)
                        {
                            Debug.Log("³ªÀÌÆ®¸Þ¾î °¨ÁöµÊ");
                            lights[i].gameObject.SetActive(true);
                            SoundManager.instance.NormalEMFStop();
                            SoundManager.instance.EMFHighSound();
                        }
                        break;

                    case GhostType.DEMON:
                        for (int i = 0; i < lights.Length; i++)
                        {
                            Debug.Log("µ¥¸ó °¨ÁöµÊ");

                            lights[i].gameObject.SetActive(true);
                            SoundManager.instance.NormalEMFStop();
                            SoundManager.instance.EMFHighSound();
                        }
                        break;
                }
            }
        }
    }
}
