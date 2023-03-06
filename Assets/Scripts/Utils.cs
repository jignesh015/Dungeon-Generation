using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DungeonGeneration
{
    public static class Utils
    {
        /// <summary>
        /// Converts a 2D float array to a 2D int List
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        public static List<List<int>> ConvertFArrayToIList(float[,] inputData)
        {
            List<List<int>> _outputData = Enumerable.Range(0, inputData.GetLength(0))
                .Select(i => Enumerable.Range(0, inputData.GetLength(1))
                .Select(j => Mathf.RoundToInt(inputData[i, j])).ToList()).ToList();
            return _outputData;
        }

        /// <summary>
        /// Generates a 2D array with values 0 and 1 randomly placed
        /// </summary>
        /// <param name="num_of_samples"></param>
        /// <param name="len_of_sample"></param>
        /// <returns></returns>
        public static float[,] GenerateRandomBinaryDataset(int num_of_samples, int len_of_sample)
        {
            float[,] randomDataset = new float[num_of_samples, len_of_sample];

            for (int i = 0; i < num_of_samples; i++)
            {
                for (int j = 0; j < len_of_sample; j++)
                {
                    randomDataset[i, j] = UnityEngine.Random.Range(0, 2);
                }
            }
            return randomDataset;
        }

        /// <summary>
        /// Converts the given tensor of shape (n,m)
        /// to a 3D list of shape (n,k,k)
        /// </summary>
        /// <param name="tensor"></param>
        /// <returns></returns>
        public static List<List<List<int>>> TensorToList(Tensor tensor)
        {
            float[] floatArray = tensor.ToReadOnlyArray();
            List<List<List<int>>> intList = new List<List<List<int>>>();
            int m = (int)Mathf.Sqrt(tensor.channels); // the size of the 2D array
            int n = tensor.batch; // calculate the number of 2D arrays that fit in the 1D array

            Debug.Log($"<color=yellow>| {tensor.batch} | {tensor.channels} |</color>");

            // iterate over each 2D array
            for (int i = 0; i < n; i++)
            {
                List<List<int>> innerList = new List<List<int>>();

                // iterate over each row of the 2D array
                for (int j = 0; j < m; j++)
                {
                    List<int> row = new List<int>();

                    // iterate over each element in the row
                    for (int k = 0; k < m; k++)
                    {
                        int index = i * m * m + j * m + k; // calculate the index of the element in the 1D array
                                                           //Debug.Log($"<color=cyan>| index : {index} | Val : {floatArray[index]} | Round : {Mathf.RoundToInt(floatArray[index])} | </color>");
                        row.Add(Mathf.RoundToInt(floatArray[index])); // convert the float to an int and add it to the row
                    }

                    innerList.Add(row); // add the row to the 2D array
                }

                intList.Add(innerList); // add the 2D array to the 3D array
            }
            Debug.Log($"<color=cyan>| i : {intList.Count} | j : {intList[0].Count} | | k : {intList[0][0].Count} </color>");
            return intList;
        }

        /// <summary>
        /// Converts a 2D list to a 1D list
        /// </summary>
        /// <param name="twoDList"></param>
        /// <returns></returns>
        public static List<int> Flatten2DList(List<List<int>> twoDList)
        {
            return twoDList.SelectMany(r => r).ToList();
        }

        /// <summary>
        /// Loads the given dataset and returns the 2D array
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static float[,] LoadDataset(string path)
        {

            // load the CSV file as a TextAsset
            TextAsset csvAsset = Resources.Load<TextAsset>(path);

            // get the contents of the CSV file as a string
            string csvContents = csvAsset.text;

            // split the contents into lines
            string[] lines = csvContents.Split('\n');
            int columns = lines[0].Split(',').Length;

            Debug.Log($"<color=magenta>| Rows : {lines.Length} | Columns : {columns} </color>");

            // create a 2D array to hold the data
            float[,] data = new float[lines.Length - 1, columns];

            // loop through the lines and parse the data
            for (int i = 0; i < lines.Length - 1; i++)
            {
                string[] fields = lines[i].Split(',');
                for (int j = 0; j < columns; j++)
                {
                    data[i, j] = float.Parse(fields[j]);
                }
            }
            return data;
        }

        /// <summary>
        /// Returns a randomly selected subset of the given dataset
        /// </summary>
        /// <param name="originalDataset"></param>
        /// <returns></returns>
        public static float[,] GetSubset(float[,] originalDataset, int num_of_samples)
        {
            int n = originalDataset.GetLength(0);
            int m = originalDataset.GetLength(1);

            int[] randomIndices = Enumerable.Range(0, n).OrderBy(i => UnityEngine.Random.value).Take(num_of_samples).ToArray();

            float[,] subsetArray = new float[num_of_samples, m];
            for (int i = 0; i < num_of_samples; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    subsetArray[i, j] = originalDataset[randomIndices[i], j];
                }
            }
            return subsetArray;
        }
    }
}

