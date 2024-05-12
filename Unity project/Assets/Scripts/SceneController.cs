using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class to use the CloudLogging
using Matoya.Common;

public class SceneController : MonoBehaviour
{
    //string that contains the currentScenario reference
    private string currentScenario = "HandSelection";

    [SerializeField] GameObject handSelectionEnvironment;
    [SerializeField] GameObject heightCalibrationEnvironment;
    [SerializeField] GameObject liftTutorialEnvironment;
    [SerializeField] GameObject sixWeightsEnvironment;
    [SerializeField] GameObject twoWeightsEnvironment;    
    [SerializeField] GameObject milkCartonEnvironment;
    [SerializeField] GameObject goodbyeEnvironment;

    [SerializeField] TwoWeightUICounter twoWeightUICounter;

    [SerializeField] Transform cameraRig;

    //array for six weights scenario
    public GameObject[] sixWeights;

    //array for two weights scenario
    public GameObject[] twoWeights;

    //array for milk carton scenario
    public GameObject[] milkCartons;

    // Array of possible masses for the weights
    private float[] massesArray = { 1f, 3f, 5f, 7f, 9f, 11f };
    private float[,] massesArrayPairs = { { 1f, 11f }, { 3f, 9f }, { 5f, 7f } };

    // Array for the order that the 2 weights will be randomized
    private int[] randomizeOrder = new int[30];
    private int twoWeightTestIndex = 0;

    //time parameters
    private float startTime;
    private float endTime;
    private float timeToComplete;

    //string that contains the time data
    private string stringTimeData;

    //string that contains the weight data
    private string stringWeightData;

    //string that should contain all data
    private string Data;

    //integer that counts the number of tests done
    private int currentTestNumber = 1;

    //reference to use the CloudLogging functions
    public CloudLogging LogStuff; 


    // Start is called before the first frame update
    void Start()
    {
        //ContinueToNext();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // Function to randomize the 6 weights
    private void RandomizeSixWeights(GameObject[] weightArray)
    {
        // Temporary array with the possible masses
        float[] possibleMasses = massesArray;
        // Randomize the mass of the weights without repeating the same mass
        for (int i = 0; i < weightArray.Length; i++)
        {
            int randomIndex = Random.Range(0, possibleMasses.Length);
            weightArray[i].GetComponent<Rigidbody>().mass = possibleMasses[randomIndex];
            float[] tempArray = possibleMasses;
            possibleMasses = new float[possibleMasses.Length - 1];
            int j = 0;
            for (int k = 0; k < tempArray.Length; k++)
            {
                if (k != randomIndex)
                {
                    possibleMasses[j] = tempArray[k];
                    j++;
                }
            }
        }
    }

    // Function that creates an array of length 30 with numbers from 0 to 2 where each number appears in a random order 10 times
    private void RandomizePairOrder()
    {
        // Create a 30 element array with numbers from 0 to 29
        int[] tempArray = new int[30];
        for (int i = 0; i < tempArray.Length; i++)
        {
            tempArray[i] = i;
        }
        // Randomize the order of the numbers in the randomizeOrder array
        for (int i = 0; i < 30; i++)
        {
            int randomIndex = Random.Range(0, tempArray.Length);
            randomizeOrder[i] = tempArray[randomIndex];
            int[] tempArray2 = tempArray;
            tempArray = new int[tempArray.Length - 1];
            int j = 0;
            for (int k = 0; k < tempArray2.Length; k++)
            {
                if (k != randomIndex)
                {
                    tempArray[j] = tempArray2[k];
                    j++;
                }
            }
        }
    }

    // Function to randomize 2 weights
    private void RandomizeTwoWeights()
    {
        if (twoWeightTestIndex == 0)
        {
            RandomizePairOrder();
        }

        // Give the two weights a random mass from one of the pairs in the massesArrayPairs array
        int randomIndex = randomizeOrder[twoWeightTestIndex] % 3;
        twoWeightTestIndex++;
        int randomIndex2 = Random.Range(0, 2);
        twoWeights[0].GetComponent<Rigidbody>().mass = massesArrayPairs[randomIndex, randomIndex2];
        twoWeights[1].GetComponent<Rigidbody>().mass = massesArrayPairs[randomIndex, 1 - randomIndex2];
    }

    //function to randomize masses
    public void RandomizeMasses()
    {
        // Randomize weights depending on current scenario
        if (currentScenario == "SixWeights")
        {
            // Randomize the mass of the six weights
            RandomizeSixWeights(sixWeights);
        }
        else if (currentScenario == "TwoWeights")
        {
            // Randomize the mass of the two weights
            RandomizeTwoWeights();
        }
        else if (currentScenario == "MilkCarton")
        {
            // Randomize the mass of the milk cartons
            RandomizeSixWeights(milkCartons);
        }

        // For each weight, get the DropOnFastLift script and set the max velocity
        if (currentScenario == "SixWeights")
        {
            foreach (GameObject weight in sixWeights)
            {
                DropOnFastLift dropOnFastLift = weight.GetComponent<DropOnFastLift>();
                dropOnFastLift.ResetData();
                dropOnFastLift.CalculateMaxVelocity();
            }
        }
        else if (currentScenario == "TwoWeights")
        {
            foreach (GameObject weight in twoWeights)
            {
                DropOnFastLift dropOnFastLift = weight.GetComponent<DropOnFastLift>();
                dropOnFastLift.ResetData();
                dropOnFastLift.CalculateMaxVelocity();
            }
        }
        else if (currentScenario == "MilkCarton")
        {
            foreach (GameObject weight in milkCartons)
            {
                DropOnFastLift dropOnFastLift = weight.GetComponent<DropOnFastLift>();
                dropOnFastLift.ResetData();
                dropOnFastLift.CalculateMaxVelocity();
            }
        }
    }

    //function that gets the startTime
    public void SetStartTime()
    {
        startTime = Time.time;
    }

    //function that gets the endTime and calculates the timeToComplete
    public void EndTime()
    {
        endTime = Time.time;
        timeToComplete = endTime - startTime;

        stringTimeData = "startTime: " + startTime + "@" + "endTime: " + endTime + "@" + "timeToComplete: " + timeToComplete;
        stringTimeData = stringTimeData.Replace("@", " " + System.Environment.NewLine);

        Debug.Log(stringTimeData);
    }

    //function that gets attaches the strings together to get the data
    public void StringAttacher(string moreData)
    {
        Data = Data + "@" + moreData;
        Data = Data.Replace("@", System.Environment.NewLine + "---------" + System.Environment.NewLine);
    }

    //function that is being called on the THUMBS UP and makes the next scene active while the other one is deactivated
    public void ContinueToNext()
    {
        if (currentScenario == "HandSelection")
        {
            currentScenario = "HeightCalibration";

            //disable the handSelectionEnvironment game object
            handSelectionEnvironment.SetActive(false);

            //enable the heightCalibrationEnvironment game object
            heightCalibrationEnvironment.SetActive(true);
        }

        else if (currentScenario == "HeightCalibration")
        {
            float cameraRigHeight = cameraRig.localPosition.y;

            float diff = 1.85f;
            // Set the height of all environments to 1.85 below the camera rig height
            liftTutorialEnvironment.transform.position = new Vector3(handSelectionEnvironment.transform.position.x, cameraRigHeight - diff, handSelectionEnvironment.transform.position.z);
            sixWeightsEnvironment.transform.position = new Vector3(handSelectionEnvironment.transform.position.x, cameraRigHeight - diff, handSelectionEnvironment.transform.position.z);
            twoWeightsEnvironment.transform.position = new Vector3(handSelectionEnvironment.transform.position.x, cameraRigHeight - diff, handSelectionEnvironment.transform.position.z);
            milkCartonEnvironment.transform.position = new Vector3(handSelectionEnvironment.transform.position.x, cameraRigHeight - diff, handSelectionEnvironment.transform.position.z);
            goodbyeEnvironment.transform.position = new Vector3(handSelectionEnvironment.transform.position.x, cameraRigHeight - diff, handSelectionEnvironment.transform.position.z);

            currentScenario = "LiftTutorial";

            //disable the heightCalibrationEnvironment game object
            heightCalibrationEnvironment.SetActive(false);

            //enable the liftTutorialEnvironment game object
            liftTutorialEnvironment.SetActive(true);
        }
        
        else if (currentScenario == "LiftTutorial")
        {
            currentScenario = "SixWeights";
            StringAttacher(currentScenario + System.Environment.NewLine + "currentTestNumber: " + currentTestNumber);

            //disable the liftTutorialEnvironment game object
            liftTutorialEnvironment.SetActive(false);

            //enable the sixWeightsEnvironment game object
            sixWeightsEnvironment.SetActive(true);

            SetStartTime();
        }
        
        else if (currentScenario == "SixWeights")
        {
            EndTime();

            StringAttacher(stringTimeData);

            Debug.Log("Data of currentTestNumber " + currentTestNumber + ": " + Data);

            //for each weight, get the DropOnFastLift script and save the data
            foreach (GameObject weight in sixWeights)
            {
                DropOnFastLift dropOnFastLift = weight.GetComponent<DropOnFastLift>();
                stringWeightData = dropOnFastLift.SaveData();

                StringAttacher(stringWeightData);
            }            

            currentScenario = "TwoWeights";
            StringAttacher(currentScenario + System.Environment.NewLine + "currentTestNumber: " + currentTestNumber);

            //disable the sixWeightsEnvironment game object
            sixWeightsEnvironment.SetActive(false);

            //enable the twoWeightsEnvironment game object
            twoWeightsEnvironment.SetActive(true);

            SetStartTime();
        }

        else if (currentScenario == "TwoWeights")
        {
            currentTestNumber++;

            twoWeightUICounter.AddOne();

            EndTime();

            StringAttacher(stringTimeData);

            SetStartTime();

            StringAttacher(currentScenario + System.Environment.NewLine + "currentTestNumber: " + currentTestNumber);

            Debug.Log("Data of currentTestNumber " + currentTestNumber + ": " + Data);

            foreach (GameObject weight in twoWeights)
            {
                DropOnFastLift dropOnFastLift = weight.GetComponent<DropOnFastLift>();
                stringWeightData = dropOnFastLift.SaveData();

                StringAttacher(stringWeightData);
            }

            if (twoWeightTestIndex == 30)
            {
                //two weight test is done
                twoWeightTestIndex = 0;

                currentScenario = "MilkCarton";
                StringAttacher(currentScenario + System.Environment.NewLine + "currentTestNumber: " + currentTestNumber);                

                //disable the twoWeightsEnvironment game object
                twoWeightsEnvironment.SetActive(false);

                //enable the milkCartonEnvironment game object
                milkCartonEnvironment.SetActive(true);

                SetStartTime();
            }            
        }
        
        else if (currentScenario == "MilkCarton")
        {
            EndTime();

            StringAttacher(stringTimeData);

            Debug.Log("Data of currentTestNumber " + currentTestNumber + ": " + Data);

            foreach (GameObject weight in milkCartons)
            {
                DropOnFastLift dropOnFastLift = weight.GetComponent<DropOnFastLift>();
                stringWeightData = dropOnFastLift.SaveData();

                StringAttacher(stringWeightData);
            }
             
            //save the data 
            LogStuff.SaveLog(Data, " ", LogType.Log);

            //upload the saved data to the Cloud
            LogStuff.UploadLogs();

            //disable the milkCartonEnvironment game object
            milkCartonEnvironment.SetActive(false);

            //enable the goodbyeEnvironment game object
            goodbyeEnvironment.SetActive(true);

        }

        // For each weight, reset its position and rotation
        GameObject[] weights;
        if (currentScenario == "SixWeights")
        {
            weights = sixWeights;
        }
        else if (currentScenario == "TwoWeights")
        {
            weights = twoWeights;
        }
        else if (currentScenario == "MilkCarton")
        {
            weights = milkCartons;
        }
        else
        {
            weights = new GameObject[0];
        }
        foreach (GameObject weight in weights)
        {
            weight.GetComponent<RespawnWeight>().Respawn();
        }

        // Randomize the masses of the weights
        RandomizeMasses();
    }
}
