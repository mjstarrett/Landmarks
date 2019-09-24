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

public class LM_LogTrialInfo : ExperimentTask
{
    public TaskList blocks;
    public TaskList trials;


    public override void startTask()
    {
        TASK_START();
    }


    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();

        if (blocks != null && trials != null)
        {
            log.log("TRIAL_INFO\tTask:\t" + blocks.gameObject.name + "\tBlock:\t" + blocks.repeatCount + "\tTrial:\t" + trials.repeatCount, 1);
        }
        else Debug.Log(this.name + "\tSufficient Logging Info not Provided");

    }


    public override bool updateTask()
    {
        return true;
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