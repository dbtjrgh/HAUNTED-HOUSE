using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wonbin
{
    public class PlayerDoorInter : MonoBehaviour
    {
        public bool IsDoorFullyOpened = false;  // 문이 완전히 열렸는지 여부
        public bool IsDoorClosed = false;       // 문이 완전히 닫혔는지 여부

        public Transform InteractionTransform;  // 상호작용 위치
        public GameObject handprintPrefab;      // 손발자국 프리팹


        public float forceAmmount = 15f;        // 힘의 양
        public float distance = 1.5f;           // 거리
        public float maxDrugVolume;             // 최대 드래그 볼륨

        Ghost ghost;

        private Collider col;
        private Rigidbody rb;

        private Camera cam;
        private bool isInterracting = false;
        private bool isDoorLocked = false;
        private HingeJoint hinge;
        private float openedRotation;           // 문이 완전히 열린 상태의 회전

        private float doorDragCurrSpeed = 0f;   // 문 드래그 현재 속도
        private Vector3 prevRotation;           // 이전 회전
        private float doorCurrRotation;         // 문 현재 회전

        private const float MinDoorDrugSpeed = 0f;      // 최소 문 드래그 속도
        private const float MaxDoorDrugSpeed = 20f;     // 최대 문 드래그 속도
        private const float averageDrugSpeed = 15f;     // 평균 문 드래그 속도

        private const float MinGhostsForcePower = 7f;   // 최소 유령 힘
        private const float MaxGhostsForcePower = 17f;  // 최대 유령 힘
        private const float CloseDoorForcePower = 250f; // 문 닫기 힘
        private const float minGhostForceTime = 1.2f;   // 최소 유령 힘 시간
        private const float maxGhostForceTime = 3f;     // 최대 유령 힘 시간

        private const float epsilon = 1f;
        private const float checkDoorStateCD = 0.3f;    // 문 상태 확인 쿨다운
        private const float checkDoorRotationCD = 0.1f; // 문 회전 확인 쿨다운
        private const float motorForce = 100f;          // 모터 힘

        private WaitForSeconds waitForDoorStateCheck;       // 문 상태 확인을 위한 대기 시간
        private WaitForSeconds waitForDoorRotationCheck;    // 문 회전 확인을 위한 대기 시간



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

            openedRotation = hinge.limits.max * hinge.axis.y;       // 문이 완전히 열린 상태의 회전
            if (openedRotation < -1f) openedRotation += 360f;             // 문이 완전히 열린 상태의 회전


            StartCoroutine(CheckDoorState());
            if (Ghost.instance.ghostType == GhostType.BANSHEE || Ghost.instance.ghostType == GhostType.DEMON)
                LeavePrintsUV();

        }

        private void FixedUpdate()
        {
            if (isInterracting && !isDoorLocked)  // 상호작용 중이고 문이 잠겨있지 않다면
            {
                DragDoor();     // 문 드래그
            }
        }

        public void GhostDrugDoor()
        {
            ghost = FindObjectOfType<Ghost>();
            if (ghost.state == changwon.GhostState.HUNTTING)  // 유령이 사냥 중이라면
            {
                GhostInterrectWithDoor(GenerateForce(), GenerateDirection(), Random.Range(minGhostForceTime, maxGhostForceTime));   // 유령이 문을 드래그        //hunttime이 enum클래스의 huntting이면
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
            GhostInterrectWithDoor(CloseDoorForcePower, 1f, 0.5f);  // 문 닫기

            while (true)
            {
                if (IsDoorFullyClosed()) { EnableColider(); yield break; }   // 문이 완전히 닫혔다면 콜라이더 활성화     //yield break는 코루틴을 끝내는 역할
                yield return null;
            }

        }

        private void EnableColider() { col.enabled = true; rb.isKinematic = true; }
        private void GhostInterrectWithDoor(float force, float direction, float time)
        {
            hinge.useMotor = true;          // 모터 사용
            var motor = hinge.motor;
            motor.force = motorForce;       // 모터 힘
            hinge.motor = motor;            // 모터
            Invoke("StopDruggingDoor", time);  // 문 드래그 멈춤      

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
            doorCurrRotation = transform.localEulerAngles.y;  // 문 현재 회전
            if (doorCurrRotation < -5f) doorCurrRotation += 360; // 문이 완전히 닫힌 상태의 회전

            if (Mathf.Abs(transform.localEulerAngles.y) <= epsilon)   // 문이 완전히 닫힌 상태인지 확인    //mathf.abs는 절대값을 반환
            {
                IsDoorClosed = true;
                IsDoorFullyOpened = false;
            }
            else if (Mathf.Abs(transform.localEulerAngles.y - openedRotation) <= epsilon) // 문이 완전히 열린 상태인지 확인
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

            Vector3 nextPos = cam.transform.position + playerAim.direction * distance;      // 다음 위치
            Vector3 currPos = transform.position;           //  현재 위치

            rb.velocity = (nextPos - currPos) * forceAmmount;    // rb.velocity는 물리적인 속도를 나타내는 변수
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
