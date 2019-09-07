using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Add the NeuralNet component to your game object 
// Reference it exp.  NeuralNet Brain = GetComponent<NeuralNetV3>();

public class NeuralNet : MonoBehaviour
{
    public float[] hiddenLayerNodesArray = new float[0];
    public float[] weightsArray = new float[0];
    public float[] biasArray = new float[0];
    public float[] MasterMindArray = new float[0];

    public List<float> hiddenLayerNodesList;
    public List<float> weightsList;
    public List<float> biasList;
    public List<float> MasterMindList;

    public  float[] inputNodesArray = new float[0];
    public int[] hiddenLayersCount = new int[0];
    public  float[] outputNodesArray = new float[0];
    public  int activationFunctionType;

    public static List<float> inputNodesList;
    public static List<int> hiddenLayersCountList;
    public static List<float> outputNodesList;

    //static int inpNdArrNr;
    //static int[] hdnNdArrNr;
    //static int outNodArrNr;
    //static int actFunTypNr;

    public void InitializeNeuralNetwork(int nrOfInputs, List<int> hiddenLayersArray, int nrOfOutputs, int activationFunctionNr)
    {
        inputNodesList = InitializeFloatList(nrOfInputs);// number of inputs. InitializeFloatList populates the list with 0
        hiddenLayersCountList =  hiddenLayersArray; // number of hidden layers and nodes in them exp. [3,5] two layers: First layer 3 nodes, Second layer 5 nodes
        outputNodesList = InitializeFloatList(nrOfOutputs);
        int activationFunctionType = activationFunctionNr; // Sigmoid = 0; RELU = 1; Tanh = 2;

        hiddenLayerNodesList = InitializeFloatList(CalculateNodesArrayCount());
        weightsList = InitializeFloatList(CalculateWeightsCount());
        biasList = InitializeFloatList(CalculateBiasArrayCount());
        MasterMindList = InitializeFloatList(CalculateWeightsCount() + CalculateBiasArrayCount()); 

        Array.Resize(ref hiddenLayerNodesArray, hiddenLayerNodesArray.Length + hiddenLayerNodesList.Count); // changes the array sizes to list sizes
        Array.Resize(ref weightsArray, weightsArray.Length + weightsList.Count);
        Array.Resize(ref biasArray, biasArray.Length + biasList.Count);
        Array.Resize(ref MasterMindArray, MasterMindArray.Length + MasterMindList.Count);
        Array.Resize(ref inputNodesArray, inputNodesArray.Length + inputNodesList.Count);
        Array.Resize(ref outputNodesArray, outputNodesArray.Length + outputNodesList.Count);
        Array.Resize(ref hiddenLayersCount, hiddenLayersCount.Length + hiddenLayersCountList.Count);

        TransferFloatListToArray(hiddenLayerNodesList, hiddenLayerNodesArray); // puts list data to arrays, becouse getting/putting data to array is faster in most cases
        TransferFloatListToArray(weightsList, weightsArray);
        TransferFloatListToArray(biasList, biasArray);
        TransferFloatListToArray(MasterMindList, MasterMindArray);
        TransferFloatListToArray(inputNodesList, inputNodesArray);
        TransferFloatListToArray(outputNodesList, outputNodesArray);
        TransferIntListToArray(hiddenLayersCountList, hiddenLayersCount);


    }

    int currentNode = 0;
    int currentWeight = 0;
    int currentBias = 0;
    int tempNodeNr = 0;
    int tempOutputNr = 0;

    public float[] InputNodesArray
    {
        get { return inputNodesArray; }
        set { inputNodesArray = value; }
    }
    public float[] OutputNodesArray
    {
        get => outputNodesArray; 
        set => inputNodesArray = value; 
    }

    private static int CalculateWeightsCount()
    {
        int weightsCount = 0;
        weightsCount += (inputNodesList.Count * hiddenLayersCountList[0]); // input layer weights
        weightsCount += hiddenLayersCountList[hiddenLayersCountList.Count - 1] * outputNodesList.Count;// output layer weights
        for (int i = 0; i < hiddenLayersCountList.Count - 1; i++)// hidden layer weights
        {
            weightsCount += hiddenLayersCountList[i] * hiddenLayersCountList[i + 1];
        }
        return weightsCount;
    }
    private static int CalculateBiasArrayCount()
    {
        int biasCount = 0;
        for (int i = 0; i < hiddenLayersCountList.Count; i++)
        {
            biasCount += hiddenLayersCountList[i];
        }
        biasCount += outputNodesList.Count;
        return biasCount;
    }
     private static int CalculateNodesArrayCount()
    {
        //print(hiddenLayersCountList.Count);
        int nodesArrayCount = 0;
        for (int i = 0; i < hiddenLayersCountList.Count; i++)
        {
            nodesArrayCount += hiddenLayersCountList[i];
        }
        return nodesArrayCount;
    }

    private float ActivationFunction(float input, int activMetod)
    {
        float activatedValue = 0;

        switch (activMetod)
        {
            case 0:// Sigmoid
                activatedValue = (Mathf.Exp(input)) / ((Mathf.Exp(input)) + 1);
                break;
            case 1:// RELU
                if (input < 0) { activatedValue = 0; } // RELU activation function
                else { activatedValue = input; }
                break;
            case 2:// Tanh
                activatedValue = (1 - Mathf.Exp(-2 * input)) / (1 + Mathf.Exp(-2 * input)); ;
                break;
        }
        return activatedValue;
    }
    private void CalculateTempNodeNr(int nr)
    {
        for (int i = 0; i < nr; i++)
        {
            tempNodeNr += hiddenLayersCount[i];
        }
    }

    public void MakeMasterMindArray()
    {
        int tempNr = 0;
        for (int i = 0; i < weightsArray.Length; i++)
        {
            MasterMindArray[i] = weightsArray[i];
            tempNr++;
        }
        for (int i = 0; i < biasArray.Length; i++)
        {
            MasterMindArray[tempNr] = biasArray[i];
            tempNr++;
        }
    }

    public void ReadWeightAndBiasFromArray(float[] tempArray)
    {
        int tempNr = 0;
        for (int i = 0; i < weightsArray.Length; i++)
        {
            weightsArray[i] = tempArray[i];
            tempNr++;
        }
        for (int i = 0; i < biasArray.Length; i++)
        {
            biasArray[i] = tempArray[tempNr];
            tempNr++;
        }

    }

    public void CalcuateNeuralNetwork()
    {
        //print("Calculating neural network");
        for (int i = 0; i < hiddenLayersCount[0]; i++)// calculates the nodes values in first hidden layer 3
        {
            for (int j = 0; j < inputNodesArray.Length; j++)// 5
            {
                hiddenLayerNodesArray[i] += (inputNodesArray[j] * weightsArray[currentWeight]);
                currentWeight++;
            }
            hiddenLayerNodesArray[currentNode] += biasArray[currentBias];
            hiddenLayerNodesArray[currentNode] = ActivationFunction(hiddenLayerNodesArray[currentNode], activationFunctionType); // activates the function using selected activation function
            currentNode++;
            currentBias++;
        }

        for (int i = 0; i < hiddenLayersCount.Length - 1; i++)// calculates the rest hiden layer nodes values
        {
            for (int j = 0; j < hiddenLayersCount[i + 1]; j++)
            {
                CalculateTempNodeNr(i);
                for (int k = 0; k < hiddenLayersCount[i]; k++)
                {
                    hiddenLayerNodesArray[currentNode] += hiddenLayerNodesArray[tempNodeNr] * weightsArray[currentWeight];
                    currentWeight++;
                    tempNodeNr++;
                }
                hiddenLayerNodesArray[currentNode] += biasArray[currentBias];
                hiddenLayerNodesArray[currentNode] = ActivationFunction(hiddenLayerNodesArray[currentNode], activationFunctionType); // activates the function using selected activation function
                currentNode++;
                currentBias++;
                tempNodeNr = 0;
            }
        }

        //--- \/ Calculates the output nodes \/ ---
        if (hiddenLayersCount.Length == 1)
        {
            tempNodeNr = hiddenLayersCount[0];
        }
        else
        {
            CalculateTempNodeNr(hiddenLayersCount[hiddenLayersCount.Length - 1]);
        }

        tempNodeNr -= hiddenLayersCount[hiddenLayersCount.Length - 1];
        for (int i = 0; i < outputNodesArray.Length; i++)
        {
            for (int j = 0; j < hiddenLayersCount[hiddenLayersCount.Length - 1]; j++)
            {
                outputNodesArray[i] += hiddenLayerNodesArray[tempNodeNr + j] * weightsArray[currentWeight];
                //print(outputNodesArray[i]);
                currentWeight++;
            }
        }

        for (int i = 0; i < outputNodesArray.Length; i++)
        {
            outputNodesArray[i] += biasArray[(biasArray.Length - outputNodesArray.Length) + i];
            outputNodesArray[i] = ActivationFunction(outputNodesArray[i], activationFunctionType);
        }

        // Zeroing the node, weight and bias nr
        currentNode = 0;
        currentWeight = 0;
        currentBias = 0;

    }

    public void PrintArray1d(float[] array) // Debug metod for printing array in a line changing it to a string
    {
        string tempString = "";
        for (int i = 0; i < array.Length; i++)
        {
            tempString += ("   " + array[i].ToString());
        }
        print(tempString);
    }

    public float[] SetValuesToArray(float[] array, float value)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = value;
        }
        return array;
    }

    void TransferFloatListToArray(List<float> inputList, float[] outputArray)
    {
        //Array.Resize(ref outputArray, outputArray.Length + inputList.Count);

        for(int i=0; i<inputList.Count; i++)
        {
            outputArray[i] = inputList[i];
        }
    }

    void TransferIntListToArray(List<int> inputList, int[] outputArray)
    {
        //Array.Resize(ref outputArray, outputArray.Length + inputList.Count);

        for (int i = 0; i < inputList.Count; i++)
        {
            outputArray[i] = inputList[i];
        }
    }

    List<float> InitializeFloatList(int listLength)
    {
        List<float>  outputList = new List<float>();
        for(int i = 0; i<listLength; i++)
        {
            outputList.Add(0f);
        }
        return outputList;
    }

    List<int> InitializeINTList(int listLength)
    {
        List<int> outputList = new List<int>();
        for (int i = 0; i < listLength; i++)
        {
            outputList.Add(0);
        }
        return outputList;
    }

    public void RandomizeWeightsArray()
    {
        SetValuesToArray(weightsArray, UnityEngine.Random.Range(-1f, 1f));
    }
}
