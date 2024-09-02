using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.AI;


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
    public Transform returnpos;
    mentalGaugeManager mental;



    private void Awake()
    {
        instance = this;
        ghosttypeRandom(value: Random.Range(0, 3));
        switch (ghostType)
        {
            case GhostType.NIGHTMARE:
                ghostNav.speed = 3f;
                break;
            case GhostType.BANSHEE:
                ghostNav.speed = 5f;
                break;
            case GhostType.DEMON:
                ghostNav.speed = 7f;
                break;
        }
        

    }

    

    private void Update()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        returnpos = GameObject.FindGameObjectWithTag("GhostSpawnPoint").transform;

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
        mental =FindObjectOfType<mentalGaugeManager>();
        while (state == changwon.GhostState.IDLE)
        {
            ghostNav.isStopped = true;
            if(mental!=null&&mental.MentalGauge==50)
            {
                ChangeState(changwon.GhostState.HUNTTING);
            }

            else if (mental == null)
            {
                Debug.LogError("mental is null");
            }
            yield return null;

        }

    }

    private IEnumerator returnPosition()
    {

        while (state == changwon.GhostState.RETURN)
        {
            {
                ghostNav.isStopped = false;
                ghostNav.SetDestination(returnpos.position);
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
            
            if (target != null)
            {

                ghostNav.isStopped = false;
                ghostNav.SetDestination(target.transform.position);
                float HunttingTargetDistance = Vector3.Distance(target.transform.position, transform.position);
                if (HunttingTargetDistance < 1)
                {
                    Debug.Log("플레이어를 찾았다");             //플레이어 킬
                    Destroy(target);
                    ghostNav.isStopped = true;
                    ChangeState(changwon.GhostState.RETURN);
                    yield return new WaitForSeconds(30f);
                    ChangeState(changwon.GhostState.HUNTTING);
                    yield return new WaitForSeconds(30f);
                    ChangeState(changwon.GhostState.IDLE);
                    StartCoroutine(ghostBlink());
                }

                


                else/*else if 플레이어*/
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

        
        Camera.main.cullingMask ^= 1 << LayerMask.NameToLayer("ghostBlink");
        yield return new WaitForSeconds(1f);
        Camera.main.cullingMask^=~(1<<LayerMask.NameToLayer("ghostBlink")); 

    }








}
