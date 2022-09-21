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

public class LM_InterleaveLists : ExperimentTask
{
    [Header("Task-specific Properties")]
    public ObjectList[] lists;
    public ObjectList outputList;
    public bool randomListOrder = true;

    public override void startTask()
    {
        TASK_START();

        // LEAVE BLANK
    }


    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();

        if (skip)
        {
            log.log("INFO    skip task    " + name, 1);
            return;
        }

        // Clear out the existing stand-in list if there is one
        outputList.objects.Clear();

        // Shuffle the order in which we draw from each list if desired
        if (randomListOrder) Experiment.Shuffle<ObjectList>(lists);

        // deterimine the longest list
        int longestListLength = 0;
        foreach (ObjectList list in lists)
        {
            if (list.objects.Count > longestListLength) longestListLength = list.objects.Count;
        }
        
        // interleave
        for (int i = 0; i < longestListLength; i++)
        {
            foreach (ObjectList list in lists)
            {
                if (list.objects[i] != null)
                {
                    outputList.objects.Add(list.objects[i]);
                }
                else Debug.LogWarning("The list being interleaved are not of equal length. " +
                                      "The remainder of the new list will comprise the only the longer list(s)");
            }
        }

        // make sure that outputList's parentObject and parentName are empty or it will ignore what we just did
        outputList.parentObject = null;
        outputList.parentName = "";

    }


    public override bool updateTask()
    {
        return true;

        // WRITE TASK UPDATE CODE HERE
    }


    public override void endTask()
    {
        TASK_END();

        // LEAVE BLANK
    }


    public override void TASK_END()
    {
        base.endTask();

        // WRITE TASK EXIT CODE HERE
    }

}