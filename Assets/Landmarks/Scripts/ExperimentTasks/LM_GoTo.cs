/*
    
    Copyright (C) 2022 Michael J. Starrett Ambrose
    University of California, Irvine
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

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
        if (atDestination & Mathf.Abs(Mathf.DeltaAngle(avatar.transform.eulerAngles.y, destination.transform.eulerAngles.y)) < orientThreshold)
        {
            if (manager.usingVR)
            {
                if (vrInput.TriggerButton.stateDown)
                {
                    log.log("INPUT_EVENT    Player Arrived at Destination    1", 1);
                    return true;
                }
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
        if (collision.transform.CompareTag("Player"))
        {
            if (effect != null) effect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            atDestination = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            if (effect != null) effect.Play(true);
            atDestination = false;
        }
    }

}