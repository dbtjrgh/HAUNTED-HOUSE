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


        private void Awake()
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
            itemSlotTransform = GameObject.Find("ItemSlot")?.transform;
        }


        private void Update()
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

        public void OnMainUse()    
        {
            if (EquipCam)  // ķ�ڴ��� ������ ���¿����� ��� ��ȯ ����
            {
                // �ʱ⿡�� �븻 ��尡 ȣ��. �븻 ��忡���� ī�޶� ȭ���� ���̰�, ��Ʈ ���� ��忡���� �ʷϻ� ȭ���� ����
                
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
            //play�� ������, RenderTexture�� RenderTexture1�� ����ǰ�



            // screenMesh�� ũ�⸦ ������
            Vector3 meshSize = screenMesh.bounds.size;

            //quad�� RenderTexture �ػ󵵸� �������� ��� ȭ�鿡 ǥ�� �� �� �ְ���.
            int width = Mathf.CeilToInt(meshSize.x * 500);  // ���� ũ��
            int height = Mathf.CeilToInt(meshSize.y * 500); // ���� ũ��

            // RenderTexture�� �ػ󵵸� Quad ũ�⿡ ���� ����
            renderTexture = new RenderTexture(width, height, 16);  // ���� ���� 16-bit

            // ī�޶� RenderTexture ����
            camcorder.targetTexture = renderTexture;
            greenScreen.targetTexture = renderTexture;

            // screenMesh�� Material�� RenderTexture ����
            screenMesh.sharedMaterial = renderTextureMat;
            screenMesh.sharedMaterial.mainTexture = renderTexture;
          
        }
    }
}
