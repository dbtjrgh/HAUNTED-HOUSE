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

        [SerializeField]
        private RenderTexture renderTexture;

        public Material renderTextureMat;

        private bool normalMode; // 노말 모드인지 고스트 오브 모드인지 확인
        private bool EquipCam; // 카메라 장착 여부

        public static bool isInItemSlot;
        private Transform itemSlotTransform;


        private void Start()
        {
            renderTexture = Resources.Load<RenderTexture>("RenderTexture1");

            CamcorderSetup();

            if (screenMesh == null)
            {
                Debug.LogError("screenMesh가 할당되지 않았습니다! 확인해주세요.");
            }
            else
            {
                Debug.Log("screenMesh가 정상적으로 할당되었습니다.");
            }
            isInItemSlot = false;
            EquipCam = false;

            // ItemSlot 오브젝트 찾기
            GameObject itemSlotObject = GameObject.Find("ItemSlot");
            if (itemSlotObject != null)
            {
                itemSlotTransform = itemSlotObject.transform;
            }
            else
            {
                Debug.LogError("ItemSlot을 찾을 수 없습니다!");
            }
        }


        private void Update()
        {
            // itemSlotTransform이 null인지 확인
            if (itemSlotTransform != null)
            {
                isInItemSlot = transform.IsChildOf(itemSlotTransform);

                if (isInItemSlot)
                {
                    EquipCam = true;
                    // R 키를 눌렀을 때 모드 전환
                    if (Input.GetKeyDown(KeyCode.R))
                    {
                        OnMainUse();
                    }
                }
                else
                {
                    EquipCam = false;
                }
            }
        }

        public void OnMainUse()
        {
            if (EquipCam)  // 캠코더가 장착된 상태에서만 모드 전환 가능
            {
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
            // RenderTexture 설정 및 적용
            Vector3 meshSize = screenMesh.bounds.size;

            int width = Mathf.CeilToInt(meshSize.x * 500);  // 가로 크기
            int height = Mathf.CeilToInt(meshSize.y * 500); // 세로 크기

            renderTexture = new RenderTexture(width, height, 16);

            camcorder.targetTexture = renderTexture;
            greenScreen.targetTexture = renderTexture;

            screenMesh.sharedMaterial = renderTextureMat;
            screenMesh.sharedMaterial.mainTexture = renderTexture;
        }
    }
}
