using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Wonbin
{
    public enum _GhostState
    {
        IDLE,
        RETURN,
        HUNTTING
    }

    public enum _GhostType
    {
        NIGHTMARE,
        BANSHEE,
        DEMON
    }

    public class _Ghost : MonoBehaviour
    {
        public _GhostState state;
        public _GhostType ghostType;
        public NavMeshAgent ghostNav;
        public GameObject target;
        public Transform returnpos;

        private void Awake()
        {
            if (ghostNav == null)
            {
                Debug.LogWarning("Ghost NavMeshAgent not assigned. Disabling the _Ghost script.");
                this.enabled = false; // Disable this script if the ghostNav is not assigned.
                return;
            }

            ghosttypeRandom(Random.Range(0, 3));

            // Assign ghost speed based on type
            switch (ghostType)
            {
                case _GhostType.NIGHTMARE:
                    ghostNav.speed = 5f;
                    break;
                case _GhostType.BANSHEE:
                    ghostNav.speed = 7f;
                    break;
                case _GhostType.DEMON:
                    ghostNav.speed = 10f;
                    break;
            }
        }

        private void Update()
        {
            // Ensure the NavMeshAgent is assigned
            if (!this.enabled) return;

            target = GameObject.FindGameObjectWithTag("Player");
            StartCoroutine(StateMechine());
        }

        public IEnumerator StateMechine()
        {
            switch (state)
            {
                case _GhostState.IDLE:
                    yield return StartCoroutine(idle());
                    break;

                case _GhostState.HUNTTING:
                    yield return StartCoroutine(Hunting());
                    break;
            }
        }

        public void ChangeState(_GhostState newState)
        {
            state = newState;
        }

        private IEnumerator idle()
        {
            while (state == _GhostState.IDLE)
            {
                ghostNav.isStopped = true;

                // Example condition to change state
                /* if (player mental gauge condition) {
                    ChangeState(_GhostState.HUNTTING);
                }*/

                yield return null;
            }
        }

        private IEnumerator returnPosition()
        {
            while (state == _GhostState.RETURN)
            {
                ghostNav.SetDestination(returnpos.position);
                yield return null;
            }
        }

        private IEnumerator Hunting()
        {
            while (state == _GhostState.HUNTTING)
            {
                if (target != null)
                {
                    ghostNav.isStopped = false;
                    ghostNav.SetDestination(target.transform.position);

                    float huntingTargetDistance = Vector3.Distance(target.transform.position, transform.position);
                    if (huntingTargetDistance < 1)
                    {
                        Debug.Log("플레이어를 찾았다"); // Player found
                        // Trigger player kill logic (e.g., call the death animation)
                        ghostNav.isStopped = true;
                        ChangeState(_GhostState.RETURN);

                        yield return new WaitForSeconds(30f);
                        ChangeState(_GhostState.HUNTTING);
                        yield return new WaitForSeconds(30f);
                        ChangeState(_GhostState.IDLE);
                    }

                    yield return new WaitForSeconds(30f);
                }
                else
                {
                    ChangeState(_GhostState.RETURN);
                }
            }
        }

        public void ghosttypeRandom(int value)
        {
            ghostType = (_GhostType)value;
        }
    }
}
