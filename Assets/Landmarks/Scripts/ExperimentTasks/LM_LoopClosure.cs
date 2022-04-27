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
using Valve.VR;
using Valve.VR.InteractionSystem;

public class LM_LoopClosure : NavigationTask
{
    public GameObject loopPivot;

    private Vector2 loopPivotPosition;
    private Vector2 loopStartPosition;
    private float loopStartAngle;
    private float angularTravel;
    private float loopEndPosition;
    private float loopEndAngle;

    public override void startTask()
    {
        TASK_START();

        // LEAVE BLANK
    }


    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();

        loopPivotPosition = new Vector2(loopPivot.transform.position.x, loopPivot.transform.position.z);
        loopStartPosition = new Vector2(avatar.transform.position.x, avatar.transform.position.z);
        loopStartAngle = Vector3Angle2D(loopPivotPosition, avatar.transform.position);
        Debug.Log(loopStartAngle);
    }


    public override bool updateTask()
    {
        base.updateTask();

        // Record/Update Loop angle
        
        // Handle user input (or lack thereof)
        if (Input.GetKeyDown(KeyCode.Return)) return true; 
        else if (vrEnabled) if (vrInput.TriggerButton.GetStateDown(SteamVR_Input_Sources.Any)) return true;
        
        return false;
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