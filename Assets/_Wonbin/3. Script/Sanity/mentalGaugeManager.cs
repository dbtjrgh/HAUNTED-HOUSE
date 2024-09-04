using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using myRooms;
using RoomLogic;

public class mentalGaugeManager : MonoBehaviour
{
    [SerializeField]
    private RoomIdentifire currentPlayerRoom;

    public float maxMentalGauge = 100;
    public float secondGaugeMinus = 5f; // 일반 방에 있을 때, 멘탈게이지가 감소하는 속도
    public float ghostRoomGaugeMinus;
    public float MentalGauge;
    private float gaugeModifier = 1f;

    private Coroutine dropCoroutine;
    private Coroutine addCoroutine;

    private void Start()
    {
        MentalGauge = maxMentalGauge;
        dropCoroutine = StartCoroutine(DropGaugeRoutine());
        addCoroutine = StartCoroutine(AddGaugeRoutine());
    }


    public void TakeMentalGauge(float ToTake)
    {
        Debug.Log("멘탈게이지 감소 로직 발동");
        Debug.Log("ToTake : " + ToTake);
        Debug.Log("gaugeModifier: " + gaugeModifier);  // gaugeModifier 확인

        MentalGauge -= (ToTake * gaugeModifier);
        MentalGauge = Mathf.Clamp(MentalGauge, 0, maxMentalGauge);

        Debug.Log("Updated MentalGauge: " + MentalGauge);  // MentalGauge 값 확인
    }

    public void AddMentalGauge(float ToAdd)
    {
        if (currentPlayerRoom.CurrRoom == Rooms.RoomsEnum.NormalRoom)
        {
            Debug.Log("멘탈게이지 증가 로직 발동");
            MentalGauge += (ToAdd / gaugeModifier);
            MentalGauge = Mathf.Clamp(MentalGauge, 0, maxMentalGauge);
        }
    }

    private void DropMentalGauge()
    {
        if (currentPlayerRoom.CurrRoom != Rooms.RoomsEnum.NormalRoom)
        {
            Debug.Log("현재 플레이어의 방은 ? : " + currentPlayerRoom.CurrRoom);
            TakeMentalGauge(secondGaugeMinus);
        }
    }

    private IEnumerator DropGaugeRoutine()
    {
        while (true)
        {
            DropMentalGauge();
            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator AddGaugeRoutine()
    {
        while (true)
        {
            AddMentalGauge(gaugeModifier);
            yield return new WaitForSeconds(2);
        }
    }
}