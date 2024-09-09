using System.Collections;
using System.Collections.Generic;
using System.Net;
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


    private void Update()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(StateMechine());
    }

    public float currentGhostType()
    {
        return (float)ghostType;
    }




    public IEnumerator StateMechine()
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

    }

    public void ChangeState(changwon.GhostState newstate)
    {
        state = newstate;
    }

    private IEnumerator idle()
    {
        mental = FindObjectOfType<mentalGaugeManager>();

        while (state == changwon.GhostState.IDLE)
        {
            ghostNav.isStopped = true;
            if (mental != null && mental.MentalGauge <= 50)
            {
                ChangeState(changwon.GhostState.HUNTTING);
            }

            else if (mental == null)
            {
                Debug.Log("mental is null");
            }
            yield return null;
        }
    }

    private IEnumerator returnPosition()
    {
        mapManager = FindObjectOfType<MapManager>();
        while (state == changwon.GhostState.RETURN)
        {
            {
                ghostNav.isStopped = false;
                ghostNav.SetDestination(mapManager.returnRandom);
                yield return new WaitForSeconds(30f);
                ChangeState(changwon.GhostState.IDLE);
            }
            yield return null;
        }
    }


    private IEnumerator Hunting()
    {
        while (state == changwon.GhostState.HUNTTING)
        {
            if (mental.MentalGauge > 50)
            {
                ChangeState(changwon.GhostState.RETURN);
            }
            if (target != null)
            {
                ghostNav.isStopped = false;
                ghostNav.SetDestination(target.transform.position);
                float HunttingTargetDistance = Vector3.Distance(target.transform.position, transform.position);
                float ghostBlinkTargetDistance = Vector3.Distance(target.transform.position, transform.position);
                StartCoroutine(ghostInter());
                if (ghostBlinkTargetDistance < 5)
                {
                    StartCoroutine(ghostBlink());
                    
                }
                if (HunttingTargetDistance < 0.5)
                {
                    Debug.Log("플레이어를 찾았다");


                    CMultiPlayer targetPlayer = target.GetComponent<CMultiPlayer>();
                    if (targetPlayer != null)
                    {
                        Debug.Log("죽음");
                        targetPlayer.Die();
                    }

                    ghostNav.isStopped = true;
                    ChangeState(changwon.GhostState.RETURN);

                    yield return new WaitForSeconds(25f);
                    ChangeState(changwon.GhostState.IDLE);
                    yield return new WaitForSeconds(10f);
                    ChangeState(changwon.GhostState.HUNTTING);
                    
                }
                else
                {
                    ghostNav.isStopped = false;
                    yield return new WaitForSeconds(30f);
                    ChangeState(changwon.GhostState.IDLE);
                    yield return new WaitForSeconds(10f);
                    ChangeState(changwon.GhostState.HUNTTING);
                }
            }
            else
            {
                ChangeState(changwon.GhostState.RETURN);
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
            yield return new WaitForSeconds(0.5f); // 초 동안 Ghost 레이어가 보임

            // Ghost 레이어를 제외하는 부분
            Camera.main.cullingMask &= ~(1 << ghostLayer);
            yield return new WaitForSeconds(0.5f); // 초 동안 Ghost 레이어가 숨김
        }
    }

    IEnumerator ghostInter()
    {
        CMultiPlayer player = FindObjectOfType<CMultiPlayer>();
        
        
        
        switch (ghostType)
        {
            case GhostType.BANSHEE:
                player.sprintMultiplier = 1f;
                break;
            case GhostType.NIGHTMARE:
                
                
                float LightBlinkTargetDistance = Vector3.Distance(target.transform.position, transform.position);
                if (LightBlinkTargetDistance<10)
                {
                    player.GetComponentInChildren<Light>().enabled = true;
                    yield return new WaitForSeconds(1f);
                    player.GetComponentInChildren<Light>().enabled = false;
                    
                }
                break;
        }
        yield return null;
    }

}
