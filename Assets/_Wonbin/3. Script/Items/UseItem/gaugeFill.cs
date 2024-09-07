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
    static bool useFill; // 사용 가능한 상태를 의미하는 변수

    private void Start()
    {
        // Player 오브젝트와 mentalGaugeManager 컴포넌트를 안전하게 가져옴
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerMentalGauge = playerObject.GetComponent<mentalGaugeManager>();

            if (playerMentalGauge == null)
            {
                Debug.LogError("Player 오브젝트에 mentalGaugeManager 컴포넌트가 없습니다!");
            }
        }
        else
        {
            Debug.LogError("Player 오브젝트를 찾을 수 없습니다! 'Player' 태그가 제대로 설정되었는지 확인하세요.");
        }

        getFill = false;
        isInItemSlot = false;
        useFill = true;

        // ItemSlot 오브젝트 찾기
        itemSlotTransform = GameObject.Find("ItemSlot")?.transform;

        if (itemSlotTransform == null)
        {
            Debug.LogError("ItemSlot 오브젝트를 찾을 수 없습니다!");
        }
    }

   

    private void Update()
    {
        // itemSlotTransform이 null이 아닌지 확인
        if (itemSlotTransform != null)
        {
            isInItemSlot = transform.IsChildOf(itemSlotTransform);

            if (Input.GetKeyDown(KeyCode.R))
            {
                // MentalGauge가 이미 최대치라면 사용 불가능
                if (isInItemSlot && playerMentalGauge != null && playerMentalGauge.MentalGauge >= playerMentalGauge.maxMentalGauge)
                {
                    useFill = false;
                    Debug.Log("정신력 게이지가 최대치입니다.");
                }
                else if (isInItemSlot && useFill == true)
                {
                    fillUse();
                    Debug.Log("정신력 게이지가 채워졌습니다.");
                }
            }
        }
    }

    public void fillUse() // 멘탈 게이지 약 사용
    {
        if (playerMentalGauge != null)
        {
            playerMentalGauge.AddMentalGauge(ToAdd);
            Destroy(this.gameObject);
        }
    }
}
