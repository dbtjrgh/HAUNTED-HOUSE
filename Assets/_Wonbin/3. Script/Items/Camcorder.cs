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

        private bool normalMode; // 노말 모드인지 고스트 오브 모드인지 확인
        private bool EquipCam; // 카메라 장착 여부

        public static bool isInItemSlot;
        private Transform itemSlotTransform;


        private void Start()
        {
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
            itemSlotTransform = GameObject.Find("ItemSlot")?.transform;
        }


        private void Update()
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

        public void OnMainUse()    
        {
            if (EquipCam)  // 캠코더가 장착된 상태에서만 모드 전환 가능
            {
                // 초기에는 노말 모드가 호출. 노말 모드에서는 카메라 화면이 보이고, 고스트 오브 모드에서는 초록색 화면이 보임
                
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
            // sharedMaterial을 사용해보세요.
            screenMesh.sharedMaterial = renderTextureMat;

            // 확인
            if (screenMesh.sharedMaterial == renderTextureMat)
            {
                Debug.Log("screenMesh의 재질(Material)이 renderTextureMat으로 정상적으로 설정되었습니다.");
            }
            else
            {
                Debug.LogError("screenMesh의 재질(Material)이 renderTextureMat으로 설정되지 않았습니다!");
            }
        }
    }
}
