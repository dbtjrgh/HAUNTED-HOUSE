using Photon.Pun;
using System.Linq;
using UnityEngine;

public class CPlayerTest : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private CharacterController charController;

    // ī�޶� ȸ�� �ӵ�
    [SerializeField]
    private float turnSpeed = 4f;

    // ī�޶� ���� ȸ��
    [SerializeField]
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

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
        if (_playerBody == null)
        {
            _playerBody = transform.Find("PlayerBody");
        }
        if(_playerHead == null)
        {
            _playerHead = FindChildByName("PlayerHead");
        }
    }

    private Transform FindChildByName(string name)
    {
        return transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == name);
    }

    private void Update()
    {
        moveInput();
        FollowHead();  // �Ӹ� ��ġ ������Ʈ ȣ��
        PlayerRotation(); // �÷��̾� ȸ��
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

        // �̵����� ���� ��ǥ�� �ݿ�(�̵��� �� ����Ҷ�, charController�� �����ؾ���. �׷��� ������ �� ����.)
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