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
    public float secondGaugeMinus;
    public float ghostRoomGaugeMinus;
    public float MentalGauge;
    private float gaugeModifier = 1f;

    private Coroutine dropCoroutine;

    private void Start()
    {
        MentalGauge = maxMentalGauge;

        // currentPlayerRoom�� null�̸�, ���� GameObject���� RoomIdentifire�� ã��
        if (currentPlayerRoom == null)
        {
            currentPlayerRoom = GetComponent<RoomIdentifire>();
        }

        // currentPlayerRoom�� null���� Ȯ��
        if (currentPlayerRoom == null)
        {
            Debug.LogError("currentPlayerRoom�� �������� �ʾҽ��ϴ�! RoomIdentifire ������Ʈ�� Ȯ���ϼ���.");
            return; // currentPlayerRoom�� ������ �� �̻� �������� ����
        }

        if (photonView.IsMine)
        {
            Debug.Log("��Ż ������ ���� ����");
            dropCoroutine = StartCoroutine(DropGaugeRoutine());
        }
        else
        {
            Debug.Log("��Ż ������ ���� ����� �ƴ�");
        }
    }

    public void TakeMentalGauge(float ToTake)
    {
        if (photonView.IsMine) // Only update if this is the local player
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
