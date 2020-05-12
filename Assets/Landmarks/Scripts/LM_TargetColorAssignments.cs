using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM_TargetColorAssignments : ExperimentTask
{
    [Header("Task-Specific Properties")]
    [Tooltip("Specify the HSV starting value - assignments will move clockwise through Hue-space" +
        ", keeping saturation and value constant")]
    public Color startingColor = Color.HSVToRGB(0f, 1f, 0.6f);

    public override void startTask()
    {
        TASK_START();

        // LEAVE BLANK
    }


    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();

        // WRITE TASK STARTUP CODE HERE
        if (manager.config.levelNumber != 0)
        {
            skip = true;
        }

        if (skip)
        {
            log.log("INFO    skip task    " + name, 1);
            return;
        }

        // -------------------------------------------------
        // Get the LM_targets and set up their colors
        // -------------------------------------------------

        var hueIncrement = 360.0f / manager.targetObjects.transform.childCount;

        // handle the start color
        float currentHue, S, V;
        Color.RGBToHSV(startingColor, out currentHue, out S, out V);
        Debug.Log("colors will be " + hueIncrement + " degrees apart");

        var targetTargets = manager.targetObjects.transform.GetComponentsInChildren<LM_Target>();
        var targetChildren = new Transform[targetTargets.Length];

        for (int i = 0; i < targetChildren.Length; i++)
        {
            targetChildren[i] = targetTargets[i].GetComponent<Transform>();
        }
        Debug.Log(targetChildren.Length + " target objects detected");

        for (int i = targetChildren.Length - 1; i > 0; i--)
        {
            var r = Random.Range(0, i);
            var tmp = targetChildren[i];
            targetChildren[i] = targetChildren[r];
            targetChildren[r] = tmp;
        }

        foreach (Transform child in targetChildren)
        {
            Debug.Log("COLOR (" + currentHue + ", " + S + ", " + V + ")");
            child.GetComponent<LM_TargetStore>().color = Color.HSVToRGB(currentHue/360.0f, S, V, true);
            currentHue += hueIncrement;
        }
        // There is almost certainly a cleaner way to do this :/
        // ----------------------------------------------------
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
