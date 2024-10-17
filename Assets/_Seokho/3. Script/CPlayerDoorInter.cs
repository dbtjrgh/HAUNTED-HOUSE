using GameFeatures;
using Photon.Pun;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PhotonView), typeof(PhotonTransformView))]
public class CPlayerDoorInter : MonoBehaviourPun
{
    #region 변수

    public Transform InteractionTransform;  // 상호작용 위치
    public GameObject handprintPrefab;      // 손발자국 프리팹
    public float detectionRadius = 3.0f;    // 플레이어 감지 범위
    private bool handprintCreated = false;  // 손자국이 한 번만 생성되도록 하는 플래그

    public float forceAmount = 15f;        // 힘의 양
    public float distance = 1.5f;          // 거리

    private Rigidbody rb;
    Ghost ghost;

    private Camera cam;
    public bool isInteracting;    // 문과 상호작용 중인지 여부
    private bool isDoorOpen = false;  // 문 상태 변수 추가

    private HingeJoint hinge;               // 문에 부착된 HingeJoint
    private float openedRotation;           // 문이 완전히 열린 상태의 회전
    private const float epsilon = 1f;       // 회전 각도를 비교하기 위한 오차 범위

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
            Debug.Log("HingeJoint가 없으므로 상호작용을 비활성화합니다.");
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

        // 모든 클라이언트가 입력을 감지할 수 있도록
        if (isPlayerNearby)
        {
            // 소유권 여부와 상관없이 모든 클라이언트가 상호작용을 시도
            if (Input.GetMouseButtonDown(0) && !isInteracting)
            {
                // 상호작용 시작 요청을 모든 클라이언트에 보냄
                photonView.RPC("RPC_RequestDragBegin", RpcTarget.All);
            }
            if (Input.GetMouseButtonUp(0) && isInteracting)
            {
                photonView.RPC("RPC_EndDrag", RpcTarget.All);
            }
        }
        else if (isInteracting)
        {
            photonView.RPC("RPC_EndDrag", RpcTarget.All);  // 플레이어가 멀어졌을 때 상호작용 종료
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
            // 상호작용 중에도 플레이어가 계속 근처에 있는지 확인
            if (DetectPlayerNearby())
            {
                DragDoor();  // 문을 드래그
            }
            else
            {
                OnDragEnd(); // 플레이어가 멀어지면 상호작용 종료
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

        // 문 상태 업데이트
        if (!isDoorOpen)
        {
            isDoorOpen = true;
            photonView.RPC("RPC_UpdateDoorState", RpcTarget.All, true);
        }
    }
    [PunRPC]
    public void RPC_UpdateDoorState(bool state)
    {
        isDoorOpen = state;  // 문 상태 업데이트
                             // 여기에 문 회전 애니메이션이나 상태 변화 로직 추가
    }


    public void OnDragBegin()
    {
        isInteracting = true;
        rb.isKinematic = false;  // 상호작용 중에는 물리 적용
    }

    public void OnDragEnd()
    {
        isInteracting = false;
        rb.isKinematic = true;

        // 문 닫기 로직
        if (isDoorOpen)
        {
            isDoorOpen = false;
            photonView.RPC("RPC_UpdateDoorState", RpcTarget.All, false);
        }
    }
    [PunRPC]
    public void RPC_RequestDragBegin()
    {
        // 모든 클라이언트가 상호작용을 시작함
        OnDragBegin(); // 상호작용 시작
    }
    [PunRPC]
    public void RPC_EndDrag()
    {
        OnDragEnd();  // 상호작용 종료
    }

    public void LeavePrintsUV()
    {
        // PhotonNetwork.Instantiate("Handprint", InteractionTransform.position, InteractionTransform.rotation);
        Instantiate(handprintPrefab, InteractionTransform.position, InteractionTransform.rotation, InteractionTransform);
    }

    private bool DetectPlayerNearby()
    {
        // 현재 플레이어 객체
        GameObject localPlayer = this.gameObject; // 스크립트가 부착된 객체, 즉 현재 플레이어

        // 모든 플레이어를 찾기
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            // 본인과는 비교하지 않도록
            if (player == localPlayer)
            {
                continue; // 본인은 무시
            }

            // 플레이어와의 거리 계산
            float distance = Vector3.Distance(transform.position, player.transform.position);

            // 지정된 거리 내에 있는지 확인
            if (distance <= detectionRadius)
            {
                return true;  // 다른 플레이어가 근처에 있음
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
                        handprintCreated = true;  // 손자국이 생성되었음을 표시
                        Debug.Log("손자국 프리팹 형성");
                        LeavePrintsUV();  // 손자국 프리팹 생성
                    }
                }
            }
        }

        return false;  // 플레이어가 근처에 없음
    }
}
