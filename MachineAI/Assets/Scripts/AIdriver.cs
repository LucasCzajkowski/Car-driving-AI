using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIdriver : MonoBehaviour
{
    //>> Car controlls
    public float ForwardSpeed = 5.0f;
    public float TurnSpeed = 2f;
    public float Turn = 0.0f; // -1 left turn 1 right turn
    float tempRayDistance = 0f;
    public float score = 0f;
    public bool isDead = false;

    static List<int> hiddenLayers = new List<int>{ 3 }; // a list that defines the hidden nodes setup exp. {2,3,3} three layers first layer 2 nodes second layer 3 nodes third layer 3 nodes
    public NeuralNet Brain;

    void Start()
    {
        Brain = GetComponent<NeuralNet>();   
        Brain.InitializeNeuralNetwork(5, hiddenLayers, 1, 2);
        Brain.SetValuesToArray(Brain.weightsArray, Random.Range(-1f, 1f));
        Brain.SetValuesToArray(Brain.biasArray, Random.Range(-1f, 1f));

        //print("INPUTS array length = " + Brain.InputNodesArray.Length);
        //Brain.PrintArray1d(Brain.InputNodesArray);

        //print("WEIGHTS array length = " + Brain.weightsArray.Length);
        //Brain.PrintArray1d(Brain.weightsArray);

        //print("HIDDEN NODES array length = " + Brain.hiddenLayerNodesArray.Length);
        //Brain.PrintArray1d(Brain.hiddenLayerNodesArray);

        //print("BIAS array length = " + Brain.biasArray.Length);
        //Brain.PrintArray1d(Brain.biasArray);

        //print("OUTPUTS array length = " + Brain.OutputNodesArray.Length);
        //Brain.PrintArray1d(Brain.OutputNodesArray);

        //Brain.MakeMasterMindArray();
        //print("MasterMind array length = " + Brain.MasterMindArray.Length);
        //Brain.PrintArray1d(Brain.MasterMindArray);

        //Brain.ReadWeightAndBiasFromArray(Brain.MasterMindArray);
        //print("Setting values from mastermind array to weights and bias");

        //print("WEIGHTS array length = " + Brain.weightsArray.Length);
        //Brain.PrintArray1d(Brain.weightsArray);

        //print("BIAS array length = " + Brain.biasArray.Length);
        //Brain.PrintArray1d(Brain.biasArray);
    }

    void Update()
    {
        if (isDead == false)
        {
            RaycastRays();
            Brain.CalcuateNeuralNetwork();
            transform.Translate(Vector3.forward * Time.deltaTime * ForwardSpeed);// this line moves the car forward 
            if(!float.IsNaN(Brain.OutputNodesArray[0] * TurnSpeed)) // sometimes brain output is NaN when the neural net is not initialized complitly 
            {
                transform.Rotate(0f, Brain.OutputNodesArray[0] * TurnSpeed, 0f); // this line turns the car based on the neural net output node
            }

            Brain.MakeMasterMindArray();
            Brain.SetValuesToArray(Brain.hiddenLayerNodesArray, 0f);
            Brain.SetValuesToArray(Brain.OutputNodesArray, 0f);

            //print("Input " + Brain.InputNodesArray[0] + " " + Brain.InputNodesArray[1] + " " + Brain.InputNodesArray[2] + " " + Brain.InputNodesArray[3] + " " + Brain.InputNodesArray[4]);
            //print("NN Output " + Brain.OutputNodesArray[0]);
            //PrintArray1d(hiddenLayerNodesArray);
            CountScore();
        }
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
        tempRayDistance = hit1.distance;
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(0, 0, 1) * 10));
        Brain.InputNodesArray[0] = tempRayDistance;

        RaycastHit hit2;
        Ray ray2 = new Ray(transform.position, transform.TransformDirection(new Vector3(1, 0, 1)));
        Physics.Raycast(ray2, out hit2, 100.0f);
        tempRayDistance = hit2.distance;
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(1, 0, 1) * 10));
        Brain.InputNodesArray[1] = tempRayDistance;

        RaycastHit hit3;
        Ray ray3 = new Ray(transform.position, transform.TransformDirection(new Vector3(-1, 0, 1)));
        Physics.Raycast(ray3, out hit3, 100.0f);
        tempRayDistance = hit3.distance;
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(-1, 0, 1) * 10));
        Brain.InputNodesArray[2] = tempRayDistance;

        RaycastHit hit4;
        Ray ray4 = new Ray(transform.position, transform.TransformDirection(new Vector3(0.25f, 0, 0.75f)));
        Physics.Raycast(ray4, out hit4, 100.0f);
        tempRayDistance = hit4.distance;
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(0.25f, 0, 0.75f) * 10));
        Brain.InputNodesArray[3] = tempRayDistance;

        RaycastHit hit5;
        Ray ray5 = new Ray(transform.position, transform.TransformDirection(new Vector3(-0.25f, 0, 0.75f)));
        Physics.Raycast(ray5, out hit5, 100.0f);
        tempRayDistance = hit5.distance;
        Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(-0.25f, 0, 0.75f) * 10));
        Brain.InputNodesArray[4] = tempRayDistance;

        //print(inputNodesArray[0] + "  " + inputNodesArray[1] + "  " + inputNodesArray[2] + "  " + inputNodesArray[3] + "  " + inputNodesArray[4]);
    }

    public void CountScore() // calculates score of the car based of the distance of the raycast
    {
        //score += ((distance1 + distance2 + distance3 + distance4 + distance5)/5)*0.1;
        for (int i = 0; i < Brain.InputNodesArray.Length; i++)
        {
            score += Brain.InputNodesArray[i];
        }
        //score = (score / 5);
    }

    public void ResetCar()
    {
        isDead = false;
        score = 0;
    }
}
