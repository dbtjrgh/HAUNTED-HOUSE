using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace changwon
{
    public enum GhostState
    {
        IDLE,
        EVENT,
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
    
    public changwon.GhostState state;
    public NavMeshAgent ghostNav;
    public Transform target;





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
        }

    }

    public void ChangeState(changwon.GhostState newstate)
    {
        state = newstate;
    }

    private IEnumerator idle()
    {
        while (state == changwon.GhostState.IDLE)
        {
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
                ghostNav.SetDestination(target.position);
                float HunttingTargetDistance = Vector3.Distance(target.position, transform.position);
                if (HunttingTargetDistance < 1)
                {
                    Debug.Log("플레이어를 찾았다");
                    ghostNav.isStopped = true;
                }
                else
                {
                    ghostNav.isStopped = false;
                }
            }
            yield return null;
        }
    }


    

    

    public void hunttime()
    {
        ChangeState(changwon.GhostState.HUNTTING);
        StartCoroutine(Hunting());
    }

    public void idleTime()
    {
        ChangeState(changwon.GhostState.IDLE);
        StartCoroutine(idle());
    }
}
