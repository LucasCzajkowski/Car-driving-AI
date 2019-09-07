using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NeuralNet_Obsolete : MonoBehaviour
{
    private void Awake()
    {
        
    }
    public NeuralNet_Obsolete(int nrOfInputs, int[] hiddenLayersArray, int nrOfOutputs, int activationFunctionNr)
    {
        inpNdArrNr = nrOfInputs;
        hdnNdArrNr = hiddenLayersArray;
        outNodArrNr = nrOfOutputs;
        actFunTypNr = activationFunctionNr;
    }

    static int inpNdArrNr;
    static int[] hdnNdArrNr;
    static int outNodArrNr;
    static int actFunTypNr;

    private static float[] inputNodesArray = new float[inpNdArrNr]; // Nr of inputs
    static int[] hiddenLayersCount = hdnNdArrNr; // number of hidden layers and nodes in them exp. [3,5] two layers: First layer 3 nodes, Second layer 5 nodes  
    static float[] outputNodesArray = new float[outNodArrNr];
    static int activationFunctionType = actFunTypNr; // Sigmoid = 0; RELU = 1; Tanh = 2;

    public float[] hiddenLayerNodesArray = new float[CalculateNodesArrayCount()];//CalculateNodesArrayCount()
    public float[] weightsArray = new float[CalculateWeightsCount()];//CalculateWeightsCount()
    public float[] biasArray = new float[CalculateBiasArrayCount()];//CalculateBiasArrayCount()
    public float[] MasterMindArray = new float[CalculateWeightsCount() + CalculateBiasArrayCount()]; //CalculateWeightsCount() + CalculateBiasArrayCount() the whole thought process array length Weights + bias

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
        weightsCount += (inputNodesArray.Length * hiddenLayersCount[0]); // input layer weights
        weightsCount += hiddenLayersCount[hiddenLayersCount.Length - 1] * outputNodesArray.Length;// output layer weights
        for (int i = 0; i < hiddenLayersCount.Length - 1; i++)// hidden layer weights
        {
            weightsCount += hiddenLayersCount[i] * hiddenLayersCount[i + 1];
        }
        return weightsCount;
    }
    private static int CalculateBiasArrayCount()
    {
        int biasCount = 0;
        for (int i = 0; i < hiddenLayersCount.Length; i++)
        {
            biasCount += hiddenLayersCount[i];
        }
        biasCount += outputNodesArray.Length;
        return biasCount;
    }
     private static int CalculateNodesArrayCount()
    {
        print(hiddenLayersCount.Length);
        int nodesArrayCount = 0;
        for (int i = 0; i < hiddenLayersCount.Length; i++)
        {
            nodesArrayCount += hiddenLayersCount[i];
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
        //print(tempString);
    }

    public float[] SetValuesToArray(float[] array, float value)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = value;
        }
        return array;
    }
}
