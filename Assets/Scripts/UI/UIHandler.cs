using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DungeonGeneration
{
    public class UIHandler : MonoBehaviour
    {
        [Header("SCREENS")]
        [SerializeField] private GameObject networkErrorScreen;
        [SerializeField] private GameObject configScreen;

        [Header("CONFIG SETTINGS")]
        [SerializeField] private TMP_Dropdown dungeonDatasetChoiceInput;
        [SerializeField] private TMP_Dropdown roomDatasetChoiceInput;
        [SerializeField] private Toggle performSanityCheckInput;

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
        }

        public void ToggleNetworkErrorScreen(bool state)
        {
            networkErrorScreen.SetActive(state);
            configScreen.SetActive(!state);
        }

        public void OnGenerateButtonClick()
        {
            SetConfigToSettings();
            gameManager.apiHandler.GenerateEntireMap(gameManager.activeSettings);
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
