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
    public float secondGaugeMinus = 5f; // �Ϲ� �濡 ���� ��, ��Ż�������� �����ϴ� �ӵ�
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
        Debug.Log("��Ż������ ���� ���� �ߵ�");
        Debug.Log("ToTake : " + ToTake);
        Debug.Log("gaugeModifier: " + gaugeModifier);  // gaugeModifier Ȯ��

        MentalGauge -= (ToTake * gaugeModifier);
        MentalGauge = Mathf.Clamp(MentalGauge, 0, maxMentalGauge);

        Debug.Log("Updated MentalGauge: " + MentalGauge);  // MentalGauge �� Ȯ��
    }

    public void AddMentalGauge(float ToAdd)
    {
        if (currentPlayerRoom.CurrRoom == Rooms.RoomsEnum.NormalRoom)
        {
            Debug.Log("��Ż������ ���� ���� �ߵ�");
            MentalGauge += (ToAdd / gaugeModifier);
            MentalGauge = Mathf.Clamp(MentalGauge, 0, maxMentalGauge);
        }
    }

    private void DropMentalGauge()
    {
        if (currentPlayerRoom.CurrRoom != Rooms.RoomsEnum.NormalRoom)
        {
            Debug.Log("���� �÷��̾��� ���� ? : " + currentPlayerRoom.CurrRoom);
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