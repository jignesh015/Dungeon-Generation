using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGeneration
{
    public class GameManager : MonoBehaviour
    {
        public LevelGenerationSettings activeSettings;

        //Script References
        [HideInInspector] public APIHandler apiHandler;
        [HideInInspector] public UIHandler uiHandler;
        [HideInInspector] public LevelBuilder levelBuilder;
        [HideInInspector] public MapsUIPlotter mapsUIPlotter;

        private static GameManager _instance;
        public static GameManager Instance { get { return _instance; } }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }

            //Get script references
            apiHandler = FindObjectOfType<APIHandler>();
            uiHandler = FindObjectOfType<UIHandler>();
            levelBuilder = FindObjectOfType<LevelBuilder>();
            mapsUIPlotter = FindObjectOfType<MapsUIPlotter>();
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
