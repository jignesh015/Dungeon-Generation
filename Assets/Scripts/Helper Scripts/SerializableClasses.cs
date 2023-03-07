using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGeneration
{
    [Serializable]
    public class DungeonGrid
    {
        public List<List<List<int>>> gridList;

        public DungeonGrid(List<List<List<int>>> _dungeonGrid)
        {
            gridList = _dungeonGrid;
        }
    }

    [Serializable]
    public class RoomsGrid
    {
        public List<List<List<int>>> gridList;

        public RoomsGrid(List<List<List<int>>> _roomsGrid)
        {
            gridList = _roomsGrid;
        }
    }

    [Serializable]
    public class LevelGenerationSettings
    {
        public string settingsName;

        public int numberOfDungeons;
        public int numberOfRooms;

        public int dungeonDatasetChoice;
        public int roomDatasetChoice;

        public bool performSanityCheck;
    }
}