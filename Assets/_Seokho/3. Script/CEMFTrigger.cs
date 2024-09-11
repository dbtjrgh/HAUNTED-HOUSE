using changwon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEMFTrigger : MonoBehaviour
{
    public _EMF parentEMF; // �θ� ������Ʈ�� _EMF ��ũ��Ʈ�� ����

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
