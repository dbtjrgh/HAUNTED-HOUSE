using Infrastructure;
using UnityEngine;
using System.Collections;
using Infrastructure.Services;
using Photon.Pun;
using Cinemachine;

public class CMultiPlayer : MonoBehaviourPunCallbacks
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
    private Wonbin.CrouchAnimation _animControl;
    private Animator animator;

    // 중력
    [SerializeField]
    private float gravity = 5f;  // 중력 값
    private float verticalVelocity = 0f; // 수직 속도
    private bool isGrounded; // 플레이어가 지면에 있는지 여부

    // 죽음 여부 체크 변수
    private bool isDead = false;  // 플레이어가 죽었는지 확인하는 변수

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

    private PhotonView pv;
    private CinemachineVirtualCamera playerCinemachine;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        CLookBoard lookBoard = FindObjectOfType<CLookBoard>();
        pv = GetComponent<PhotonView>();

        if (pv.IsMine)
        {
            playerCinemachine = _playerHead.GetComponentInChildren<CinemachineVirtualCamera>();
            if (playerCinemachine == null)
            {
                Debug.LogError("CinemachineVirtualCamera not found in _playerHead.");
            }
            if (lookBoard != null)
            {
                lookBoard.SetPlayerReferences(playerCinemachine, transform);
            }
            else
            {
                Debug.LogError("CLookBoard script not found in the scene.");
            }
        }
        else
        {
            playerCinemachine = _playerHead.GetComponentInChildren<CinemachineVirtualCamera>();
            if (playerCinemachine != null)
            {
                playerCinemachine.gameObject.SetActive(false);
            }
        }
    }

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        initialHeadPositionY = _playerHead.position.y; // 초기 머리 위치 저장
    }

    private void Update()
    {
        if (pv.IsMine && !isDead)  // isDead가 false일 때만 조작 허용
        {
            moveInput();
            Sprint();
            FollowHead();  // 머리 위치 업데이트 호출
            PlayerRotation(); // 플레이어 회전
            CrouchHandle();

            // 중력 및 지면 체크
            isGrounded = charController.isGrounded;

            if (isGrounded && verticalVelocity < 0)
            {
                verticalVelocity = -2f;  // 지면에 있을 때 수직 속도를 리셋
            }

            ApplyGravity();  // 중력 계산 및 적용

            // 수평 이동 계산
            Vector3 move = GetMoveDirection();

            // **수평 이동만 반영하고, 중력을 따로 적용**
            move.y = verticalVelocity;

            // 이동 반영
            charController.Move(move * Time.deltaTime);
        }
    }

    // 중력 적용 함수
    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            verticalVelocity -= gravity * Time.deltaTime;  // 중력에 따라 수직 속도 증가
        }
    }

    private Vector3 GetMoveDirection()
    {
        // 마우스의 이동량
        float y_RotateSize = Input.GetAxis("Mouse X") * turnSpeed;
        float y_Rotate = _playerBody.eulerAngles.y + y_RotateSize;

        // 마우스 상하 이동에 따른 이동값 계산
        float x_RotationSize = -Input.GetAxis("Mouse Y") * turnSpeed;
        xRotation = Mathf.Clamp(xRotation + x_RotationSize, -80f, 80f);

        _playerBody.eulerAngles = new Vector3(_playerBody.eulerAngles.x, y_Rotate, 0);

        // 수평 이동량 측정
        Vector3 move = _playerBody.forward * Input.GetAxis("Vertical") + _playerBody.right * Input.GetAxis("Horizontal");

        // **수평 이동만을 별도로 처리하여, 중력 영향을 받지 않도록 함**
        Vector3 horizontalMove = new Vector3(move.x, 0, move.z);
        horizontalMove *= moveSpeed;  // 스프린트 등을 반영한 이동 속도

        return horizontalMove;  // 수평 이동만 반환
    }

    // 죽음 처리 함수
    public void Die()
    {
        isDead = true;  // 플레이어 사망 상태로 설정

        if (animator != null)
        {
            animator.SetTrigger("Die");  // Die 애니메이션 트리거
        }

        Debug.Log("Player has died and controls are disabled.");
    }

    private void CrouchHandle()
    {
        if (Input.GetKeyDown(KeyCode.C) && !isDead)  // 플레이어가 죽지 않은 경우에만 crouch 허용
        {
            _crouch = !_crouch;

            if (animator != null)
            {
                animator.SetBool("IsCrouch", _crouch);
            }

            if (_crouch)
            {
                AdjustHeadPosition(initialHeadPositionY + crouchHeadOffset);
            }
            else
            {
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
        if (Input.GetKey(KeyCode.LeftShift) && !isDead)  // 죽지 않은 경우에만 스프린트 허용
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
        float y_Rotate = _playerBody.eulerAngles.y + y_RotateSize;

        // 마우스 상하 이동에 따른 이동값 계산
        float x_RotationSize = -Input.GetAxis("Mouse Y") * turnSpeed;
        xRotation = Mathf.Clamp(xRotation + x_RotationSize, -80f, 80f);

        _playerBody.eulerAngles = new Vector3(_playerBody.eulerAngles.x, y_Rotate, 0);

        // 이동량 측정
        Vector3 move = _playerBody.forward * Input.GetAxis("Vertical") + _playerBody.right * Input.GetAxis("Horizontal");

        // 이동량을 현재 좌표에 반영
        charController.Move(move * moveSpeed * Time.deltaTime);

        // 애니메이션 상태 업데이트
        if (animator != null)
        {
            bool isWalking = move.magnitude > 0;
            animator.SetBool("isWalking", isWalking);
        }
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
        _headPosition = _playerHead.position;
        _headPosition.y = _playerBody.position.y + _playerHeadOffset;
        _playerHead.position = _headPosition;

        _playerHead.localRotation = Quaternion.Euler(xRotation_head, 0, 0);
    }
}
