using GameFeatures;
using Photon.Pun;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PhotonView), typeof(PhotonTransformView))]
public class CPlayerDoorInter : MonoBehaviourPun
{
    #region ����

    public Transform InteractionTransform;  // ��ȣ�ۿ� ��ġ
    public GameObject handprintPrefab;      // �չ��ڱ� ������
    public float detectionRadius = 3.0f;    // �÷��̾� ���� ����
    private bool handprintCreated = false;  // ���ڱ��� �� ���� �����ǵ��� �ϴ� �÷���

    public float forceAmount = 15f;        // ���� ��
    public float distance = 1.5f;          // �Ÿ�

    private Rigidbody rb;
    Ghost ghost;

    private Camera cam;
    public bool isInteracting;    // ���� ��ȣ�ۿ� ������ ����
    private bool isDoorOpen = false;  // �� ���� ���� �߰�

    private HingeJoint hinge;               // ���� ������ HingeJoint
    private float openedRotation;           // ���� ������ ���� ������ ȸ��
    private const float epsilon = 1f;       // ȸ�� ������ ���ϱ� ���� ���� ����

    #endregion

    private void Awake()
    {
        cam = Camera.main;
        hinge = GetComponent<HingeJoint>();
        rb = GetComponent<Rigidbody>();
        ghost = FindAnyObjectByType<Ghost>();

        isInteracting = false;

        if (hinge == null)
        {
            Debug.Log("HingeJoint�� �����Ƿ� ��ȣ�ۿ��� ��Ȱ��ȭ�մϴ�.");
            enabled = false;
            return;
        }

        openedRotation = hinge.limits.max * hinge.axis.y;
        if (openedRotation < -1f) openedRotation += 360f;
    }

    private void Update()
    {
        if (cam == null || hinge == null)
        {
            return;
        }

        bool isPlayerNearby = DetectPlayerNearby();

        // ��� Ŭ���̾�Ʈ�� �Է��� ������ �� �ֵ���
        if (isPlayerNearby)
        {
            // ������ ���ο� ������� ��� Ŭ���̾�Ʈ�� ��ȣ�ۿ��� �õ�
            if (Input.GetMouseButtonDown(0) && !isInteracting)
            {
                // ��ȣ�ۿ� ���� ��û�� ��� Ŭ���̾�Ʈ�� ����
                photonView.RPC("RPC_RequestDragBegin", RpcTarget.All);
            }
            if (Input.GetMouseButtonUp(0) && isInteracting)
            {
                photonView.RPC("RPC_EndDrag", RpcTarget.All);
            }
        }
        else if (isInteracting)
        {
            photonView.RPC("RPC_EndDrag", RpcTarget.All);  // �÷��̾ �־����� �� ��ȣ�ۿ� ����
        }
    }

    private void FixedUpdate()
    {
        if (!handprintCreated)
        {
            DetectPlayerNearby();
        }
        if (isInteracting)
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



    private void DragDoor()
    {
        Ray playerAim = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 nextPos = cam.transform.position + playerAim.direction * distance;
        Vector3 currPos = transform.position;

        Vector3 forceDirection = (nextPos - currPos).normalized;
        rb.AddForce(forceDirection * forceAmount, ForceMode.VelocityChange);

        // �� ���� ������Ʈ
        if (!isDoorOpen)
        {
            isDoorOpen = true;
            photonView.RPC("RPC_UpdateDoorState", RpcTarget.All, true);
        }
    }
    [PunRPC]
    public void RPC_UpdateDoorState(bool state)
    {
        isDoorOpen = state;  // �� ���� ������Ʈ
                             // ���⿡ �� ȸ�� �ִϸ��̼��̳� ���� ��ȭ ���� �߰�
    }


    public void OnDragBegin()
    {
        isInteracting = true;
        rb.isKinematic = false;  // ��ȣ�ۿ� �߿��� ���� ����
    }

    public void OnDragEnd()
    {
        isInteracting = false;
        rb.isKinematic = true;

        // �� �ݱ� ����
        if (isDoorOpen)
        {
            isDoorOpen = false;
            photonView.RPC("RPC_UpdateDoorState", RpcTarget.All, false);
        }
    }
    [PunRPC]
    public void RPC_RequestDragBegin()
    {
        // ��� Ŭ���̾�Ʈ�� ��ȣ�ۿ��� ������
        OnDragBegin(); // ��ȣ�ۿ� ����
    }
    [PunRPC]
    public void RPC_EndDrag()
    {
        OnDragEnd();  // ��ȣ�ۿ� ����
    }

    public void LeavePrintsUV()
    {
        // PhotonNetwork.Instantiate("Handprint", InteractionTransform.position, InteractionTransform.rotation);
        Instantiate(handprintPrefab, InteractionTransform.position, InteractionTransform.rotation, InteractionTransform);
    }

    private bool DetectPlayerNearby()
    {
        // ���� �÷��̾� ��ü
        GameObject localPlayer = this.gameObject; // ��ũ��Ʈ�� ������ ��ü, �� ���� �÷��̾�

        // ��� �÷��̾ ã��
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            // ���ΰ��� ������ �ʵ���
            if (player == localPlayer)
            {
                continue; // ������ ����
            }

            // �÷��̾���� �Ÿ� ���
            float distance = Vector3.Distance(transform.position, player.transform.position);

            // ������ �Ÿ� ���� �ִ��� Ȯ��
            if (distance <= detectionRadius)
            {
                return true;  // �ٸ� �÷��̾ ��ó�� ����
            }
        }

        if (photonView.IsMine)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);
            foreach (Collider collider in colliders)
            {
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
        }

        return false;  // �÷��̾ ��ó�� ����
    }
}
