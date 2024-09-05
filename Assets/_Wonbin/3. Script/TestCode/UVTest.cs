using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVTest : MonoBehaviour
{

    public Transform InteractionTransform;  // 상호작용 위치
    public GameObject handprintPrefab;      // 손발자국 프리팹




    public void LeavePrintsUV()
    {
        Instantiate(handprintPrefab, InteractionTransform.position, InteractionTransform.rotation, InteractionTransform);
    }

}
