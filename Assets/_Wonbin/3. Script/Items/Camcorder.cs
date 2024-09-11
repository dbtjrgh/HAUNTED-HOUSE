using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wonbin
{
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(PhotonTransformView))]
    [RequireComponent(typeof(PhotonRigidbodyView))]
    public class Camcorder : MonoBehaviourPun
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

        private static int instanceCount = 0; // ������ �ν��Ͻ� ��ȣ ����
        private int camcorderID;

        private void Awake()
        {
            // �ν��Ͻ� ���� ����
            instanceCount++;
            camcorderID = instanceCount;  // ķ�ڴ����� ���� ID �ο�

            // ī�޶� ID�� ���� RenderTexture ��� ����
            string renderTexturePath = $"Camera{camcorderID}/Render";  // ķ�ڴ��� ������      ������ �ڵ� ����
            renderTexture = Resources.Load<RenderTexture>(renderTexturePath);

            CamcorderSetup();
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

            if (renderTexture != null)
            {
                renderTexture.Release(); // ���� RenderTexture�� �ִٸ� ���ҽ� ����
            }

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
