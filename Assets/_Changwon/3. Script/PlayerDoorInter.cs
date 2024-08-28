using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDoorInter : MonoBehaviour
{
    public bool IsDoorFullyOpened = false;  // ���� ������ ���ȴ��� ����
    public bool IsDoorClosed = false;       // ���� ������ �������� ����

    public Transform InteractionTransform;  // ��ȣ�ۿ� ��ġ

    public float forceAmmount = 15f;        // ���� ��
    public float distance = 1.5f;           // �Ÿ�
    public float maxDrugVolume;             // �ִ� �巡�� ����

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
        waitForDoorStateCheck = new WaitForSeconds(checkDoorStateCD);
        waitForDoorRotationCheck = new WaitForSeconds(checkDoorRotationCD);

        openedRotation = hinge.limits.max * hinge.axis.y;       // ���� ������ ���� ������ ȸ��
        if(openedRotation<-1f)openedRotation+=360f;             // ���� ������ ���� ������ ȸ��


    }

    private void FixedUpdate()
    {
        if (isInterracting&&!isDoorLocked)  // ��ȣ�ۿ� ���̰� ���� ������� �ʴٸ�
        {
            
        }
    }

    public void GhostDrugDoor()
    {
        
    }

    public void LockTheDoor()
    {

    }

    public void UnlockTheDoor()
    {

    }

    /*private IEnumerator CloseDoorCoroutine()
    {
        col.enabled = false; 
        isDoorLocked = true;
        
    }*/

    private void EnableColider() { col.enabled = true;rb.isKinematic = true;    }
    private void GhostInterrectWithDoor(float force, float direction, float time)
    {
        hinge.useMotor = true;          // ���� ���
        var motor = hinge.motor;
        motor.force = motorForce;       // ���� ��
        hinge.motor = motor;            // ����
        Invoke(nameof(StopDruggingDoor), time);  // �� �巡�� ����

    }

    private void StopDruggingDoor()
    {
        var motor = hinge.motor;
        motor.targetVelocity = 0f;
        hinge.motor = motor;
    }

    private float GenerateForce()=>Random.Range(MinGhostsForcePower, MaxGhostsForcePower);

    /*private int GenerateDirection()
    {
        int randomNum
    }*/


}
