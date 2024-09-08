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


        private void Awake()
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
            //play를 누르면, RenderTexture에 RenderTexture1이 적용되게



            // screenMesh의 크기를 가져옴
            Vector3 meshSize = screenMesh.bounds.size;

            //quad의 RenderTexture 해상도를 수동으로 잡아 화면에 표기 될 수 있게함.
            int width = Mathf.CeilToInt(meshSize.x * 500);  // 가로 크기
            int height = Mathf.CeilToInt(meshSize.y * 500); // 세로 크기

            // RenderTexture의 해상도를 Quad 크기에 맞춰 설정
            renderTexture = new RenderTexture(width, height, 16);  // 깊이 버퍼 16-bit

            // 카메라에 RenderTexture 적용
            camcorder.targetTexture = renderTexture;
            greenScreen.targetTexture = renderTexture;

            // screenMesh의 Material에 RenderTexture 적용
            screenMesh.sharedMaterial = renderTextureMat;
            screenMesh.sharedMaterial.mainTexture = renderTexture;
          
        }
    }
}
