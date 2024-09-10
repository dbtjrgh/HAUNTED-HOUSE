using UnityEngine;
using Cinemachine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class CLookBoard : MonoBehaviourPunCallbacks
{
    #region ����
    [SerializeField]
    public CinemachineVirtualCamera boardCinemachine; // ���� ī�޶�
    public CinemachineVirtualCamera playerCinemachine; // �÷��̾� ī�޶�
    public Transform playerTransform; // �÷��̾� Ʈ������ ����
    [SerializeField]
    private float activationDistance; // ����� �÷��̾� ��ȣ�ۿ� �Ÿ�

    // ���� ī�޶� Ȱ��ȭ �Ǿ� �ִ��� ����
    private bool isInBoard = false;
    #endregion

    private void Awake()
    {
        // ���� ������ �ڽ��� PlayerPrefab�� ã�Ƽ� �Ҵ�
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            AssignLocalPlayerReferences();
        }
    }

    private void Start()
    {
        // Start���� �߰����� �ʱ�ȭ�� �ʿ��� ��� ó��
        // ��: �⺻ ī�޶� ���� ��
    }

    private void Update()
    {
        // ���� ī�޶� Ȱ��ȭ �Ǿ� �ְ�, escŰ�� ������ �÷��̾� ī�޶�� ���ư���
        if (isInBoard && Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToPlayerCamera();
        }

        // space�� ������ ���� ī�޶�� �̵�
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LookAtBoard();
        }
    }

    // OnMouseUpAsButton : ���콺 Ŭ���� ���� ���� ��
    private void OnMouseUpAsButton()
    {
        // ���忡 ������ ���� �� ���� ī�޶�� ��ȯ
        if (IsPlayerCloseEnough())
        {
            LookAtBoard();
        }
    }

    /// <summary>
    /// �÷��̾�� ���� ���� �Ÿ� ���
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
        // �Ÿ��� Ȱ��ȭ �Ÿ� ������ ��� ��
        return distance <= activationDistance;
    }

    /// <summary>
    /// ���� ī�޶�� ��ȯ
    /// </summary>
    private void LookAtBoard()
    {
        if (playerCinemachine == null)
        {
            Debug.LogError("playerCinemachine is not assigned.");
            return;
        }

        // ���� ī�޶� ��Ȱ��ȭ�� Ȱ��ȭ
        if (!boardCinemachine.gameObject.activeSelf)
        {
            boardCinemachine.gameObject.SetActive(true);
        }

        // ī�޶� �켱���� ���� | �÷��̾� -> ����
        boardCinemachine.Priority = 10;
        playerCinemachine.Priority = 0;

        // ���� ī�޶� Ȱ��ȭ
        isInBoard = true;
        // Ŀ�� ȭ�� �� ����
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ReturnToPlayerCamera()
    {
        if (playerCinemachine == null)
        {
            Debug.Log("playerCinemachine is not assigned.");
            return;
        }

        // ī�޶� �켱���� ���� | ���� -> �÷��̾�
        boardCinemachine.Priority = 0;
        playerCinemachine.Priority = 10;

        // ���� ī�޶� ��Ȱ��ȭ
        isInBoard = false;
        // Ŀ�� ��� ���
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// PlayerPrefab�� ������ �� ȣ���Ͽ� playerCinemachine�� playerTransform�� �����ϴ� �޼���
    /// </summary>
    /// <param name="playerCam">Player�� Cinemachine ī�޶�</param>
    /// <param name="playerTrans">Player�� Transform</param>
    public void SetPlayerReferences(CinemachineVirtualCamera playerCam, Transform playerTrans)
    {
        playerCinemachine = playerCam;
        playerTransform = playerTrans;

        InitializePlayerCamera();
    }

    /// <summary>
    /// �÷��̾� ī�޶� �ʱ�ȭ
    /// </summary>
    private void InitializePlayerCamera()
    {
        if (playerCinemachine != null)
        {
            playerCinemachine.Priority = 10; // �⺻������ �÷��̾� ī�޶� �켱������ ����
        }

        // ������ �� �÷��̾� ���� ���콺 ���
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// ���� �÷��̾��� ī�޶�� Ʈ�������� �ڵ����� �Ҵ��ϴ� �޼���
    /// </summary>
    private void AssignLocalPlayerReferences()
    {
        // ���� �÷��̾��� CinemachineVirtualCamera ã��
        playerCinemachine = FindObjectOfType<CinemachineVirtualCamera>();

        // CinemachineVirtualCamera�� ���������� ã�Ҵ��� Ȯ��
        if (playerCinemachine == null)
        {
            Debug.LogError("���� �÷��̾ ���� CinemachineVirtualCamera�� ã�� ���߽��ϴ�.");
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