using UnityEngine;
using Infrastructure.Services;
using Infrastructure;
using System.Collections;

namespace Wonbin
{
    public class TruckDoorButton : MonoBehaviour
    {
        [SerializeField]
        private Animator _anim;

        private bool _isDoorsOpened = false;
        private bool _canBeOpened = true;
        private float _delayForEnd = 3f;
        private float _closingDoorsTime = 4f;
        private Animator truckAnimator;

        private void Start()
        {
            truckAnimator = GameObject.FindWithTag("Truck").GetComponent<Animator>();
            _anim = truckAnimator;
        }

        private void Update()
        {
            // 마우스 좌클릭을 인식하여 openDoor 또는 closeDoor 호출
            if (Input.GetMouseButtonDown(0))
            {
                // 현재 상태에 따라 열거나 닫기
                if (_isDoorsOpened)
                {
                    closeDoor();
                }
                else
                {
                    openDoor();
                }
            }
        }

        public void openDoor()
        {
            if (_canBeOpened && !_isDoorsOpened && !_anim.GetCurrentAnimatorStateInfo(0).IsName("ClosingDoors"))
            {
                _anim.SetTrigger("OpenDoors");
                _isDoorsOpened = true;
            }
        }

        public void closeDoor()
        {
            if (_isDoorsOpened && !_anim.GetCurrentAnimatorStateInfo(0).IsName("OpeningDoors"))
            {
                _anim.SetTrigger("CloseDoors");
                _isDoorsOpened = false;
            }
        }
    }
}