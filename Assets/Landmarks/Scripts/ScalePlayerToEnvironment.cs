/*
    ScalePlayerToEnvironment
       
    Takes a Player controller and scale it manually or automatically to an 
    environment or reverse a previous scaling.   

    Copyright (C) 2019 Michael J. Starrett

    Navigate by StarrLite (Powered by LandMarks)
    Human Spatial Cognition Laboratory
    Department of Psychology - University of Arizona   
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEditor;
using System;


public class ScalePlayerToEnvironment : ExperimentTask
{

    public GameObject scaledEnvironment;
    public GameObject startLocationsParent; // must have at least 1 child
    public bool randomStartLocation = false;
    public bool autoscale = true;
    public float scaleRatio = 1;
    private CharacterController characterController;
    // public bool isScaled = false;
    
    public override void startTask()
    {
        TASK_START();
        avatarLog.navLog = true;
        scaledAvatarLog.navLog = true;
    }

    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();


        //---------------------------------------------
        // Set up basic parameters
        //---------------------------------------------

        // make the cursor functional and visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (autoscale)
        {
            scaleRatio = scaledEnvironment.transform.localScale.x;
        }


        Debug.Log("The environment is currently scaled" + isScaled);
        // Are we reversing the scale?
        if (isScaled == true)
        {
            scaledEnvironment.SetActive(false);
            scaleRatio = 1 / scaleRatio;
        } 
        else
        // Grab the scale of the scaled environment from it's gameobject
        {
            scaledEnvironment.SetActive(true);
        }


        //---------------------------------------------
        // Select from starting locations in scaled Env.
        //---------------------------------------------

        //------FIXME Handle list of start locations-------- 
        //// Select a starting location for the scaled player
        //if (startLocationsParent.transform.childCount != 0)
        //{
        //    List<GameObject> startLocations = new List<GameObject>();
        //    foreach (Transform child in startLocationsParent.transform)
        //    {
        //        startLocations.Add(child.gameObject);
        //    }
        //}

        //---------------------------------------------
        // Scale the player controller
        //---------------------------------------------

        // Make the player bigger/smaller
        manager.player.transform.localScale = scaleRatio * manager.player.transform.localScale;
        // Adjust the radius
        //manager.player.GetComponent<CharacterController>().radius = manager.player.GetComponent<CharacterController>().radius / scaleRatio;
        // Do this for capsule collider as well
        //manager.player.GetComponent<CapsuleCollider>().radius = manager.player.GetComponent<CapsuleCollider>().radius / scaleRatio;

        //Move the player to the starting position and appropriate rotation;
        manager.player.transform.position = startLocationsParent.transform.position;
       
        // Rotate the player
        manager.player.transform.localEulerAngles = startLocationsParent.transform.localEulerAngles;

        // Scale up the movement speed as well
        if (manager.userInterface == UserInterface.DesktopDefault)
        {
            manager.player.GetComponent<FirstPersonController>().m_WalkSpeed = scaleRatio * manager.player.GetComponent<FirstPersonController>().m_WalkSpeed;
        }
        else if (manager.userInterface == UserInterface.ViveAndVirtualizer)
        {
            manager.player.GetComponent<CVirtPlayerController>().movementSpeedMultiplier *= scaleRatio;
        }
        else Debug.Log("WARNING: A speed multiplier is not set up for your player controller. See ScalePlayerToEnvironment.cs");
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
        isScaled = !isScaled;
    }
}


