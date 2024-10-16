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
        public static bool isInItemSlot; // �������� ItemSlot�� �ִ��� ���θ� Ȯ��
        private Transform itemSlotTransform;

        PlayerInventory Inventory;

        private void Start()
        {
            playerGetLight = false;
            isInItemSlot = false; // �ʱ� ���´� ItemSlot�� ����

            if (myLight == null)
            {
                myLight = GetComponent<Light>(); // �ʿ��� ��� �ٽ� �Ҵ�
            }


            myLight.intensity = 0;
            itemSlotTransform = GameObject.Find("ItemSlot")?.transform;
            uvLight = GetComponent<changwon.UVLight>();
        }

        public void lightOnOFF()
        {
            photonView.RPC("SyncUVLightState", RpcTarget.All);
        }

        [PunRPC]
        public void SyncUVLightState()
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