using System.Collections;
using UnityEngine;

public class CPlayerDoorInter : MonoBehaviour
{
    public bool IsDoorFullyOpened = false;  // ���� ������ ���ȴ��� ����
    public bool IsDoorClosed = false;       // ���� ������ �������� ����

    public Transform InteractionTransform;  // ��ȣ�ۿ� ��ġ
    public GameObject handprintPrefab;      // �չ��ڱ� ������

    public float forceAmmount = 15f;        // ���� ��
    public float distance = 1.5f;           // �Ÿ�
    public float lockHoldTime = 1.0f;       // ���/������ ���� TŰ Ȧ�� �ð�

    private Collider col;
    private Rigidbody rb;

    private Camera cam;
    private bool isInterracting = false;    // ���� ��ȣ�ۿ� ������ ����
    private bool isDoorLocked = false;      // ���� ��� �ִ��� ����
    private HingeJoint hinge;               // ���� ������ HingeJoint
    private float openedRotation;           // ���� ������ ���� ������ ȸ��
    private const float epsilon = 1f;       // ȸ�� ������ ���ϱ� ���� ���� ����
    private WaitForSeconds waitForDoorStateCheck;  // �� ���� Ȯ���� ���� ��� �ð�
    private const float checkDoorStateCD = 0.3f;   // �� ���� Ȯ�� �ֱ�

    private float tKeyHoldTime = 0f;        // T Ű ���� �ð�

    private void Awake()
    {
        cam = Camera.main;
        hinge = GetComponent<HingeJoint>();
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

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

    private void FixedUpdate()
    {
        if (isInterracting && !isDoorLocked)
        {
            DragDoor();  // ���� �巡��
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
        rb.isKinematic = false;  // ��ȣ�ۿ� �߿��� ���� ����
    }

    public void OnDragEnd()
    {
        isInterracting = false;
        rb.isKinematic = true;  // ��ȣ�ۿ� ������ ���� ���� ����
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
