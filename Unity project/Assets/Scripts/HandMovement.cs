using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class for XR Interaction Toolkit and XR Hands
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Hands;

public class HandMovement : MonoBehaviour
{
    //the C/D ratio
    public float movementRatio = 1f;

    //get the info about the rigid body
    private Rigidbody interactableRigidbody;

    //get the body parts of the weight
    public GameObject mesh;
    private Vector3 objPos;

    //get the transform of the grabbed mesh object´s parent and of the Scene in which it is located
    public Transform ParentTransform;
    public Transform SceneTransform;

    //get the info about the weight body
    private Vector3 interactableBody;

    //get the game objects (direct interactors) for both hands
    public GameObject leftHand;
    public GameObject rightHand;

    //the initial position of the hand
    private Vector3 initialPositionHand;
    private Vector3 initialWeightPosition;

    //get the game objects (L/R_Wrists) for both hands
    public GameObject offleftHand;
    public GameObject offrightHand;

    //the initial position of the offhand
    private Vector3 initialPosition;

    //the current position of the virtual hand
    private Vector3 handPosition;

    //is the left/right hand grabbing anything
    private bool leftHandGrabbing;
    private bool rightHandGrabbing;

    //is C/D ratio modification enabled
    public bool CDRatioEnabled = false;

    // The mass of this weight
    private int mass = 0;

    //snap vector that acts as an attach point
    private Vector3 snapPoint;
    public string currentScene;


    //function that turns left/rightHandGrabbing true when Select Entered
    public void Grab(SelectEnterEventArgs argssss)
    {
        if (argssss.interactor.GetComponent<XRHandTrackingEvents>().handedness == Handedness.Left)
        {
            Debug.Log("CD - I grabbed SOMETHINGGGG LEFT.");
            leftHandGrabbing = true;
            initialPosition = offleftHand.transform.position;
            initialPositionHand = leftHand.transform.position;
            initialWeightPosition = mesh.GetComponentInParent<Transform>().position;

            //gets the Scene transform in which the object is located in
            //SceneTransform = ParentTransform.GetComponentInParent<Transform>();
            Debug.Log("SceneTransform is: " + SceneTransform);
            //sets the offlefthand transform as the Parent to the grabbed object
            //ParentTransform.SetParent(offleftHand.transform);

            //Debug.Log("currentScene: " + currentScene);
            //if (currentScene == "milk")
            //{
            //snapPoint = new Vector3(0f, -0.2f, 0.08f);
            //Debug.Log("MILKsnapPoint: " + snapPoint);
            //}
            //if (currentScene == "weight")
            //{
            //snapPoint = new Vector3(0f, -0.21f, 0.075f);
            //Debug.Log("WEIGHTsnapPoint: " + snapPoint);

            //mesh.transform.localRotation = Quaternion.Euler(0, -90, 0);
            //}

            //mesh.transform.localPosition = snapPoint;
            //mesh.transform.Rotate(0, -90, 0);
            Invoke("Delllayy", 0.1f);


            Debug.Log("initialPosition: " + initialPosition);
            Debug.Log("initialPositionHand: " + initialPositionHand);
        }

        if (argssss.interactor.GetComponent<XRHandTrackingEvents>().handedness == Handedness.Right)
        {
            Debug.Log("CD - I grabbed SOMETHINGGGG RIGHT.");
            rightHandGrabbing = true;
            initialPosition = offrightHand.transform.position;
            initialPositionHand = rightHand.transform.position;
            initialWeightPosition = mesh.GetComponentInParent<Transform>().position;

            //gets the Scene transform in which the object is located in
            //SceneTransform = ParentTransform.GetComponentInParent<Transform>();
            //sets the offrighthand transform as the Parent to the grabbed object
            //ParentTransform.SetParent(offrightHand.transform);
            Invoke("Delllayy", 0.1f);

            Debug.Log("initialPosition: " + initialPosition);
            Debug.Log("initialPositionHand: " + initialPositionHand);
        }

        interactableRigidbody = gameObject.GetComponent<Rigidbody>();

        //calculate the movementRatio based on the mass of the grabbed object
        mass = Mathf.RoundToInt(interactableRigidbody.mass);
        movementRatio = 0.7f + 0.3f * (1f - ((mass - 1f) / 10f)); //Kari´s formula for movementRatio

        Debug.Log("CD - Mass of weight: " + mass);
        Debug.Log("movementRatio: " + movementRatio);        
    }
       
    //function that turns left/rightHandGrabbing false when Select Exit
    public void UnGrab(SelectExitEventArgs args)
    {
        if (args.interactor.GetComponent<XRHandTrackingEvents>().handedness == Handedness.Left)
        {
            Debug.Log("CD - I LOST SOMETHINGGGG LEFT.");
            leftHandGrabbing = false;
            offleftHand.transform.position = initialPosition;

            //ParentTransform.parent = SceneTransform;
            //Debug.Log("Parent of weight: " + ParentTransform.parent.name);
            Invoke("UnDelllayy", 0.2f);

            //snapPoint = new Vector3(0f, 0f, 0f);
            //mesh.transform.localPosition = snapPoint;
            //mesh.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        if (args.interactor.GetComponent<XRHandTrackingEvents>().handedness == Handedness.Right)
        {
            Debug.Log("CD - I LOST SOMETHINGGGG RIGHT.");
            rightHandGrabbing = false;
            offrightHand.transform.position = initialPosition;

            //ParentTransform.parent = SceneTransform;
            //Debug.Log("Parent of weight: " + ParentTransform.parent.name);
            Invoke("UnDelllayy", 0.2f);


            //snapPoint = new Vector3(0f, 0f, 0f);
            //mesh.transform.localPosition = snapPoint;
            //mesh.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        //Debug.Log("Mass of weight: " + interactableRigidbody.mass);
    }

    public void Delllayy()
    {
        if (leftHandGrabbing)
        {
            ParentTransform.SetParent(offleftHand.transform);
        }
        else
        {
            ParentTransform.SetParent(offrightHand.transform);
        }
        
    }

    public void UnDelllayy()
    {
        ParentTransform.parent = SceneTransform;
        Debug.Log("I returned it back!! to " + ParentTransform.parent.name);

        Debug.Log("SceneTransform is: " + SceneTransform);
    }

    //function that applies the movementRatio to the hand
    public void UpdateLocation()
    {
        if (CDRatioEnabled)
        {
            if (leftHandGrabbing) {
                handPosition = Vector3.Scale((leftHand.transform.position - initialPositionHand), new Vector3(1f, movementRatio, 1f)) + initialPositionHand;

                offleftHand.transform.position = handPosition;

                //Debug.Log("Position of offleftHandPos: " + offleftHand.transform.position);
            }

            if (rightHandGrabbing) {
                handPosition = Vector3.Scale((rightHand.transform.position - initialPositionHand), new Vector3(1f, movementRatio, 1f)) + initialPositionHand;

                offrightHand.transform.position = handPosition;

                //Debug.Log("Position of offrightHandPos: " + offrightHand.transform.position);
            }
        }
    }

    private void LateUpdate()
    {
        UpdateLocation();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
