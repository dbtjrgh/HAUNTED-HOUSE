using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace changwon
{
    public class _UVLight : MonoBehaviour
    {
        public Light uvLight; // UV ����Ʈ ������Ʈ ����
        static bool getLight; // UV ����Ʈ ȹ�� ���� Ȯ��
        public LayerMask handprintLayerMask; // ���ڱ��� ���Ե� ���̾�
        public static bool isInItemSlot; // UV ����Ʈ�� ItemSlot�� �ִ��� ���θ� Ȯ��
        private Transform itemSlotTransform;

        playerInventory Inventory;

        private void Start() // ������ ������ �� �ʱ�ȭ
        {
            getLight = false;
            isInItemSlot = false; // �ʱ� ���´� ItemSlot�� ����
            uvLight = GetComponent<Light>(); // UV ����Ʈ�� Light ������Ʈ�� ������
            uvLight.intensity = 0; // ���� ��, uv ����Ʈ�� ���� �־�� ��.
            uvLight.enabled = false;
            itemSlotTransform = GameObject.Find("ItemSlot")?.transform; // ������ ���Կ� �߰��ϱ� ���� ������ ���� ������Ʈ�� ��ġ��.
        }


        void Update()
        {

            if (itemSlotTransform == null) //������ ���Կ� �������� ������, UV ����Ʈ�� ��Ȱ��ȭ.
            {

                uvLight.enabled = false;
                return;
            }




                if (uvLight.enabled)
            {
                Ray ray = new Ray(uvLight.transform.position, uvLight.transform.forward);
                RaycastHit hit;

                // UV ����Ʈ�� �����ڱ� ������Ʈ�� ����� Ȯ��
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
