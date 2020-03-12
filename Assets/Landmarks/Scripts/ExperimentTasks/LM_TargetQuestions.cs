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

public class LM_TargetQuestions : ExperimentTask
{
    [Header("Task-specific Properties")]
    public TextAsset questionText;
    public LM_PermutedList questionList;
    public bool blackout = true;

    private List<GameObject> questionItems; 
    private GameObject location; // standing at the...
    private GameObject orientation; // facing the...
    private GameObject target; // point to the...
    private float answer; // what should they say?
    private float response; // what did they say?
    private float signedError; // how far off were they to the right or left?
    private float absError; // how far off were they regardless of direction?

    public override void startTask()
    {
        TASK_START();

        // LEAVE BLANK
    }


    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();

        // --------------------------
        // Set up Task parameters
        // --------------------------

        // Restrict Movement
        manager.player.GetComponent<CharacterController>().enabled = false;

        // what should the player see (JRD/SOP/other)
        if (blackout)
        {
            hud.showOnlyHUD();
        }
        else hud.showEverything();
        // Get the objects for this question (account for the non-zero index of repeatCount
        questionItems = questionList.permutedList[parentTask.repeatCount - 1];


        // Grab the triad that defines this trial
        location = questionItems[0];
        orientation = questionItems[1];
        target = questionItems[2];

        // fill in the blanks for the question
        hud.setMessage(string.Format(questionText.ToString(), location.name, orientation.name, target.name));

        // What is the correct answer?
        answer = Vector3.SignedAngle(orientation.transform.position - location.transform.position,
                                     target.transform.position - location.transform.position,
                                     Vector3.up);
        Debug.Log("Answer is " + answer);


        // point the player at the orientation -- FIXME for SOP
        
    }


    public override bool updateTask()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            return true;
        }

        hud.ForceShowMessage();

        return false;

        // WRITE TASK UPDATE CODE HERE
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
            trialLog.AddData(transform.name + "_answer", answer.ToString());
            //trialLog.AddData(transform.name + "_response", );
            //trialLog.AddData(transform.name + "_signedError", );
            //trialLog.AddData(transform.name + "_absError", );
        }
    }


    public override void TASK_END()
    {
        base.endTask();

        // Free Movement
        manager.player.GetComponent<CharacterController>().enabled = false;
    }

}