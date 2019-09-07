using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIdriverV3 : MonoBehaviour
{
    //>> Neural network controlls
    static float[] inputNodesArray = new float[5] ;
    //static float[] inputNodesArray = { 1f, 2f, 3f, 4f, 5f };// for debug and putting Your own values
    static int[] hiddenLayersCount = {3}; // number of hidden layers and nodes in them exp. [3,5] two layers: First layer 3 nodes, Second layer 5 nodes  
    static float[] outputNodesArray = new float[1];
    static int activationFunctionType = 2; // Sigmoid = 0; RELU = 1; Tanh = 2;

    float[] hiddenLayerNodesArray = new float[CalculateNodesArrayCount()];

    public float[] weightsArray = new float[CalculateWeightsCount()] ;
    //float[] weightsArray = {0.05f, 0.1f, 0.15f, 0.2f, 0.25f, 0.3f, 0.35f, 0.4f, 0.45f, 0.5f, 0.55f, 0.6f, 0.65f, 0.7f, 0.75f, 0.8f, 0.85f, 0.9f};
    //{ 0.1f, 0.5f, 0.1f, 0.5f, 0.1f, 0.5f, 0.1f, 0.5f, 0.1f, 0.5f, 0.1f, 0.5f, 0.1f, 0.5f, 0.1f, 0.5f, 0.1f, 0.5f, 0.1f, 0.5f, 0.1f, 0.5f, 0.1f, 0.5f, };
    float[] biasArray = new float[CalculateBiasArrayCount()] ;
    //float[] biasArray = { 0.1f, 0.2f, 0.3f, 0.4f};
    //{ 0.1f, 0.2f, 0.1f, 0.2f, 0.1f, 0.2f, 0.1f, 0.2f, 0.1f, 0.2f};

    public float[] MasterMindArray = new float[CalculateWeightsCount()+CalculateBiasArrayCount()]; // the whole thought process Weights + bias

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
    public float score = 0f;
    public bool isDead = false;

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
    //static int CalculateMasterMindArrayCount()
    //{
    //    int arrayCount = biasArray.Length 
    //}
    public float[] SetValuesToArray(float[] array, float value)
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

    public void RandomizeWeights(float min, float max) {
        for (int i = 0; i < weightsArray.Length; i++)
        {
            weightsArray[i] = Random.Range(min,max);
        }
       
    }


    public void MakeMasterMindArray()
    {
        int tempNr = 0;
        for (int i =0; i<weightsArray.Length; i++)
        {
            MasterMindArray[i] = weightsArray[i];
            tempNr++;
        }
        for (int i = 0; i<biasArray.Length; i++)
        {
            MasterMindArray[tempNr] = biasArray[i];
            tempNr++;
        }
    }

    public void ReadWeightAndBiasFromArray(float[] tempArray)
    {
        int tempNr = 0;
        for(int i=0; i<weightsArray.Length; i++)
        {
            weightsArray[i] = tempArray[i];
            tempNr++;
        }
        for(int i=0; i<biasArray.Length; i++)
        {
            biasArray[i] = tempArray[tempNr];
            tempNr++;
        }

    }

    void CalcuateNeuralNetwork() 
    {
        for(int i =0; i< hiddenLayersCount[0]; i++)// calculates the nodes values in first hidden layer 3
        {
            for(int j =0; j<inputNodesArray.Length; j++)// 5
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
                CalculateTempNodeNr(i);
                for (int k = 0; k< hiddenLayersCount[i]; k++)
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
            for(int j = 0; j< hiddenLayersCount[hiddenLayersCount.Length - 1]; j++)
            {
                outputNodesArray[i] += hiddenLayerNodesArray[tempNodeNr+j]* weightsArray[currentWeight];
                //print(outputNodesArray[i]);
                currentWeight++;
            }
        }

        for (int i=0; i<outputNodesArray.Length; i++)
        {
            outputNodesArray[i] += biasArray[(biasArray.Length - outputNodesArray.Length)+i];
            outputNodesArray[i] = ActivationFunction(outputNodesArray[i], activationFunctionType);
        }

        // Zeroing the node, weight and bias nr
        currentNode=0;
        currentWeight=0;
        currentBias=0;
        
    }

    void Start()
    {
       SetValuesToArray(weightsArray, Random.Range(-1f, 0f));
       SetValuesToArray(biasArray, Random.Range(-1f, 0f));
        //For debug only
        //inputNodesArray = SetValuesToArray(inputNodesArray, 0.5f);
        //weightsArray = SetValuesToArray(weightsArray, 0.5f);
        //biasArray = SetValuesToArray(biasArray, 0.1f);

        //CalcuateNeuralNetwork();
        //MakeMasterMindArray();
        //PrintArray1d(MasterMindArray);

        //ReadWeightAndBiasFromArray(MasterMindArray);

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
        if (isDead == false)
        {
            RaycastRays();
            CalcuateNeuralNetwork();
            transform.Translate(Vector3.forward * Time.deltaTime * ForwardSpeed);// to porusza samochod do przodu
            transform.Rotate(0, outputNodesArray[0] * TurnSpeed, 0); // to skreca samochod , Space.Self
            SetValuesToArray(hiddenLayerNodesArray, 0f);
            SetValuesToArray(outputNodesArray, 0f);

            print("Input " + inputNodesArray[0] + " " + inputNodesArray[1] + " " + inputNodesArray[2] + " " + inputNodesArray[3] + " " + inputNodesArray[4]);
            print("NN Output " + outputNodesArray[0]);
            //PrintArray1d(hiddenLayerNodesArray);
            CountScore();
        }

        //print("INPUTS array length = " + inputNodesArray.Length);
        //PrintArray1d(inputNodesArray);

        //print("WEIGHTS array length = " + weightsArray.Length);
        //PrintArray1d(weightsArray);

        //print("HIDDEN NODES array length = " + hiddenLayerNodesArray.Length);
        //PrintArray1d(hiddenLayerNodesArray);

        //print("BIAS array length = " + biasArray.Length);
        //PrintArray1d(biasArray);

        //print("OUTPUTS array length = " + outputNodesArray.Length);
        //PrintArray1d(outputNodesArray);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "car")
        {
            isDead = true;
            // print("BOOOOM Score: "+score);
        }
    }

    void RaycastRays()
    {

        //inputNodesArray[0] = 5f;
        //inputNodesArray[1] = 4f;
        //inputNodesArray[2] = 6f;
        //inputNodesArray[3] = 2f;
        //inputNodesArray[4] = 1f;

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

        //print(inputNodesArray[0] + "  " + inputNodesArray[1] + "  " + inputNodesArray[2] + "  " + inputNodesArray[3] + "  " + inputNodesArray[4]);
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

    public void ResetCar()
    {
        isDead = false;
        score = 0;
    }
}
