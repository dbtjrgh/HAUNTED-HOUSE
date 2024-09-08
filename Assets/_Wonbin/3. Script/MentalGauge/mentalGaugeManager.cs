using System.Collections;
using UnityEngine;
using Photon.Pun;
using static myRooms.Rooms;
using GameFeatures;

public class mentalGaugeManager : MonoBehaviourPun, IPunObservable
{
    [SerializeField]
    private RoomIdentifire currentPlayerRoom; // RoomIdentifire 컴포넌트 참조

    public float maxMentalGauge = 100f;
    public float secondGaugeMinus;
    public float ghostRoomGaugeMinus;
    public float MentalGauge;
    private float gaugeModifier = 1f;

    private Coroutine dropCoroutine;

    private void Start()
    {
        MentalGauge = maxMentalGauge;

        // currentPlayerRoom이 null이면, 같은 GameObject에서 RoomIdentifire를 찾음
        if (currentPlayerRoom == null)
        {
            currentPlayerRoom = GetComponent<RoomIdentifire>();
        }

        // currentPlayerRoom이 null인지 확인
        if (currentPlayerRoom == null)
        {
            Debug.LogError("currentPlayerRoom이 설정되지 않았습니다! RoomIdentifire 컴포넌트를 확인하세요.");
            return; // currentPlayerRoom이 없으면 더 이상 진행하지 않음
        }

        if (photonView.IsMine)
        {
            Debug.Log("멘탈 게이지 관리 시작");
            dropCoroutine = StartCoroutine(DropGaugeRoutine());
        }
        else
        {
            Debug.Log("멘탈 게이지 관리 대상이 아님");
        }
    }

    public void TakeMentalGauge(float ToTake)
    {
        if (photonView.IsMine) // Only update if this is the local player
        {
            MentalGauge -= (ToTake * gaugeModifier);
            MentalGauge = Mathf.Clamp(MentalGauge, 0, maxMentalGauge);
            Debug.Log("멘탈 게이지 감소: " + ToTake + " 남은 게이지: " + MentalGauge);
        }
        else
        {
            Debug.LogWarning("이 플레이어의 멘탈 게이지가 아닙니다.");
        }
    }

    public void AddMentalGauge(float ToAdd)
    {
        if (photonView.IsMine && currentPlayerRoom != null && currentPlayerRoom.CurrRoom == RoomsEnum.NormalRoom)
        {
            MentalGauge += ToAdd / gaugeModifier;
            MentalGauge = Mathf.Clamp(MentalGauge, 0, maxMentalGauge);
            Debug.Log("멘탈 게이지 추가: " + ToAdd + " 남은 게이지: " + MentalGauge);
        }
    }

    private void DropMentalGauge()
    {
        if (photonView.IsMine && currentPlayerRoom != null)
        {
            if (currentPlayerRoom.CurrRoom != RoomsEnum.NormalRoom)
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
