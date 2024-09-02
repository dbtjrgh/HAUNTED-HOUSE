using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace changwon
{
    public class _UVLight : MonoBehaviour
    {
        public Light uvLight; // UV 라이트 오브젝트 관리
        static bool getLight; // UV 라이트 획득 여부 확인
        public LayerMask handprintLayerMask; // 손자국이 포함된 레이어
        public static bool isInItemSlot; // UV 라이트가 ItemSlot에 있는지 여부를 확인
        private Transform itemSlotTransform;

        playerInventory Inventory;

        private void Start() // 선언한 변수들 값 초기화
        {
            getLight = false;
            isInItemSlot = false; // 초기 상태는 ItemSlot에 없음
            uvLight = GetComponent<Light>(); // UV 라이트의 Light 컴포넌트를 가져옴
            uvLight.intensity = 0; // 시작 시, uv 라이트는 꺼져 있어야 함.
            uvLight.enabled = false;
            itemSlotTransform = GameObject.Find("ItemSlot")?.transform; // 아이템 슬롯에 추가하기 위해 아이템 슬롯 오브젝트를 서치함.
        }


        void Update()
        {

            if (itemSlotTransform == null) //아이템 슬롯에 존재하지 않으면, UV 라이트를 비활성화.
            {

                uvLight.enabled = false;
                return;
            }




                if (uvLight.enabled)
            {
                Ray ray = new Ray(uvLight.transform.position, uvLight.transform.forward);
                RaycastHit hit;

                // UV 라이트가 ㄴ손자국 오브젝트에 닿는지 확인
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, handprintLayerMask))
                {
                    Renderer handprintRenderer = hit.collider.GetComponent<Renderer>();
                    if (handprintRenderer != null)
                    {
                        handprintRenderer.enabled = true; // 손자국 보이게 하기
                    }
                }
                else
                {
                    DisableAllHandprints();
                }
            }
            else
            {
                DisableAllHandprints();
            }
        }

        void DisableAllHandprints()
        {
            // 모든 손자국을 비활성화하는 메서드
            foreach (GameObject handprint in GameObject.FindGameObjectsWithTag("Handprint"))
            {
                Renderer handprintRenderer = handprint.GetComponent<Renderer>();
                if (handprintRenderer != null)
                {
                    handprintRenderer.enabled = false;
                }
            }
        }
    }
}
