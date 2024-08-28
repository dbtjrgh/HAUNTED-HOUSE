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
        ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //�÷��̾�� ���� ray�� ����.

        if(Physics.Raycast(ray, out hit))
        {
            if(hit.collider.gameObject.tag == "FlashLight")
            {
                Debug.Log("ray�� �������� �ν��߽��ϴ�."); //������Ʈ�� �������� �ν����� ���.
                flashLight.lightCheck();
            }
        }
    }


}
