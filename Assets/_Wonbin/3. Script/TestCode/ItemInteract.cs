using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteract : MonoBehaviour
{
    private RaycastHit hit;
    private Ray ray;

    private void Update()
    {
        objectInteract();
    }


    void objectInteract()
    {
        ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //플레이어로 부터 ray를 생성.

        if(Physics.Raycast(ray, out hit))
        {
            if(hit.collider.gameObject.tag == "FlashLight")
            {
                Debug.Log("ray가 손전등을 인식했습니다."); //오브젝트가 아이템을 인식했을 경우.
                flashLight.lightCheck();
            }
        }
    }


}
