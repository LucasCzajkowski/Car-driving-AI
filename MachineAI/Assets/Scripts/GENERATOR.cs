using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GENERATOR : MonoBehaviour
{
    public GameObject CAR;
    public Transform spawnPoint;
    static public GameObject[] carArray;
    static public int numberOfInstantiatedCars = 100;

    void Start()
    {
        print("Instantiating cars");
        carArray = InstantiateCars(numberOfInstantiatedCars);
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    print("Instantiating cars");
        //    carArray = InstantiateCars(nomberOfInstantiatedCars);
        //}

        if (carArray.Length != 0)
        {

            if (AreCarsDead() == true)
            {
                sortCarArray();
                MutateAllV3();
                ResetCarToStart();
            }
            
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            print("Reseting cars to start");
            ResetCarToStart();
        }
    }

    bool AreCarsDead() // returns tru/false if all cars are alive (did not bumped to walls)
    {
        for (int i = 0; i < carArray.Length; i++)
        {
            if (carArray[i].GetComponent<AIdriver>().isDead == false)
            {
                return false;
            }
        }
        return true;
    }

    void KillAllCars()// artificially bumps all cars so there are dead
    {
        for (int i = 0; i < carArray.Length; i++)
        {
            carArray[i].GetComponent<AIdriver>().isDead = true;
        }
    }

    public void ResetCarToStart()// sets the car at the GENERATOR spawning point
    {
        for (int i = 0; i < carArray.Length; i++)
        {
            carArray[i].transform.position = spawnPoint.position;
            carArray[i].transform.rotation = spawnPoint.rotation;
            carArray[i].GetComponent<AIdriver>().ResetCar();
        }
    }

    public GameObject[] InstantiateCars(int nrOfCars)
    {
        GameObject[] CarArray = new GameObject[nrOfCars];
        for (int i = 0; i < nrOfCars; i++)
        {
            CarArray[i] = Instantiate(CAR, spawnPoint.transform.position, spawnPoint.transform.rotation);
            CarArray[i].GetComponent<AIdriver>().ResetCar();
        }
        return CarArray;
    }

    public static void sortCarArray() // sorts carArray by car score highest 0.. lowest ...n
    {
        GameObject[] tempcararray = new GameObject[numberOfInstantiatedCars];
        tempcararray = carArray.OrderBy(c => -c.GetComponent<AIdriver>().score).ToArray();
        carArray = tempcararray;
    }

    public static void PrintArrayDebug()// for debug purposes prints the sorted car a rray scores
    {
        for(int i = 0; i<carArray.Length; i++)
        {
            print(carArray[i].GetComponent<AIdriver>().score + "\n");
        }
    }

    public float[] Mutate(GameObject X, GameObject Y)// the genetic algorithm 
    {
        float[] masterMindX;
        //X.GetComponent<AIdriver>().Brain.MakeMasterMindArray();
        masterMindX = X.GetComponent<AIdriver>().Brain.MasterMindArray;

        float[] masterMindY;
        //Y.GetComponent<AIdriver>().Brain.MakeMasterMindArray();
        masterMindY = Y.GetComponent<AIdriver>().Brain.MasterMindArray;

        float[] tempDifferenceArray = new float[masterMindX.Length]; // X - Y

        for (int i = 0; i < masterMindX.Length; i++)
        {
            tempDifferenceArray[i] = Mathf.Abs(masterMindX[i] - masterMindY[i]);
        }

        float tempMinMindDistance = tempDifferenceArray.Min();
        float tempMaxMindDistance = tempDifferenceArray.Min();
        float maxAllowedDistance = ((tempMaxMindDistance - tempMinMindDistance) * 0.5f) + tempMinMindDistance;

        float[] mutatedMind = new float[masterMindX.Length];

        for (int i = 0; i < masterMindX.Length; i++)
        {
            if (tempDifferenceArray[i] > maxAllowedDistance)
            {
                //mutatedMind[i] = masterMindX[i];
                mutatedMind[i] = Random.Range(-10f, 10f);
            }
            else
            {
                mutatedMind[i] = masterMindX[i];
            }
            //mutatedMind[i] = Random.Range(-10f, 10f);

        }

        return mutatedMind;
    }

    public void MutateAll() // hacked mind arrays generation for 10 cars. NOT A VERY GOOD INPLEMENTATION, but works for tests 
    {
        float[] tempMindArray0 = new float[22];// lenght of mastermind array
        float[] tempMindArray1 = new float[22];// lenght of mastermind array
        float[] tempMindArray2 = new float[22];// lenght of mastermind array
        float[] tempMindArray3 = new float[22];// lenght of mastermind array


        tempMindArray0 = Mutate(carArray[0], carArray[1]);
        tempMindArray1 = Mutate(carArray[1], carArray[2]);
        tempMindArray2 = Mutate(carArray[2], carArray[3]);
        tempMindArray3 = Mutate(carArray[3], carArray[4]);

        carArray[1].GetComponent<AIdriver>().Brain.ReadWeightAndBiasFromArray(tempMindArray0);
        carArray[2].GetComponent<AIdriver>().Brain.ReadWeightAndBiasFromArray(tempMindArray1);
        carArray[3].GetComponent<AIdriver>().Brain.ReadWeightAndBiasFromArray(tempMindArray2);
        carArray[4].GetComponent<AIdriver>().Brain.ReadWeightAndBiasFromArray(tempMindArray3);

        carArray[5].GetComponent<AIdriver>().Brain.RandomizeWeightsArray();
        carArray[6].GetComponent<AIdriver>().Brain.RandomizeWeightsArray();
        carArray[7].GetComponent<AIdriver>().Brain.RandomizeWeightsArray();
        carArray[8].GetComponent<AIdriver>().Brain.RandomizeWeightsArray();
        carArray[9].GetComponent<AIdriver>().Brain.RandomizeWeightsArray();



    }

    public float[] MutateAllV2(GameObject[] inputArray)
    {
        float[] outputArray = new float[inputArray.Length];
        float mutateAmmount = 1 / (inputArray.Length - 2);

        for (int i = 1; i < inputArray.Length - 1; i++)// because we wont change the best score car and we will not change the car with the lowest score we will randomize it later
        {
            float[] tempMastermindArray = inputArray[i].GetComponent<AIdriver>().Brain.MasterMindArray;
            for (int j = 0; j < tempMastermindArray.Length; j++)
            {
                tempMastermindArray[j] += Random.Range(-tempMastermindArray[j] * mutateAmmount, tempMastermindArray[j] * mutateAmmount);
            }
            inputArray[i].GetComponent<AIdriver>().Brain.ReadWeightAndBiasFromArray(tempMastermindArray); // loads the mutated mastermind array to car brain
            mutateAmmount++;
        }

        inputArray[inputArray.Length - 1].GetComponent<AIdriver>().Brain.RandomizeWeightsArray(); // randomizes the car with the lowest score (last car)

        return outputArray;
    }

    public void MutateAllV3()
    {
        int currentCar = 0;
        
        

        for(int i = 1; i<carArray.Length; i++)
        {
            if (currentCar < carArray.Length)
            {
                carArray[i].GetComponent<AIdriver>().Brain.ReadWeightAndBiasFromArray( Mutate(carArray[currentCar], carArray[currentCar+1]));
                currentCar += 2;
            }
            else
            {
                carArray[i].GetComponent<AIdriver>().Brain.RandomizeWeightsArray();
            }
        }
    }

}
