using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIdriverV2 : MonoBehaviour
{
    //>> Neural network controlls
    static float[] inputNodesArray = new float[5] ;
    //static float[] inputNodesArray = { 0.1f, 0.9f };// for debug and putting Your own values
    static int[] hiddenLayersCount = {3}; // number of hidden layers and nodes in them exp. [3,5] two layers: First layer 3 nodes, Second layer 5 nodes  
    static float[] outputNodesArray = new float[2];
    static int activationFunctionType = 2; // Sigmoid = 0; RELU = 1; Tanh = 2;

    float[] hiddenLayerNodesArray = new float[CalculateNodesArrayCount()];

    float[] weightsArray = new float[CalculateWeightsCount()] ;
    //float[] weightsArray = {0.05f, 0.1f, 0.15f, 0.2f, 0.25f, 0.3f, 0.35f, 0.4f, 0.45f, 0.5f, 0.55f, 0.6f, 0.65f, 0.7f, 0.75f, 0.8f, 0.85f, 0.9f, 0.95f, 1f, 1.05f, 1.1f, 1.15f, 1.2f };
    //{ 0.1f, 0.5f, 0.1f, 0.5f, 0.1f, 0.5f, 0.1f, 0.5f, 0.1f, 0.5f, 0.1f, 0.5f, 0.1f, 0.5f, 0.1f, 0.5f, 0.1f, 0.5f, 0.1f, 0.5f, 0.1f, 0.5f, 0.1f, 0.5f, };
    float[] biasArray = new float[CalculateBiasArrayCount()] ;
    //float[] biasArray = { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1f };
                      //{ 0.1f, 0.2f, 0.1f, 0.2f, 0.1f, 0.2f, 0.1f, 0.2f, 0.1f, 0.2f};
    int currentNode = 0;
    int currentWeight = 0;
    int currentBias = 0;
    int tempNodeNr = 0;
    int tempOutputNr = 0;

    //>> Car controlls
    public float ForwardSpeed = 5.0f;
    public float TurnSpeed = 2f;
    public float Turn = 0.0f; // -1 left turn 1 right turn
    float tempRayDistance=0f;
    float score = 0f;

    static int CalculateWeightsCount()
    {
        int weightsCount = 0;
        weightsCount += (inputNodesArray.Length * hiddenLayersCount[0]); // input layer weights
        weightsCount += hiddenLayersCount[hiddenLayersCount.Length - 1] * outputNodesArray.Length;// output layer weights
        for(int i =0; i<hiddenLayersCount.Length-1; i++)// hidden layer weights
        {
            weightsCount += hiddenLayersCount[i] * hiddenLayersCount[i + 1];
        }
        return weightsCount;
    }
    static int CalculateBiasArrayCount()
    {
        int biasCount = 0;
        for (int i = 0; i < hiddenLayersCount.Length; i++)
        {
            biasCount += hiddenLayersCount[i] ;
        }
        biasCount += outputNodesArray.Length;
        return biasCount;
    }
    static int CalculateNodesArrayCount()
    {
        int nodesArrayCount = 0;
        for (int i = 0; i < hiddenLayersCount.Length; i++)
        {
            nodesArrayCount += hiddenLayersCount[i];
        }
        return nodesArrayCount;
    }
    float[] SetValuesToArray(float[] array, float value)
    {
        for(int i=0; i<array.Length; i++)
        {
            array[i] = value;
        }
        return array;
    }
    float ActivationFunction(float input, int activMetod)
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

    void CalculateTempNodeNr(int nr)
    {
        for(int i=0; i<nr; i++)
        {
            tempNodeNr += hiddenLayersCount[i];
        }
    }

    public void PrintArray1d(float[] array) // Debug metod for printing array in a line changing it to a string
    {
        string tempString = "";
        for (int i = 0; i < array.Length; i++)
        {
            tempString += ("   "+array[i].ToString());
        }
        print(tempString);
    }

    void CalcuateNeuralNetwork() 
    {
        for(int i =0; i< hiddenLayersCount[0]; i++)// calculates the nodes values in first hidden layer
        {
            for(int j =0; j<inputNodesArray.Length; j++)
            {
                hiddenLayerNodesArray[i] += (inputNodesArray[j]*weightsArray[currentWeight]);
                currentWeight++;
            }
            hiddenLayerNodesArray[currentNode] += biasArray[currentBias]; 
            hiddenLayerNodesArray[currentNode] = ActivationFunction(hiddenLayerNodesArray[currentNode], activationFunctionType); // activates the function using selected activation function
            currentNode++;
            currentBias++;
        }

        for(int i = 0; i< hiddenLayersCount.Length-1; i++)// calculates the rest hiden layer nodes values
        {
            for (int j=0; j < hiddenLayersCount[i+1]; j++ )
            {
                //print("J loop indicator " + hiddenLayersCount[i + 1]);
                print(i);
                CalculateTempNodeNr(i);

                //print("temp node nr before loop " + tempNodeNr);

                for (int k = 0; k< hiddenLayersCount[i]; k++)
                {

                    //print("K loop indicator " + hiddenLayersCount[i]);

                    hiddenLayerNodesArray[currentNode] += hiddenLayerNodesArray[tempNodeNr] * weightsArray[currentWeight];

                    //print("Calculated Node nr " + currentNode + " prev node nr = "+ tempNodeNr + 
                    //        " prev node value = " + hiddenLayerNodesArray[tempNodeNr] + 
                    //        " *  weight " + weightsArray[currentWeight] + " = " + hiddenLayerNodesArray[currentNode]);

                    currentWeight++;
                    tempNodeNr++;
 
                }
                //print("Current bias " +currentBias);
                hiddenLayerNodesArray[currentNode] += biasArray[currentBias]; 
                //print("FINAL NODE nr " + currentNode + "after adding bias = " + hiddenLayerNodesArray[currentNode] + " tempNodeNr AFTER loop " + tempNodeNr);
                hiddenLayerNodesArray[currentNode] = ActivationFunction(hiddenLayerNodesArray[currentNode], activationFunctionType); // activates the function using selected activation function
                currentNode++;
                currentBias++;
                tempNodeNr = 0;

            }
        }

        //--- \/ Calculates the output nodes \/ ---
        CalculateTempNodeNr(hiddenLayersCount[hiddenLayersCount.Length - 1]);
        tempNodeNr -= hiddenLayersCount[hiddenLayersCount.Length - 1];
        for (int i = 0; i < outputNodesArray.Length; i++)
        {
            for(int j = 0; j< hiddenLayersCount[hiddenLayersCount.Length - 1]; j++)
            {
                outputNodesArray[i] += hiddenLayerNodesArray[tempNodeNr+j]* weightsArray[currentWeight];
                currentWeight++;
            }
        }

        for (int i=0; i<outputNodesArray.Length; i++)
        {
            outputNodesArray[i] += biasArray[(biasArray.Length - outputNodesArray.Length)+i];
            outputNodesArray[i] = ActivationFunction(outputNodesArray[i], activationFunctionType);
        }
    }

    void Start()
    {
        //For debug only
        //inputNodesArray = SetValuesToArray(inputNodesArray, 0.5f);
        //weightsArray = SetValuesToArray(weightsArray, 0.5f);
        //biasArray = SetValuesToArray(biasArray, 0.1f);
       
        CalcuateNeuralNetwork();
        //print(hiddenLayerNodesArray[0]);
        //print(hiddenLayerNodesArray[1]);
        //print(hiddenLayerNodesArray[2]);
        //print(hiddenLayerNodesArray[3]);
        //print(hiddenLayerNodesArray[4]);
        //print(hiddenLayerNodesArray[5]);
        //print(hiddenLayerNodesArray[6]);
        //print(hiddenLayerNodesArray[7]);
        //CalculateTempNodeNr(0);
        //print("tempNodeNr" + tempNodeNr);


        print("INPUTS array length = " + inputNodesArray.Length);
        PrintArray1d(inputNodesArray);

        print("WEIGHTS array length = " + weightsArray.Length);
        PrintArray1d(weightsArray);

        print("HIDDEN NODES array length = " + hiddenLayerNodesArray.Length);
        PrintArray1d(hiddenLayerNodesArray);

        print("BIAS array length = " + biasArray.Length);
        PrintArray1d(biasArray);

        print("OUTPUTS array length = " + outputNodesArray.Length);
        PrintArray1d(outputNodesArray);

    }

    void Update()
    {
        //RaycastRays();
        //CalcuateNeuralNetwork();
        //transform.Translate(Vector3.forward * Time.deltaTime * ForwardSpeed);// to porusza samochod do przodu
        //transform.Rotate(0, outputNodesArray[0] * TurnSpeed, 0); // to skreca samochod , Space.Self

        //CountScore();
    }

    void RaycastRays()
    {
        RaycastHit hit1;
        Ray ray1 = new Ray(transform.position, transform.TransformDirection(new Vector3(0, 0, 1)));
        Physics.Raycast(ray1, out hit1, 100.0f);
        tempRayDistance = hit1.distance;
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(0, 0, 1) * 10));
        inputNodesArray[0] = tempRayDistance;

        RaycastHit hit2;
        Ray ray2 = new Ray(transform.position, transform.TransformDirection(new Vector3(1, 0, 1)));
        Physics.Raycast(ray2, out hit2, 100.0f);
        tempRayDistance = hit2.distance;
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(1, 0, 1) * 10));
        inputNodesArray[1] = tempRayDistance;

        RaycastHit hit3;
        Ray ray3 = new Ray(transform.position, transform.TransformDirection(new Vector3(-1, 0, 1)));
        Physics.Raycast(ray3, out hit3, 100.0f);
        tempRayDistance = hit3.distance;
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(-1, 0, 1) * 10));
        inputNodesArray[2] = tempRayDistance;

        RaycastHit hit4;
        Ray ray4 = new Ray(transform.position, transform.TransformDirection(new Vector3(0.25f, 0, 0.75f)));
        Physics.Raycast(ray4, out hit4, 100.0f);
        tempRayDistance = hit4.distance;
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(0.25f, 0, 0.75f) * 10));
        inputNodesArray[3] = tempRayDistance;

        RaycastHit hit5;
        Ray ray5 = new Ray(transform.position, transform.TransformDirection(new Vector3(-0.25f, 0, 0.75f)));
        Physics.Raycast(ray5, out hit5, 100.0f);
        tempRayDistance = hit5.distance;
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(-0.25f, 0, 0.75f) * 10));
        inputNodesArray[4] = tempRayDistance;

        //print(distance1 + "  " + distance2 + "  " + distance3 + "  " + distance4 + "  " + distance5);
    }

    public void CountScore()
    {
        //score += ((distance1 + distance2 + distance3 + distance4 + distance5)/5)*0.1;
        for (int i = 0; i < inputNodesArray.Length; i++)
        {
            score += inputNodesArray[i];
        }
        //score = (score / 5);
    }
}
