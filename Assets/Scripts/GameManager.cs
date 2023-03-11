using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DungeonGeneration
{
    public class GameManager : MonoBehaviour
    {
        public LevelGenerationSettings activeSettings;

        [Header("PLAYER REFERENCES")]
        [SerializeField] private GameObject player;

        //Script References
        [HideInInspector] public APIHandler apiHandler;
        [HideInInspector] public UIHandler uiHandler;
        [HideInInspector] public LevelBuilder levelBuilder;
        [HideInInspector] public MapsUIPlotter mapsUIPlotter;
        [HideInInspector] public StarterAssetsInputs input;

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
            input = FindObjectOfType<StarterAssetsInputs>();
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        private void Update()
        {
            
        }

        //Place player in the respective room as per the given parameters
        public bool PlacePlayerInRoom(int _index, int _xVal, int _zVal)
        {
            List<GameObject> rooms = levelBuilder.dungeonRooms;

            //Check if the corresponding room exists
            GameObject _room = rooms.Find(r => r.name.Equals($"{_index},{_xVal},{_zVal}"));
            if (_room != null)
            {
                //Place the player in that particular room
                Vector3 _roomPos = _room.transform.position;
                player.SetActive(false);
                player.transform.position = new Vector3(_roomPos.x, player.transform.position.y + 2f, _roomPos.z);

                Debug.Log($"<color=yellow>Room : {_roomPos} | Player : {player.transform.position}</color>");
                player.SetActive(true);

                //Change camera view to first person
                uiHandler.SetCameraView(2);

                return true;
            }
            else
                return false;
        }
    }
}
