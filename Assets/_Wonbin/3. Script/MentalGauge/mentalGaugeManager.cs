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
    public float secondGaugeMinus = 0.5f;
    public float ghostRoomGaugeMinus = 10f; // 고스트 방
    public float changeRoomGaugeMinus = 5f;
    public float MentalGauge;
    private float gaugeModifier = 1f; // 난이도에 따라 다르게

    private void Start()
    {
        MentalGauge = maxMentalGauge;

        // currentPlayerRoom이 null이면, 같은 GameObject에서 RoomIdentifire를 찾음
        if (currentPlayerRoom == null)
        {
            currentPlayerRoom = GetComponent<RoomIdentifire>();
        }

        if (currentPlayerRoom == null)
        {
            Debug.LogError("currentPlayerRoom이 설정되지 않았습니다! RoomIdentifire 컴포넌트를 확인하세요.");
            return;
        }

        if (photonView.IsMine)
        {
            StartCoroutine(DropGaugeRoutine());
        }
        else
        {
            return;
        }
    }

    public void TakeMentalGauge(float ToTake)
    {
        if (photonView.IsMine)
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

    public void SecondTakeMentalGauge()
    {
        if (photonView.IsMine && currentPlayerRoom != null)
        {
            MentalGauge -= (secondGaugeMinus * gaugeModifier);
            MentalGauge = Mathf.Clamp(MentalGauge, 0, maxMentalGauge);
            Debug.Log("초당 멘탈 게이지 감소: " + secondGaugeMinus + " 남은 게이지: " + MentalGauge);
        }
        else
        {
            Debug.LogWarning("이 플레이어의 멘탈 게이지가 아닙니다.");
        }
    }

    private void DropMentalGauge()
    {
        if (photonView.IsMine && currentPlayerRoom != null)
        {
            // 고스트 방에서의 게이지 감소는 ChangeRoom에서 처리됨
            if (currentPlayerRoom.CurrRoom != RoomsEnum.NormalRoom)
            {
                SecondTakeMentalGauge();
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
