
using changwon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMFTrigger : MonoBehaviour
{
    
    changwon.EMF emf;

    private void Start()
    {
        emf = GameObject.Find("EMF").GetComponent<changwon.EMF>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (emf.EMFOnOff == true)
        {
            print(emf.EMFOnOff);
            print(Ghost.instance.state);
            print(Ghost.instance.ghostType);
            if (other.gameObject.tag=="Ghost")
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

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ghost")
        {
            emf.lights[0].gameObject.SetActive(true);
        }
    }
}

