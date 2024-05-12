using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class for XR Interaction Toolkit and XR Hands
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Hands;

public class DropOnFastLift : MonoBehaviour
{
    //setting the max Velocity with which the objects can move
    public float maxVelocity = 2.0f;

    //get the info about the rigid body
    private Rigidbody interactableRigidbody;

    //get the game objects (direct interactors) for both hands
    public GameObject leftHand;
    public GameObject rightHand;

    public XRDirectInteractor leftInteractor;
    public XRDirectInteractor rightInteractor;

    public XRGrabInteractable grabInteractable;

    //is the left/right hand grabbing anything
    private bool leftHandGrabbing;
    private bool rightHandGrabbing;    

    //vectors for left/right hand position (first and last)
    private Vector3 leftHandPos;
    private Vector3 rightHandPos;

    private Vector3 lastLeftHandPos;
    private Vector3 lastRightHandPos;

    //get the interaction manager so I can use their method (SelectExit)
    public XRInteractionManager interactionManager;

    //is velocity limiting enabled
    public bool velocityLimitEnabled = false;


    // *** DATA COLLECTION ***
    // How often this weight has been dropped
    private int dropCount = 0;
    // How often this weight has been picked up
    private int pickUpCount = 0;
    // The mass of this weight
    private int mass = 0;
    // The initial position of this weight
    public int weightNumber = 0;
    // The index of the platform this weight is on
    private int platformIndex = -1;

    //string that contains the various data
    private string stringData;

    void Awake()
    {
        interactableRigidbody = gameObject.GetComponent<Rigidbody>();
        CalculateMaxVelocity();
    }

    //function that turns left/rightHandGrabbing true when Select Entered
    public void Grab(SelectEnterEventArgs argssss)
    {
        if (argssss.interactor.GetComponent<XRHandTrackingEvents>().handedness == Handedness.Left)
        {
            Debug.Log("I grabbed SOMETHINGGGG LEFT.");
            leftHandGrabbing = true;
        }

        if (argssss.interactor.GetComponent<XRHandTrackingEvents>().handedness == Handedness.Right)
        {
            Debug.Log("I grabbed SOMETHINGGGG RIGHT.");
            rightHandGrabbing = true;
        }

        Debug.Log("Mass of weight: " + interactableRigidbody.mass);

        IncrementWeightPickedUpCounter();
    }

    //function that turns left/rightHandGrabbing false when Select Exit
    public void UnGrab(SelectExitEventArgs args)
    {
        if (args.interactor.GetComponent<XRHandTrackingEvents>().handedness == Handedness.Left)
        {
            Debug.Log("I LOST SOMETHINGGGG LEFT.");
            leftHandGrabbing = false;
        }

        if (args.interactor.GetComponent<XRHandTrackingEvents>().handedness == Handedness.Right)
        {
            Debug.Log("I LOST SOMETHINGGGG RIGHT.");
            rightHandGrabbing = false;
        }

        Debug.Log("Mass of weight: " + interactableRigidbody.mass);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() 
    {
        
    }

    //function to increment the dropCount
    private void IncrementWeightDroppedCounter()
    {
        dropCount++;
    }

    //function to increment the pickUpCount
    public void IncrementWeightPickedUpCounter()
    {
        pickUpCount++;
    }

    [System.Obsolete]
    void FixedUpdate()
    {
        if (!velocityLimitEnabled)
        {
            return;
        }

        //assigns a vector to leftHandPos
        leftHandPos = leftHand.transform.position;

        //calculates the distance travelled in the y-axis and then the upspeed
        float upMovement = leftHandPos.y - lastLeftHandPos.y;
        float upspeed = upMovement / Time.fixedDeltaTime;

        //assigns the previous leftHandPos as the lastLeftHandPos
        lastLeftHandPos = leftHandPos;

        //assigns a vector to leftHandPos
        rightHandPos = rightHand.transform.position;

        //calculates the distance travelled in the y-axis and then the upspeed
        float upRMovement = rightHandPos.y - lastRightHandPos.y;
        float uprightspeed = upRMovement / Time.fixedDeltaTime;

        //assigns the previous leftHandPos as the lastLeftHandPos
        lastRightHandPos = rightHandPos;

        //if the speed exceeds the maxVelocity calculated and the hand holds sth the object is deselected/released
        if (((upspeed > maxVelocity) && leftHandGrabbing) || ((uprightspeed > maxVelocity) && rightHandGrabbing))
        {
            if (leftHandGrabbing)
            {
                interactionManager.SelectExit(leftInteractor, grabInteractable);

                Debug.Log("upspeed: " + upspeed);
            }

            else
            {
                interactionManager.SelectExit(rightInteractor, grabInteractable);

                Debug.Log("uprightspeed: " + uprightspeed);
            }

            Debug.Log("Releaseeeee meeee");
            Debug.Log("Mass of weight: " + interactableRigidbody.mass);
            Debug.Log("max Velocity: " + maxVelocity);

            IncrementWeightDroppedCounter();
        }
    }

    //function that calculates the maxVelocity limit based on the weights mass
    public void CalculateMaxVelocity()
    {
        //Kari´s formula for maxVelocity
        maxVelocity = (1 / (Mathf.Log(interactableRigidbody.mass + 1, 10))) - 0.5f;

        //Debug.Log("max Velocity: " + maxVelocity);
    }

    //function that gets all the data parameters and concatenates them into one string
    public string SaveData()
    {
        stringData = "initialPosition: " + weightNumber + "@" + "mass: " + mass + "@" + "dropCount: " + dropCount + "@" + 
                     "pickUpCount: " + pickUpCount + "@" + "finalPosition: " + platformIndex;
        stringData = stringData.Replace("@", " " + System.Environment.NewLine);

        Debug.Log(stringData);

        return stringData;
    }

    //function that resets the weights
    public void ResetData()
    {
        dropCount = 0;
        pickUpCount = 0;
        platformIndex = -1;
        Debug.Log("resetting data, rigidbody mass: " + interactableRigidbody.mass);
        mass = Mathf.RoundToInt(interactableRigidbody.mass);
        Debug.Log("resetting data, mass: " + mass);
    }

    //function for when weight is placed on PlacementCube then gets its index
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "WeightPlacementCollider")
        {
            // Get the platform index from the WeightPlacementCollider script
            platformIndex = other.gameObject.GetComponent<WeightPlacementCollider>().GetPlatformIndex();

            Debug.Log("Mass of weight: " + interactableRigidbody.mass + " is on platform Index: " + platformIndex);
        }
    }

    //function for when weight is NOT placed on PlacementCube then gets -1 as its index
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "WeightPlacementCollider")
        {
            platformIndex = -1;

            Debug.Log("Mass of weight: " + interactableRigidbody.mass + " is on platform Index OUT: " + platformIndex);
        }
    }
}
