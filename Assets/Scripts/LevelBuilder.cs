using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonGeneration
{
    public class LevelBuilder : MonoBehaviour
    {
        [Header("DUNGEON PREFABS")]
        [SerializeField] private GameObject dungeonPrefab;

        [Header("ROOM PREFABS")]
        [SerializeField] private List<GameObject> roomPrefabs;

        [Header("ROOM PREFABS PLACEMENT SETTINGS")]
        public bool performSanityCheck;
        [SerializeField] private float roomXPosStart = -7.5f;
        [SerializeField] private float roomZPosStart = 7.5f;
        [SerializeField] private float roomXPosOffset = 1f;
        [SerializeField] private float roomZPosOffset = -1f;

        //Dungeon references
        private List<GameObject> dungeonFloors;
        private GameObject generatedDungeon;

        //Room references
        private List<GameObject> roomObjects;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        #region GENERATE DUNGEON
        public void GenerateDungeon(DungeonGrid grid)
        {
            StartCoroutine(GenerateDungeonAsync(grid));
        }

        private IEnumerator GenerateDungeonAsync(DungeonGrid grid)
        {
            //Delete previously created dungeon and rooms
            if(generatedDungeon != null)
            {
                Destroy(generatedDungeon);
                dungeonFloors = null;
                yield return null;
            }
            if (roomObjects != null && roomObjects.Count > 0)
            {
                foreach (GameObject obj in roomObjects)
                {
                    Destroy(obj);
                }
                yield return null;
            }

            //Instantiate the 64-grid dungeon
            generatedDungeon = Instantiate(dungeonPrefab);
            dungeonFloors = GetAllChild(generatedDungeon);

            List<List<int>> gridValues = grid.gridList[0];
            int size = gridValues.Count;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    int index = i * size + j;
                    //Debug.Log($"<color=cyan>| i : {i} | j : {j} | index : {index} | Val : {gridValues[i][j]} | </color>");
                    dungeonFloors[index].SetActive(gridValues[i][j] == 1);
                }
            }
        }
        #endregion

        #region GENERATE ROOMS
        public void GenerateRooms(RoomsGrid grid)
        {
            StartCoroutine(GenerateRoomsAsync(grid));
        }

        private IEnumerator GenerateRoomsAsync(RoomsGrid grid)
        {
            if (generatedDungeon == null || dungeonFloors == null || dungeonFloors.Count == 0)
                yield break;

            //Delete previously created rooms
            if(roomObjects != null && roomObjects.Count > 0)
            {
                foreach(GameObject obj in roomObjects)
                {
                    Destroy(obj);
                }
                yield return null;
            }

            //Select a respective Dungeon room
            GameObject _currentDungeonFloor = dungeonFloors[0];
            roomObjects = new List<GameObject>();

            //Perform a sanity check on the generated room data
            List<List<int>> gridValues = performSanityCheck ? RoomSanityCheck(grid.gridList[0]) : grid.gridList[0];

            //Instatiate objects in the selected room
            int size = gridValues.Count;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    int cellValue = gridValues[i][j];
                    Vector3 parentPos = _currentDungeonFloor.transform.position;
                    Vector3 cellPosition = new Vector3(roomXPosStart + i * roomXPosOffset + parentPos.x,
                        0, roomZPosStart + j * roomZPosOffset + parentPos.z);

                    //Instantiate a room prefab as per cell value
                    if(cellValue != 0)
                    {
                        GameObject _roomObj = Instantiate(roomPrefabs[cellValue], _currentDungeonFloor.transform);
                        _roomObj.transform.position = new Vector3(cellPosition.x,
                            _roomObj.transform.position.y, cellPosition.z);
                        roomObjects.Add(_roomObj);
                    }
                }
            }
        }

        /// <summary>
        /// Run a sanity check on the generated data
        /// </summary>
        /// <param name="gridValues"></param>
        /// <returns></returns>
        private List<List<int>> RoomSanityCheck(List<List<int>> gridValues)
        {
            int size = gridValues.Count;
            for(int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    int cellValue = gridValues[i][j];
                    bool _sanityCondition = true;
                    int _replacementValue = 0;

                    switch (cellValue)
                    {
                        case 0:
                            break;
                        case 1:
                            //Remove walls which are not on the edges
                            _sanityCondition = i == 0 || j == 0 || i == size - 1 || j == size - 1;
                            break;
                        case 5:
                            //Remove bookshelves which are not next to the walls
                            _sanityCondition = i == 1 || j == 1 || i == size - 2 || j == size - 2;
                            break;
                        case 6:
                            //Remove torches which are not on the walls
                            _sanityCondition = i == 1 || j == 1 || i == size - 2 || j == size - 2;
                            break;
                        case 7:
                            //Remove doors which are not on the edges
                            _sanityCondition = i == 0 || j == 0 || i == size - 1 || j == size - 1;
                            break;
                    }

                    //Replace everything on the edges which is not wall or door with a wall
                    if(i == 0 || j == 0 || i == size - 1 || j == size - 1)
                    {
                        _sanityCondition = cellValue == 1 || cellValue == 7;
                        _replacementValue = 1;
                    }

                    gridValues[i][j] = _sanityCondition ? cellValue : _replacementValue;

                }
            }

            //Put a hard limit on other objects
            Dictionary<int,int> objectsMaxAllowed = new Dictionary<int, int>()
            { 
                {2,6}, {3,12}, {4,4}, {5,6}, {8,10} 
            };

            foreach(KeyValuePair<int, int> keyVal in objectsMaxAllowed)
            {
                int targetValue = keyVal.Key;
                int maxAllowed = keyVal.Value;

                // Find the positions of all occurrences of the target value
                List<(int x, int y)> positions = Enumerable.Range(0, size)
                    .SelectMany(x => Enumerable.Range(0, size).Select(y => (x, y)))
                    .Where(pos => gridValues[pos.x][pos.y] == targetValue)
                    .ToList();

                // If there are more than maxAllowed occurrences, randomly choose which ones to keep
                if (positions.Count > maxAllowed)
                {
                    List<(int x, int y)> positionsToConvert = positions
                        .OrderBy(x => Random.value)
                        .Take(positions.Count - maxAllowed)
                        .ToList();

                    // Convert the values in the selected positions to 0
                    foreach ((int x, int y) pos in positionsToConvert)
                    {
                        gridValues[pos.x][pos.y] = 0;
                    }
                }
            }
            return gridValues;
        }

        #endregion

        private List<GameObject> GetAllChild(GameObject parentObj)
        {
            List<GameObject> children = new List<GameObject>();
            foreach(Transform t in parentObj.transform)
            {
                children.Add(t.gameObject);
            }
            return children;
        }

    }
}
