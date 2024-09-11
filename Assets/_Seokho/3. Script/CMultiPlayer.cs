using System.Collections;
using UnityEngine;
using Infrastructure.Services;
using Photon.Pun;
using Cinemachine;

public class CMultiPlayer : MonoBehaviourPunCallbacks
{
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
    public float sprintMultiplier = 2f;

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

    private float smoothTime = 0.2f; // ī�޶� ��ġ ��ȯ�� ���� �ð�

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
        initialHeadPositionY = _playerHead.position.y; // �ʱ� �Ӹ� ��ġ ����
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

    public void Die()
    {
        isDead = true;

        if (animator != null)
        {
            animator.SetBool("Die", true);
            SoundManager.instance.GhostLaughSound();
        }

        Debug.Log("Player has died and controls are disabled.");

        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.CheckAllPlayersDead();
        }
    }

    private void CrouchHandle()
    {
        if (Input.GetKeyDown(KeyCode.C) && !isDead)
        {
            _crouch = !_crouch;

            if (animator != null)
            {
                animator.SetBool("IsCrouch", _crouch);
            }

            // �ɾ��� �� �ӵ� ���� �� �Ӹ� ��ġ ����
            if (_crouch)
            {
                moveSpeed = normalSpeed / 2f;  // ���� ���¿����� �̵� �ӵ��� �������� ����
                StartCoroutine(SmoothHeadPosition(initialHeadPositionY + crouchHeadOffset));  // �ɴ� ��ġ�� �ε巴�� �̵�
            }
            else
            {
                // �Ͼ ���� �ٽ� �� �ִ� �Ӹ� ��ġ�� ����
                moveSpeed = normalSpeed;  // �ٽ� �Ͼ�� �ӵ� ���󺹱�
                StartCoroutine(SmoothHeadPosition(initialHeadPositionY));  // �� �ִ� �Ӹ� ��ġ�� �ε巴�� ����
            }
        }
    }

    private IEnumerator SmoothHeadPosition(float targetY)
    {
        float startY = _playerHead.position.y; // ���� Y ��ġ
        float timeElapsed = 0f; // ��� �ð�

        while (timeElapsed < smoothTime)
        {
            float newY = Mathf.Lerp(startY, targetY, timeElapsed / smoothTime); // Lerp ���
            _playerHead.position = new Vector3(_playerHead.position.x, newY, _playerHead.position.z);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // ���������� ��Ȯ�ϰ� ��ǥ ��ġ�� �����ϰ� ����
        _playerHead.position = new Vector3(_playerHead.position.x, targetY, _playerHead.position.z);
    }

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
        _headPosition.y = _playerBody.position.y + crouchHeadOffset;
        _playerHead.position = _headPosition;

        _playerHead.localRotation = Quaternion.Euler(xRotation_head, 0, 0);
    }
}
