using Items.Logic;
using UnityEngine;
using System.Collections;

namespace Items.ItemsLogic
{
    public class UVFlashlight : MonoBehaviour, IMainUsable, IDroppable
    {
        [SerializeField] private UVLight _uvLight;

        [SerializeField]
        private Light _light;


        [SerializeField]
        public Material _revealableMaterial;

        private AudioSource _audioSource;

        private bool _isEnabled = false;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }
        public void OnMainUse()
        {
            if (_isEnabled == false)            // UV 라이트가 꺼져있을 때
            {
                _uvLight.EnableUVLight();
                _light.enabled = true;
                _isEnabled = true;
            }
            else                               // UV 라이트가 켜져있을 때        
            {
                _uvLight.DisableUVLight();
                _light.enabled = false;
                _isEnabled = false;
            }

            _audioSource.Play();
        }
        private void OnDisable()                    // UV 라이트 비활성화
        {
            _uvLight.DisableUVLight();
        }
        private void OnEnable()                     // UV 라이트 활성화
        {
            if (_isEnabled) _uvLight.EnableUVLight();
        }

        public void DropItem()                      
        {
            if (_isEnabled) _uvLight.EnableUVLight();
        }
    }
}