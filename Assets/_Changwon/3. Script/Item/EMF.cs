using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace changwon
{
    public class EMF : MonoBehaviour
    {
        public Collider interaction;
        
        public Light[] lights;

        Ghost ghost;

        private bool EMFOnOff = false;


        private void Awake()
        {
            ghost = GetComponent<Ghost>();
            
            
        }

        private void Update()
        {
            EMFSwitching();
        }

        public void EMFSwitching()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (EMFOnOff == false)
                {
                    EMFOnOff = true;
                    lights[0].gameObject.SetActive(true);
                }
                else
                {
                    EMFOnOff = false;
                    lights[0].gameObject.SetActive(false);
                }
            }
        }



        private void OnTriggerStay(Collider other)
        {
            other=interaction;
            if (EMFOnOff == true)
            {
                if (other.tag == "Ghost")
                {
                    if (ghost.state == GhostState.HUNTTING)
                    {
                        for (int i = 0; i < lights.Length; i++)
                        {
                            lights[i].gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        switch (ghost.ghostType)
                        {
                            case GhostType.BANSHEE:

                                
                                break;

                            case GhostType.NIGHTMARE:
                                for (int i = 0; i < 3; i++)
                                {
                                    lights[i].gameObject.SetActive(true);
                                }
                                break;

                            case GhostType.DEMON:
                                for (int i = 0; i < lights.Length; i++)
                                {
                                    lights[i].gameObject.SetActive(true);
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
                lights[0].gameObject.SetActive(true);
            }
        }
    }



}