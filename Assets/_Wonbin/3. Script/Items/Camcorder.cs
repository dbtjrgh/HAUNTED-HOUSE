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

        private bool normalMode; // �븻 ������� ��Ʈ ���� ������� Ȯ��
        private bool EquipCam; // ī�޶� ���� ����

        public static bool isInItemSlot;
        private Transform itemSlotTransform;


        private void Start()
        {
            renderTexture = Resources.Load<RenderTexture>("RenderTexture1");

            CamcorderSetup();

            if (screenMesh == null)
            {
                Debug.LogError("screenMesh�� �Ҵ���� �ʾҽ��ϴ�! Ȯ�����ּ���.");
            }
            else
            {
                Debug.Log("screenMesh�� ���������� �Ҵ�Ǿ����ϴ�.");
            }
            isInItemSlot = false;
            EquipCam = false;

            // ItemSlot ������Ʈ ã��
            GameObject itemSlotObject = GameObject.Find("ItemSlot");
            if (itemSlotObject != null)
            {
                itemSlotTransform = itemSlotObject.transform;
            }
            else
            {
                Debug.LogError("ItemSlot�� ã�� �� �����ϴ�!");
            }
        }


        private void Update()
        {
            // itemSlotTransform�� null���� Ȯ��
            if (itemSlotTransform != null)
            {
                isInItemSlot = transform.IsChildOf(itemSlotTransform);

                if (isInItemSlot)
                {
                    EquipCam = true;
                    // R Ű�� ������ �� ��� ��ȯ
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
            if (EquipCam)  // ķ�ڴ��� ������ ���¿����� ��� ��ȯ ����
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
            // RenderTexture ���� �� ����
            Vector3 meshSize = screenMesh.bounds.size;

            int width = Mathf.CeilToInt(meshSize.x * 500);  // ���� ũ��
            int height = Mathf.CeilToInt(meshSize.y * 500); // ���� ũ��

            renderTexture = new RenderTexture(width, height, 16);

            camcorder.targetTexture = renderTexture;
            greenScreen.targetTexture = renderTexture;

            screenMesh.sharedMaterial = renderTextureMat;
            screenMesh.sharedMaterial.mainTexture = renderTexture;
        }
    }
}
