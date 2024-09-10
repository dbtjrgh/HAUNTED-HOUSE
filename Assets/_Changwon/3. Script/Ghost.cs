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
        // StateMechine 코루틴을 한 번만 시작
        StartCoroutine(StateMechine());
    }

    private void Update()
    {
        target = FindClosestLivingPlayer(); // 죽은 플레이어가 아닌 가장 가까운 플레이어를 찾음
    }

    // 시작하자마자 고스트가 있는곳이 고스트방으로 처리
    private void OnTriggerEnter(Collider other)
    {
        // 레이어를 'Room'이라는 이름으로 가져옵니다.
        int roomLayer = LayerMask.NameToLayer("Room");

        // 충돌한 오브젝트의 레이어가 'Room'인지 확인합니다.
        if (findRoom && other.gameObject.layer == roomLayer)
        {
            room = other.GetComponent<Room>();
            findRoom = false;
            Debug.Log("고스트방 설정");
        }
        else
        {
            return;
        }
    }


    // 죽은 플레이어가 아닌 가장 가까운 플레이어 찾기
    private GameObject FindClosestLivingPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject closestPlayer = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject player in players)
        {
            CMultiPlayer playerScript = player.GetComponent<CMultiPlayer>();

            if (playerScript != null && !playerScript.isDead) // 살아있는 플레이어만 타겟으로 지정
            {
                float distanceToGhost = Vector3.Distance(transform.position, player.transform.position);
                if (distanceToGhost < closestDistance)
                {
                    closestDistance = distanceToGhost;
                    closestPlayer = player;
                }
            }
        }

        return closestPlayer; // 가장 가까운 살아있는 플레이어 반환 (없으면 null 반환)
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

            yield return null; // 다음 프레임으로 넘어가기 위해 필요
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

            // 무작위 위치로 이동
            Vector3 randomPosition = GetRandomNavMeshPosition(transform.position, 10f); // 반경 10 내에서 무작위 위치 선택
            ghostNav.SetDestination(randomPosition);

            // 일정 시간 기다린 후 다시 무작위로 이동
            yield return new WaitForSeconds(5f); // 5초 후에 다시 무작위로 이동

            // 정신 게이지에 따른 상태 전환
            if (mental != null && mental.MentalGauge <= 50)
            {
                ChangeState(changwon.GhostState.HUNTTING);
                yield break; // 현재 코루틴 종료
            }
            else if (mental == null)
            {
                Debug.Log("mental is null");
            }

            yield return null;
        }
    }

    // 무작위로 NavMesh 상에서 위치를 찾는 함수
    private Vector3 GetRandomNavMeshPosition(Vector3 origin, float distance)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance; // 주어진 거리 내에서 무작위 방향
        randomDirection += origin; // 시작 위치를 더해 줍니다.

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, distance, NavMesh.AllAreas); // 유효한 NavMesh 위의 위치 찾기

        return navHit.position;
    }

    private IEnumerator returnPosition()
    {
        mapManager = FindObjectOfType<MapManager>();

        while (state == changwon.GhostState.RETURN)
        {
            ghostNav.isStopped = false;
            ghostNav.SetDestination(mapManager.returnRandom); // 랜덤 위치로 이동

            // 목적지로 이동 중
            while (ghostNav.remainingDistance > ghostNav.stoppingDistance)
            {
                yield return null; // 계속 대기하며 목표에 도달할 때까지 기다림
            }

            // 목적지에 도착하면 IDLE 상태로 전환
            Debug.Log("Return 목적지에 도착, IDLE 상태로 전환.");
            ChangeState(changwon.GhostState.IDLE);

            // 상태 전환 후에는 코루틴 종료
            yield break;
        }
    }


    private IEnumerator Hunting()
    {
        mental = FindObjectOfType<mentalGaugeManager>();
        float huntingTimer = 0f; // 헌팅 상태 유지 시간을 계산할 타이머
        float maxHuntingDuration = 30f; // 헌팅 상태에서 최대 유지 시간

        while (state == changwon.GhostState.HUNTTING)
        {
            huntingTimer += Time.deltaTime; // 헌팅 상태에서 경과 시간 증가

            if (mental != null && (mental.MentalGauge > 50 || huntingTimer >= maxHuntingDuration)) // 정신 게이지가 50 이상이거나, 일정 시간이 지나면
            {
                Debug.Log("헌팅 실패, Return 상태로 전환.");
                ChangeState(changwon.GhostState.RETURN); // 헌팅 실패 시 Return 상태로 전환
                yield break; // 현재 코루틴 종료
            }

            if (target != null) // 타겟이 존재할 때
            {
                ghostNav.isStopped = false;
                ghostNav.SetDestination(target.transform.position);

                float huntingTargetDistance = Vector3.Distance(target.transform.position, transform.position);
                float ghostBlinkTargetDistance = Vector3.Distance(target.transform.position, transform.position);

                StartCoroutine(ghostInter()); // 인터랙션 처리
                if (ghostBlinkTargetDistance < 5)
                {
                    StartCoroutine(ghostBlink());
                }
                if (huntingTargetDistance < 0.5f) // 타겟에 도착한 경우
                {
                    Debug.Log("플레이어를 찾았다");

                    CMultiPlayer targetPlayer = target.GetComponent<CMultiPlayer>();
                    if (targetPlayer != null)
                    {
                        Debug.Log("죽음");
                        targetPlayer.Die(); // 플레이어 죽이기
                    }

                    ghostNav.isStopped = true;
                    ChangeState(changwon.GhostState.RETURN); // 타겟을 찾았을 때 Return 상태로 전환
                    yield break; // 현재 코루틴 종료
                }
            }
            else
            {
                Debug.Log("타겟을 잃어 Return 상태로 전환");
                ChangeState(changwon.GhostState.RETURN); // 타겟을 잃었을 때 바로 Return 상태로 전환
                yield break; // 현재 코루틴 종료
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

        for (int i = 0; i < 5; i++) // 5번 반복하도록 설정
        {
            // Ghost 레이어를 포함시키는 부분
            Camera.main.cullingMask |= 1 << ghostLayer;
            yield return new WaitForSeconds(0.5f); // 0.5초 동안 Ghost 레이어가 보임

            // Ghost 레이어를 제외하는 부분
            Camera.main.cullingMask &= ~(1 << ghostLayer);
            yield return new WaitForSeconds(0.5f); // 0.5초 동안 Ghost 레이어가 숨김
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
