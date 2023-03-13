using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

namespace DungeonGeneration
{
    public class DungeonFloorCollider : MonoBehaviour
    {
        [HideInInspector] public bool isSelected;

        private Renderer rend;
        private Material ogMat;
        private GameManager gameManager;

        private MapGridButton correspondingMapGridButton;

        private EventSystem myEventSystem;


        // Start is called before the first frame update
        void Start()
        {
            gameManager = GameManager.Instance;
            myEventSystem = FindObjectOfType<EventSystem>();

            rend = transform.GetChild(0).GetComponent<Renderer>();
            ogMat = rend.material;

            //Find corresponding map grid button
            List<string> indices = transform.name.Split(',').ToList();
            List<MapGridButton> buttons = gameManager.mapsUIPlotter.GetDungeonMapGridButtons();
            correspondingMapGridButton = buttons.Find(b =>
            b.index == int.Parse(indices[0]) &&
            b.xIndex == int.Parse(indices[1]) &&
            b.zIndex == int.Parse(indices[2]));

            correspondingMapGridButton.correspondingDungeonFloor = this;
        }

        // Update is called once per frame
        void Update()
        {
            bool _highlightCondition = gameManager.uiHandler.currentViewIndex == 1;
            rend.material = (isSelected && _highlightCondition) ? gameManager.uiHandler.dungeonFloorHighlightMat : ogMat;
            if (gameManager.input.hover != null && _highlightCondition)
            {
                Ray ray = Camera.main.ScreenPointToRay(gameManager.input.hover);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo))
                {
                    if (hitInfo.collider == GetComponent<Collider>())
                    {
                        //Debug.Log($"Hitting : {hitInfo.transform.name}");
                        rend.material = gameManager.uiHandler.dungeonFloorHighlightMat;
                        correspondingMapGridButton.gridButton.Select();

                        if (gameManager.input.leftClick)
                        {
                            gameManager.input.leftClick = false;
                            correspondingMapGridButton.OnCellClick();
                        }
                    }
                }
                //else
                //{
                //    rend.material = ogMat;
                //}
            }

            if(rend.material == ogMat)
            {
                myEventSystem.SetSelectedGameObject(null);
            }
        }
    }
}
