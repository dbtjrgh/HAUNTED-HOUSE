using UnityEngine;
using Cinemachine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class CLookBoard : MonoBehaviourPunCallbacks
{
    #region 변수
    [SerializeField]
    public CinemachineVirtualCamera boardCinemachine; // 보드 카메라
    public CinemachineVirtualCamera playerCinemachine; // 플레이어 카메라
    public Transform playerTransform; // 플레이어 트랜스폼 기준
    [SerializeField]
    private float activationDistance; // 보드와 플레이어 상호작용 거리

    // 보드 카메라가 활성화 되어 있는지 여부
    private bool isInBoard = false;
    #endregion

    private void Awake()
    {
        // 포톤 내에서 자신의 PlayerPrefab을 찾아서 할당
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            AssignLocalPlayerReferences();
        }
    }

    private void Start()
    {
        // Start에서 추가적인 초기화가 필요한 경우 처리
        // 예: 기본 카메라 설정 등
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
        if (playerCinemachine == null)
        {
            Debug.LogError("playerCinemachine is not assigned.");
            return;
        }

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
        if (playerCinemachine == null)
        {
            Debug.Log("playerCinemachine is not assigned.");
            return;
        }

        // 카메라 우선순위 변경 | 보드 -> 플레이어
        boardCinemachine.Priority = 0;
        playerCinemachine.Priority = 10;

        // 보드 카메라 비활성화
        isInBoard = false;
        // 커서 잠금 모드
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// PlayerPrefab이 생성될 때 호출하여 playerCinemachine과 playerTransform을 설정하는 메서드
    /// </summary>
    /// <param name="playerCam">Player의 Cinemachine 카메라</param>
    /// <param name="playerTrans">Player의 Transform</param>
    public void SetPlayerReferences(CinemachineVirtualCamera playerCam, Transform playerTrans)
    {
        playerCinemachine = playerCam;
        playerTransform = playerTrans;

        InitializePlayerCamera();
    }

    /// <summary>
    /// 플레이어 카메라 초기화
    /// </summary>
    private void InitializePlayerCamera()
    {
        if (playerCinemachine != null)
        {
            playerCinemachine.Priority = 10; // 기본적으로 플레이어 카메라가 우선순위가 높음
        }

        // 시작할 때 플레이어 시점 마우스 잠금
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// 로컬 플레이어의 카메라와 트랜스폼을 자동으로 할당하는 메서드
    /// </summary>
    private void AssignLocalPlayerReferences()
    {
        // 로컬 플레이어의 CinemachineVirtualCamera 찾기
        playerCinemachine = FindObjectOfType<CinemachineVirtualCamera>();

        // CinemachineVirtualCamera를 성공적으로 찾았는지 확인
        if (playerCinemachine == null)
        {
            Debug.LogError("로컬 플레이어를 위한 CinemachineVirtualCamera를 찾지 못했습니다.");
            return;
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MultiLobby") 
        {
            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            {
                AssignLocalPlayerReferences();
            }
        }
    }
}