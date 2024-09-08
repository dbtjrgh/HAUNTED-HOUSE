using System.Collections;
using UnityEngine;
using Photon.Pun;
using myRooms;
using RoomLogic;

public class mentalGaugeManager : MonoBehaviourPun, IPunObservable
{
    [SerializeField]
    private RoomIdentifire currentPlayerRoom;

    public float maxMentalGauge = 100f;
    public float secondGaugeMinus = 2f;
    public float ghostRoomGaugeMinus = 5f;
    public float MentalGauge;
    private float gaugeModifier = 1f;

    private Coroutine dropCoroutine;

    private void Start()
    {
        MentalGauge = maxMentalGauge;

        if (photonView.IsMine)
        {
            dropCoroutine = StartCoroutine(DropGaugeRoutine());
        }
    }

    public void TakeMentalGauge(float ToTake)
    {
        if (photonView.IsMine) // Only update if this is the local player
        {
            MentalGauge -= (ToTake * gaugeModifier);
            MentalGauge = Mathf.Clamp(MentalGauge, 0, maxMentalGauge);
        }
    }

    public void AddMentalGauge(float ToAdd)
    {
        if (photonView.IsMine && currentPlayerRoom.CurrRoom == Rooms.RoomsEnum.NormalRoom)
        {
            MentalGauge += ToAdd / gaugeModifier;
            MentalGauge = Mathf.Clamp(MentalGauge, 0, maxMentalGauge);
        }
    }

    private void DropMentalGauge()
    {
        if (photonView.IsMine)
        {
            if (currentPlayerRoom.CurrRoom != Rooms.RoomsEnum.NormalRoom)
            {
                TakeMentalGauge(secondGaugeMinus);
            }
        }
    }

    private IEnumerator DropGaugeRoutine()
    {
        while (true)
        {
            DropMentalGauge();
            yield return new WaitForSeconds(2f);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(MentalGauge);
        }
        else
        {
            MentalGauge = (float)stream.ReceiveNext();
        }
    }
}
