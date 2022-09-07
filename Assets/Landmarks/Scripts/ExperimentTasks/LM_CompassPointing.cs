/*
    LM Dummy
       
    Attached object holds task components that need to be effectively ignored 
    by Tasklist but are required for the script. Thus the object this is 
    attached to can be detected by Tasklist (won't throw error), but does nothing 
    except start and end.   

    Copyright (C) 2019 Michael J. Starrett

    Navigate by StarrLite (Powered by LandMarks)
    Human Spatial Cognition Laboratory
    Department of Psychology - University of Arizona   
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public enum Format
{
    SOP,
    JRD,
    RVD
}

public class LM_CompassPointing : ExperimentTask
{
    [Header("Task-specific Properties")]
    public Format format;
    public LM_Compass compass;
    public TextAsset sopText;
    public TextAsset jrdText;
    public TextAsset rvdText;
    public LM_PermutedList listOfTriads;
    public bool randomStartRotation;
    public Vector3 compassPosOffset = new Vector3(0f, 0f, -2f);
    public Vector3 compassRotOffset = new Vector3(15f, 0f, 0f);
    [Min(0f)]
    public float secondsBeforeResponse = 5.0f; // how long before they can submit answer

    private List<GameObject> questionItems; 
    private GameObject location; // standing at the...
    private GameObject orientation; // facing the...
    private GameObject target; // point to the...
    private float startAngle; // where is the compass pointer at the start?
    private float answer; // what should they say?
    private float response; // what did they say?
    private float signedError; // how far off were they to the right or left?
    private float absError; // how far off were they regardless of direction?
    private string formattedQuestion;
    private bool oriented;

    private float startTime; // mark the start of the task timer
    private float orientTime; // save the time to orient (SOP only)
    private float responseTime; // save the time to answer

    public override void startTask()
    {
        TASK_START();

        // LEAVE BLANK
    }


    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();

        startTime = Time.time;

        // Restrict Movement
        manager.player.GetComponent<CharacterController>().enabled = false;


        // ---------------------------------------------------------------------
        // Configure the Task-independent variables ----------------------------
        // ---------------------------------------------------------------------

        questionItems = listOfTriads.GetCurrentSubset(); // Get the objects for this question
        location = questionItems[0]; // where to the player is positioned (anchor 1)
        orientation = questionItems[1]; // where the player is facing (anchor 2)
        target = questionItems[2]; // where the player is estimating 


        // ---------------------------------------------------------------------
        // Configure the Task based on selected format -------------------------
        // ---------------------------------------------------------------------

        switch (format)
        {
            case Format.SOP:
                // Prepare SOP hud and question
                hud.showEverything(); // configure the hud for the format
                formattedQuestion = string.Format(sopText.ToString(), target.name); // prepare to present the question
                break;

            case Format.JRD:
                // Prepare JRD hud and question
                hud.showOnlyHUD(); 
                formattedQuestion = string.Format(jrdText.ToString(), location.name, orientation.name, target.name); 

                // Calculate the correct answer (done later for SOP)
                answer = Vector3.SignedAngle(orientation.transform.position - location.transform.position,
                                            target.transform.position - location.transform.position, Vector3.up);
                if (answer < 0) answer += 360;
                Debug.Log("Answer is " + answer);
                break;

            case Format.RVD:
                Debug.LogError("RVD task is in development, but not ready for use");
                break;
        }


        // ---------------------------------------------------------------------
        // Make the player face the orientation target -------------------------
        // ---------------------------------------------------------------------

        // If using 1st person controller (e.g., keyboard mouse controller
        if (avatar.GetComponent<FirstPersonController>() != null)
        {
            avatar.GetComponent<FirstPersonController>().enabled = false; // disable the controller
            avatar.transform.position = location.transform.position; // move player to the pointing location

            // Point the player at the orientation for JRD or a random orientation for SOP start
            avatar.transform.LookAt(orientation.transform); 

            // Reset the camera to be zeroed on the controller position (i.e. looking straight forward)
            avatar.GetComponentInChildren<Camera>().transform.localEulerAngles = Vector3.zero; // reset the camera
            avatar.GetComponent<FirstPersonController>().ResetMouselook(); // reset the zero position to be our current cam orientation

            // Give control back to 1stPerson controller
            if (format == Format.SOP) avatar.GetComponent<FirstPersonController>().enabled = true; // re-enable the controller
        }
        // FIXME add reoreintation for VR controllers
        // ---------------------------------------------------------------------


        // ---------------------------------------------------------------------
        // Set up the compass object for this trial (physical compass to point)
        // ---------------------------------------------------------------------

        var compassparent = compass.transform.parent;
        compass.transform.parent = avatar.GetComponentInChildren<LM_SnapPoint>().transform; // make it the child of the snappoint
        compass.transform.localPosition = compassPosOffset; // adjust position
        compass.transform.localEulerAngles = compassRotOffset; // adjust rotation
        compass.transform.parent = compassparent; // send it back to its old parent to avoid funky movement effects
        compass.ResetPointer(random:randomStartRotation); // set the compass arrow to zero (or a random rotation)
        startAngle = compass.pointer.transform.localEulerAngles.y;
        

        // Put up the HUD
        if (format == Format.SOP)
        {
            hud.setMessage("Orient yourself to the best of your ability.\nPress Enter when you are ready.");
        }
        else if (format == Format.JRD)
        {
            hud.setMessage(formattedQuestion);
            compass.gameObject.SetActive(true);
            compass.interactable = true;
            oriented = true;
        }
        
    }


    public override bool updateTask()
    {
        if (Time.time - startTime > secondsBeforeResponse) // don't let them submit until the wait time has passed
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (!oriented)
                {
                    if (format == Format.SOP)
                    {
                        avatar.GetComponent<FirstPersonController>().enabled = false; // disable the controller to work
                        avatar.GetComponentInChildren<Camera>().transform.localEulerAngles = Vector3.zero; // reset the camera
                        avatar.GetComponent<FirstPersonController>().ResetMouselook(); // reset the zero position to be our current cam orientation

                        var compassparent = compass.transform.parent;
                        compass.transform.parent = avatar.GetComponentInChildren<LM_SnapPoint>().transform; // make it the child of the snappoint
                        compass.transform.localPosition = compassPosOffset; // adjust position
                        compass.transform.localEulerAngles = compassRotOffset; // adjust rotation
                        compass.transform.parent = compassparent; // send it back to its old parent to avoid funky movement effects

                        // Calculate the correct answer (using the new oriented facing direction)
                        var newOrientation = avatar.GetComponentInChildren<LM_SnapPoint>().gameObject;
                        answer = Vector3.SignedAngle(newOrientation.transform.position - location.transform.position,
                                                    target.transform.position - location.transform.position, Vector3.up);
                        if (answer < 0) answer += 360;
                        Debug.Log("Answer is " + answer);
                    }



                    oriented = true; // mark them oriented
                    hud.setMessage(formattedQuestion);
                    compass.gameObject.SetActive(true);
                    compass.interactable = true;
                    orientTime = Time.time - startTime; // save the orientation time
                    startTime = Time.time; // reset the start clock for the answer portion

                    return false; // don't end the trial
                }
                else
                {
                    // record response time
                    responseTime = Time.time - startTime;

                    // Record the response as an angle between -180 and 180
                    response = compass.pointer.transform.localEulerAngles.y;

                    Debug.Log("RESPONSE: " + response.ToString());

                    // Calculate the (signed) error - should be between -180 and 180
                    signedError = response - answer;
                    if (signedError > 180) signedError -= 360;
                    else if (signedError < -180) signedError += 360;
                    Debug.Log("Signed Error: " + signedError);
                    absError = Mathf.Abs(signedError);
                    Debug.Log("Absolute Error: " + absError);

                    return true; // end trial
                }

            }
        }

        hud.ForceShowMessage(); // keep the question up
        return false;

    }


    public override void endTask()
    {
        TASK_END();

        // --------------------------
        // Log data
        // --------------------------
        taskLog.AddData(transform.name + "_task", format.ToString());
        taskLog.AddData(transform.name + "_location", location.name);
        taskLog.AddData(transform.name + "_orientation", orientation.name);
        taskLog.AddData(transform.name + "_target", target.name);
        taskLog.AddData(transform.name + "_compassStartAngle", startAngle.ToString()); // record where we started the compass at
        taskLog.AddData(transform.name + "_responseCW", response.ToString());
        taskLog.AddData(transform.name + "_answerCW", answer.ToString());
        taskLog.AddData(transform.name + "_signedError", signedError.ToString());
        taskLog.AddData(transform.name + "_absError", absError.ToString());
        taskLog.AddData(transform.name + "_SOPorientingTime", orientTime.ToString());
        taskLog.AddData(transform.name + "_responseTime", responseTime.ToString());
        
    }


    public override void TASK_END()
    {
        base.endTask();

        listOfTriads.IncrementCurrentSubset(); // next set of targets
        oriented = false; // reset for next SOP trial (if any)
        compass.interactable = false; // shut off the compass object's function
        compass.gameObject.SetActive(false); // hide the compass
        // Free Movement
        if (avatar.GetComponent<FirstPersonController>() != null) avatar.GetComponent<FirstPersonController>().enabled = true; // if using 1stPerson controller
        manager.player.GetComponent<CharacterController>().enabled = false;
    }

}