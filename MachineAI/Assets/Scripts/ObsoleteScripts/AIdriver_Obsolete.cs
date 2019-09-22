using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AIdriver_Obsolete : MonoBehaviour
{
    public float ForwardSpeed = 5.0f;
    public float TurnSpeed = 2f;
    public float Turn = 0.0f; // -1 left turn 1 right turn

    float distance1;
    float distance2;
    float distance3;
    float distance4;
    float distance5;

    int inputNodesNr = 5; // Number of input nodes
    int hiddenLayerNr1Nr = 4; // Number of hidden layer 1 nodes
    int outputNodesNr = 1;
    public float[] I = new float[5]; //Input vector(array) length 
    public float[] W = new float[24]; //{0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2}; //All layers weights array (Input nodes * H1 + H2*H3 + ... + Hn * Output nodes)
    public float[] H1 = new float[4]; //First layer neuron output array
    public float[] B = new float[5] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f }; // Bias array hidden layers nodes + output nodes
    public float O; // Neural Network output
    public float[] MasterMind = new float[29]; // the whole thought process Weights + bias

    public float[] distance = new float[5];

    public float score = 0;

    public bool isDead = false;



    void Start()
    {
        //W = RandomizeArray1d(-1f, 1f, 24);
    }

    void Update()
    {
        if (isDead == false)
        {

            RaycastRays();
            I = distance;
            CalculateH();
            CalculateO();
            print(O);
            Turn = O;
            //KeyboardStering();
            //SterowanieRandomowe();

            transform.Translate(Vector3.forward * Time.deltaTime * ForwardSpeed);// to porusza samochod do przodu
            transform.Rotate(0, Turn * TurnSpeed, 0); // to skreca samochod , Space.Self
           

            CountScore();
        }
        
    }

    public void PrintW()
    {
        PrintArray1d(W);
    }
    public void RandomizeW()
    {
        W = RandomizeArray1d(-1f, 1f, W.Length);
    }

    void KeyboardStering() // if You want to drive it yourself. Use in update
    {
        Turn = Input.GetAxis("Horizontal"); 
    }

    void RandomStering() // if You want the turn value to be generated randomly. Use in update
    {
        Turn = Random.Range(-1.0f, 1.0f);
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
        RaycastHit hit1;
        Ray ray1 = new Ray(transform.position, transform.TransformDirection(new Vector3(0, 0, 1)));
        Physics.Raycast(ray1, out hit1, 100.0f);
        distance1 = hit1.distance;
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(0, 0, 1) * 10));
        distance[0] = distance1;

        RaycastHit hit2;
        Ray ray2 = new Ray(transform.position, transform.TransformDirection(new Vector3(1, 0, 1)));
        Physics.Raycast(ray2, out hit2, 100.0f);
        distance2 = hit2.distance;
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(1, 0, 1) * 10));
        distance[1] = distance2;

        RaycastHit hit3;
        Ray ray3 = new Ray(transform.position, transform.TransformDirection(new Vector3(-1, 0, 1)));
        Physics.Raycast(ray3, out hit3, 100.0f);
        distance3 = hit3.distance;
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(-1, 0, 1) * 10));
        distance[2] = distance3;

        RaycastHit hit4;
        Ray ray4 = new Ray(transform.position, transform.TransformDirection(new Vector3(0.25f, 0, 0.75f)));
        Physics.Raycast(ray4, out hit4, 100.0f);
        distance4 = hit4.distance;
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(0.25f, 0, 0.75f) * 10));
        distance[3] = distance4;

        RaycastHit hit5;
        Ray ray5 = new Ray(transform.position, transform.TransformDirection(new Vector3(-0.25f, 0, 0.75f)));
        Physics.Raycast(ray5, out hit5, 100.0f);
        distance5 = hit5.distance;
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(-0.25f, 0, 0.75f) * 10));
        distance[4] = distance5;

        //print(distance1 + "  " + distance2 + "  " + distance3 + "  " + distance4 + "  " + distance5);
    }

    public void SetMasterMind(float[] mindArray)
    {
        int currentArrayElement = 0;
        for (int i = 0; i < W.Length; i++)
        {
            W[i] = mindArray[currentArrayElement];
            currentArrayElement++;
        }
        for (int i = 0; i < B.Length; i++)
        {
            B[i] = mindArray[currentArrayElement];
            currentArrayElement++;
        }

    }

    public void GenerateMind()
    {
        List<float> tempList = new List<float>();
        for (int i = 0; i < W.Length; i++)
        {
            tempList.Add(W[i]);
        }
        for (int i = 0; i < B.Length; i++)
        {
            tempList.Add(B[i]);
        }
        for (int i = 0; i < tempList.Count; i++)
        {
            MasterMind[i] = tempList[i];
        }
    }

    public void CalculateO()
    {
        float tempO = 0;
        int currentW = inputNodesNr * hiddenLayerNr1Nr;
        for (int i = 0; i < hiddenLayerNr1Nr; i++)
        {
            tempO += (H1[i] * W[currentW]);
            currentW++;
        }
        tempO += B[hiddenLayerNr1Nr];

        //O = (Mathf.Exp(tempO)) / ((Mathf.Exp(tempO)) + 1); // Sigmoid activation function

        //if (tempO < 0) { O = 0; } // RELU activation function
        //else if (tempO > 0) { O = tempO; }

        O = (1 - Mathf.Exp(-2 * tempO)) / (1 + Mathf.Exp(-2 * tempO));// Tanh activation function
        
        tempO = 0;
    }

    public void CalculateH()
    {
        float tempH = 0;
        int currentW = 0;
        int currentB = 0;
        for (int i = 0; i < hiddenLayerNr1Nr; i++)
        {
            for (int j = 0; j < inputNodesNr; j++)
            {
                tempH += W[currentW] * I[j];
                currentW++;
            }
            tempH += B[currentB];
            currentB++;
            //H1[i] = (Mathf.Exp(tempH)) / ((Mathf.Exp(tempH)) + 1); // Sigmoid activation function

            //if(tempH < 0) { H1[i] = 0; } // RELU activation function
            //else if(tempH > 0) { H1[i] = tempH; }

            H1[i] = (1 - Mathf.Exp(-2 * tempH)) / (1 + Mathf.Exp(-2 * tempH)); // Tanh activation function
            
            tempH = 0;
        }
    }

    public void CountScore()
    {
        //score += ((distance1 + distance2 + distance3 + distance4 + distance5)/5)*0.1;
        for (int i = 0; i < distance.Length; i++)
        {
            score += distance[i];
        }
        //score = (score / 5);
    }

    public void ResetCar()
    {
        isDead = false;
        score = 0;
    }

    public float[] RandomizeArray1d(float min, float max, int nrOfValues)
    {
        float[] outputArray = new float[nrOfValues];
        for (int i = 0; i < nrOfValues; i++)
        {
            outputArray[i] = Random.Range(min, max);
        }
        return outputArray;
    }

    public void PrintArray1d(float[] array) // Debug metod for printing array
    {
        for (int i = 0; i < array.Length; i++)
        {
            print(array[i] + " ");
        }
    }
}

