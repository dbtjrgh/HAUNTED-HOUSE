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

        public static bool isInItemSlot;
        private Transform itemSlotTransform;
        public bool isLightOn = false;

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
            itemSlotTransform = GameObject.Find("ItemSlot")?.transform;
        }

        public void OnMainUse()    
        {
            photonView.RPC("SyncCamcorderState", RpcTarget.All);
        }

        [PunRPC]
        public void SyncCamcorderState()
        {
            SetGhostOrbMode();
            isLightOn = !isLightOn;
            if (isLightOn)
            {
                SetGhostOrbMode();
            }
            else
            {
                SetNormalMode();
            }
        }


        private void SetNormalMode()
        {
            renderTextureMat.color = Color.white;
            greenScreen.gameObject.SetActive(false);
            camcorder.gameObject.SetActive(true);
        }

        private void SetGhostOrbMode()
        {
            renderTextureMat.color = Color.green;
            camcorder.gameObject.SetActive(false);
            greenScreen.gameObject.SetActive(true);
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
