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
using UnityStandardAssets.Characters.ThirdPerson;

public class MentalNavigation : ExperimentTask
{

    [Header("Task-specific Properties")]

    public ObjectList objects; 
    public bool restrictMovement = true; // MJS do we want to keep them still during this?
    private float startTime;
    private float navTime = 0;
    public float minimumRT = 0.5f;
    public bool blackout = true;


    public override void startTask()
    {
        TASK_START();
    }


    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();

        if (restrictMovement)
        {
            avatar.GetComponent<CharacterController>().enabled = false;
            scaledAvatar.GetComponent<ThirdPersonCharacter>().immobilized = true;
        }

        startTime = Time.time;
        if (blackout)
        {
            hud.showNothing();
        }
        else hud.showEverything();

    }


    public override bool updateTask()
    {

        if (skip)
        {
            //log.log("INFO    skip task    " + name,1 );
            return true;
        }
        if (interval > 0 && Experiment.Now() - task_start >= interval)
        {
            return true;
        }


        //------------------------------------------
        // Handle buttons to advance the task - MJS
        //------------------------------------------
        navTime = Time.time - startTime; // how long have we been up

        while (navTime < minimumRT) return false;

        if (Input.GetButtonDown("Return") || (vrEnabled && vrInput.TriggerButton.GetStateDown(Valve.VR.SteamVR_Input_Sources.Any)))
        {
            // // Output log for this task in tab delimited format
            // log.log("LM_OUTPUT\tMentalNavigation.cs\t" + masterTask.name + "\t" + this.name + "\n" +
            // "Task\tBlock\tTrial\tTargetName\tDuration\n" +
            // masterTask.name + "\t" + masterTask.repeatCount + "\t" + parent.repeatCount + "\t" + objects.currentObject().name + "\t" + navTime
            // , 1);

            taskLog.AddData(transform.name + "_target", objects.currentObject().name);
            taskLog.AddData(transform.name + "_duration", navTime.ToString());

            // end the current task
            return true; 
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
    }


    public override void TASK_END()
    {
        base.endTask();


    }

}