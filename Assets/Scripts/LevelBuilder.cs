using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonGeneration
{
    public class LevelBuilder : MonoBehaviour
    {
        [Header("Building Settings")]
        [SerializeField] private float buildDelay;

        [Header("Dungeon Prefabs")]
        [SerializeField] private GameObject dungeonPrefab;

        [SerializeField] private List<GameObject> dungeonFloors;
        private GameObject generatedDungeon;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        public void GenerateDungeon(DungeonGrid grid)
        {
            StartCoroutine(GenerateDungeonAsync(grid));
        }

        private IEnumerator GenerateDungeonAsync(DungeonGrid grid)
        {
            //Delete previously created dungeon
            if(generatedDungeon != null)
            {
                Destroy(generatedDungeon);
                dungeonFloors = null;
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
                    yield return new WaitForSeconds(buildDelay);
                }
            }
            gridValues = null;
        }

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
