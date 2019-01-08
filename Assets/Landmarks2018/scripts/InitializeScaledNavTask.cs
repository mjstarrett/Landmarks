using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitializeScaledNavTask : ExperimentTask
{

    public GameObject scaledEnvironment;

    public override void startTask()
    {
        TASK_START();
        avatarLog.navLog = true;
    }

    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();

        //// make the cursor functional and visible
        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;

        //// Turn off Player movement
        //avatar.GetComponent<CharacterController>().enabled = false;

        // Make the scaled environment visible
        scaledEnvironment.SetActive(true);

        //// Copy the player from Experiment so it functions the same way
        //GameObject ScaledPlayer = Instantiate<GameObject>(avatar, transform.parent);
        //ScaledPlayer.name = "ScaledPlayer";
        //// Adjust the scale

    //    // Swap from 1st-person camera to overhead view
    //    firstPersonCamera.enabled = false;
    //    overheadCamera.enabled = true;

    //    // Change text and turn on the map action button
    //    actionButton.GetComponentInChildren<Text>().text = "Continue to Test";
    //    manager.actionButton.SetActive(true);
    //    actionButton.onClick.AddListener(OnActionClick);
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
