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
        private changwon.UVLight uvLight; //�ڵ� ����Ʈ ��ȣ�ۿ�

        [SerializeField]
        private Light myLight; // Light ������Ʈ ����


        [SerializeField]
        public Material _revealableMaterial;

        //private AudioSource _audioSource;

        bool playerGetLight; // �÷��̾ �������� on�� �������� Ȯ��
        static bool getLight; // ������ ȹ�� ���� Ȯ��
        public static bool isInItemSlot; // �������� ItemSlot�� �ִ��� ���θ� Ȯ��
        private Transform itemSlotTransform;

        PlayerInventory Inventory;

        private void Start()
        {
            playerGetLight = false;
            getLight = false;
            isInItemSlot = false; // �ʱ� ���´� ItemSlot�� ����

            if (myLight == null)
            {
                myLight = GetComponent<Light>(); // �ʿ��� ��� �ٽ� �Ҵ�
            }


            myLight.intensity = 0;
            itemSlotTransform = GameObject.Find("ItemSlot")?.transform;
            uvLight = GetComponent<changwon.UVLight>();
        }

        private void Update()
        {
            // �������� ItemSlot�� �ڽ����� Ȯ��
            bool isInItemSlot = transform.IsChildOf(itemSlotTransform);

            if (isInItemSlot)
            {
                lightOnOFF(); // �������� ItemSlot�� ���� ���� ȣ��
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
                    flashLightObject.transform.localPosition = Vector3.zero; // ��ġ �ʱ�ȭ
                    flashLightObject.transform.localRotation = Quaternion.identity; // ȸ�� �ʱ�ȭ

                    isInItemSlot = true; // ItemSlot�� �߰��Ǿ����� ǥ��

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
                    playerGetLight = !playerGetLight; // ������ on/off
                    myLight.intensity = playerGetLight ? 10 : 0; // ������ ��� ����

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