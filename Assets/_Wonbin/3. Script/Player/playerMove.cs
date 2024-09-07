using Infrastructure;
using UnityEngine;
using System;
using System.Collections;
using Infrastructure.Services;


namespace Wonbin
{
    public class playerMove : MonoBehaviour
    {
        [SerializeField]
        private CharacterController charController;

        // 카메라 회전 속도
        [SerializeField]
        private float turnSpeed = 4f;

        // 카메라 상하 회전   
        private float _mouseX, _mouseY;
        private float xRotation = 0f;
        private float xRotation_head = 0f;

        // 이동 속도
        [SerializeField]
        private float normalSpeed = 2f;

        private float moveSpeed;
        private float sprintMultiplier = 2f;

        //앉기 담당 함수
        private bool _crouch = false;
        private CrouchAnimation _animControl;
        private Animator animator;


        // 중력
        [SerializeField]
        private float gravity = 3f;

        // 몸통 각도 조절 변수
        [SerializeField]
        private Transform _playerBody;   // 몸 Transform (상체나 전체 몸이 포함된 Transform)


        // 머리 각도 조절 변수
        [SerializeField]
        private Transform _playerHead;   // 머리 Transform
        private Vector3 _headPosition;
        private float _currFollowHeadTime = 0f;
        private float _playerHeadOffset = 0f;
        private const float FollowHeadTime = 2f;
        private float initialHeadPositionY; // 서있을 때의 머리의 Y좌표를 저장.
        public float crouchHeadOffset = -1f; //숙였을때 머리의 Y좌표를 저장.

        private void Start()
        {
            animator = GetComponentInChildren<Animator>();
            initialHeadPositionY = _playerHead.position.y; // 초기 머리 위치 저장
        }


        private void Update()
        {
            moveInput();
            Sprint();
            FollowHead();  // 머리 위치 업데이트 호출
            PlayerRotation(); // 플레이어 회전
            CrouchHandle();
        }



        private void CrouchHandle()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                Debug.Log("앉기 키를 눌렀습니다.");

                // _crouch 토글
                _crouch = !_crouch;

                // 애니메이터의 상태 변경
                if (animator != null)
                {
                    animator.SetBool("IsCrouch", _crouch);
                }

                if (_crouch)
                {
                    // 앉는 동작 처리
                    if (_animControl != null && !_animControl.SitDown())
                    {
                        return;
                    }

                    // 머리 위치를 앉은 상태로 조정
                    AdjustHeadPosition(initialHeadPositionY + crouchHeadOffset);
                }
                else
                {
                    // 일어나는 동작 처리
                    if (_animControl != null && !_animControl.StandUp())
                    {
                        return;
                    }

                    // 머리 위치를 원래 위치로 초기화
                    AdjustHeadPosition(initialHeadPositionY);
                }
            }
        }

        private void AdjustHeadPosition(float targetY)
        {
            if (_playerHead != null)
            {
                Vector3 currentPosition = _playerHead.position;
                _playerHead.position = new Vector3(currentPosition.x, targetY, currentPosition.z);
            }
        }

     


        private void Sprint()
        {
            // 기본 이동 속도에 스프린트 배수 적용
            if (Input.GetKey(KeyCode.LeftShift))
            {
                moveSpeed = normalSpeed * sprintMultiplier;  // 스프린트 중
            }
            else
            {
                moveSpeed = normalSpeed;  // 기본 속도
            }
        }



        private void moveInput()
        {
            // 마우스의 이동량
            float y_RotateSize = Input.GetAxis("Mouse X") * turnSpeed;
            // 현재 회전값에 새로운 회전값을 더해줌
            float y_Rotate = _playerBody.eulerAngles.y + y_RotateSize;

            // 마우스 상하 이동에 따른 이동값 계산
            float x_RotationSize = -Input.GetAxis("Mouse Y") * turnSpeed;
            xRotation = Mathf.Clamp(xRotation + x_RotationSize, -80f, 80f);  // 상하 회전 범위 제한

            // 몸의 회전값을 몸에 반영(y축 회전)
            _playerBody.eulerAngles = new Vector3(_playerBody.eulerAngles.x, y_Rotate, 0);

            // 이동량 측정
            Vector3 move = _playerBody.forward * Input.GetAxis("Vertical") + _playerBody.right * Input.GetAxis("Horizontal");

            // 이동량을 현재 좌표에 반영(이동량 값 계산할때, charController를 참조해야함. 그렇지 않으면 벽 뚫음.)
            charController.Move(move * moveSpeed * Time.deltaTime);
        }

        private void FollowHead()
        {
            if (_currFollowHeadTime > 0f)
            {
                SetHeadPosition();
                _currFollowHeadTime -= Time.deltaTime;
            }
        }

        private void PlayerRotation()
        {
            _playerBody.Rotate(Vector3.up * _mouseX);

            xRotation -= _mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            _playerHead.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        private void SetHeadPosition()
        {
            // 머리 위치와 회전 업데이트
            _headPosition = _playerHead.position;
            _headPosition.y = _playerBody.position.y + _playerHeadOffset; // 머리 위치 조정
            _playerHead.position = _headPosition;

            // 머리 회전 적용
            _playerHead.localRotation = Quaternion.Euler(xRotation_head, 0, 0); // 상하 회전
        }
    }
}