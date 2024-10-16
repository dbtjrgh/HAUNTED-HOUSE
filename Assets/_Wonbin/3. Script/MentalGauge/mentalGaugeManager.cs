using System.Collections;
using UnityEngine;
using Photon.Pun;
using static myRooms.Rooms;
using GameFeatures;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class mentalGaugeManager : MonoBehaviourPunCallbacks, IPunObservable
{
    #region
    [SerializeField]
    private RoomIdentifire currentPlayerRoom; // RoomIdentifire ������Ʈ ����
    public float maxMentalGauge;
    public float secondGaugeMinus;
    public float ghostRoomGaugeMinus; // ��Ʈ ��
    public float changeRoomGaugeMinus;
    public float MentalGauge;
    private float gaugeModifier = 1f; // ���̵��� ���� �ٸ���
    string diffText;
    #endregion

    private void Awake()
    {
        CRoomScreen roomScreen = FindObjectOfType<CRoomScreen>();
        maxMentalGauge = 100f;
        
    }

    public override void OnRoomPropertiesUpdate(PhotonHashtable props)
    {
        props = PhotonNetwork.CurrentRoom.CustomProperties;

        if (props.ContainsKey("Diff"))
        {
            diffText = ((Difficulty)props["Diff"]).ToString();
        }
        if(diffText == "Easy")
        {
            secondGaugeMinus = 0.20f;
            changeRoomGaugeMinus = 2f;
        }
        else if (diffText == "Normal")
        {
            secondGaugeMinus = 0.30f;
            changeRoomGaugeMinus = 3f;
        }
        else if(diffText == "Hard")
        {
            secondGaugeMinus = 0.50f;
            changeRoomGaugeMinus = 5f;
        }
        ghostRoomGaugeMinus = changeRoomGaugeMinus * 2;
    }

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
