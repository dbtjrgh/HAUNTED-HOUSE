

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMFTrigger : MonoBehaviour
{
    public changwon._EMF emf;
    public GameObject ghost;

    private void Start()
    {
        emf = GameObject.FindObjectOfType<changwon._EMF>();
        ghost = GameObject.Find("ghost1529(Clone)");
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ghost"))
        {
            float TargetDistance = Vector3.Distance(ghost.transform.position, transform.position);
            if (emf.EMFOn == true)
            {
                if (TargetDistance < 5)
                {
                    if (Ghost.instance.state == changwon.GhostState.HUNTTING)
                    {
                        for (int i = 0; i < emf.lights.Length; i++)
                        {
                            emf.lights[i].gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        switch (Ghost.instance.ghostType)
                        {
                            case GhostType.BANSHEE:
                                break;

                            case GhostType.NIGHTMARE:

                                for (int i = 0; i < 3; i++)
                                {
                                    emf.lights[i].gameObject.SetActive(true);
                                }
                                break;

                            case GhostType.DEMON:

                                for (int i = 0; i < emf.lights.Length; i++)
                                {
                                    emf.lights[i].gameObject.SetActive(true);
                                }
                                break;
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ghost"))
        {
            for (int i = 1; i < emf.lights.Length; i++)
            {
                emf.lights[i].gameObject.SetActive(false);
            }
            emf.lights[0].gameObject.SetActive(true);
        }
    }
}