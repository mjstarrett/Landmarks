using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphPrefs : ExperimentTask
{
    [Header("Task-specific Properties")]
    public int totalTurns_left;
    public int totalTurns_right;
    public float leg1distance_left;
    public float leg1distance_right;
    public float turn1angle_left;
    public float turn1angle_right;

    public bool diffTurns;
    public bool diffDistances;
    public bool diffAngles;


    private void Awake()
    {
        base.Awake();

        if (totalTurns_left != totalTurns_right) diffTurns = true;
        if (leg1distance_left != leg1distance_right) diffDistances = true;
        if (turn1angle_left != turn1angle_right) diffAngles = true;
    }

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

        // More concise LM_TrialLog logging
        taskLog.AddData("totalTurnsLeft", totalTurns_left.ToString());
        taskLog.AddData("totalTurnsRight", totalTurns_right.ToString());
        taskLog.AddData("leg1distanceLeft", leg1distance_left.ToString());
        taskLog.AddData("leg1distanceRight", leg1distance_right.ToString());
        taskLog.AddData("turn1angleLeft", turn1angle_left.ToString());
        taskLog.AddData("turn1angleRight", turn1angle_right.ToString());
        taskLog.AddData("diffTurns", diffTurns.ToString());
        taskLog.AddData("diffDistances", diffDistances.ToString());
        taskLog.AddData("diffAngles", diffAngles.ToString());

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
