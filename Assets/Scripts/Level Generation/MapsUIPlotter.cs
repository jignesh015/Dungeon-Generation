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

        [Header("MAP GRIDS")]
        [SerializeField] private MapGrid dungeonGrid;
        [SerializeField] private MapGrid roomGrid;

        [Header("COLOR SCHEMES")]
        [SerializeField] private List<Color> dungeonColors;
        [SerializeField] private List<Color> roomColors;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void PlotDungeonHeatmap(List<List<int>> gridValues)
        {
            dungeonGrid.SetGridColor(gridValues,dungeonColors);
        }

        public void PlotRoomHeatmap(List<List<int>> gridValues)
        {
            roomGrid.SetGridColor(gridValues, roomColors);
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
        public void SetGridColor(List<List<int>> gridValues, List<Color> colors, bool isBinary = true)
        {
            if (isBinary)
            {
                int size = gridValues.Count;
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        int index = i * size + j;
                        //Debug.Log($"<color=cyan>| i : {i} | j : {j} | index : {index} " +
                        //    $"| Val : {gridValues[i][j]} | Colors {colors.Count} | </color>");
                        mapGrid[index].color = colors[gridValues[i][j]];
                    }
                }
            }
        }

    }
}



