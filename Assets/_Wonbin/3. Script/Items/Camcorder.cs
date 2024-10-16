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

        private static int instanceCount = 0; // 프리팹 인스턴스 번호 관리
        private int camcorderID;

        private void Awake()
        {
            // 인스턴스 개수 증가
            instanceCount++;
            camcorderID = instanceCount;  // 캠코더마다 고유 ID 부여

            // 카메라 ID에 따라 RenderTexture 경로 설정
            string renderTexturePath = $"Camera{camcorderID}/Render";  // 캠코더의 개수에      따라경로 자동 설정
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
            //play를 누르면, RenderTexture에 RenderTexture1이 적용되게



            // screenMesh의 크기를 가져옴
            Vector3 meshSize = screenMesh.bounds.size;

            //quad의 RenderTexture 해상도를 수동으로 잡아 화면에 표기 될 수 있게함.
            int width = Mathf.CeilToInt(meshSize.x * 500);  // 가로 크기
            int height = Mathf.CeilToInt(meshSize.y * 500); // 세로 크기

            if (renderTexture != null)
            {
                renderTexture.Release(); // 기존 RenderTexture가 있다면 리소스 해제
            }

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
