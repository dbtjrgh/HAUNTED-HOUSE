using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVTest : MonoBehaviour
{

    public Transform InteractionTransform;  // ��ȣ�ۿ� ��ġ
    public GameObject handprintPrefab;      // �չ��ڱ� ������




    public void LeavePrintsUV()
    {
        Instantiate(handprintPrefab, InteractionTransform.position, InteractionTransform.rotation, InteractionTransform);
    }

}
