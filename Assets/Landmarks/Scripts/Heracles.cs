using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class Heracles : MonoBehaviour
{
    public static int heraclesCalled;
    public static bool heraclesIsEmpty;
    //Getting the movement direction of the mountain from the user
    public enum MovementDirection
    {
        None,
        Clockwise,
        CounterClockwise,
        Random
    }

    //Give the mountain object
    public GameObject objectToMove;
    //Give the object to move the mountain around
    public GameObject centralObject;
    //how far we want the mountain to move around a circle where the center is the current target and the radius is the distance between the current target and respective mountain
    public float degreesToMove;
    //the value of the movement direction received from the researcher in the editor
    public MovementDirection movementDirection;

    //coordinates of the current target acting as the circle center that the mountain will rotate around
    private Vector3 center;
    //float that will represent clockwise or counter-clockwise movement (1 or -1)
    private float randomMovement;

    public void Start()
    {
        heraclesCalled = 0;
        if ((objectToMove != null && objectToMove != null) && (movementDirection != MovementDirection.None && degreesToMove != 0))
        {
            heraclesIsEmpty = false;
        }
        else
        {
            heraclesIsEmpty = true;
        }
    }

    public void Update()
    {
        if (heraclesCalled == 1)
        {
            MoveMountain();
            heraclesCalled = 0;
        }
        if (heraclesCalled == -1)
        {
            MoveMountainBack();
            heraclesCalled = 0;
        }
    }

    // Start is called before the first frame update
    public void MoveMountain()
    {
        Debug.Log("This is where we move the mountain");
        Debug.Log("Center = " + objectToMove.name);
        Debug.Log("Center = " + centralObject.name);
        if (centralObject == null)
        {
            center = new Vector3(0.0f, 0.0f, 0.0f);
        }
        else if (centralObject != null)
        {
            center = centralObject.transform.position;
        }

        Debug.Log("Center = " + center);

            //the amount the researcher wants to move is dependent on the enum of cw,ccw, or random; it should never be negative because that will cause movement in the opposite direction
            degreesToMove = Mathf.Abs(degreesToMove);

            //if the researcher wants to move counter clockwise
            if (movementDirection == MovementDirection.CounterClockwise)
            {
                //set the degrees to be negative (moves ccw based on rotate around function)
                degreesToMove = -degreesToMove;
            }
            //else, if theresearcher wants movement direction to be random
            else if (movementDirection == MovementDirection.Random)
            {
                //create a random number that is either a -1 or a 1
                randomMovement = (float)(Random.Range(0, 2) * 2 - 1);
                //multiply the degrees to move by either -1 or 1
                degreesToMove = degreesToMove * randomMovement;
            } //if neither of these conditions are met, that means the researcher wants to go clockwise and degreesToMove does not need to be changed from the positibe value

            Debug.Log("Degrees To Move = " + degreesToMove);

            //rotate the appropriate mountain around the center (appropriate target) the appropriate amount of degrees
            objectToMove.transform.RotateAround(center, Vector3.up, degreesToMove);

        
    }

    public void MoveMountainBack()
    {
        Debug.Log("This is where we move the mountain back");
        //if we are actually moving
        if (movementDirection != MovementDirection.None)
        {
            //inverse the degrees we previously moved the mountain and move it back
            objectToMove.transform.RotateAround(center, Vector3.up, -degreesToMove);
        }
    }

}
