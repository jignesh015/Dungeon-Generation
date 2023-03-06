using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonGeneration
{
    public class MapsUIPlotter : MonoBehaviour
    {
        [SerializeField] private MapGrid mapGrid;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void PlotHeatmap(List<List<int>> gridValues)
        {
            mapGrid.SetGridColor(gridValues);
        }

    }

    [Serializable]
    public class MapGrid
    {
        public List<Image> mapGrid;

        /// <summary>
        /// Should be a square grid of size nxn
        /// </summary>
        /// <param name="gridValues"></param>
        /// <param name="isBinary"></param>
        public void SetGridColor(List<List<int>> gridValues, bool isBinary = true)
        {
            if (isBinary)
            {
                int size = gridValues.Count;
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        int index = i * size + j;
                        //Debug.Log($"<color=cyan>| i : {i} | j : {j} | index : {index} | Val : {gridValues[i][j]} | </color>");
                        mapGrid[index].color = gridValues[i][j] == 0 ? Color.black : Color.white;
                    }
                }
            }
        }

    }
}



