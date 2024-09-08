using myRooms;
using UnityEngine;
using Utilities.Constants;

namespace GameFeatures
{
    [RequireComponent(typeof(Collider))]
    public class Room : MonoBehaviour
    {
        public Rooms.RoomsEnum RoomType;
    }
}