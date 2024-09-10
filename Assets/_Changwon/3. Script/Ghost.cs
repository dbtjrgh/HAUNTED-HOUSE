using GameFeatures;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Wonbin;

namespace changwon
{
    public enum GhostState
    {
        IDLE,
        RETURN,
        HUNTTING
    }
}

public enum GhostType
{
    NIGHTMARE,
    BANSHEE,
    DEMON
}

public class Ghost : MonoBehaviour
{
    public static Ghost instance;

    public changwon.GhostState state;
    public GhostType ghostType;
    public NavMeshAgent ghostNav;
    public GameObject target;
    private bool findRoom = true;
    Room room;

    mentalGaugeManager mental;
    MapManager mapManager;

    private void Awake()
    {
        instance = this;
        ghosttypeRandom(value: Random.Range(0, 3));
        switch (ghostType)
        {
            case GhostType.NIGHTMARE:
                ghostNav.speed = 1f;
                break;
            case GhostType.BANSHEE:
                ghostNav.speed = 2f;
                break;
            case GhostType.DEMON:
                ghostNav.speed = 3f;
                break;
        }
    }

    private void Start()
    {
        // StateMechine �ڷ�ƾ�� �� ���� ����
        StartCoroutine(StateMechine());
    }

    private void Update()
    {
        target = FindClosestLivingPlayer(); // ���� �÷��̾ �ƴ� ���� ����� �÷��̾ ã��
    }

    // �������ڸ��� ��Ʈ�� �ִ°��� ��Ʈ������ ó��
    private void OnTriggerEnter(Collider other)
    {
        // ���̾ 'Room'�̶�� �̸����� �����ɴϴ�.
        int roomLayer = LayerMask.NameToLayer("Room");

        // �浹�� ������Ʈ�� ���̾ 'Room'���� Ȯ���մϴ�.
        if (findRoom && other.gameObject.layer == roomLayer)
        {
            room = other.GetComponent<Room>();
            findRoom = false;
            Debug.Log("��Ʈ�� ����");
        }
        else
        {
            return;
        }
    }


    // ���� �÷��̾ �ƴ� ���� ����� �÷��̾� ã��
    private GameObject FindClosestLivingPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject closestPlayer = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject player in players)
        {
            CMultiPlayer playerScript = player.GetComponent<CMultiPlayer>();

            if (playerScript != null && !playerScript.isDead) // ����ִ� �÷��̾ Ÿ������ ����
            {
                float distanceToGhost = Vector3.Distance(transform.position, player.transform.position);
                if (distanceToGhost < closestDistance)
                {
                    closestDistance = distanceToGhost;
                    closestPlayer = player;
                }
            }
        }

        return closestPlayer; // ���� ����� ����ִ� �÷��̾� ��ȯ (������ null ��ȯ)
    }


    public float currentGhostType()
    {
        return (float)ghostType;
    }

    public IEnumerator StateMechine()
    {
        while (true)
        {
            switch (state)
            {
                case changwon.GhostState.IDLE:
                    yield return StartCoroutine(idle());
                    break;

                case changwon.GhostState.HUNTTING:
                    yield return StartCoroutine(Hunting());
                    break;

                case changwon.GhostState.RETURN:
                    yield return StartCoroutine(returnPosition());
                    break;
            }

            yield return null; // ���� ���������� �Ѿ�� ���� �ʿ�
        }
    }

    public void ChangeState(changwon.GhostState newstate)
    {
        if (state != newstate)
        {
            state = newstate;
            Debug.Log($"State changed to: {state}");
        }
    }

    private IEnumerator idle()
    {
        mental = FindObjectOfType<mentalGaugeManager>();
        mapManager = FindObjectOfType<MapManager>();

        while (state == changwon.GhostState.IDLE)
        {
            ghostNav.isStopped = false;

            // ������ ��ġ�� �̵�
            Vector3 randomPosition = GetRandomNavMeshPosition(transform.position, 10f); // �ݰ� 10 ������ ������ ��ġ ����
            ghostNav.SetDestination(randomPosition);

            // ���� �ð� ��ٸ� �� �ٽ� �������� �̵�
            yield return new WaitForSeconds(5f); // 5�� �Ŀ� �ٽ� �������� �̵�

            // ���� �������� ���� ���� ��ȯ
            if (mental != null && mental.MentalGauge <= 50)
            {
                ChangeState(changwon.GhostState.HUNTTING);
                yield break; // ���� �ڷ�ƾ ����
            }
            else if (mental == null)
            {
                Debug.Log("mental is null");
            }

            yield return null;
        }
    }

    // �������� NavMesh �󿡼� ��ġ�� ã�� �Լ�
    private Vector3 GetRandomNavMeshPosition(Vector3 origin, float distance)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance; // �־��� �Ÿ� ������ ������ ����
        randomDirection += origin; // ���� ��ġ�� ���� �ݴϴ�.

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, distance, NavMesh.AllAreas); // ��ȿ�� NavMesh ���� ��ġ ã��

        return navHit.position;
    }

    private IEnumerator returnPosition()
    {
        mapManager = FindObjectOfType<MapManager>();

        while (state == changwon.GhostState.RETURN)
        {
            ghostNav.isStopped = false;
            ghostNav.SetDestination(mapManager.returnRandom); // ���� ��ġ�� �̵�

            // �������� �̵� ��
            while (ghostNav.remainingDistance > ghostNav.stoppingDistance)
            {
                yield return null; // ��� ����ϸ� ��ǥ�� ������ ������ ��ٸ�
            }

            // �������� �����ϸ� IDLE ���·� ��ȯ
            Debug.Log("Return �������� ����, IDLE ���·� ��ȯ.");
            ChangeState(changwon.GhostState.IDLE);

            // ���� ��ȯ �Ŀ��� �ڷ�ƾ ����
            yield break;
        }
    }


    private IEnumerator Hunting()
    {
        mental = FindObjectOfType<mentalGaugeManager>();
        float huntingTimer = 0f; // ���� ���� ���� �ð��� ����� Ÿ�̸�
        float maxHuntingDuration = 30f; // ���� ���¿��� �ִ� ���� �ð�

        while (state == changwon.GhostState.HUNTTING)
        {
            huntingTimer += Time.deltaTime; // ���� ���¿��� ��� �ð� ����

            if (mental != null && (mental.MentalGauge > 50 || huntingTimer >= maxHuntingDuration)) // ���� �������� 50 �̻��̰ų�, ���� �ð��� ������
            {
                Debug.Log("���� ����, Return ���·� ��ȯ.");
                ChangeState(changwon.GhostState.RETURN); // ���� ���� �� Return ���·� ��ȯ
                yield break; // ���� �ڷ�ƾ ����
            }

            if (target != null) // Ÿ���� ������ ��
            {
                ghostNav.isStopped = false;
                ghostNav.SetDestination(target.transform.position);

                float huntingTargetDistance = Vector3.Distance(target.transform.position, transform.position);
                float ghostBlinkTargetDistance = Vector3.Distance(target.transform.position, transform.position);

                StartCoroutine(ghostInter()); // ���ͷ��� ó��
                if (ghostBlinkTargetDistance < 5)
                {
                    StartCoroutine(ghostBlink());
                }
                if (huntingTargetDistance < 0.5f) // Ÿ�ٿ� ������ ���
                {
                    Debug.Log("�÷��̾ ã�Ҵ�");

                    CMultiPlayer targetPlayer = target.GetComponent<CMultiPlayer>();
                    if (targetPlayer != null)
                    {
                        Debug.Log("����");
                        targetPlayer.Die(); // �÷��̾� ���̱�
                    }

                    ghostNav.isStopped = true;
                    ChangeState(changwon.GhostState.RETURN); // Ÿ���� ã���� �� Return ���·� ��ȯ
                    yield break; // ���� �ڷ�ƾ ����
                }
            }
            else
            {
                Debug.Log("Ÿ���� �Ҿ� Return ���·� ��ȯ");
                ChangeState(changwon.GhostState.RETURN); // Ÿ���� �Ҿ��� �� �ٷ� Return ���·� ��ȯ
                yield break; // ���� �ڷ�ƾ ����
            }

            yield return null;
        }
    }


    public void ghosttypeRandom(int value)
    {
        ghostType = (GhostType)value;
    }

    IEnumerator ghostBlink()
    {
        int ghostLayer = LayerMask.NameToLayer("Ghost");

        for (int i = 0; i < 5; i++) // 5�� �ݺ��ϵ��� ����
        {
            // Ghost ���̾ ���Խ�Ű�� �κ�
            Camera.main.cullingMask |= 1 << ghostLayer;
            yield return new WaitForSeconds(0.5f); // 0.5�� ���� Ghost ���̾ ����

            // Ghost ���̾ �����ϴ� �κ�
            Camera.main.cullingMask &= ~(1 << ghostLayer);
            yield return new WaitForSeconds(0.5f); // 0.5�� ���� Ghost ���̾ ����
        }
    }

    IEnumerator ghostInter()
    {
        CMultiPlayer player = FindObjectOfType<CMultiPlayer>();

        if (player == null)
        {
            yield break;
        }

        switch (ghostType)
        {
            case GhostType.BANSHEE:
                player.sprintMultiplier = 1f;
                break;
            case GhostType.NIGHTMARE:
                float LightBlinkTargetDistance = Vector3.Distance(target.transform.position, transform.position);
                if (LightBlinkTargetDistance < 10)
                {
                    Light playerLight = player.GetComponentInChildren<Light>();
                    if (playerLight != null)
                    {
                        playerLight.enabled = true;
                        yield return new WaitForSeconds(1f);
                        playerLight.enabled = false;
                    }
                }
                break;
        }
        yield return null;
    }
}
