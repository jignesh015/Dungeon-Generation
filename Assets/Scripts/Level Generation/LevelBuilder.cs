using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonGeneration
{
    public class LevelBuilder : MonoBehaviour
    {
        [HideInInspector] public bool isBuildingLevel;

        [Header("LEVEL GENERATION SETTINGS")]
        [SerializeField] private List<LevelGenerationSettings> settings;

        [Header("DUNGEON PREFABS")]
        [SerializeField] private GameObject dungeonPrefab;

        [Header("ROOM PREFABS")]
        [SerializeField] private List<GameObject> roomPrefabs;

        [Header("ROOM PREFABS PLACEMENT SETTINGS")]
        [SerializeField] private float roomXPosStart = -7.5f;
        [SerializeField] private float roomZPosStart = 7.5f;
        [SerializeField] private float roomXPosOffset = 1f;
        [SerializeField] private float roomZPosOffset = -1f;

        //Dungeon references
        [HideInInspector] public List<GameObject> dungeonRooms;
        private GameObject generatedDungeon;

        //Room references
        private List<GameObject> roomObjects;

        private GameManager gameManager;

        // Start is called before the first frame update
        void Start()
        {
            gameManager = GameManager.Instance;

            //Set a default level generation setting
            gameManager.activeSettings = settings[0];
        }

        private void Update()
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
                dungeonRooms = null;
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

            isBuildingLevel = true;

            //Set camera view  to top-down
            gameManager.uiHandler.SetCameraView(0);

            //Instantiate the 64-grid dungeon
            generatedDungeon = Instantiate(dungeonPrefab);
            dungeonRooms = GetAllChild(generatedDungeon);

            List<List<int>> gridValues = grid.gridList[0];
            int size = gridValues.Count;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    int index = i * size + j;
                    //Debug.Log($"<color=cyan>| i : {i} | j : {j} | index : {index} | Val : {gridValues[i][j]} | </color>");
                    dungeonRooms[index].SetActive(gridValues[i][j] == 1);
                    dungeonRooms[index].name = $"{index},{i},{j}";
                }
            }

            //Remove inactive floors
            dungeonRooms = dungeonRooms.Where(obj => obj.activeSelf).ToList();
        }
        #endregion

        #region GENERATE ROOMS
        public void GenerateRooms(RoomsGrid grid)
        {
            StartCoroutine(GenerateRoomsAsync(grid));
        }

        private IEnumerator GenerateRoomsAsync(RoomsGrid grid)
        {
            //Delete previously created rooms
            if(roomObjects != null && roomObjects.Count > 0)
            {
                foreach(GameObject obj in roomObjects)
                {
                    Destroy(obj);
                }
                yield return null;
            }

            if (generatedDungeon == null || dungeonRooms == null || dungeonRooms.Count == 0)
                yield break;

            //Make sure that number of dungeon rooms are equal to room data generated
            if (dungeonRooms.Count != grid.gridList.Count)
            {
                Debug.LogFormat($"<color=magenta>Mismatch in level gen!" +
                    $"Dungeon Room : {dungeonRooms.Count}|Rooms generated : {grid.gridList.Count}</color>");
                yield break;
            }

            //Check again if the dataset is empty or not
            if (generatedDungeon == null || dungeonRooms == null || dungeonRooms.Count == 0)
                yield break;

            //Select a respective Dungeon room
            for (int _index = 0; _index < grid.gridList.Count; _index++)
            {
                GameObject _currentDungeonFloor = dungeonRooms[_index];
                roomObjects = new List<GameObject>();

                //Perform a sanity check on the generated room data
                //List<List<int>> gridValues = gameManager.activeSettings.useCorrectiveAlgoRooms ? RoomSanityCheck(grid.gridList[_index]) : grid.gridList[_index];
                List<List<int>> gridValues = grid.gridList[_index];

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
                        if (cellValue != 0)
                        {
                            GameObject _roomObj = Instantiate(roomPrefabs[cellValue], _currentDungeonFloor.transform);

                            //Set object position
                            _roomObj.transform.position = new Vector3(cellPosition.x,
                                _roomObj.transform.position.y, cellPosition.z);

                            //Set object rotation
                            float yRot = 0;
                            if (cellValue == 7)
                            {
                                yRot = j == 0 ? 0 : j == size - 1 ? 180 : i == 0 ? 270 : i == size - 1 ? 90 : 0;
                            }
                            else if (cellValue == 4 || cellValue == 5 || cellValue == 6 || cellValue == 8)
                            {
                                yRot = j == 1 ? 0 : j == size - 2 ? 180 : i == 1 ? 270 : i == size - 2 ? 90 : 0;
                            }
                            _roomObj.transform.rotation = Quaternion.Euler(0, yRot, 0);
                            _roomObj.name = $"({i},{j},{yRot})";
                            roomObjects.Add(_roomObj);
                        }
                    }
                }

                yield return null;
            }

            isBuildingLevel = false;

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
                {2,6}, {3,12}, {4,4}, {5,6}, {6,6}, {8,10} 
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

        public List<int> GetGridIndicesByPosition(float xPos, float zPos, Vector3 parentPos)
        {
            int _xIndex = (int)((xPos - roomXPosStart - parentPos.x) / roomXPosOffset);
            int _zIndex = (int)((zPos - roomZPosStart - parentPos.z) / roomZPosOffset);

            int size = (int)Mathf.Sqrt(gameManager.mapsUIPlotter.GetRoomMapGridButtons().Count);

            _xIndex = Mathf.Clamp(_xIndex,1, size - 2);
            _zIndex = Mathf.Clamp(_zIndex, 1, size - 2);

            List<int> indices = new List<int> { _xIndex, _zIndex };
            return indices;
        }

    }
}
