using UnityEngine;
using Cinemachine;

public class CLookBoard : MonoBehaviour
{
    #region 변수
    [SerializeField]
    private CinemachineVirtualCamera boardCinemachine; // 보드 카메라
    [SerializeField]
    private CinemachineVirtualCamera playerCinemachine; // 플레이어 카메라
    [SerializeField]
    private Transform playerTransform; // 플레이어 트랜스폼 기준
    [SerializeField]
    private float activationDistance; // 보드와 플레이어 상호작용 거리

    // 보드 카메라가 활성화 되어 있는지 여부
    private bool isInBoard = false;
    #endregion

    private void Start()
    {
        // 시작할 때 플레이어 시점 마우스 없애기
        Cursor.lockState = CursorLockMode.Locked;
        playerCinemachine.Priority = 0;
    }

    private void Update()
    {
        // 보드 카메라가 활성화 되어 있고, esc키를 누르면 플레이어 카메라로 돌아가기
        if (isInBoard && Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToPlayerCamera();
        }

        // space를 누르면 보드 카메라로 이동
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LookAtBoard();
        }
    }

    // OnMouseUpAsButton : 마우스 클릭한 것을 뗐을 때
    private void OnMouseUpAsButton()
    {
        // 보드에 가까이 있을 때 보드 카메라로 전환
        if (IsPlayerCloseEnough())
        {
            LookAtBoard();
        }
    }

    /// <summary>
    /// 플레이어와 보드 사이 거리 계산
    /// </summary>
    /// <returns></returns>
    private bool IsPlayerCloseEnough()
    {
        if (playerTransform == null)
        {
            Debug.LogError("playerTransform is Null");
            return false;
        }

        float distance = Vector3.Distance(playerTransform.position, transform.position);
        // 거리가 활성화 거리 이하일 경우 참
        return distance <= activationDistance;
    }

    /// <summary>
    /// 보드 카메라로 전환
    /// </summary>
    private void LookAtBoard()
    {
        // 보드 카메라가 비활성화면 활성화
        if (!boardCinemachine.gameObject.activeSelf)
        {
            boardCinemachine.gameObject.SetActive(true);
        }

        // 카메라 우선순위 변경 | 플레이어 -> 보드
        boardCinemachine.Priority = 10;
        playerCinemachine.Priority = 0;

        // 보드 카메라 활성화
        isInBoard = true;
        // 커서 화면 내 고정
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ReturnToPlayerCamera()
    {
        // 카메라 우선순위 변경 | 보드 -> 플레이어
        boardCinemachine.Priority = 0;
        playerCinemachine.Priority = 10;

        // 보드 카메라 비활성화
        isInBoard = false;
        // 커서 잠금 모드
        Cursor.lockState = CursorLockMode.Locked;
    }
}