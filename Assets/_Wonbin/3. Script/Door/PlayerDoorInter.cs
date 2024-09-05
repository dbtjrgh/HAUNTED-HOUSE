using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wonbin
{
    public class PlayerDoorInter : MonoBehaviour
    {
        public bool IsDoorFullyOpened = false;  // ���� ������ ���ȴ��� ����
        public bool IsDoorClosed = false;       // ���� ������ �������� ����

        public Transform InteractionTransform;  // ��ȣ�ۿ� ��ġ
        public GameObject handprintPrefab;      // �չ��ڱ� ������


        public float forceAmmount = 15f;        // ���� ��
        public float distance = 1.5f;           // �Ÿ�
        public float maxDrugVolume;             // �ִ� �巡�� ����

        Ghost ghost;

        private Collider col;
        private Rigidbody rb;

        private Camera cam;
        private bool isInterracting = false;
        private bool isDoorLocked = false;
        private HingeJoint hinge;
        private float openedRotation;           // ���� ������ ���� ������ ȸ��

        private float doorDragCurrSpeed = 0f;   // �� �巡�� ���� �ӵ�
        private Vector3 prevRotation;           // ���� ȸ��
        private float doorCurrRotation;         // �� ���� ȸ��

        private const float MinDoorDrugSpeed = 0f;      // �ּ� �� �巡�� �ӵ�
        private const float MaxDoorDrugSpeed = 20f;     // �ִ� �� �巡�� �ӵ�
        private const float averageDrugSpeed = 15f;     // ��� �� �巡�� �ӵ�

        private const float MinGhostsForcePower = 7f;   // �ּ� ���� ��
        private const float MaxGhostsForcePower = 17f;  // �ִ� ���� ��
        private const float CloseDoorForcePower = 250f; // �� �ݱ� ��
        private const float minGhostForceTime = 1.2f;   // �ּ� ���� �� �ð�
        private const float maxGhostForceTime = 3f;     // �ִ� ���� �� �ð�

        private const float epsilon = 1f;
        private const float checkDoorStateCD = 0.3f;    // �� ���� Ȯ�� ��ٿ�
        private const float checkDoorRotationCD = 0.1f; // �� ȸ�� Ȯ�� ��ٿ�
        private const float motorForce = 100f;          // ���� ��

        private WaitForSeconds waitForDoorStateCheck;       // �� ���� Ȯ���� ���� ��� �ð�
        private WaitForSeconds waitForDoorRotationCheck;    // �� ȸ�� Ȯ���� ���� ��� �ð�



        private void Awake()
        {
            cam = Camera.main;
        }

        private void Start()
        {
            hinge = GetComponent<HingeJoint>();
            col = GetComponent<Collider>();
            rb = GetComponent<Rigidbody>();
            Ghost ghost = GetComponent<Ghost>();


            waitForDoorStateCheck = new WaitForSeconds(checkDoorStateCD);
            waitForDoorRotationCheck = new WaitForSeconds(checkDoorRotationCD);

            openedRotation = hinge.limits.max * hinge.axis.y;       // ���� ������ ���� ������ ȸ��
            if (openedRotation < -1f) openedRotation += 360f;             // ���� ������ ���� ������ ȸ��


            StartCoroutine(CheckDoorState());
            if (Ghost.instance.ghostType == GhostType.BANSHEE || Ghost.instance.ghostType == GhostType.DEMON)
                LeavePrintsUV();

        }

        private void FixedUpdate()
        {
            if (isInterracting && !isDoorLocked)  // ��ȣ�ۿ� ���̰� ���� ������� �ʴٸ�
            {
                DragDoor();     // �� �巡��
            }
        }

        public void GhostDrugDoor()
        {
            ghost = FindObjectOfType<Ghost>();
            if (ghost.state == changwon.GhostState.HUNTTING)  // ������ ��� ���̶��
            {
                GhostInterrectWithDoor(GenerateForce(), GenerateDirection(), Random.Range(minGhostForceTime, maxGhostForceTime));   // ������ ���� �巡��        //hunttime�� enumŬ������ huntting�̸�
            }
        }

        public void LockTheDoor()
        {
            StartCoroutine(CloseDoorCoroutine());
        }

        public void UnlockTheDoor()
        {
            rb.isKinematic = false;
            isDoorLocked = false;
        }

        private IEnumerator CloseDoorCoroutine()
        {
            col.enabled = false;
            isDoorLocked = true;
            GhostInterrectWithDoor(CloseDoorForcePower, 1f, 0.5f);  // �� �ݱ�

            while (true)
            {
                if (IsDoorFullyClosed()) { EnableColider(); yield break; }   // ���� ������ �����ٸ� �ݶ��̴� Ȱ��ȭ     //yield break�� �ڷ�ƾ�� ������ ����
                yield return null;
            }

        }

        private void EnableColider() { col.enabled = true; rb.isKinematic = true; }
        private void GhostInterrectWithDoor(float force, float direction, float time)
        {
            hinge.useMotor = true;          // ���� ���
            var motor = hinge.motor;
            motor.force = motorForce;       // ���� ��
            hinge.motor = motor;            // ����
            Invoke("StopDruggingDoor", time);  // �� �巡�� ����      

        }

        private void StopDruggingDoor()
        {
            var motor = hinge.motor;
            motor.targetVelocity = 0f;
            hinge.motor = motor;
        }

        private float GenerateForce() => Random.Range(MinGhostsForcePower, MaxGhostsForcePower);

        private int GenerateDirection()
        {
            int randomNum = Random.Range(0, 1);
            if (randomNum == 0) randomNum = -1;
            if (IsDoorClosed) randomNum = 1;
            else if (IsDoorFullyOpened) randomNum = -1;
            return randomNum;
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
            doorCurrRotation = transform.localEulerAngles.y;  // �� ���� ȸ��
            if (doorCurrRotation < -5f) doorCurrRotation += 360; // ���� ������ ���� ������ ȸ��

            if (Mathf.Abs(transform.localEulerAngles.y) <= epsilon)   // ���� ������ ���� �������� Ȯ��    //mathf.abs�� ���밪�� ��ȯ
            {
                IsDoorClosed = true;
                IsDoorFullyOpened = false;
            }
            else if (Mathf.Abs(transform.localEulerAngles.y - openedRotation) <= epsilon) // ���� ������ ���� �������� Ȯ��
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

        private bool IsDoorFullyClosed() => (Mathf.Abs(transform.localEulerAngles.y) <= epsilon);

        private void DragDoor()
        {
            Ray playerAim = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            Vector3 nextPos = cam.transform.position + playerAim.direction * distance;      // ���� ��ġ
            Vector3 currPos = transform.position;           //  ���� ��ġ

            rb.velocity = (nextPos - currPos) * forceAmmount;    // rb.velocity�� �������� �ӵ��� ��Ÿ���� ����
        }



        public void OnDragBegin()
        {
            isInterracting = true;
        }


        public void OnDragEnd()
        {
            isInterracting = false;


        }

        public void LeavePrintsUV()
        {
            Instantiate(handprintPrefab, InteractionTransform.position, InteractionTransform.rotation, InteractionTransform);
        }

    }
}
