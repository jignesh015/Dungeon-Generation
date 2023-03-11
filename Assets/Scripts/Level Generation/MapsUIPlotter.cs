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
        [SerializeField] private MapGrid dungeonMainGrid;
        [SerializeField] private MapGrid dungeonInteractableGrid;
        [SerializeField] private MapGrid roomGrid;

        [Header("COLOR SCHEMES")]
        [SerializeField] private Color activeRoomColor;
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

        public void PlotDungeonMainHeatmap(List<List<int>> gridValues)
        {
            dungeonMainGrid.SetGridColor(gridValues,dungeonColors);
        }

        public void PlotDungeonInteractableHeatmap(List<List<int>> gridValues)
        {
            dungeonInteractableGrid.SetGridColor(gridValues, dungeonColors);
        }

        public void PlotRoomHeatmap(List<List<int>> gridValues)
        {
            roomGrid.SetGridColor(gridValues, roomColors);
        }

        public void UpdateInteractableHeatmapCellColor(int _activeCellIndex = -1)
        {
            for(int i = 0; i < dungeonInteractableGrid.mapGrid.Count; i++)
            {
                dungeonInteractableGrid.mapGrid[i].ResetToDefault();
                if (i == _activeCellIndex)
                    dungeonInteractableGrid.mapGrid[i].SetActiveCellColor(activeRoomColor);
            }
        }

        public List<MapGridButton> GetDungeonMapGridButtons()
        {
            return dungeonInteractableGrid.mapGrid;
        }
    }

    [Serializable]
    public class MapGrid
    {
        public List<MapGridButton> mapGrid;

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
                        mapGrid[index].SetDefaultCellColor(colors[gridValues[i][j]]);
                        mapGrid[index].SetIndices(index, i, j);
                    }
                }
            }
        }

    }
}



