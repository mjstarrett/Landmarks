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
    public TextAsset sopText;
    public LM_PermutedList sopList;
    public TextAsset jrdText;
    public LM_PermutedList jrdList;
    public bool randomStartRotation;
    public Vector3 compassPosOffset = new Vector3(0f, 0f, -2f);
    public Vector3 compassRotOffset = new Vector3(15f, 0f, 0f);

    private List<GameObject> questionItems; 
    private GameObject location; // standing at the...
    private GameObject orientation; // facing the...
    private GameObject target; // point to the...
    private LM_Compass compass; 
    private float answer; // what should they say?
    private float response; // what did they say?
    private float signedError; // how far off were they to the right or left?
    private float absError; // how far off were they regardless of direction?
    private string formattedQuestion;
    private bool oriented;

    public override void startTask()
    {
        TASK_START();

        // LEAVE BLANK
    }


    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();

        // Restrict Movement
        manager.player.GetComponent<CharacterController>().enabled = false;



        // ---------------------------------------------------------------------
        // Configure the Task based on selected format -------------------------
        // ---------------------------------------------------------------------

        switch (format)
        {
            case Format.SOP:
                // Set up the question for presentation
                hud.showEverything(); // configure the hud for the format
                questionItems = sopList.permutedList[parentTask.repeatCount - 1]; // Get the objects for this question
                location = questionItems[0];
                orientation = avatar.GetComponentInChildren<LM_SnapPoint>().gameObject; 
                target = questionItems[1];
                formattedQuestion = string.Format(sopText.ToString(), target.name); // prepare to present the question
                break;

            case Format.JRD:
                // Set up the question for presentation
                hud.showOnlyHUD(); // configure the hud for the format
                questionItems = jrdList.permutedList[parentTask.repeatCount - 1]; // Get the objects for this question
                location = questionItems[0];
                orientation = questionItems[1];
                target = questionItems[2];
                formattedQuestion = string.Format(jrdText.ToString(), location.name, orientation.name, target.name); // prepare to present the question

                // Calculate the correct answer?
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
            if (format == Format.JRD) avatar.transform.LookAt(orientation.transform); 
            else if (format == Format.SOP) avatar.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360 - Mathf.Epsilon));

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

        compass = FindObjectOfType<LM_Compass>();
        var compassparent = compass.transform.parent;
        compass.transform.parent = avatar.GetComponentInChildren<LM_SnapPoint>().transform; // make it the child of the snappoint
        compass.transform.localPosition = compassPosOffset; // adjust position
        compass.transform.localEulerAngles = compassRotOffset; // adjust rotation
        compass.transform.parent = compassparent; // send it back to its old parent to avoid funky movement effects
        compass.ResetPointer(random:randomStartRotation);
        compass.gameObject.SetActive(false);

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

                    // Calculate the correct answer?
                    answer = Vector3.SignedAngle(orientation.transform.position - location.transform.position,
                                                target.transform.position - location.transform.position, Vector3.up);
                    if (answer < 0) answer += 360;
                    Debug.Log("Answer is " + answer);
                }

                

                oriented = true; // mark them oriented
                hud.setMessage(formattedQuestion); 
                compass.gameObject.SetActive(true);
                compass.interactable = true;

                return false; // don't end the trial
            }
            else
            {
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

        hud.ForceShowMessage(); // keep the question up
        return false;

    }


    public override void endTask()
    {
        TASK_END();

        // --------------------------
        // Log data
        // --------------------------

        if (trialLog.active)
        {
            trialLog.AddData(transform.name + "_location", location.name);
            trialLog.AddData(transform.name + "_orientation", orientation.name);
            trialLog.AddData(transform.name + "_target", target.name);
            trialLog.AddData(transform.name + "_answerCW", answer.ToString());
            trialLog.AddData(transform.name + "_responseCW", response.ToString());
            trialLog.AddData(transform.name + "_signedError", signedError.ToString());
            trialLog.AddData(transform.name + "_absError", absError.ToString());
        }
    }


    public override void TASK_END()
    {
        base.endTask();

        oriented = false;
        compass.interactable = false;
        // Free Movement
        if (avatar.GetComponent<FirstPersonController>() != null) avatar.GetComponent<FirstPersonController>().enabled = true; // if using 1stPerson controller
        manager.player.GetComponent<CharacterController>().enabled = false;
    }

}