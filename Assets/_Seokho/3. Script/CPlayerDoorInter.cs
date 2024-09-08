using System.Collections;
using UnityEngine;

public class CPlayerDoorInter : MonoBehaviour
{
    public bool IsDoorFullyOpened = false;  // 문이 완전히 열렸는지 여부
    public bool IsDoorClosed = false;       // 문이 완전히 닫혔는지 여부

    public Transform InteractionTransform;  // 상호작용 위치
    public GameObject handprintPrefab;      // 손발자국 프리팹

    public float forceAmmount = 15f;        // 힘의 양
    public float distance = 1.5f;           // 거리
    public float lockHoldTime = 1.0f;       // 잠금/해제를 위한 T키 홀드 시간

    private Collider col;
    private Rigidbody rb;

    private Camera cam;
    private bool isInterracting = false;    // 문과 상호작용 중인지 여부
    private bool isDoorLocked = false;      // 문이 잠겨 있는지 여부
    private HingeJoint hinge;               // 문에 부착된 HingeJoint
    private float openedRotation;           // 문이 완전히 열린 상태의 회전
    private const float epsilon = 1f;       // 회전 각도를 비교하기 위한 오차 범위
    private WaitForSeconds waitForDoorStateCheck;  // 문 상태 확인을 위한 대기 시간
    private const float checkDoorStateCD = 0.3f;   // 문 상태 확인 주기

    private float tKeyHoldTime = 0f;        // T 키 누른 시간

    private void Awake()
    {
        cam = Camera.main;
        hinge = GetComponent<HingeJoint>();
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        if (hinge == null)
        {
            Debug.Log("HingeJoint가 없으므로 상호작용을 비활성화합니다.");
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
        // 왼쪽 마우스 버튼으로 상호작용 시작/종료
        if (Input.GetMouseButtonDown(0) && !isInterracting && !isDoorLocked)  // 왼쪽 클릭으로 시작
        {
            OnDragBegin();
        }
        if (Input.GetMouseButtonUp(0) && isInterracting)  // 왼쪽 클릭을 놓으면 상호작용 종료
        {
            OnDragEnd();
        }

        // T 키를 누르고 있는 시간 측정 (잠금/해제)
        if (Input.GetKey(KeyCode.T))
        {
            tKeyHoldTime += Time.deltaTime;

            if (tKeyHoldTime >= lockHoldTime)  // T키를 일정 시간 이상 눌렀을 때
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

                tKeyHoldTime = 0f;  // 시간 초기화
            }
        }

        // T키에서 손을 뗐을 때, 타이머 초기화
        if (Input.GetKeyUp(KeyCode.T))
        {
            tKeyHoldTime = 0f;
        }
    }

    private void FixedUpdate()
    {
        if (isInterracting && !isDoorLocked)
        {
            DragDoor();  // 문을 드래그
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
        if (doorCurrRotation < -5f) doorCurrRotation += 360;

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

        rb.velocity = (nextPos - currPos) * forceAmmount;
    }

    public void OnDragBegin()
    {
        isInterracting = true;
        rb.isKinematic = false;  // 상호작용 중에는 물리 적용
    }

    public void OnDragEnd()
    {
        isInterracting = false;
        rb.isKinematic = true;  // 상호작용 끝나면 물리 적용 해제
    }

    public void LockTheDoor()
    {
        col.enabled = false;
        isDoorLocked = true;
    }

    public void UnlockTheDoor()
    {
        rb.isKinematic = false;
        isDoorLocked = false;
        col.enabled = true;
    }

    public void LeavePrintsUV()
    {
        Instantiate(handprintPrefab, InteractionTransform.position, InteractionTransform.rotation, InteractionTransform);
    }
}
