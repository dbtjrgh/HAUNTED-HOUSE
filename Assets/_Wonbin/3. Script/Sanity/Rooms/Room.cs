using UnityEngine;
using myRooms;

namespace RoomLogic
{
    [RequireComponent(typeof(Collider))]
    public class Room : MonoBehaviour
    {
        public Rooms.RoomsEnum RoomType;
    }
}