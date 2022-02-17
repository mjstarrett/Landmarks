/*
    
    Copyright (C) 2022 Michael J. Starrett Ambrose
    University of California, Irvine
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class LM_GoTo : ExperimentTask
{
    [Header("Task-specific Properties")]
    public GameObject destination;
    public float orientThreshold = 15.0f;

    private ParticleSystem effect;
    private bool atDestination;

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

        hud.SecondsToShow = 0; // we don't need a hud

        if (destination.GetComponentInChildren<ParticleSystem>() != null) effect = destination.GetComponentInChildren<ParticleSystem>();
        destination.SetActive(true); // show the destination if it's hidden
        if (effect != null) effect.Play(true); // start particles if we have them
        hud.showOnlyHUD(); // FIXME for now. Eventually you will need to add a hud.ShowTerrain, or something like that.
        manager.environment.SetActive(false);
    }


    public override bool updateTask()
    {
        // Is the player at and aligned with the destination?
        if (atDestination & Mathf.Abs(Mathf.DeltaAngle(manager.playerCamera.transform.eulerAngles.y, destination.transform.eulerAngles.y)) < orientThreshold)
        {
            if (vrInput.TriggerButton.GetStateDown(SteamVR_Input_Sources.Any))
            {
                Debug.Log("VR trying to start the task");
                log.log("INPUT_EVENT    Player Arrived at Destination    1", 1);
                return true;
            }
            else if (Input.GetButtonDown("Return") | Input.GetKeyDown(KeyCode.Return))
            {
                log.log("INPUT_EVENT    Player Arrived at Destination    1", 1);
                return true;
            }
        }

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
        destination.SetActive(false);
        manager.environment.SetActive(true);
        hud.SecondsToShow = hud.GeneralDuration;
    }

    private void OnTriggerEnter(Collider collision)
    {

        Debug.Log(collision.name);
        if (collision.name == avatar.GetComponentInChildren<LM_PlayerController>().collisionObject.gameObject.name)
        {
            if (effect != null) effect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            atDestination = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision == avatar.GetComponentInChildren<LM_PlayerController>().collisionObject)
        {
            if (effect != null) effect.Play(true);
            atDestination = false;
        }
    }

}