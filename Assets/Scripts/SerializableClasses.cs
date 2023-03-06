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
}
