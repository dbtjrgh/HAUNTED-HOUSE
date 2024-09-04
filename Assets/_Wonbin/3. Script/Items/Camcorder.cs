using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wonbin
{
    public class Camcorder : MonoBehaviour
    {
        public Camera camcorder;
        public Camera greenScreen;
        public MeshRenderer screenMesh;

        public RenderTexture renderTexture;
        public Material renderTextureMat;

        private bool normalMode = true;
        private bool EquipCam; // ī�޶� ���� ����

        public static bool isInItemSlot;
        private Transform itemSlotTransform;


        private void Start()
        {
            CamcorderSetup();
            isInItemSlot = false;
            EquipCam = false;
            itemSlotTransform = GameObject.Find("ItemSlot")?.transform;
        }

        public void OnMainUse()    // ����Լ�.
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (itemSlotTransform == null)
                {
                    return;
                }

               if(isInItemSlot)
                {
                    EquipCam = true;
                    Debug.Log("ī�޶� �����߽��ϴ�.");
                }

                if (normalMode)
                {
                    SetGhostOrbMode();
                }
                else
                {
                    SetNormalMode();
                }
            }
        }


        private void SetNormalMode()
        {
            renderTextureMat.color = Color.white;
            greenScreen.gameObject.SetActive(false);
            camcorder.gameObject.SetActive(true);
            normalMode = true;
        }

        private void SetGhostOrbMode()
        {
            renderTextureMat.color = Color.green;
            camcorder.gameObject.SetActive(false);
            greenScreen.gameObject.SetActive(true);
            normalMode = false;
        }

        private void CamcorderSetup()
        {
            camcorder.targetTexture = renderTexture;
            greenScreen.targetTexture = renderTexture;
            screenMesh.material = renderTextureMat;
        }
    }
}
