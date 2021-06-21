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

    [Header("Task-specific Properties")]

    private GameObject scaledEnvironment;
    public bool autoscale = true;
    public float scaleRatio = 1;
    [Min(0f)]
    public float speedAdjustFactor = 3f; // in case scaling with env makes large player too fast/slow
    private CharacterController characterController;
    // public bool isScaled = false;

    public override void startTask()
    {
        TASK_START();
        avatarLog.navLog = false;
        scaledAvatarLog.navLog = false;
    }

    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();

        scaledEnvironment = manager.scaledEnvironment;
        if (scaledEnvironment == null)
        {
            throw new Exception("Missing a Scaled Environment. Try adding the LM_ScaledEnvironment prefab to your scene.");
        }

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


        // Are we reversing the scale?
        if (isScaled == true)
        {
            scaledEnvironment.SetActive(false);
            scaledAvatar.SetActive(false);
            scaleRatio = 1 / scaleRatio;
        }
        else
        // Grab the scale of the scaled environment from it's gameobject
        {
            scaledEnvironment.SetActive(true);
            scaledAvatar.SetActive(true);
        }

        //---------------------------------------------
        // Scale the player controller
        //---------------------------------------------

        // Make the player bigger/smaller
        manager.player.transform.localScale *= scaleRatio;
        // Adjust the radius
        manager.player.GetComponent<CharacterController>().radius /= scaleRatio;
        // Do this for capsule collider as well
        manager.player.GetComponent<CapsuleCollider>().radius /= scaleRatio;

        // Calculate how to scale the movement speed
        float speedScaler;
        if (isScaled)
        {
            speedScaler = scaleRatio * speedAdjustFactor;
        }
        else speedScaler = scaleRatio / speedAdjustFactor;
        // Depending on the controller, implement the speed scaling
        if (manager.userInterface == UserInterface.KeyboardMouse)
        {
            manager.player.GetComponent<FirstPersonController>().m_WalkSpeed *= speedScaler;
        }
        else if (manager.userInterface == UserInterface.ViveVirtualizer)
        {
            // reign in the scaling (cVirt uses a multiplier, not an actual speed value... it would move too fast
            // manager.player.GetComponent<CVirtPlayerController>().movementSpeedMultiplier *= speedScaler;

        }
        else Debug.Log("WARNING: A speed multiplier is not set up for your player controller. See ScalePlayerToEnvironmentEditor.cs");
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
