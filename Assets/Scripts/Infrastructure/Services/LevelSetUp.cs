using myRooms;
using System;
using System.Collections;
using UnityEngine;
using Utilities.Constants;
namespace Infrastructure.Services
{
    public class LevelSetUp : IService
    {
        public Action OnLevelSetedUp;
        public Rooms.RoomsEnum CurrGhostRoom
        {
            get { return _currRoom; }
        }

        public LevelSizeConst.LevelSize CurrLevelSize
        {
            get { return _currLevelSize; }
        }
        public Transform CurrGhostRoomTransform
        {
            get { return _currRoomTransform; }
        }
        public DoorDraggable[] MainDoors
        {
            get { return _mainDoors; }
        }
        public LightButton[] LightButtons
        {
            get { return _lightButtons; }
        }
        public int[] AddedItems
        {
            get { return _addedItems; }
        }
        public SceneNames.LevelNames SelectedMap
        {
            get { return _selectedMap; }
        }

        public DifficultySO SelectedDifficulty
        {
            get { return _difficulty; }
        }

        public bool IsInitialized = false;
        public GameObject MainPlayer;
        public GhostInfo GhostInfo;

        private DoorDraggable[] _mainDoors;
        private LightButton[] _lightButtons;
        private SceneNames.LevelNames _selectedMap = SceneNames.LevelNames.Turkwood;
        private Transform _currRoomTransform;
        private Rooms.RoomsEnum _currRoom = Rooms.RoomsEnum.NormalRoom;

        private GameObjectivesService _gameObjectivesService;
        private LevelInfo _currLevelInfo;
        private LevelSizeConst.LevelSize _currLevelSize;

        private DifficultySO _difficulty;

        private int[] _addedItems;

        public LevelSetUp(GameObjectivesService gameObjectivesService)
        {
            _gameObjectivesService = gameObjectivesService;
        }

        public void ChooseMap(SceneNames.LevelNames selectedMap)
        {
            _selectedMap = selectedMap;
        }

        public void ChooseDifficulty(DifficultySO difficulty)
        {
            _difficulty = difficulty;
        }
        public void SetAddedItems(int[] addedItems)
        {
            _addedItems = addedItems;
        }
        public Transform[] GetGhostPatrolPoints()
        {
            return _currLevelInfo.PatrolPoints;
        }


        public void InitializeLevel()
        {
            _currLevelInfo = GameObject.FindObjectOfType<LevelInfo>();
            if (_currLevelInfo == null)
            {
                Debug.LogWarning("Current level info = null!");
            };
            RandomizeCurrentRoom();
        }

        public void ResetLevel()
        {
            _currRoom = Rooms.RoomsEnum.NormalRoom;
            _currRoomTransform = null;
            IsInitialized = false;
        }

        private void RandomizeCurrentRoom()
        {
            int randomLevelNum = UnityEngine.Random.Range(0, _currLevelInfo.AllRooms.Length);
            _currRoom = _currLevelInfo.AllRooms[randomLevelNum].RoomType;
            _currLevelSize = _currLevelInfo.LevelSize;
            _currRoomTransform = _currLevelInfo.AllRooms[randomLevelNum].transform;
            _mainDoors = _currLevelInfo.MainDoors;
            _lightButtons = _currLevelInfo.LightButtons;
            //Debug.Log("curr room = " + _currRoom.ToString());
        }
    }
}