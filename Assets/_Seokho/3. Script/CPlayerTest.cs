using Photon.Pun;
using System.Linq;
using UnityEngine;

public class CPlayerTest : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private CharacterController charController;

    // 카메라 회전 속도
    [SerializeField]
    private float turnSpeed = 4f;

    // 카메라 상하 회전
    [SerializeField]
    private float _mouseX, _mouseY;
    private float xRotation = 0f;
    private float xRotation_head = 0f;

    // 이동 속도
    [SerializeField]
    private float moveSpeed = 4f;

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
        FollowHead();  // 머리 위치 업데이트 호출
        PlayerRotation(); // 플레이어 회전
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