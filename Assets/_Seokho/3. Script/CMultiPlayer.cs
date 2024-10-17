using System.Collections;
using UnityEngine;
using Infrastructure.Services;
using Photon.Pun;
using Cinemachine;

public class CMultiPlayer : MonoBehaviourPunCallbacks
{
    #region 변수
    [SerializeField]
    private CharacterController charController;

    [SerializeField]
    private float turnSpeed = 4f;

    private float _mouseX, _mouseY;
    private float xRotation = 0f;
    private float xRotation_head = 0f;

    [SerializeField]
    private float normalSpeed = 2f;

    private float moveSpeed;
    public float sprintMultiplier = 1.5f;

    private bool _crouch = false;
    private Wonbin.CrouchAnimation _animControl;
    private Animator animator;

    [SerializeField]
    private float gravity = 5f;
    private float verticalVelocity = 0f;
    private bool isGrounded;

    public bool isDead = false;

    [SerializeField]
    private Transform _playerBody;

    [SerializeField]
    private Transform _playerHead;
    private Vector3 _headPosition;
    private float _currFollowHeadTime = 0f;
    private float initialHeadPositionY;
    public float crouchHeadOffset;

    private PhotonView pv;
    private CinemachineVirtualCamera playerCinemachine;
    public Light deadLight;

    private float smoothTime = 0.2f; // 카메라 위치 전환을 위한 시간
    #endregion

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
                Debug.Log("CinemachineVirtualCamera not found in _playerHead.");
            }
            if (lookBoard != null)
            {
                lookBoard.SetPlayerReferences(playerCinemachine, transform);
            }
            else
            {
                Debug.Log("CLookBoard script not found in the scene.");
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
        if (pv.IsMine && !isDead)
        {
            moveInput();
            Sprint();
            FollowHead();
            PlayerRotation();
            CrouchHandle();

            isGrounded = charController.isGrounded;

            if (isGrounded && verticalVelocity < 0)
            {
                verticalVelocity = -2f;
            }

            ApplyGravity();

            Vector3 move = GetMoveDirection();
            move.y = verticalVelocity;
            charController.Move(move * Time.deltaTime);
        }
    }

    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }
    }
    /// <summary>
    /// 마우스, 키보드 플레이어 조작 함수
    /// </summary>
    /// <returns></returns>
    private Vector3 GetMoveDirection()
    {
        float y_RotateSize = Input.GetAxis("Mouse X") * turnSpeed;
        float y_Rotate = _playerBody.eulerAngles.y + y_RotateSize;

        float x_RotationSize = -Input.GetAxis("Mouse Y") * turnSpeed;
        xRotation = Mathf.Clamp(xRotation + x_RotationSize, -80f, 80f);

        _playerBody.eulerAngles = new Vector3(_playerBody.eulerAngles.x, y_Rotate, 0);

        Vector3 move = _playerBody.forward * Input.GetAxis("Vertical") + _playerBody.right * Input.GetAxis("Horizontal");

        Vector3 horizontalMove = new Vector3(move.x, 0, move.z);
        horizontalMove *= moveSpeed;

        return horizontalMove;
    }
    /// <summary>
    /// 죽었을 때 불러오는 함수
    /// </summary>
    [PunRPC]
    public void Die()
    {
        isDead = true;

        if (animator != null)
        {
            animator.SetBool("Die", true);
            SoundManager.instance.GhostLaughSound();
        }

        // 카메라 위치 조정
        AdjustCameraForDeath();
        deadLight.enabled = true;

        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.CheckAllPlayersDead();
        }
    }


    private void AdjustCameraForDeath()
    {
        if (playerCinemachine != null)
        {
            // 카메라 위치를 조정 (예: 플레이어 머리 높이에서 약간 높여서)
            Vector3 newCameraPosition = _playerHead.position + new Vector3(0, 1.0f, -2f);
            playerCinemachine.transform.position = newCameraPosition;

            // 카메라 회전 조정 (예: 플레이어를 바라보게 설정)
            playerCinemachine.transform.LookAt(_playerHead.position);
        }
    }

    /// <summary>
    /// 앉기, 일어나기 불러오는 함수
    /// </summary>
    private void CrouchHandle()
    {
        if (Input.GetKeyDown(KeyCode.C) && !isDead)
        {
            _crouch = !_crouch;

            if (animator != null)
            {
                animator.SetBool("IsCrouch", _crouch);
            }

            // 앉았을 때 속도 감소 및 머리 위치 조정
            if (_crouch)
            {
                moveSpeed = normalSpeed / 2f;  // 앉은 상태에서는 이동 속도를 절반으로 감소
                StartCoroutine(SmoothHeadPosition(initialHeadPositionY + crouchHeadOffset));  // 앉는 위치로 부드럽게 이동
            }
            else
            {
                // 일어설 때는 다시 서 있는 머리 위치로 복구
                moveSpeed = normalSpeed;  // 다시 일어나면 속도 원상복구
                StartCoroutine(SmoothHeadPosition(initialHeadPositionY));  // 서 있는 머리 위치로 부드럽게 복구
            }
        }
    }
    /// <summary>
    /// 앉았다 일어날때 자연스럽게 카메라 무빙시켜주는 함수
    /// </summary>
    /// <param name="targetY"></param>
    /// <returns></returns>
    private IEnumerator SmoothHeadPosition(float targetY)
    {
        float startY = _playerHead.position.y; // 시작 Y 위치
        float timeElapsed = 0f; // 경과 시간

        while (timeElapsed < smoothTime)
        {
            float newY = Mathf.Lerp(startY, targetY, timeElapsed / smoothTime); // Lerp 사용
            _playerHead.position = new Vector3(_playerHead.position.x, newY, _playerHead.position.z);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // 마지막으로 정확하게 목표 위치에 도달하게 설정
        _playerHead.position = new Vector3(_playerHead.position.x, targetY, _playerHead.position.z);
    }
    /// <summary>
    /// 달리기 함수
    /// </summary>
    private void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift) && !isDead)
        {
            moveSpeed = normalSpeed * sprintMultiplier;
        }
        else
        {
            moveSpeed = normalSpeed;
        }
    }

    /// <summary>
    /// 움직일 때 불러오는 함수로 애니메이션 및 사운드 또한 적용
    /// </summary>
    private void moveInput()
    {
        float y_RotateSize = Input.GetAxis("Mouse X") * turnSpeed;
        float y_Rotate = _playerBody.eulerAngles.y + y_RotateSize;

        float x_RotationSize = -Input.GetAxis("Mouse Y") * turnSpeed;
        xRotation = Mathf.Clamp(xRotation + x_RotationSize, -80f, 80f);

        _playerBody.eulerAngles = new Vector3(_playerBody.eulerAngles.x, y_Rotate, 0);

        Vector3 move = _playerBody.forward * Input.GetAxis("Vertical") + _playerBody.right * Input.GetAxis("Horizontal");

        charController.Move(move * moveSpeed * Time.deltaTime);

        if (animator != null)
        {
            bool isWalking = move.magnitude > 0;
            animator.SetBool("isWalking", isWalking);

            if (isWalking)
            {
                SoundManager.instance.PlayWalkingSound();
            }
            else
            {
                SoundManager.instance.StopWalkingSound();
            }
        }
    }
    /// <summary>
    /// 카메라 머리쪽에서 유지시키도록하는 함수
    /// </summary>
    private void FollowHead()
    {
        if (_currFollowHeadTime > 0f)
        {
            SetHeadPosition();
            _currFollowHeadTime -= Time.deltaTime;
        }
    }
    /// <summary>
    /// 마우스 조작에 따라 시점이 바뀌게 하는 함수
    /// </summary>
    private void PlayerRotation()
    {
        _playerBody.Rotate(Vector3.up * _mouseX);

        xRotation -= _mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        _playerHead.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
    /// <summary>
    /// 머리 위치 재조정해주는 함수
    /// </summary>
    private void SetHeadPosition()
    {
        _headPosition = _playerHead.position;
        _headPosition.y = _playerBody.position.y + crouchHeadOffset;
        _playerHead.position = _headPosition;

        _playerHead.localRotation = Quaternion.Euler(xRotation_head, 0, 0);
    }
}
