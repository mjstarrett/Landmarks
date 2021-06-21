using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Actions
{
    Ignore,
    onlyUnused,
    onlyUsed
}

public class LM_UpdateTargetObjects : ExperimentTask
{
    [Header("Task-specific Properties")]
    public Actions priorTargetAction = Actions.onlyUnused;
    private GameObject parentOfNewTargets;

    public override void startTask()
    {
        TASK_START();

        // LEAVE BLANK
    }


    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();

        // Never run on the first scene (there won't be any used/unused targets to update with)
        if (manager.config.levelNumber == 0)
        {
            skip = true;
        }

        switch (priorTargetAction)
        {
            case Actions.Ignore:
                return;

            case Actions.onlyUnused:
                // Make sure our new objects can be found, otherwise skip and run as new scene
                if (GameObject.Find("UnusedTargets"))
                {
                    parentOfNewTargets = GameObject.Find("UnusedTargets");
                }
                else
                {
                    Debug.Log("GameObject 'UnusedTargets' does not exist");
                    goto default;
                }
                break;

            case Actions.onlyUsed:
                // Make sure our new objects can be found, otherwise skip and run as new scene
                if (GameObject.Find("UsedTargets") != null)
                {
                    parentOfNewTargets = GameObject.Find("UsedTargets");
                }
                else
                {
                    Debug.Log("GameObject 'UsedTargets' does not exist");
                    goto default;
                }
                break;

            default:
                skip = true;
                break;
        }


        // end the task if we're skipping it
        if (skip)
        {
            return;
        }

        // Update the target list if we're still here
        // If our new parent exists, destroy the scene's included target objects...
        foreach (Transform child in manager.targetObjects.transform)
        {
            Destroy(child.gameObject);
        }
        // And repopulate TargetObjects with these new objects
        foreach (Transform child in parentOfNewTargets.transform)
        {
            child.parent = manager.targetObjects.transform;
            child.gameObject.SetActive(true);
        }

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
