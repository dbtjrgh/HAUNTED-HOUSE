using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace changwon
{
    public class UVLight : MonoBehaviour
    {
        public Light uvlight;
        public Material revealableMaterial;
        public float lightAngle = 360f;

        private Vector3 curpos;
        private Vector3 lastpos=new Vector3(0f,0f,0f);
        public float epsilon = 0.001f;
        private float distance;

        private bool isEnabled = false;

        private void Start()
        {
            lastpos = transform.position;
            DisableUVLight();
        }

        IEnumerator CheckPosition()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.02f);
                if (isEnabled)
                {
                    curpos = transform.position;
                    distance = (lastpos - curpos).magnitude;

                    if (distance > epsilon)
                    {
                        lastpos = curpos;
                        ChangeMaterialParameters();
                    }
                }
            }
        }

        public void DisableUVLight()
        {
            revealableMaterial.SetFloat("lightAngle", 0f);
            ChangeMaterialParameters();
            isEnabled = false;
            StopCoroutine(CheckPosition());
        }

        public void EnableUVLight()
        {
            revealableMaterial.SetFloat("lightAngle", lightAngle);
            ChangeMaterialParameters();
            isEnabled = true;
            StartCoroutine(CheckPosition());
        }

        private void ChangeMaterialParameters()
        {
            revealableMaterial.SetVector("lightPosition", uvlight.transform.position);
            revealableMaterial.SetVector("lightDirection", uvlight.transform.forward);
        }
    }
}
