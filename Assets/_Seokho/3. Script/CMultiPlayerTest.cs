using Cinemachine;
using Photon.Pun;
using System.Linq;
using UnityEngine;

public class CMultiPlayerTest : MonoBehaviourPunCallbacks
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
    private float moveSpeed = 4f;

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

    private PhotonView pv;
    private CinemachineVirtualCamera playerCinemachine;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        charController = GetComponent<CharacterController>();
        if (_playerBody == null)
        {
            _playerBody = transform.Find("PlayerBody");
            if (_playerBody == null)
            {
                Debug.LogError("PlayerBody not found in PlayerPrefab.");
            }
        }
        if (_playerHead == null)
        {
            _playerHead = FindChildByName("PlayerHead");
            if (_playerHead == null)
            {
                Debug.LogError("PlayerHead not found in PlayerPrefab.");
            }
        }

        pv = GetComponent<PhotonView>();

        if (pv.IsMine)
        {
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
        else
        {
            // ��Ʈ��ũ ���� �ٸ� �÷��̾��� ���
            // �÷��̾� ī�޶� ��Ȱ��ȭ
            playerCinemachine = _playerHead.GetComponentInChildren<CinemachineVirtualCamera>();
            if (playerCinemachine != null)
            {
                playerCinemachine.gameObject.SetActive(false);
            }
        }
    }

    private void Start()
    {
        // �߰����� �ʱ�ȭ�� �ʿ��� ��� ���⼭ ����
    }

    private Transform FindChildByName(string name)
    {
        return transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == name);
    }

    private void Update()
    {
        if (pv.IsMine)
        {
            moveInput();
            FollowHead();  // �Ӹ� ��ġ ������Ʈ ȣ��
            PlayerRotation(); // �÷��̾� ȸ��
        }
    }

    private void moveInput()
    {
        // ���콺�� �̵���
        float y_RotateSize = Input.GetAxis("Mouse X") * turnSpeed;
        // ���� ȸ������ ���ο� ȸ������ ������
        float y_Rotate = _playerBody.eulerAngles.y + y_RotateSize;

        // ���콺 ���� �̵��� ���� �̵��� ���
        float x_RotationSize = -Input.GetAxis("Mouse Y") * turnSpeed;
        xRotation = Mathf.Clamp(xRotation + x_RotationSize, -80f, 80f);  // ���� ȸ�� ���� ����

        // ���� ȸ������ ���� �ݿ�(y�� ȸ��)
        _playerBody.eulerAngles = new Vector3(_playerBody.eulerAngles.x, y_Rotate, 0);

        // �̵��� ����
        Vector3 move = _playerBody.forward * Input.GetAxis("Vertical") + _playerBody.right * Input.GetAxis("Horizontal");

        // �̵����� ���� ��ǥ�� �ݿ�
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
        // �Ӹ� ��ġ�� ȸ�� ������Ʈ
        _headPosition = _playerHead.position;
        _headPosition.y = _playerBody.position.y + _playerHeadOffset; // �Ӹ� ��ġ ����
        _playerHead.position = _headPosition;

        // �Ӹ� ȸ�� ����
        _playerHead.localRotation = Quaternion.Euler(xRotation_head, 0, 0); // ���� ȸ��
    }
}
