using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gaugeFill : MonoBehaviour
{
    mentalGaugeManager playerMentalGauge;

    [SerializeField]
    private GameObject _Player;

    private float ToAdd = 30;
    static bool getFill;
    public static bool isInItemSlot;
    private Transform itemSlotTransform;
    static bool useFill; //��� ������ ���¸� �ǹ��ϴ� ����


    private void Start()
    {
        playerMentalGauge = GameObject.Find("Player").GetComponent<mentalGaugeManager>();
        getFill = false;
        isInItemSlot = false;
        useFill = true;
        itemSlotTransform = GameObject.Find("ItemSlot")?.transform;
    }



    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (itemSlotTransform == null)
            {
                return;
            }

            else if (playerMentalGauge.MentalGauge > playerMentalGauge.maxMentalGauge)
            {
                useFill = false;
                Debug.Log("���ŷ� �������� �ִ�ġ�Դϴ�.");

            }


            bool isInItemSlot = transform.IsChildOf(itemSlotTransform);

            if (isInItemSlot && useFill == true)
            {
                fillUse();
                Debug.Log("���ŷ� �������� ä�������ϴ�.");
            }
        }
    }

    public void fillUse() // ��Ż ������ �� ���
    {
        playerMentalGauge.AddMentalGauge(ToAdd);
        Destroy(this.gameObject);
    }

}
