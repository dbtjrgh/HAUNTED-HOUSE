using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using myRooms;
using RoomLogic;
using System.Runtime.CompilerServices;

public class mentalGaugeManager : MonoBehaviour
{

    [SerializeField]
    private RoomIdentifire currentPlayerRoom;

    [SerializeField]
    private float maxMentalGauge = 100;

    [SerializeField]
    private float normalRoomGaugeMinus;

    [SerializeField]
    private float secondGaugeMinus;

    [SerializeField]
    private float ghostRoomGaugeMinus;

    private Rooms.RoomsEnum currentRoom = Rooms.RoomsEnum.NormalRoom;

    public float MentalGauge;

    private float gaugeModifier = 1f;


    private void Start()
    {
        MentalGauge = maxMentalGauge;
    }

    public void TakeMentalGauge(float ToTake)
    {
        MentalGauge -= (ToTake * gaugeModifier);
        MentalGauge = Mathf.Clamp(MentalGauge, 0, maxMentalGauge);

    }

    public void AddMentalGauge(float ToAdd)
    {
        MentalGauge += (ToAdd / gaugeModifier);
        MentalGauge = Mathf.Clamp(MentalGauge, 0, maxMentalGauge);
    }

    private void DropMentalGauge()
    {
        if (currentPlayerRoom.CurrRoom == Rooms.RoomsEnum.NormalRoom)
            return;

        //else if (currentPlayerRoom.CurrRoom == GhostRoom)
        //{
        //    TakeMentalGauge(ghostRoomGaugeMinus);
        //}
        else
        {
            TakeMentalGauge(normalRoomGaugeMinus);
        }

    }

    private IEnumerator dropGaugeSecondTime()
    {
        while (true)
        {
            DropMentalGauge();
            yield return new WaitForSeconds(1);
        }
    }

    private void Update()
    {
       //secondGaugeMinus =
       //ghostRoomGaugeMinus =
        
        StartCoroutine(dropGaugeSecondTime());

    }




}


