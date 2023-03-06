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

        public void GenerateDungeons(int datasetChoice)
        {
            StartCoroutine(GenerateDungeonsReq(datasetChoice));
        }

        IEnumerator GenerateDungeonsReq(int datasetChoice)
        {
            string url = "http://127.0.0.1:8080/generate_dungeons/";
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
                plot.PlotHeatmap(generatedDungeons.gridList[0]);
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
    }
}
