using UnityEngine;
using System.Collections;
using Photon.Pun;


namespace changwon
{
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(PhotonTransformView))]
    [RequireComponent(typeof(PhotonRigidbodyView))]
    public class UVFlashlight : MonoBehaviourPun
    {
        [SerializeField]
        private changwon.UVLight uvLight; //핸드 프린트 상호작용

        [SerializeField]
        private Light myLight; // Light 컴포넌트 관리


        [SerializeField]
        public Material _revealableMaterial;

        //private AudioSource _audioSource;

        bool playerGetLight; // 플레이어가 손전등을 on한 상태인지 확인
        static bool getLight; // 손전등 획득 여부 확인
        public static bool isInItemSlot; // 손전등이 ItemSlot에 있는지 여부를 확인
        private Transform itemSlotTransform;

        PlayerInventory Inventory;

        private void Start()
        {
            playerGetLight = false;
            getLight = false;
            isInItemSlot = false; // 초기 상태는 ItemSlot에 없음

            if (myLight == null)
            {
                myLight = GetComponent<Light>(); // 필요한 경우 다시 할당
            }


            myLight.intensity = 0;
            itemSlotTransform = GameObject.Find("ItemSlot")?.transform;
            uvLight = GetComponent<changwon.UVLight>();
        }

        private void Update()
        {
            // 손전등이 ItemSlot의 자식인지 확인
            bool isInItemSlot = transform.IsChildOf(itemSlotTransform);

            if (isInItemSlot)
            {
                lightOnOFF(); // 손전등이 ItemSlot에 있을 때만 호출
            }

        }

        static internal void lightEquip()
        {
            getLight = true;
            GameObject flashLightObject = GameObject.FindGameObjectWithTag("Items");

            if (flashLightObject != null)
            {
                GameObject itemSlot = GameObject.Find("ItemSlot");
                if (itemSlot != null)
                {
                    flashLightObject.transform.SetParent(itemSlot.transform);
                    flashLightObject.transform.localPosition = Vector3.zero; // 위치 초기화
                    flashLightObject.transform.localRotation = Quaternion.identity; // 회전 초기화

                    isInItemSlot = true; // ItemSlot에 추가되었음을 표시

                }
            }
        }

        public void lightOnOFF()
        {
            getLight = true;
            if (getLight)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    SoundManager.instance.FlashLightSound();
                    playerGetLight = !playerGetLight; // 손전등 on/off
                    myLight.intensity = playerGetLight ? 10 : 0; // 손전등 밝기 조정

                    if (playerGetLight)
                    {
                        uvLight.EnableUVLight();
                    }
                    else
                    {

                        uvLight.DisableUVLight();
                    }
                }
            }
        }
        public IEnumerator Blink()
        {
            if (Ghost.instance.state == changwon.GhostState.HUNTTING)
            {
                float ghostBlinkTargetDistance = Vector3.Distance(Ghost.instance.target.transform.position, Ghost.instance.transform.position);
                if (ghostBlinkTargetDistance < 30)
                {
                    myLight.intensity = 0;
                    yield return new WaitForSeconds(0.5f);
                    myLight.intensity = 10;
                }
            }
            else
            {
                myLight.intensity = 10;
            }
            yield return null;
        }
    }
}