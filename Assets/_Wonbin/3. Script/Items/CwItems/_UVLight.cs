using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace changwon
{
    public class _UVLight : MonoBehaviour
    {
        public Light uvLight; // UV 라이트 오브젝트
        public LayerMask handprintLayerMask; // 손자국이 포함된 레이어

        void Update()
        {
            if (uvLight.enabled)
            {
                Ray ray = new Ray(uvLight.transform.position, uvLight.transform.forward);
                RaycastHit hit;

                // UV 라이트가 손자국 오브젝트에 닿는지 확인
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
