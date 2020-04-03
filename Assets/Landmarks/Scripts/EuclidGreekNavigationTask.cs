using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
/*
|   ASM 2019: GreekNavigationTask.cs
|   Includes multiple functions to be concurrently called with a Navigation Task
|       1). Heracles was a Greek demigod who was said to be able to move mountains (hence the moving mountain function is called heracles)
|       2). Antaeus was a Greek demigod who was said to get his strength from being in contact with the earth (hence the rotate ground function is called antaeus).
|
|       Fun fact: Heracles killed Antaeus by lifting him up during a wrestling match (to separate him from his power source) and crushing him in a bear hug.
*/

public enum EucHeraclesHideTargetOnStart
{
    Off,
    SetInactive,
    SetInvisible,
    SetProbeTrial
}
//ASM 2019: Gives researcher the choice to select the direction that the moving object rotates around the central object
public enum EucMovementDirection
{
    None,
    Clockwise,
    CounterClockwise,
    Random
}
//ASM 2019: Gives researcher the choice to select rotation direction of ground
public enum EucRotationDirection
{
    None,
    Clockwise,
    CounterClockwise,
    Random
}

public class EuclidGreekNavigationTask : ExperimentTask
{
    //variables that MountainTaskOutput.cs will use
    //what trial we are on (this gets incremented in taskstart)
    [HideInInspector] public static int trialNumber;
    //when the button is pressed
    [HideInInspector] public bool buttonPressed;
    [HideInInspector] public static int prediction;
    //target name
    [HideInInspector] public static string targetName;
    [HideInInspector] public static bool running;

    [Header("Navigation Task")]
    public ObjectList destinations;
    private GameObject current;

    //private int score = 0;
    //public int scoreIncrement = 50;
    //public int penaltyRate = 2000;
    //private float penaltyTimer = 0;
   // public bool showScoring;

    public TextAsset NavigationInstruction;

    public HeraclesHideTargetOnStart hideTargetOnStart;
    [Range(0, 60)] public float showTargetAfterSeconds;
    public bool hideNonTargets;

    private float startTime;

    //ASM 2019: Researcher provided objects, float value, and enum value
    [Header("Heracles Function")]
    public bool implementHeraclesFunction;
    public GameObject objectToMove;
    public GameObject centralObject;
    [Range(0, 360)] public float degreesToMove;
    public MovementDirection movementDirection;
    public bool moveBackAtEnd;
    //ASM 2019: Coordinates used by later RotateAround() call
    private Vector3 center;
    //ASM 2019: Value to later represent if the objectToMove is moving CW or CCW
    private float randomMovement;

    //ASM 2019: Researcher provided object, float value, and enum value for rotating ground functionality
    [Header("Antaeus Function")]
    public bool implementAntaeusFunction;
    public GameObject objectToRotate;
    [Range(0, 360)] public float degreesToRotate;
    public RotationDirection rotationDirection;
    public bool rotateBackAtEnd;
    //ASM 2019: Value to later represent if the ground is rotating CW or CCW
    private float randomRotation;
    private GameObject[] wallsToEnable, wallsToDisable;

    public override void startTask()
    {
        TASK_START();
        //avatarLog.navLog = true;
        //if (isScaled) scaledAvatarLog.navLog = true;
    }

    public override void TASK_START()
    {

        Renderer[] renderedEnvironment = GameObject.FindGameObjectWithTag("Environment").GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderedEnvironment)
        {
            r.enabled = true;
        }

        wallsToEnable = GameObject.FindGameObjectsWithTag("enable");
        wallsToDisable = GameObject.FindGameObjectsWithTag("disable");

        int i = 0;
        int j = 0;

        foreach (GameObject wall in wallsToEnable)
        {
            wallsToEnable[i].GetComponent<MeshRenderer>().enabled = false;
            i++;
        }

        foreach (GameObject wall in wallsToDisable)
        {
            wallsToDisable[j].GetComponent<MeshRenderer>().enabled = true;
            j++;
        }

        //OutputFileLoggingInformation
        trialNumber += 1;
        targetName = destinations.currentObject().name;
        running = true;
        buttonPressed = false;
        prediction = 0;
        MountainTaskOutput.wasItPushed = false;


        //OutputFileLoggingInformation
        Debug.Log("WE ARE ON TRIAL " + trialNumber);

        if (!manager) Start();
        base.startTask();

        if (skip)
        {
            log.log("INFO    skip task    " + name, 1);
            return;
        }

        //hud.showEverything();
        //hud.showScore = showScoring;
        current = destinations.currentObject();
        Debug.Log("Find " + destinations.currentObject().name);

        //When the task is started, if we are performing heracles, we want to call MoveObject() once.
        if (implementHeraclesFunction && degreesToMove != 0 && movementDirection != MovementDirection.None)
        {
            MoveObject();
        }
        //When the task is started, if we are performing antaeus, we want to call RotateGround() once.
        if (implementAntaeusFunction && degreesToRotate != 0 && rotationDirection != RotationDirection.None)
        {
            RotateObject();
        }

        if (NavigationInstruction)
        {
            string msg = NavigationInstruction.text;
            if (destinations != null) msg = string.Format(msg, current.name);
            hud.setMessage(msg);
        }
        else
        {
            hud.SecondsToShow = 0;
            hud.setMessage("Please find the " + current.name);
        }

        // Handle if we're hiding all the non-targets
        if (hideNonTargets)
        {
            foreach (GameObject item in destinations.objects)
            {
                if (item.name != destinations.currentObject().name)
                {
                    item.SetActive(false);
                }
                else item.SetActive(true);
            }
        }

        // Handle if we're hiding the target object
        if (hideTargetOnStart != HeraclesHideTargetOnStart.Off)
        {
            if (hideTargetOnStart == HeraclesHideTargetOnStart.SetInactive)
            {
                destinations.currentObject().SetActive(false);
            }
            else if (hideTargetOnStart == HeraclesHideTargetOnStart.SetInvisible)
            {
                destinations.currentObject().GetComponent<MeshRenderer>().enabled = false;
            }
            else if (hideTargetOnStart == HeraclesHideTargetOnStart.SetProbeTrial)
            {
                destinations.currentObject().SetActive(false);
                destinations.currentObject().GetComponent<MeshRenderer>().enabled = false;
            }
        }
        else
        {
            destinations.currentObject().SetActive(true); // make sure the target is visible unless the bool to hide was checked
            destinations.currentObject().GetComponent<MeshRenderer>().enabled = true;
        }

        // startTime = Current time in seconds
        startTime = Time.time;
        Debug.Log(startTime);

        //// MJS 2019 - Move HUD to top left corner
        //hud.hudPanel.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
        //hud.hudPanel.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.9f);
    }

    public override bool updateTask()
    {

        base.updateTask();

        if (skip)
        {
            //log.log("INFO    skip task    " + name,1 );
            return true;
        }

        //if (score > 0) penaltyTimer = penaltyTimer + (Time.deltaTime * 1000);


        //if (penaltyTimer >= penaltyRate)
        //{
        //    penaltyTimer = penaltyTimer - penaltyRate;
        //    if (score > 0)
        //    {
        //        score = score - 1;
        //        hud.setScore(score);
        //    }
        //}

        //VR capabiity with showing target
        if (vrEnabled)
        {
            if (hideTargetOnStart != HeraclesHideTargetOnStart.Off && hideTargetOnStart != HeraclesHideTargetOnStart.SetProbeTrial && ((Time.time - startTime > (showTargetAfterSeconds) || vrInput.TouchpadButton.GetStateDown(Valve.VR.SteamVR_Input_Sources.Any))))
            {
                if (vrInput.TouchpadButton.GetStateDown(Valve.VR.SteamVR_Input_Sources.Any))
                {
                    buttonPressed = true;
                }
                
                destinations.currentObject().SetActive(true);
            }

            if (hideTargetOnStart == HeraclesHideTargetOnStart.SetProbeTrial && vrInput.TouchpadButton.GetStateDown(Valve.VR.SteamVR_Input_Sources.Any))
            {
                //get current location and then log it
                buttonPressed = true;
                destinations.currentObject().SetActive(true);
                destinations.currentObject().GetComponent<MeshRenderer>().enabled = true;
            }
        }

        //show target on button click or after set time
        if (hideTargetOnStart != HeraclesHideTargetOnStart.Off && hideTargetOnStart != HeraclesHideTargetOnStart.SetProbeTrial && ((Time.time - startTime > (showTargetAfterSeconds) || Input.GetButtonDown("Return"))))
        {
            if (Input.GetButtonDown("Return"))
            {
                buttonPressed = true;
            }
            destinations.currentObject().SetActive(true);
        }

        if (hideTargetOnStart == HeraclesHideTargetOnStart.SetProbeTrial && Input.GetButtonDown("Return"))
        {
            //get current location and then log it
            buttonPressed = true;
            destinations.currentObject().SetActive(true);
            destinations.currentObject().GetComponent<MeshRenderer>().enabled = true;
        }

        if (buttonPressed)
        {
            prediction = 1;
        }

        if (killCurrent == true)
        {
            return KillCurrent();
        }

        return false;
    }

    public override void endTask()
    {
        TASK_END();
        //avatarController.handleInput = false;
    }

    public override void TASK_PAUSE()
    {
        avatarLog.navLog = false;
        if (isScaled) scaledAvatarLog.navLog = false;
        //base.endTask();
        log.log("TASK_PAUSE\t" + name + "\t" + this.GetType().Name + "\t", 1);
        //avatarController.stop();

        hud.setMessage("");
        hud.showScore = false;

    }

    public override void TASK_END()
    {
        Renderer[] renderedEnvironment = GameObject.FindGameObjectWithTag("Environment").GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderedEnvironment)
        {
            r.enabled = false;
        }
        running = false;
        //When the task is done and the conditions are met, we want to call MoveObjectBack() once to essentially "reset" the environment.
        if (implementHeraclesFunction && moveBackAtEnd && degreesToMove != 0 && movementDirection != MovementDirection.None)
        {
            MoveObjectBack();
        }
        //When the task is done and the conditions are met, we want to call RotateObjectBack() once to essentially "reset" the environment.
        if (implementAntaeusFunction && rotateBackAtEnd && degreesToRotate != 0 && rotationDirection != RotationDirection.None)
        {
            RotateObjectBack();
        }
        base.endTask();
        //avatarController.stop();
        avatarLog.navLog = false;
        if (isScaled) scaledAvatarLog.navLog = false;

        if (canIncrementLists)
        {
            destinations.incrementCurrent();
        }
        current = destinations.currentObject();
        hud.setMessage("");
        hud.showScore = false;

        hud.SecondsToShow = hud.GeneralDuration;

        // Move hud back to center and reset
        hud.hudPanel.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        hud.hudPanel.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);

    }

    public override bool OnControllerColliderHit(GameObject hit)
    {
        if (hit == current)
        {
            //if (showScoring)
            //{
            //    score = score + scoreIncrement;
            //    hud.setScore(score);
            //}
            return true;
        }

        //        Debug.Log (hit.transform.parent.name + " = " + current.name);
        if (hit.transform.parent == current.transform)
        {
            //if (showScoring)
            //{
            //    score = score + scoreIncrement;
            //    hud.setScore(score);
            //}
            return true;
        }
        return false;
    }


    //ASM 2019: Function gets called to move objectToMove around centralObject
    //          Also handles random movement and if non-values are given.
    public void MoveObject()
    {
        //if no objectToMove is given
        if (objectToMove == null)
        {
            //throw a custom error message
            Debug.LogError("CustomError: In GreekNavigationTask, there is no Object To Move provided\nThe object to move will default to the NorthMountains object if no object is provided");
            //set the object to be the northern mountain range
            objectToMove = GameObject.Find("NorthMountains");

        }
        //if centralObject is not given
        if (centralObject == null)
        {
            //throw a custom error message
            Debug.LogError("CustomError: In GreekNavigationTask, there is no Central Object provided\nThe center will default to (0,0,0) if no object is provided");
            // set the center coordinates to world center
            center = new Vector3(0.0f, 0.0f, 0.0f);
        }
        //if centralObject is actually something
        else if (centralObject != null)
        {
            //if the given object's name is the same as the current target's name
            if (current.name == centralObject.name)
            {
                //set as such (this is done so we don't mess with current)
                centralObject = current;
            }
            //set the center Vector3 variable to be the position of the given object
            center = centralObject.transform.position;
        }

        //movement directionality is dependent on the enum of cw,ccw, or random; The given value shouldn't be negative w/ the enum choice b/c that reverses directionality
        degreesToMove = Mathf.Abs(degreesToMove);

        //if the researcher wants to move counter clockwise
        if (movementDirection == MovementDirection.CounterClockwise)
        {
            //set the degrees to be negative (moves ccw based on rotate around function)
            degreesToMove = -degreesToMove;
        }
        //else, if the researcher wants movement direction to be random
        else if (movementDirection == MovementDirection.Random)
        {
            //create a random number that is either a -1 or a 1
            randomMovement = (float)(Random.Range(0, 2) * 2 - 1);
            //multiply the degrees to move by either -1 or 1
            degreesToMove = degreesToMove * randomMovement;
        } //if neither of these conditions are met, that means the researcher wants to go clockwise and degreesToMove does not need to be changed from the positive value

        //rotate the appropriate object around the center (appropriate target) by the appropriate amount of degrees
        objectToMove.transform.RotateAround(center, Vector3.up, degreesToMove);


    }

    //ASM 2019: Function gets called to move objectToMove around centralObject in the opposite direction as before
    public void MoveObjectBack()
    {
        Debug.Log("This is where we move the mountain back");
        //inverse the degrees we previously moved the object and move it back
        objectToMove.transform.RotateAround(center, Vector3.up, -degreesToMove);
    }

    //ASM 2019: Function gets called to move objectToMove around centralObject in the opposite direction as before
    public void RotateObject()
    {
        Debug.Log("This is where we rotate the ground");
        //if no objectToMove is given
        if (objectToRotate == null)
        {
            //throw a custom error message
            Debug.LogError("CustomError: In GreekNavigationTask, there is no ground provided\nThe object to move will default to the object called ground if no object is provided");
            //set the object to be the ground if it exists
            objectToRotate = GameObject.Find("Ground");
        }

        //rotation directionality is dependent on the enum of cw,ccw, or random; The given value shouldn't be negative w/ the enum choice b/c that reverses directionality
        degreesToRotate = Mathf.Abs(degreesToMove);

        //if the researcher wants to rotate counter clockwise
        if (rotationDirection == RotationDirection.CounterClockwise)
        {
            //set the degrees to be negative (moves ccw based on rotate around function)
            degreesToRotate = -degreesToRotate;
        }
        //else, if the researcher wants rotationdirection to be random
        else if (movementDirection == MovementDirection.Random)
        {
            //create a random number that is either a -1 or a 1
            randomRotation = (float)(Random.Range(0, 2) * 2 - 1);
            //multiply the degrees to move by either -1 or 1
            degreesToRotate = degreesToRotate * randomRotation;
        } //if neither of these conditions are met, that means the researcher wants to go clockwise and degreesToMove does not need to be changed from the positive value

        //rotate the appropriate object around the center (appropriate target) by the appropriate amount of degrees
        objectToRotate.transform.Rotate(0.0f, degreesToRotate, 0.0f);
    }

    public void RotateObjectBack()
    {
        Debug.Log("This is where we rotate the ground back");
        //inverse the degrees we previously moved the object and move it back
        objectToRotate.transform.Rotate(0.0f, -degreesToRotate, 0.0f);
    }


}

