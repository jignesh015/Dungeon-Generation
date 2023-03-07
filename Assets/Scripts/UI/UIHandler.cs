using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGeneration
{
    public class UIHandler : MonoBehaviour
    {
        [Header("SCREENS")]
        [SerializeField] private GameObject networkErrorScreen;
        [SerializeField] private GameObject configScreen;

        private GameManager gameManager;

        // Start is called before the first frame update
        void Start()
        {
            gameManager = GameManager.Instance;
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void ToggleNetworkErrorScreen(bool state)
        {
            networkErrorScreen.SetActive(state);
            configScreen.SetActive(!state);
        }

        public void OnGenerateButtonClick()
        {
            gameManager.apiHandler.GenerateEntireMap(gameManager.activeSettings);
        }
    }
}
