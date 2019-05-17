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

public class CheckForSavedObjects : ExperimentTask
{
    public GameObject targets;
    public GameObject unusedTargets;

    public override void startTask()
    {
        TASK_START();
    }


    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();

        //----------------------------------------------------------------------
        // compare Environment>TargetObjects with Environment>UnusedTargets
        // Remove any targets that are not in Unused targets (i.e. use only targets
        // that have not been used previously)
        //----------------------------------------------------------------------
        unusedTargets.SetActive(true);

        // Make a list of the previously used targets
        List<string> unusedTargetList = new List<string>();
        foreach (Transform child in unusedTargets.transform)
        {
            unusedTargetList.Add(child.gameObject.name);
        }

        // Destroy the previously used game objects from our pool of targets
        foreach (Transform child in targets.transform)
        {
            if (unusedTargetList.Contains(child.gameObject.name))
            {
                Destroy(child.gameObject);
            }
        }

        // Now destroy all the children of UnusedTargets (to reset it in case there is a third levl)
        foreach (Transform child in unusedTargets.transform)
        {
            Destroy(child.gameObject);
        }
        unusedTargets.SetActive(false);
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