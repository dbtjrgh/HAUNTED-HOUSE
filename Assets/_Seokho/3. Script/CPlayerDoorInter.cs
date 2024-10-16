using GameFeatures;
using Photon.Pun;
using System.Collections;
using UnityEngine;

public class CPlayerDoorInter : MonoBehaviourPun
{
    #region ����
    public bool IsDoorFullyOpened = false;  // ���� ������ ���ȴ��� ����
    public bool IsDoorClosed = false;       // ���� ������ �������� ����

    public Transform InteractionTransform;  // ��ȣ�ۿ� ��ġ
    public GameObject handprintPrefab;      // �չ��ڱ� ������
    public float detectionRadius = 3.0f;    // �÷��̾� ���� ����
    private bool handprintCreated = false;  // ���ڱ��� �� ���� �����ǵ��� �ϴ� �÷���

    public float forceAmount = 15f;        // ���� ��
    public float distance = 1.5f;          // �Ÿ�

    private Rigidbody rb;
    Ghost ghost;

    private Camera cam;
    private bool isInteracting = false;    // ���� ��ȣ�ۿ� ������ ����
    private bool isDoorLocked = false;      // ���� ��� �ִ��� ����
    private HingeJoint hinge;               // ���� ������ HingeJoint
    private float openedRotation;           // ���� ������ ���� ������ ȸ��
    private const float epsilon = 1f;       // ȸ�� ������ ���ϱ� ���� ���� ����
    private WaitForSeconds waitForDoorStateCheck;  // �� ���� Ȯ���� ���� ��� �ð�
    private const float checkDoorStateCD = 0.3f;   // �� ���� Ȯ�� �ֱ�

    #endregion

    private void Awake()
    {
        cam = Camera.main;
        hinge = GetComponent<HingeJoint>();
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
            if (Input.GetMouseButtonDown(0) && !isInteracting && !isDoorLocked)  // ���� Ŭ������ ����
            {
                OnDragBegin();
            }
            if (Input.GetMouseButtonUp(0) && isInteracting)  // ���� Ŭ���� ������ ��ȣ�ۿ� ����
            {
                OnDragEnd();
            }
        }
        else if (isInteracting) // �÷��̾ �־����� �� ��ȣ�ۿ� ����
        {
            OnDragEnd(); // ��ȣ�ۿ� ���� ����
        }
    }

    private void FixedUpdate()
    {
        if (!handprintCreated)
        {
            DetectPlayerNearby();
        }
        if (isInteracting && !isDoorLocked)
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

    private IEnumerator CheckDoorState()
    {
        while (true)
        {
            CheckDoorRotation();
            yield return waitForDoorStateCheck;
        }
    }

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

    private void DragDoor()
    {
        Ray playerAim = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 nextPos = cam.transform.position + playerAim.direction * distance;
        Vector3 currPos = transform.position;

        rb.velocity = (nextPos - currPos) * forceAmount;
    }

    public void OnDragBegin()
    {
        isInteracting = true;
        rb.isKinematic = false;  // ��ȣ�ۿ� �߿��� ���� ����

        // �� ������ RPC�� ȣ��
        photonView.RPC("SyncDoorInteraction", RpcTarget.All, true);
    }

    public void OnDragEnd()
    {
        isInteracting = false;
        rb.isKinematic = true;  // ��ȣ�ۿ� ������ ���� ���� ����

        // �� ������ RPC�� ȣ��
        photonView.RPC("SyncDoorInteraction", RpcTarget.All, false);
    }

    [PunRPC]
    public void SyncDoorInteraction(bool interacting)
    {
        isInteracting = interacting;

        if (interacting)
        {
            rb.isKinematic = false;  // ��ȣ�ۿ� �߿��� ���� ����
        }
        else
        {
            rb.isKinematic = true;  // ��ȣ�ۿ� ������ ���� ���� ����
        }
    }

    public void LeavePrintsUV()
    {
        Instantiate(handprintPrefab, InteractionTransform.position, InteractionTransform.rotation, InteractionTransform);
    }

    private bool DetectPlayerNearby()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);

        foreach (Collider collider in colliders)
        {
            if (ghost == null)
            {
                return false;
            }

            if (collider.CompareTag("Player"))
            {
                return true;  // �÷��̾ ��ó�� ����
            }

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
