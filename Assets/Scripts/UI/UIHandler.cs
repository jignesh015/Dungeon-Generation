using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;

namespace DungeonGeneration
{
    public class UIHandler : MonoBehaviour
    {
        [Header("VIRTUAL CAMERAS")]
        [SerializeField] private CinemachineVirtualCamera levelTopDownVC;
        [SerializeField] private CinemachineVirtualCamera levelRotateVC;
        [SerializeField] private CinemachineVirtualCamera firstPersonVC;

        [Header("VIRTUAL CAMERAS SETTINGS")]
        [SerializeField] private float vcRotateSpeed;

        [Header("SCREENS")]
        [SerializeField] private GameObject networkErrorScreen;
        [SerializeField] private GameObject configScreen;
        [SerializeField] private GameObject exploreScreen;

        [Header("CONFIG SETTINGS")]
        [SerializeField] private TMP_Dropdown dungeonDatasetChoiceInput;
        [SerializeField] private TMP_Dropdown roomDatasetChoiceInput;
        [SerializeField] private Toggle performSanityCheckInput;
        [SerializeField] private Button generateButton;

        [Header("CURSOR SETTINGS")]
        [SerializeField] private Texture2D cursorTexture;
        [SerializeField] private CursorMode cursorMode = CursorMode.Auto;
        [SerializeField] private Vector2 hotSpot = Vector2.zero;

        private GameManager gameManager;

        // Start is called before the first frame update
        void Start()
        {
            gameManager = GameManager.Instance;
            Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
        }

        // Update is called once per frame
        void Update()
        {
            // Releases the cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            generateButton.interactable = !gameManager.levelBuilder.isBuildingLevel;

            //Rotate the rotateVC if active
            if(levelRotateVC != null && levelRotateVC.gameObject.activeSelf) 
            {
                Transform _parent = levelRotateVC.transform.parent;
                _parent.RotateAround(_parent.position, Vector3.up, vcRotateSpeed * Time.deltaTime);
            }
        }

        public void ToggleNetworkErrorScreen(bool state)
        {
            networkErrorScreen.SetActive(state);
            configScreen.SetActive(!state);
            exploreScreen.SetActive(false);
        }

        /// <summary>
        /// Sets the camera view as per the given index
        /// | 0 = Top Down | 1 = Rotate | 2 = First Person
        /// </summary>
        /// <param name="view"></param>
        public void SetCameraView(int view)
        {
            levelTopDownVC.gameObject.SetActive(false);
            levelRotateVC.gameObject.SetActive(false);
            firstPersonVC.gameObject.SetActive(false);

            switch(view)
            {
                case 0:
                    levelTopDownVC.gameObject.SetActive(true);
                    break;
                case 1:
                    levelRotateVC.gameObject.SetActive(true);
                    break;
                case 2:
                    firstPersonVC.gameObject.SetActive(true);
                    break;
                default:
                    levelTopDownVC.gameObject.SetActive(true);
                    break;
            }
        }

        public void OnGenerateButtonClick()
        {
            SetConfigToSettings();
            gameManager.apiHandler.GenerateEntireMap(gameManager.activeSettings);
        }

        public void OnExploreButtonClick()
        {
            configScreen.SetActive(false);
            exploreScreen.SetActive(true);

            SetCameraView(1);
            gameManager.mapsUIPlotter.UpdateInteractableHeatmapCellColor();
        }

        public void OnBackButtonClick()
        {
            configScreen.SetActive(true);
            exploreScreen.SetActive(false);

            SetCameraView(0);
        }

        private void SetConfigToSettings()
        {
            gameManager.activeSettings.settingsName = "New Setting";
            gameManager.activeSettings.numberOfDungeons = 1;
            gameManager.activeSettings.numberOfRooms = 1;
            gameManager.activeSettings.dungeonDatasetChoice = dungeonDatasetChoiceInput.value;
            gameManager.activeSettings.roomDatasetChoice = roomDatasetChoiceInput.value;
            gameManager.activeSettings.performSanityCheck = performSanityCheckInput.isOn;
        }
    }
}
