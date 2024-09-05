using Cinemachine;
using Photon.Pun;
using System.Linq;
using UnityEngine;

public class CMultiPlayerTest : MonoBehaviourPunCallbacks
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
            // 로컬 플레이어의 경우
            // _playerHead의 자식 오브젝트에서 CinemachineVirtualCamera 컴포넌트를 찾음
            playerCinemachine = _playerHead.GetComponentInChildren<CinemachineVirtualCamera>();
            if (playerCinemachine == null)
            {
                Debug.LogError("CinemachineVirtualCamera not found in _playerHead.");
            }

            // CLookBoard 스크립트를 찾음
            CLookBoard lookBoard = FindObjectOfType<CLookBoard>();
            if (lookBoard != null)
            {
                // Player의 카메라와 Transform을 CLookBoard에 설정
                lookBoard.SetPlayerReferences(playerCinemachine, transform);
            }
            else
            {
                Debug.LogError("CLookBoard script not found in the scene.");
            }
        }
        else
        {
            // 네트워크 상의 다른 플레이어의 경우
            // 플레이어 카메라 비활성화
            playerCinemachine = _playerHead.GetComponentInChildren<CinemachineVirtualCamera>();
            if (playerCinemachine != null)
            {
                playerCinemachine.gameObject.SetActive(false);
            }
        }
    }

    private void Start()
    {
        // 추가적인 초기화가 필요한 경우 여기서 수행
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
            FollowHead();// 머리 위치 업데이트 호출
            PlayerRotation(); // 플레이어 회전

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

        // 이동량을 현재 좌표에 반영
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
