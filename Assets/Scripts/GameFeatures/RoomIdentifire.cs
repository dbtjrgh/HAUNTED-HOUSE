using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static myRooms.Rooms;

namespace GameFeatures
{
    [RequireComponent(typeof(Collider))]
    public class RoomIdentifire : MonoBehaviour
    {
        public Action OnRoomChanged;
        public RoomsEnum CurrRoom = RoomsEnum.NormalRoom;
        public List<RoomsEnum> _nextRooms = new List<RoomsEnum>();
        private mentalGaugeManager mentalGaugeManager;

        private void Start()
        {
            // mentalGaugeManager 컴포넌트를 플레이어에게서 가져옴
            mentalGaugeManager = GetComponent<mentalGaugeManager>();

            if (mentalGaugeManager == null)
            {
                Debug.LogError("mentalGaugeManager 컴포넌트를 찾을 수 없습니다!");
            }
            else
            {
                Debug.Log("mentalGaugeManager가 정상적으로 연결되었습니다.");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Room roomComponent = other.GetComponent<Room>();
            Debug.Log("다른 방에 들어왔습니다. Trigger Enter: " + other.name);

            if (roomComponent != null)
            {
                Debug.Log("RoomComponent를 찾았습니다. 방 바꾸기 조건 실행: " + roomComponent.RoomType);
                if (CurrRoom == RoomsEnum.NormalRoom)
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
                Debug.Log("방에서 나갔습니다. Trigger Exit: " + other.name);

                if (CurrRoom == other.GetComponent<Room>().RoomType)
                {
                    if (_nextRooms.Count > 0)
                    {
                        ChangeRoom(_nextRooms[0]);
                        _nextRooms.Remove(CurrRoom);
                    }
                    else
                    {
                        ChangeRoom(RoomsEnum.NormalRoom);
                    }
                }
                else if (_nextRooms.Contains(other.GetComponent<Room>().RoomType))
                {
                    _nextRooms.Remove(other.GetComponent<Room>().RoomType);
                }
            }
        }

        private void ChangeRoom(RoomsEnum NextRoom)
        {
            Debug.Log($"방을 변경합니다. 현재 방: {CurrRoom}, 다음 방: {NextRoom}");
            CurrRoom = NextRoom;
            OnRoomChanged?.Invoke();

            // 방을 바꿀 때마다 멘탈 게이지 감소
            if (mentalGaugeManager != null && CurrRoom != RoomsEnum.NormalRoom)
            {
                Debug.Log("멘탈 게이지 감소 호출");
                mentalGaugeManager.TakeMentalGauge(mentalGaugeManager.ghostRoomGaugeMinus); // 방을 옮길 때마다 멘탈 게이지 5 감소
            }
            else
            {
                Debug.LogWarning("멘탈 게이지 감소 조건을 충족하지 않았습니다.");
            }
        }
    }
}
