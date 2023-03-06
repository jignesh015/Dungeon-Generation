using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;
using System.Linq;

namespace DungeonGeneration
{
    public class LoadModels : MonoBehaviour
    {
        [SerializeField] private MapsUIPlotter plot;
        [SerializeField] private NNModel dungeonAutoencoderModel;
        private Model dungeonAutoencoderRuntimeModel;
        private IWorker dungeonAutoencoderWorker;
        private string dungeonAutoencoderLayerName;

        private float[,] dungeonDataset;


        // Start is called before the first frame update
        void Start()
        {
            dungeonAutoencoderRuntimeModel = ModelLoader.Load(dungeonAutoencoderModel);
            dungeonAutoencoderWorker = WorkerFactory.CreateWorker(WorkerFactory.Type.Auto, dungeonAutoencoderRuntimeModel);
            dungeonAutoencoderLayerName = dungeonAutoencoderRuntimeModel.outputs[dungeonAutoencoderRuntimeModel.outputs.Count - 1];
            Debug.Log($"<color=cyan>{dungeonAutoencoderLayerName}</color>");

            //Load the datasets
            //dungeonDataset = Utils.LoadDataset("Datasets/dungeons_dataset");
        }

        private void Update()
        {
            // Releases the cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void Predict(int datasetChoice)
        {
            int num_of_samples = 20;
            int len_of_sample = 64;
            float[,] inputData = datasetChoice == 0 ? Utils.GenerateRandomBinaryDataset(num_of_samples, len_of_sample)
                : Utils.GetSubset(dungeonDataset, num_of_samples);

            List<List<int>> inputDataList = Utils.ConvertFArrayToIList(inputData);

            Debug.Log($"<color=olive>{JsonUtility.ToJson(inputDataList)} | {inputData.Length}</color>");

            // Prepare the input data as a Tensor object
            int[] shape = new int[] { num_of_samples, len_of_sample };
            Tensor inputTensor = new Tensor(shape, inputData, "input");
            dungeonAutoencoderWorker.Execute(inputTensor);

            Tensor outputTensor = dungeonAutoencoderWorker.PeekOutput(dungeonAutoencoderLayerName);
            var outputList = Utils.TensorToList(outputTensor);
            plot.PlotHeatmap(outputList[0]);
            //Debug.Log($"<color=cyan>{outputTensor.ShallowCopy().shape} | {outputTensor.GetTensorDataStatistics()} | {outputTensor.ToReadOnlyArray().Length}</color>");
        }

        private void OnDestroy()
        {
            dungeonAutoencoderWorker?.Dispose();
        }
    }
}

