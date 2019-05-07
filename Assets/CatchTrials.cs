/*  CatchTrials

    Used to set up subsequent loops where various modular components are to be
    skipped, such as catch trials where perhaps a cue never comes or a test 
    never follows as is typical with other trials in that loop.

    Copyright (C) 2019 Michael J. Starrett

    Navigate by StarrLite (Powered by LandMarks)
    Human Spatial Cognition Laboratory
    Department of Psychology - University of Arizona   
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchTrials : ExperimentTask
{
    // Public Variables
    public TaskList targetLoop;
    public GameObject[] skipOnCatch;
    public int catchFlagsPerLoop; // how many trials should get flagged?


    public override void startTask()
    {
        TASK_START();
    }


    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();
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
