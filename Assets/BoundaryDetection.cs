using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryDetection : MonoBehaviour
    /*
     | The purpose of this script is to check a player's head, hands, etc. location
     | against preestablished boundaries, and warn them if they get too close.
     |
     | Position tracking used instead of collision detection to save on resources
     | Useful for when the VR chaperone is forced off
     */
{
    public float bufferRadius;
    private float xPosBound, xNegBound, zPosBound, zNegBound;

    // Start is called before the first frame update
    void Start()
    {
        xPosBound = GameObject.FindWithTag("X+Bound").transform.position.x - bufferRadius;
        xNegBound = GameObject.FindWithTag("X-Bound").transform.position.x + bufferRadius;
        zPosBound = GameObject.FindWithTag("Z+Bound").transform.position.z - bufferRadius;
        zNegBound = GameObject.FindWithTag("Z-Bound").transform.position.z + bufferRadius;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (this.transform.position.x >= xPosBound || this.transform.position.x <= xNegBound || this.transform.position.z >= zPosBound || this.transform.position.z <= zNegBound)
        {
            Debug.Log("Your " + this.name + " is getting close to a boundary");
        }
    }
}
