using Infrastructure;
using UnityEngine;
using System;
using System.Collections;
using Infrastructure.Services;
using Photon.Pun;
using Cinemachine;


public class CSinglePlayer : MonoBehaviour
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
    private float gravity = 3f;

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


    private CinemachineVirtualCamera playerCinemachine;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        // ���� �÷��̾��� ���
        // _playerHead�� �ڽ� ������Ʈ���� CinemachineVirtualCamera ������Ʈ�� ã��
        playerCinemachine = _playerHead.GetComponentInChildren<CinemachineVirtualCamera>();
        if (playerCinemachine == null)
        {
            Debug.LogError("CinemachineVirtualCamera not found in _playerHead.");
        }

        // CLookBoard ��ũ��Ʈ�� ã��
        CLookBoard lookBoard = FindObjectOfType<CLookBoard>();
        if (lookBoard != null)
        {
            // Player�� ī�޶�� Transform�� CLookBoard�� ����
            lookBoard.SetPlayerReferences(playerCinemachine, transform);
        }
        else
        {
            Debug.LogError("CLookBoard script not found in the scene.");
        }
    }


    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        initialHeadPositionY = _playerHead.position.y; // �ʱ� �Ӹ� ��ġ ����
    }


    private void Update()
    {
        moveInput();
        Sprint();
        FollowHead();  // �Ӹ� ��ġ ������Ʈ ȣ��
        PlayerRotation(); // �÷��̾� ȸ��
        CrouchHandle();
    }



    private void CrouchHandle()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("�ɱ� Ű�� �������ϴ�.");

            // _crouch ���
            _crouch = !_crouch;

            // �ִϸ������� ���� ����
            if (animator != null)
            {
                animator.SetBool("IsCrouch", _crouch);
            }

            if (_crouch)
            {
                // �ɴ� ���� ó��
                if (_animControl != null && !_animControl.SitDown())
                {
                    return;
                }

                // �Ӹ� ��ġ�� ���� ���·� ����
                AdjustHeadPosition(initialHeadPositionY + crouchHeadOffset);
            }
            else
            {
                // �Ͼ�� ���� ó��
                if (_animControl != null && !_animControl.StandUp())
                {
                    return;
                }

                // �Ӹ� ��ġ�� ���� ��ġ�� �ʱ�ȭ
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
        // �⺻ �̵� �ӵ��� ������Ʈ ��� ����
        if (Input.GetKey(KeyCode.LeftShift))
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
            // �÷��̾ �����̰� ������ isWalking�� true�� ����
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
        // �Ӹ� ��ġ�� ȸ�� ������Ʈ
        _headPosition = _playerHead.position;
        _headPosition.y = _playerBody.position.y + _playerHeadOffset; // �Ӹ� ��ġ ����
        _playerHead.position = _headPosition;

        // �Ӹ� ȸ�� ����
        _playerHead.localRotation = Quaternion.Euler(xRotation_head, 0, 0); // ���� ȸ��
    }
}