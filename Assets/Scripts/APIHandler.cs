using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using Newtonsoft.Json;

namespace DungeonGeneration
{
    public class APIHandler : MonoBehaviour
    {
        [SerializeField] private MapsUIPlotter plot;
        [SerializeField] private LevelBuilder levelBuilder;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            // Releases the cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        #region APIs for Dungeons
        public void GenerateDungeons(int datasetChoice)
        {
            StartCoroutine(GenerateDungeonsReq(datasetChoice));
        }

        IEnumerator GenerateDungeonsReq(int datasetChoice)
        {
            string url = Data.APP_URL + Data.GENERATE_DUNGEONS;
            WWWForm form = new WWWForm();
            form.AddField("num_of_samples", 1);
            form.AddField("mode_of_generation", datasetChoice);

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JSON.Parse(request.downloadHandler.text);
                Debug.Log($"{response} |");
                List<List<List<int>>> dungeonRes = JsonConvert.DeserializeObject<List<List<List<int>>>>(response["data"]);
                DungeonGrid generatedDungeons = new DungeonGrid(dungeonRes);
                Debug.Log($"{generatedDungeons.gridList.Count} | {generatedDungeons.gridList[0].Count} | {generatedDungeons.gridList[0][0].Count}");
                plot.PlotDungeonHeatmap(generatedDungeons.gridList[0]);
                levelBuilder.GenerateDungeon(generatedDungeons);

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
        public void GenerateRooms(int datasetChoice)
        {
            StartCoroutine(GenerateRoomsReq(datasetChoice));
        }

        IEnumerator GenerateRoomsReq(int datasetChoice)
        {
            string url = Data.APP_URL + Data.GENERATE_ROOMS;
            WWWForm form = new WWWForm();
            form.AddField("num_of_samples", 1);
            form.AddField("mode_of_generation", datasetChoice);

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JSON.Parse(request.downloadHandler.text);
                Debug.Log($"{response} |");
                List<List<List<int>>> roomsRes = JsonConvert.DeserializeObject<List<List<List<int>>>>(response["data"]);
                RoomsGrid generatedRooms = new RoomsGrid(roomsRes);
                Debug.Log($"{generatedRooms.gridList.Count} | {generatedRooms.gridList[0].Count} | {generatedRooms.gridList[0][0].Count}");
                plot.PlotRoomHeatmap(generatedRooms.gridList[0]);
                levelBuilder.GenerateRooms(generatedRooms);

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
