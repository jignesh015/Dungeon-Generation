using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonGeneration
{
    public class MapGridButton : MonoBehaviour
    {
        [HideInInspector] public int index;
        [HideInInspector] public int xIndex;
        [HideInInspector] public int zIndex;

        [HideInInspector] public Button gridButton;
        [HideInInspector] public DungeonFloorCollider correspondingDungeonFloor;

        private Image cellImage;
        private Color defaultColor;

        private GameManager gameManager;

        // Start is called before the first frame update
        void Start()
        {
            cellImage= GetComponent<Image>();
            gridButton =GetComponent<Button>();
            gameManager = GameManager.Instance;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetDefaultCellColor(Color color)
        {
            defaultColor = color;
            SetColor(color);
        }

        public void ResetToDefault()
        {
            SetColor(defaultColor);
        }

        public void SetActiveCellColor(Color color)
        {
            SetColor(color);
        }

        private void SetColor(Color color)
        {
            if(cellImage == null) 
            {
                cellImage = GetComponent<Image>();
            }
            cellImage.color = color;
        }

        public void SetIndices(int _index, int _x, int _z)
        {
            index = _index;
            xIndex = _x;
            zIndex = _z;
        }

        public void OnCellHoverEnter()
        {
            //Highlight the correspoding dungeon floor
            if(correspondingDungeonFloor != null)
            {
                correspondingDungeonFloor.isSelected = true;
                Debug.Log($"<color=blue>OnCellHoverEnter {correspondingDungeonFloor.name}</color>");
            }
        }

        public void OnCellHoverExit()
        {
            //De-Highlight the correspoding dungeon floor
            if (correspondingDungeonFloor != null)
            {
                correspondingDungeonFloor.isSelected = false;
            }
        }

        public void OnCellClick()
        {
            //Place player inside this particular cell/room if available
            bool _isRoom = gameManager.PlacePlayerInRoom(index, xIndex, zIndex);

            if (_isRoom)
            {
                //Update the cell color if room is avaiable
                gameManager.mapsUIPlotter.UpdateInteractableHeatmapCellColor(index);

                //Plot the respective room heat map
                int _roomIndex = gameManager.levelBuilder.dungeonRooms.FindIndex(r =>
                    r.name.Equals($"{index},{xIndex},{zIndex}"));
                Debug.Log($"<color=olive>Room index = {_roomIndex} | " +
                    $"Generated Rooms = {gameManager.apiHandler.generatedRooms.gridList.Count}</color>");
                gameManager.mapsUIPlotter.PlotRoomHeatmap(
                    gameManager.apiHandler.generatedRooms.gridList[_roomIndex]);
            }
        }
    }
}
