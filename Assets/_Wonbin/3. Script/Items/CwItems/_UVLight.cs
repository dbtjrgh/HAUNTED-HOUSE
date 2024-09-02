using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace changwon
{
    public class _UVLight : MonoBehaviour
    {
        public Light uvLight; // UV ����Ʈ ������Ʈ
        public LayerMask handprintLayerMask; // ���ڱ��� ���Ե� ���̾�

        void Update()
        {
            if (uvLight.enabled)
            {
                Ray ray = new Ray(uvLight.transform.position, uvLight.transform.forward);
                RaycastHit hit;

                // UV ����Ʈ�� ���ڱ� ������Ʈ�� ����� Ȯ��
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, handprintLayerMask))
                {
                    Renderer handprintRenderer = hit.collider.GetComponent<Renderer>();
                    if (handprintRenderer != null)
                    {
                        handprintRenderer.enabled = true; // ���ڱ� ���̰� �ϱ�
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
            // ��� ���ڱ��� ��Ȱ��ȭ�ϴ� �޼���
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
