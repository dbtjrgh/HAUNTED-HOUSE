using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace changwon
{
    public class EMF : MonoBehaviour
    {
        public Light[] lights;
        public bool EMFOnOff = false;




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







    }
}