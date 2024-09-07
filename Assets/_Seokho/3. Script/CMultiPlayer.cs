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

    // ī�޶� ȸ�� �ӵ�
    [SerializeField]
    private float turnSpeed = 4f;

    // ī�޶� ���� ȸ��   
    private float _mouseX, _mouseY;
    private float xRotation = 0f;
    private float xRotation_head = 0f;

    // �̵� �ӵ�
    [SerializeField]
    private float normalSpeed = 2f;

    private float moveSpeed;
    private float sprintMultiplier = 2f;

    //�ɱ� ��� �Լ�
    private bool _crouch = false;
    private Wonbin.CrouchAnimation _animControl;
    private Animator animator;

    // �߷�
    [SerializeField]
    private float gravity = 5f;  // �߷� ��
    private float verticalVelocity = 0f; // ���� �ӵ�
    private bool isGrounded; // �÷��̾ ���鿡 �ִ��� ����

    // ���� ���� üũ ����
    private bool isDead = false;  // �÷��̾ �׾����� Ȯ���ϴ� ����

    // ���� ���� ���� ����
    [SerializeField]
    private Transform _playerBody;   // �� Transform (��ü�� ��ü ���� ���Ե� Transform)

    // �Ӹ� ���� ���� ����
    [SerializeField]
    private Transform _playerHead;   // �Ӹ� Transform
    private Vector3 _headPosition;
    private float _currFollowHeadTime = 0f;
    private float _playerHeadOffset = 0f;
    private const float FollowHeadTime = 2f;
    private float initialHeadPositionY; // ������ ���� �Ӹ��� Y��ǥ�� ����.
    public float crouchHeadOffset = -1f; //�������� �Ӹ��� Y��ǥ�� ����.

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
        initialHeadPositionY = _playerHead.position.y; // �ʱ� �Ӹ� ��ġ ����
    }

    private void Update()
    {
        if (pv.IsMine && !isDead)  // isDead�� false�� ���� ���� ���
        {
            moveInput();
            Sprint();
            FollowHead();  // �Ӹ� ��ġ ������Ʈ ȣ��
            PlayerRotation(); // �÷��̾� ȸ��
            CrouchHandle();

            // �߷� �� ���� üũ
            isGrounded = charController.isGrounded;

            if (isGrounded && verticalVelocity < 0)
            {
                verticalVelocity = -2f;  // ���鿡 ���� �� ���� �ӵ��� ����
            }

            ApplyGravity();  // �߷� ��� �� ����

            // ���� �̵� ���
            Vector3 move = GetMoveDirection();

            // **���� �̵��� �ݿ��ϰ�, �߷��� ���� ����**
            move.y = verticalVelocity;

            // �̵� �ݿ�
            charController.Move(move * Time.deltaTime);
        }
    }

    // �߷� ���� �Լ�
    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            verticalVelocity -= gravity * Time.deltaTime;  // �߷¿� ���� ���� �ӵ� ����
        }
    }

    private Vector3 GetMoveDirection()
    {
        // ���콺�� �̵���
        float y_RotateSize = Input.GetAxis("Mouse X") * turnSpeed;
        float y_Rotate = _playerBody.eulerAngles.y + y_RotateSize;

        // ���콺 ���� �̵��� ���� �̵��� ���
        float x_RotationSize = -Input.GetAxis("Mouse Y") * turnSpeed;
        xRotation = Mathf.Clamp(xRotation + x_RotationSize, -80f, 80f);

        _playerBody.eulerAngles = new Vector3(_playerBody.eulerAngles.x, y_Rotate, 0);

        // ���� �̵��� ����
        Vector3 move = _playerBody.forward * Input.GetAxis("Vertical") + _playerBody.right * Input.GetAxis("Horizontal");

        // **���� �̵����� ������ ó���Ͽ�, �߷� ������ ���� �ʵ��� ��**
        Vector3 horizontalMove = new Vector3(move.x, 0, move.z);
        horizontalMove *= moveSpeed;  // ������Ʈ ���� �ݿ��� �̵� �ӵ�

        return horizontalMove;  // ���� �̵��� ��ȯ
    }

    // ���� ó�� �Լ�
    public void Die()
    {
        isDead = true;  // �÷��̾� ��� ���·� ����

        if (animator != null)
        {
            animator.SetTrigger("Die");  // Die �ִϸ��̼� Ʈ����
        }

        Debug.Log("Player has died and controls are disabled.");
    }

    private void CrouchHandle()
    {
        if (Input.GetKeyDown(KeyCode.C) && !isDead)  // �÷��̾ ���� ���� ��쿡�� crouch ���
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
        if (Input.GetKey(KeyCode.LeftShift) && !isDead)  // ���� ���� ��쿡�� ������Ʈ ���
        {
            moveSpeed = normalSpeed * sprintMultiplier;  // ������Ʈ ��
        }
        else
        {
            moveSpeed = normalSpeed;  // �⺻ �ӵ�
        }
    }

    private void moveInput()
    {
        // ���콺�� �̵���
        float y_RotateSize = Input.GetAxis("Mouse X") * turnSpeed;
        float y_Rotate = _playerBody.eulerAngles.y + y_RotateSize;

        // ���콺 ���� �̵��� ���� �̵��� ���
        float x_RotationSize = -Input.GetAxis("Mouse Y") * turnSpeed;
        xRotation = Mathf.Clamp(xRotation + x_RotationSize, -80f, 80f);

        _playerBody.eulerAngles = new Vector3(_playerBody.eulerAngles.x, y_Rotate, 0);

        // �̵��� ����
        Vector3 move = _playerBody.forward * Input.GetAxis("Vertical") + _playerBody.right * Input.GetAxis("Horizontal");

        // �̵����� ���� ��ǥ�� �ݿ�
        charController.Move(move * moveSpeed * Time.deltaTime);

        // �ִϸ��̼� ���� ������Ʈ
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
