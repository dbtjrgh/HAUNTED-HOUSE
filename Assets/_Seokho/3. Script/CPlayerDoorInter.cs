using GameFeatures;
using System.Collections;
using UnityEngine;

public class CPlayerDoorInter : MonoBehaviour
{
    #region ����
    public bool IsDoorFullyOpened = false;  // ���� ������ ���ȴ��� ����
    public bool IsDoorClosed = false;       // ���� ������ �������� ����

    public Transform InteractionTransform;  // ��ȣ�ۿ� ��ġ
    public GameObject handprintPrefab;      // �չ��ڱ� ������
    public float detectionRadius = 3.0f;    // �÷��̾� ���� ����
    private bool handprintCreated = false;  // ���ڱ��� �� ���� �����ǵ��� �ϴ� �÷���
    private Collider[] colliders;           // ������ �浹ü��

    public float forceAmmount = 15f;        // ���� ��
    public float distance = 1.5f;           // �Ÿ�
    public float lockHoldTime = 1.0f;       // ���/������ ���� TŰ Ȧ�� �ð�

    private Collider col;
    private Rigidbody rb;
    Ghost ghost;

    private Camera cam;
    private bool isInterracting = false;    // ���� ��ȣ�ۿ� ������ ����
    private bool isDoorLocked = false;      // ���� ��� �ִ��� ����
    private HingeJoint hinge;               // ���� ������ HingeJoint
    private float openedRotation;           // ���� ������ ���� ������ ȸ��
    private const float epsilon = 1f;       // ȸ�� ������ ���ϱ� ���� ���� ����
    private WaitForSeconds waitForDoorStateCheck;  // �� ���� Ȯ���� ���� ��� �ð�
    private const float checkDoorStateCD = 0.3f;   // �� ���� Ȯ�� �ֱ�

    private float tKeyHoldTime = 0f;        // T Ű ���� �ð�

    #endregion
    private void Awake()
    {
        cam = Camera.main;
        hinge = GetComponent<HingeJoint>();
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        ghost = FindAnyObjectByType<Ghost>();

        if (hinge == null)
        {
            Debug.Log("HingeJoint�� �����Ƿ� ��ȣ�ۿ��� ��Ȱ��ȭ�մϴ�.");
            enabled = false;
            return;
        }

        waitForDoorStateCheck = new WaitForSeconds(checkDoorStateCD);
        openedRotation = hinge.limits.max * hinge.axis.y;
        if (openedRotation < -1f) openedRotation += 360f;

        StartCoroutine(CheckDoorState());
    }


    private void Update()
    {
        // �÷��̾ �ֺ��� �ִ��� ����
        bool isPlayerNearby = DetectPlayerNearby();

        // �÷��̾ ���� ���� �� ���� ����
        if (isPlayerNearby)
        {
            // ���� ���콺 ��ư���� ��ȣ�ۿ� ����/����
            if (Input.GetMouseButtonDown(0) && !isInterracting && !isDoorLocked)  // ���� Ŭ������ ����
            {
                OnDragBegin();
            }
            if (Input.GetMouseButtonUp(0) && isInterracting)  // ���� Ŭ���� ������ ��ȣ�ۿ� ����
            {
                OnDragEnd();
            }

            // T Ű�� ������ �ִ� �ð� ���� (���/����)
            if (Input.GetKey(KeyCode.T))
            {
                tKeyHoldTime += Time.deltaTime;

                if (tKeyHoldTime >= lockHoldTime)  // TŰ�� ���� �ð� �̻� ������ ��
                {
                    if (isDoorLocked)
                    {
                        UnlockTheDoor();
                        Debug.Log("Door Unlocked");
                    }
                    else
                    {
                        LockTheDoor();
                        Debug.Log("Door Locked");
                    }

                    tKeyHoldTime = 0f;  // �ð� �ʱ�ȭ
                }
            }

            // TŰ���� ���� ���� ��, Ÿ�̸� �ʱ�ȭ
            if (Input.GetKeyUp(KeyCode.T))
            {
                tKeyHoldTime = 0f;
            }
        }
        else if (isInterracting) // �÷��̾ �־����� �� ��ȣ�ۿ� ����
        {
            OnDragEnd(); // ��ȣ�ۿ� ���� ����
        }
        // �� ���� ���ڱ� ������ �ǵ��� ����
       
    }

    private void FixedUpdate()
    { 
        if (!handprintCreated)
        {
            DetectPlayerNearby();
        }
        if (isInterracting && !isDoorLocked)
        {
            // ��ȣ�ۿ� �߿��� �÷��̾ ��� ��ó�� �ִ��� Ȯ��
            if (DetectPlayerNearby())
            {
                DragDoor();  // ���� �巡��
            }
            else
            {
                OnDragEnd(); // �÷��̾ �־����� ��ȣ�ۿ� ����
            }
        }
    }
    /// <summary>
    /// �� ���¸� �ֱ������� Ȯ���ϴ� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckDoorState()
    {
        while (true)
        {
            CheckDoorRotation();
            yield return waitForDoorStateCheck;
        }
    }
    /// <summary>
    /// ���� ���ȴ���, ���������� Ȯ���ϴ� �Լ�
    /// �� ȸ�� ������ ���� ���¸� ����
    /// </summary>
    private void CheckDoorRotation()
    {
        float doorCurrRotation = transform.localEulerAngles.y;
        if (doorCurrRotation < -5f)
        {
            doorCurrRotation += 360;
        }

            if (Mathf.Abs(doorCurrRotation) <= epsilon)
        {
            IsDoorClosed = true;
            IsDoorFullyOpened = false;
        }
        else if (Mathf.Abs(doorCurrRotation - openedRotation) <= epsilon)
        {
            IsDoorClosed = false;
            IsDoorFullyOpened = true;
        }
        else
        {
            IsDoorClosed = false;
            IsDoorFullyOpened = false;
        }
    }
    /// <summary>
    /// �÷��̾ ���� �巡���� �� ���� �����̴� ������ ������ ó���ϴ� �Լ�
    /// </summary>
    private void DragDoor()
    {
        Ray playerAim = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 nextPos = cam.transform.position + playerAim.direction * distance;
        Vector3 currPos = transform.position;

        rb.velocity = (nextPos - currPos) * forceAmmount;
    }
    /// <summary>
    /// ��ȣ�ۿ��� ������ �� ȣ��Ǵ� �Լ���, ������ ��ȣ�ۿ��� Ȱ��ȭ
    /// </summary>
    public void OnDragBegin()
    {
        isInterracting = true;
        rb.isKinematic = false;  // ��ȣ�ۿ� �߿��� ���� ����
    }
    /// <summary>
    /// ��ȣ�ۿ��� ���� �� ȣ��Ǵ� �Լ���, ������ ��ȣ�ۿ��� ��Ȱ��ȭ
    /// </summary>
    public void OnDragEnd()
    {
        isInterracting = false;
        rb.isKinematic = true;  // ��ȣ�ۿ� ������ ���� ���� ����
    }
    /// <summary>
    /// ���� ��״� �Լ���, �ݶ��̴��� ��Ȱ��ȭ�Ͽ� ���� �������� ���ϰ� ��
    /// </summary>
    public void LockTheDoor()
    {
        col.enabled = false;
        isDoorLocked = true;
    }
    /// <summary>
    /// ���� ��� �����ϴ� �Լ���, �ݶ��̴��� Ȱ��ȭ�Ͽ� ���� �����̰� ��
    /// </summary>
    public void UnlockTheDoor()
    {
        rb.isKinematic = false;
        isDoorLocked = false;
        col.enabled = true;
    }
    /// <summary>
    /// uv ���ڱ��� ���� �����ϴ� �Լ�
    /// ���ڱ� �������� ���� ����
    /// </summary>
    public void LeavePrintsUV()
    {
        Instantiate(handprintPrefab, InteractionTransform.position, InteractionTransform.rotation, InteractionTransform);
    }

    /// <summary>
    /// �÷��̾ �ֺ��� �ִ��� �����ϴ� �Լ�
    /// </summary>
    /// <returns></returns>
    private bool DetectPlayerNearby()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);

        foreach (Collider collider in colliders)
        {
            // Ghost ��ü�� null���� ���� Ȯ��
            if (ghost == null)
            {
                return false;
            }

            // Player �±׸� ���� ������Ʈ�� �ִ��� Ȯ��
            if (collider.CompareTag("Player"))
            {
                return true;  // �÷��̾ ��ó�� ����
            }

            // Room ������Ʈ�� �ִ��� Ȯ��
            Room room = collider.GetComponent<Room>();
            if (!handprintCreated && collider.CompareTag("Room") && room != null)
            {
                if (room.RoomType == myRooms.Rooms.RoomsEnum.GhostRoom && ghost.ghostType != 0)
                {
                    handprintCreated = true;  // ���ڱ��� �����Ǿ����� ǥ��
                    Debug.Log("���ڱ� ������ ����");
                    LeavePrintsUV();  // ���ڱ� ������ ����
                }
            }
        }

        return false;  // �÷��̾ ��ó�� ����
    }


    /// <summary>
    /// ���� ������ �ð������� ������ϱ� ���� �Լ�
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // ���� ������ ��������� �ð�ȭ
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
