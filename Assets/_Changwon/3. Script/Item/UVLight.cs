using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace changwon
{
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(PhotonTransformView))]
    [RequireComponent(typeof(PhotonRigidbodyView))]
    public class UVLight : MonoBehaviourPun
    {
        public Light uvlight;
        public Material revealableMaterial;
        public float lightAngle = 360f;

        private Vector3 curpos;
        private Vector3 lastpos = new Vector3(0f, 0f, 0f);
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

            // Raycast로 빛이 닿는 곳에만 손자국이 드러나도록 설정
            RaycastHit hit;
            if (Physics.Raycast(uvlight.transform.position, uvlight.transform.forward, out hit))
            {
                revealableMaterial.SetVector("lightPosition", hit.point);
                revealableMaterial.SetVector("lightDirection", uvlight.transform.forward);
            }
        }

        private void ChangeMaterialParameters()
        {
            revealableMaterial.SetVector("lightPosition", uvlight.transform.position);
            revealableMaterial.SetVector("lightDirection", uvlight.transform.forward);
        }
    }
}
