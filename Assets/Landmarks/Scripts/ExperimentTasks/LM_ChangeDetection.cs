/*


    Copyright (C) 2019 Michael J. Starrett

    Navigate by StarrLite (Powered by LandMarks)
    Human Spatial Cognition Laboratory
    Department of Psychology - University of Arizona   
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class LM_ChangeDetection : ExperimentTask
{
    [Header("Task-specific Properties")]

    [Range(0, 100)]
    public int changeProbability = 50; // probability that any given trial has a change
    [Min(0f)]
    public float feedbackTimeinSeconds = 3;
    public bool cumulativeChanges; // (if false) change objects back to original color at end of trial
    public string correctMessage = "Correct";
    public string incorrectMessage = "Incorrect";

    private bool changePresent; // is this a change trial?
    private bool responded; // has the participant provided a valid response yet?
    private KeyCode correctAnswer; // What button is the correct one?
    private KeyCode response; // which button button did they press?
    private GameObject changedObject; // container for which object was changed
    private Color originalColor; // container for reverting the changed object back
    private Color changeColor;
    private bool feedbackProvided;

    public override void startTask()
    {
        TASK_START();

        // LEAVE BLANK
    }


    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();

        // --------------------
        // Set up basics
        // --------------------

        hud.showEverything();
        hud.setMessage("Press [1] if any object changed. Otherwise press [2]");
        avatarLog.navLog = true;  // track player position


        //FIXME
        // ------------------------------------------
        // Determine which trials are change (50%) 
        // ------------------------------------------
        //FIXME

        // randomly generate an int between 0 and 100
        var changeDeterminant = Random.Range(0, 100);
        Debug.Log(changeDeterminant.ToString());

        // if the number is within our change probability, it's a change trial
        if (changeDeterminant <= changeProbability)
        {
            changePresent = true;
            correctAnswer = KeyCode.Alpha1;
            Debug.Log("There is a change!");
        }
        // if it exceeds our change prob., it's a no-change trial
        else
        {
            correctAnswer = KeyCode.Alpha2;
            changePresent = false;
            Debug.Log("There is no change!");
        }


        // ------------------------------------------
        // Select the change candidate target
        // ------------------------------------------

        var iChange = Random.Range(0, manager.targetObjects.transform.childCount);
        for (int i = 0; i < manager.targetObjects.transform.childCount; i++)
        {
            
            // change the color of the object (if necessary)
            if (i == iChange)
            {
                changedObject = manager.targetObjects.transform.GetChild(i).gameObject;
                originalColor = changedObject.GetComponent<Renderer>().material.color;
                if (changePresent)
                {
                    // record which object is getting changed and it's original color
                    changeColor = Random.ColorHSV();

                    // actually change the color
                    changedObject.GetComponentInChildren<Renderer>().material.color = changeColor;
                }
                else
                {
                    changeColor = originalColor;
                }
            }
        }

    }


    public override bool updateTask()
    {

        hud.ForceShowMessage(); // show scene during the trial


        // ------------------------------------------
        // Handle taking a response 
        // ------------------------------------------

        // Stop taking responses after a valide response is given
        if (!responded)
        {
            // Handle each of the valid responses
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                responded = true;
                response = KeyCode.Alpha1;
                Debug.Log("You said there was a change");
                StartCoroutine(CheckAnswer());

            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                responded = true;
                response = KeyCode.Alpha2;
                Debug.Log("You said there was NO change");
                StartCoroutine(CheckAnswer());
            }
            // otherwise don't do anything
            else
            {
                
            }
        }


        // ------------------------------------------
        // Handle ending the trial, when it's time
        // ------------------------------------------

        if (feedbackProvided)
        {
            return true;
        }
        else return false;
        
    }


    public override void endTask()
    {
        TASK_END();

        // LEAVE BLANK
    }


    public override void TASK_END()
    {
        base.endTask();


        // --------------------------------
        // Log Data
        // --------------------------------
        taskLog.AddData(transform.name + "changePresent", changePresent.ToString());
        taskLog.AddData(transform.name + "changedObject", changedObject.name);
        taskLog.AddData(transform.name + "originalColor", originalColor.ToString());
        taskLog.AddData(transform.name + "changedColor", changeColor.ToString());
        taskLog.AddData(transform.name + "participantResponse", response.ToString());
        taskLog.AddData(transform.name + "correctAnswer", correctAnswer.ToString());
        taskLog.AddData(transform.name + "correct", (response == correctAnswer).ToString());


        // --------------------------------
        // clean up
        // --------------------------------

        hud.setMessage("");
        hud.showOnlyHUD();
        avatarLog.navLog = false;
        if (!cumulativeChanges) changedObject.GetComponent<Renderer>().material.color = originalColor; // change it back
        feedbackProvided = false;
        responded = false;


    }


    // -----------------------------------
    // Handle when a response is given
    // -----------------------------------

    IEnumerator CheckAnswer()
    {
        
        hud.showOnlyHUD(); // hide the scenery

        // check the answer
        if (response == correctAnswer)
        {
            hud.setMessage(correctMessage);
        }
        else
        {
            hud.setMessage(incorrectMessage);
        }

        // Configure the hud based on the answer
        hud.ForceShowMessage();

        // Show the feedback for specified time
        yield return new WaitForSeconds(feedbackTimeinSeconds);

        // Don't let the task end until the feedback time has passed
        feedbackProvided = true;
    }


}