using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnWeight : MonoBehaviour
{
    // The original position of the game object
    private Vector3 originalPosition;

    // The original euler angles of the game object
    private Vector3 originalEulerAngles;

    // Start is called before the first frame update
    void Awake()
    {
        // Set the original position of the game object
        originalPosition = transform.localPosition;
        originalEulerAngles = transform.eulerAngles;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Respawn()
    {
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        GetComponent<Rigidbody>().position = Vector2.zero;
        GetComponent<Rigidbody>().rotation = Quaternion.identity;
        GetComponent<Rigidbody>().useGravity = true;
        transform.eulerAngles = originalEulerAngles;
        transform.localPosition = originalPosition;
    }

    void LateUpdate()
    {
        // Check if the game object is below 0.2 in the y-axis OR above 1.2 in the z-axis and respawn it if it is
        if (transform.position.y < 0.2f || transform.position.z > 1.2)
        {
            Respawn();
        }
    }
}
