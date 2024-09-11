using System.Collections;
using UnityEngine;
using Photon.Pun;
using static myRooms.Rooms;
using GameFeatures;

public class mentalGaugeManager : MonoBehaviourPun, IPunObservable
{
    [SerializeField]
    private RoomIdentifire currentPlayerRoom; // RoomIdentifire ������Ʈ ����

    public float maxMentalGauge = 100f;
    public float secondGaugeMinus = 0.5f;
    public float ghostRoomGaugeMinus = 10f; // ��Ʈ ��
    public float changeRoomGaugeMinus = 5f;
    public float MentalGauge;
    private float gaugeModifier = 1f; // ���̵��� ���� �ٸ���

    private void Start()
    {
        MentalGauge = maxMentalGauge;

        // currentPlayerRoom�� null�̸�, ���� GameObject���� RoomIdentifire�� ã��
        if (currentPlayerRoom == null)
        {
            currentPlayerRoom = GetComponent<RoomIdentifire>();
        }

        if (currentPlayerRoom == null)
        {
            Debug.LogError("currentPlayerRoom�� �������� �ʾҽ��ϴ�! RoomIdentifire ������Ʈ�� Ȯ���ϼ���.");
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
            Debug.Log("��Ż ������ ����: " + ToTake + " ���� ������: " + MentalGauge);
        }
        else
        {
            Debug.LogWarning("�� �÷��̾��� ��Ż �������� �ƴմϴ�.");
        }
    }

    public void AddMentalGauge(float ToAdd)
    {
        if (photonView.IsMine && currentPlayerRoom != null && currentPlayerRoom.CurrRoom == RoomsEnum.NormalRoom)
        {
            MentalGauge += ToAdd / gaugeModifier;
            MentalGauge = Mathf.Clamp(MentalGauge, 0, maxMentalGauge);
            Debug.Log("��Ż ������ �߰�: " + ToAdd + " ���� ������: " + MentalGauge);
        }
    }

    public void SecondTakeMentalGauge()
    {
        if (photonView.IsMine && currentPlayerRoom != null)
        {
            MentalGauge -= (secondGaugeMinus * gaugeModifier);
            MentalGauge = Mathf.Clamp(MentalGauge, 0, maxMentalGauge);
            Debug.Log("�ʴ� ��Ż ������ ����: " + secondGaugeMinus + " ���� ������: " + MentalGauge);
        }
        else
        {
            Debug.LogWarning("�� �÷��̾��� ��Ż �������� �ƴմϴ�.");
        }
    }

    private void DropMentalGauge()
    {
        if (photonView.IsMine && currentPlayerRoom != null)
        {
            // ��Ʈ �濡���� ������ ���Ҵ� ChangeRoom���� ó����
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
