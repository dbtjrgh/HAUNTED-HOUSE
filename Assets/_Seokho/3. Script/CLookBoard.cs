using UnityEngine;
using Cinemachine;

public class CLookBoard : MonoBehaviour
{
    #region ����
    [SerializeField]
    private CinemachineVirtualCamera boardCinemachine; // ���� ī�޶�
    [SerializeField]
    private CinemachineVirtualCamera playerCinemachine; // �÷��̾� ī�޶�
    [SerializeField]
    private Transform playerTransform; // �÷��̾� Ʈ������ ����
    [SerializeField]
    private float activationDistance; // ����� �÷��̾� ��ȣ�ۿ� �Ÿ�

    // ���� ī�޶� Ȱ��ȭ �Ǿ� �ִ��� ����
    private bool isInBoard = false;
    #endregion

    private void Start()
    {
        // ������ �� �÷��̾� ���� ���콺 ���ֱ�
        Cursor.lockState = CursorLockMode.Locked;
        playerCinemachine.Priority = 0;
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
        // ī�޶� �켱���� ���� | ���� -> �÷��̾�
        boardCinemachine.Priority = 0;
        playerCinemachine.Priority = 10;

        // ���� ī�޶� ��Ȱ��ȭ
        isInBoard = false;
        // Ŀ�� ��� ���
        Cursor.lockState = CursorLockMode.Locked;
    }
}