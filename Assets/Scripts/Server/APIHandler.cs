using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using Newtonsoft.Json;
using System.Linq;

namespace DungeonGeneration
{
    public class APIHandler : MonoBehaviour
    {
        [SerializeField] private LevelGenerationSettings settings;
        
        private DungeonGrid generatedDungeons;
        private RoomsGrid generatedRooms;

        private GameManager gameManager;
        // Start is called before the first frame update
        void Start()
        {
            gameManager = GameManager.Instance;
            CheckForServerConnection();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void CheckForServerConnection()
        {
            StartCoroutine(CheckForServerConnectionAsync());
        }

        private IEnumerator CheckForServerConnectionAsync()
        {
            string url = Data.APP_URL;
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JSON.Parse(request.downloadHandler.text);
                Debug.Log($"{response} |");
                gameManager.uiHandler.ToggleNetworkErrorScreen(false);
            }
            else
            {
                Debug.Log("Error: " + request.error);
                gameManager.uiHandler.ToggleNetworkErrorScreen(true);
            }
        }

        public void GenerateEntireMap(LevelGenerationSettings _settings)
        {
            settings = _settings;
            StartCoroutine(GenerateEntireMapAsync());
        }

        private IEnumerator GenerateEntireMapAsync()
        {
            //Wait to generate the dungeon data
            yield return StartCoroutine(GenerateDungeonsReq());

            if(generatedDungeons == null || generatedDungeons.gridList?.Count == 0)
            {
                Debug.LogFormat("<color=magenta>Couldn't fetch Dungeon data</color>");
                yield break;
            }

            //Record how many number of rooms were generated in the current dungeon
            settings.numberOfRooms = Enumerable.Range(0, generatedDungeons.gridList[0].Count)
            .SelectMany(i => Enumerable.Range(0, generatedDungeons.gridList[0].Count)
            .Select(j => generatedDungeons.gridList[0][i][j]))
            .Sum(x => x);

            //Generate the rooms data
            yield return StartCoroutine(GenerateRoomsReq());
        }

        #region APIs for Dungeons
        public void GenerateDungeons()
        {
            StartCoroutine(GenerateDungeonsReq());
        }

        IEnumerator GenerateDungeonsReq()
        {
            string url = Data.APP_URL + Data.GENERATE_DUNGEONS;
            WWWForm form = new WWWForm();
            form.AddField("num_of_samples", settings.numberOfDungeons);
            form.AddField("mode_of_generation", settings.dungeonDatasetChoice);

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JSON.Parse(request.downloadHandler.text);
                Debug.Log($"{response} |");
                List<List<List<int>>> dungeonRes = JsonConvert.DeserializeObject<List<List<List<int>>>>(response["data"]);
                generatedDungeons = new DungeonGrid(dungeonRes);
                Debug.Log($"{generatedDungeons.gridList.Count} | {generatedDungeons.gridList[0].Count} | {generatedDungeons.gridList[0][0].Count}");
                gameManager.mapsUIPlotter.PlotDungeonMainHeatmap(generatedDungeons.gridList[0]);
                gameManager.mapsUIPlotter.PlotDungeonInteractableHeatmap(generatedDungeons.gridList[0]);
                gameManager.levelBuilder.GenerateDungeon(generatedDungeons);

                //Delete the unwanted variables
                dungeonRes = null;
                response = null;
            }
            else
            {
                Debug.Log("Error: " + request.error);
            }
        }
        #endregion

        #region APIs for Rooms
        public void GenerateRooms()
        {
            StartCoroutine(GenerateRoomsReq());
        }

        IEnumerator GenerateRoomsReq()
        {
            string url = Data.APP_URL + Data.GENERATE_ROOMS;
            WWWForm form = new WWWForm();
            form.AddField("num_of_samples", settings.numberOfRooms);
            form.AddField("mode_of_generation", settings.roomDatasetChoice);

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JSON.Parse(request.downloadHandler.text);
                Debug.Log($"{response} |");
                List<List<List<int>>> roomsRes = JsonConvert.DeserializeObject<List<List<List<int>>>>(response["data"]);
                generatedRooms = new RoomsGrid(roomsRes);
                Debug.Log($"{generatedRooms.gridList.Count} | {generatedRooms.gridList[0].Count} | {generatedRooms.gridList[0][0].Count}");
                gameManager.mapsUIPlotter.PlotRoomHeatmap(generatedRooms.gridList[0]);
                gameManager.levelBuilder.GenerateRooms(generatedRooms);

                //Delete the unwanted variables
                roomsRes = null;
                response = null;
            }
            else
            {
                Debug.Log("Error: " + request.error);
            }
        }
        #endregion
    }
}
