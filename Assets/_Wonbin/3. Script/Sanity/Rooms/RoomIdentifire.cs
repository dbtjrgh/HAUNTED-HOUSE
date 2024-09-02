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

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Room>() != null)
            {
                if (CurrRoom == Rooms.RoomsEnum.NormalRoom) ChangeRoom(other.GetComponent<Room>().RoomType);
                else _nextRooms.Add(other.GetComponent<Room>().RoomType);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<Room>() != null)
            {
                if (CurrRoom == other.GetComponent<Room>().RoomType)
                {
                    if (_nextRooms.Count > 0)// && _nextRoom != CurrRoom)
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
            CurrRoom = NextRoom;
            OnRoomChanged?.Invoke();
        }
    }
}