using Photon.Pun;
using UnityEngine;

public class CPlayerTest : MonoBehaviourPunCallbacks
{
    [SerializeField] private float turnSpeed = 4f;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float gravity = 3f;
    [SerializeField] private Transform _playerBody;
    [SerializeField] private Transform _playerHead;
    [SerializeField] private Vector3 _headPosition;

    private float xRotation = 0f;
    private float xRotation_head = 0f;
    private float _currFollowHeadTime = 0f;
    private float _playerHeadOffset = 0f;
    [SerializeField] 
    private CharacterController characterController;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            FollowHead();
            PlayerRotation();
            moveInput();
        }
    }

    private void moveInput()
    {
        if (!photonView.IsMine) return;

        float y_RotateSize = Input.GetAxis("Mouse X") * turnSpeed;
        float y_Rotate = _playerBody.eulerAngles.y + y_RotateSize;
        float x_RotationSize = -Input.GetAxis("Mouse Y") * turnSpeed;
        xRotation = Mathf.Clamp(xRotation + x_RotationSize, -80f, 80f);
        _playerBody.eulerAngles = new Vector3(_playerBody.eulerAngles.x, y_Rotate, 0);

        Vector3 move = _playerBody.forward * Input.GetAxis("Vertical") + _playerBody.right * Input.GetAxis("Horizontal");
        Vector3 movement = move * moveSpeed * Time.deltaTime;

        // CharacterController를 사용한 이동 적용
        if (characterController != null)
        {
            Vector3 gravityMove = Vector3.up * -gravity;
            characterController.Move(movement + gravityMove);
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
        _playerBody.Rotate(Vector3.up * Input.GetAxis("Mouse X") * turnSpeed);
        xRotation -= Input.GetAxis("Mouse Y") * turnSpeed;
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