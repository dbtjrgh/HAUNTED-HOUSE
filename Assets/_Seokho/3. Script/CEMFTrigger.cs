using changwon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEMFTrigger : MonoBehaviour
{
    public _EMF parentEMF; // 부모 오브젝트의 _EMF 스크립트를 참조

    private void OnTriggerEnter(Collider other)
    {
        if (parentEMF != null)
        {
            parentEMF.OnChildTriggerEnter(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (parentEMF != null)
        {
            parentEMF.OnChildTriggerStay(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (parentEMF != null)
        {
            parentEMF.OnChildTriggerExit(other);
        }
    }
}
