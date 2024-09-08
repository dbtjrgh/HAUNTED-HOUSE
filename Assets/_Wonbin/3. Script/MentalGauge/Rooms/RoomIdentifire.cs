using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using myRooms;

namespace RoomLogic
{
    [RequireComponent(typeof(Collider))]
    public class RoomIdentifire : MonoBehaviour
    {
        public Action OnRoomChanged;
        public Rooms.RoomsEnum CurrRoom = Rooms.RoomsEnum.NormalRoom;
        public List<Rooms.RoomsEnum> _nextRooms = new List<Rooms.RoomsEnum>();
        private mentalGaugeManager mentalGaugeManager;

        private void Start()
        {
            // mentalGaugeManager ������Ʈ�� �÷��̾�Լ� ������
            mentalGaugeManager = GetComponent<mentalGaugeManager>();

            // mentalGaugeManager�� ����� �����Ǿ����� Ȯ��
            if (mentalGaugeManager == null)
            {
                Debug.LogError("mentalGaugeManager ������Ʈ�� ã�� �� �����ϴ�!");
            }
            else
            {
                Debug.Log("mentalGaugeManager�� ���������� ����Ǿ����ϴ�.");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Room roomComponent = other.GetComponent<Room>();
            Debug.Log("�ٸ� �濡 ���Խ��ϴ�. Trigger Enter: " + other.name);

            if (roomComponent != null)
            {
                Debug.Log("RoomComponent�� ã�ҽ��ϴ�. �� �ٲٱ� ���� ����: " + roomComponent.RoomType);
                if (CurrRoom == Rooms.RoomsEnum.NormalRoom)
                {
                    ChangeRoom(roomComponent.RoomType);
                }
                else
                {
                    _nextRooms.Add(roomComponent.RoomType);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<Room>() != null)
            {
                Debug.Log("�濡�� �������ϴ�. Trigger Exit: " + other.name);

                if (CurrRoom == other.GetComponent<Room>().RoomType)
                {
                    if (_nextRooms.Count > 0)
                    {
                        ChangeRoom(_nextRooms[0]);
                        _nextRooms.Remove(CurrRoom);
                    }
                    else
                    {
                        ChangeRoom(Rooms.RoomsEnum.NormalRoom);
                    }
                }
                else if (_nextRooms.Contains(other.GetComponent<Room>().RoomType))
                {
                    _nextRooms.Remove(other.GetComponent<Room>().RoomType);
                }
            }
        }

        private void ChangeRoom(Rooms.RoomsEnum NextRoom)
        {
            Debug.Log($"���� �����մϴ�. ���� ��: {CurrRoom}, ���� ��: {NextRoom}");
            CurrRoom = NextRoom;
            OnRoomChanged?.Invoke();

            // ���� �ٲ� ������ ��Ż ������ ����
            if (mentalGaugeManager != null && CurrRoom != Rooms.RoomsEnum.NormalRoom)
            {
                Debug.Log("��Ż ������ ���� ȣ��");
                mentalGaugeManager.TakeMentalGauge(5f); // ���� �ű� ������ ��Ż ������ 5 ����
            }
            else
            {
                Debug.LogWarning("��Ż ������ ���� ������ �������� �ʾҽ��ϴ�.");
            }
        }
    }
}
