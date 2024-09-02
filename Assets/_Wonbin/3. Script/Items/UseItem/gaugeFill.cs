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
    static bool useFill; //사용 가능한 상태를 의미하는 변수


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
                Debug.Log("정신력 게이지가 최대치입니다.");

            }


            bool isInItemSlot = transform.IsChildOf(itemSlotTransform);

            if (isInItemSlot && useFill == true)
            {
                fillUse();
                Debug.Log("정신력 게이지가 채워졌습니다.");
            }
        }
    }

    public void fillUse() // 멘탈 게이지 약 사용
    {
        playerMentalGauge.AddMentalGauge(ToAdd);
        Destroy(this.gameObject);
    }

}
