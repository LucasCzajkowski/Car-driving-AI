using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GENERATOR : MonoBehaviour
{
    public GameObject CAR;
    public Transform spawnPoint;
    static public GameObject[] carArray;
    static int nomberOfInstantiatedCars = 10;

    void Start()
    {
        print("Instantiating cars");
        carArray = InstantiateCars(nomberOfInstantiatedCars);
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
                MutateAll();
                ResetCarToStart();
            }
            //print("New W array \n");
            //carArray[0].GetComponent<AIdriver>().PrintW();
            //print(carArray[0].GetComponent<AIdriver>().O);
        }

        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    print("Sorting cars by score");
        //    sortCarArray();
        //    //PrintArrayDebug();
        //}
        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    print("Mutating");
        //    MutateAll();
        //}

        //if (Input.GetKeyDown(KeyCode.G))
        //{
        //    print("Reseting cars to start");
        //    ResetCarToStart();
        //}

        if (Input.GetKeyDown(KeyCode.F4))
        {
            print("Reseting cars to start");
            ResetCarToStart();
        }
    }

    bool AreCarsDead()
    {
        for (int i = 0; i < carArray.Length; i++)
        {
           if( carArray[i].GetComponent<AIdriver>().isDead == false)
           {
               return false;
               
           }
           
        }
        return true;
    }

    void KillAllCars()
    {
        for(int i=0; i<carArray.Length; i++)
        {
            carArray[i].GetComponent<AIdriver>().isDead = true;
        }
    }

    public void ResetCarToStart()
    {
        for(int i = 0; i< carArray.Length; i++)
        {
            //carArray[i].SetActive(false);
            carArray[i].transform.position = spawnPoint.position;
            carArray[i].transform.rotation = spawnPoint.rotation;
            //carArray[i].SetActive(true);
            carArray[i].GetComponent<AIdriver>().ResetCar();
        }
    }

    public  GameObject[] InstantiateCars(int nrOfCars)
    {
        GameObject[] CarArray = new GameObject[nrOfCars];
        for (int i = 0; i < nrOfCars; i++)
        {
            CarArray[i] = Instantiate(CAR, spawnPoint.transform.position, Quaternion.identity);

            CarArray[i].GetComponent<AIdriver>().ResetCar();
            CarArray[i].GetComponent<AIdriver>().RandomizeW();
        }
        return CarArray;
    }

    public static void sortCarArray() // sorts car array by car score
    {
        GameObject[] tempcararray = new GameObject[nomberOfInstantiatedCars];
        tempcararray = carArray.OrderBy(c => -c.GetComponent<AIdriver>().score).ToArray();
        carArray = tempcararray;
    }

    public static void PrintArrayDebug()
    {
        print(carArray[0].GetComponent<AIdriver>().score + "\n");
        print(carArray[1].GetComponent<AIdriver>().score + "\n");
        print(carArray[2].GetComponent<AIdriver>().score + "\n");
        print(carArray[3].GetComponent<AIdriver>().score + "\n");
        print(carArray[4].GetComponent<AIdriver>().score + "\n");
        print(carArray[5].GetComponent<AIdriver>().score + "\n");
        print(carArray[6].GetComponent<AIdriver>().score + "\n");
        print(carArray[7].GetComponent<AIdriver>().score + "\n");
        print(carArray[8].GetComponent<AIdriver>().score + "\n");
        print(carArray[9].GetComponent<AIdriver>().score + "\n");
    }

    public float[] Mutate(GameObject X, GameObject Y)// the genetic algorithm 
    {
        float[] masterMindX;
        X.GetComponent<AIdriver>().GenerateMind();
        masterMindX = X.GetComponent<AIdriver>().MasterMind;

        float[] masterMindY;
        Y.GetComponent<AIdriver>().GenerateMind();
        masterMindY = Y.GetComponent<AIdriver>().MasterMind;

        float[] tempDifferenceArray = new float[masterMindX.Length]; // X - Y

        for(int i=0; i<masterMindX.Length; i++)
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
        float[] tempMindArray0 = new float[29];// lenght of mastermind array
        float[] tempMindArray1 = new float[29];// lenght of mastermind array
        float[] tempMindArray2 = new float[29];// lenght of mastermind array
        float[] tempMindArray3 = new float[29];// lenght of mastermind array
       

        tempMindArray0 = Mutate(carArray[0], carArray[1]);
        tempMindArray1 = Mutate(carArray[1], carArray[2]);
        tempMindArray2 = Mutate(carArray[2], carArray[3]);
        tempMindArray3 = Mutate(carArray[3], carArray[4]);

        carArray[1].GetComponent<AIdriver>().SetMasterMind(tempMindArray0);
        carArray[2].GetComponent<AIdriver>().SetMasterMind(tempMindArray1);
        carArray[3].GetComponent<AIdriver>().SetMasterMind(tempMindArray2);
        carArray[4].GetComponent<AIdriver>().SetMasterMind(tempMindArray3);

        carArray[5].GetComponent<AIdriver>().RandomizeW();
        carArray[6].GetComponent<AIdriver>().RandomizeW();
        carArray[7].GetComponent<AIdriver>().RandomizeW();
        carArray[8].GetComponent<AIdriver>().RandomizeW();
        carArray[9].GetComponent<AIdriver>().RandomizeW();



    }

}
