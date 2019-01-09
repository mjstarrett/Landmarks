using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitializeScaledNavTask : ExperimentTask
{

    public GameObject scaledEnvironment;
    public GameObject startLocationsParent; // must have at least 1 child
    public bool randomStartLocation = false;
    public float scaledPlayerSpeed;

    public override void startTask()
    {
        TASK_START();
        avatarLog.navLog = true;
    }

    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();

        //---------------------------------------------
        // Adjust basic parameters for task
        //---------------------------------------------

        // make the cursor functional and visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // Turn off Player movement
        avatar.GetComponent<CharacterController>().enabled = false;
        // Make the scaled environment visible
        scaledEnvironment.SetActive(true);


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
        // Instantiate the scaled 1st-person controller
        //---------------------------------------------

        // Copy the player from Experiment so it functions the same way
        GameObject scaledPlayer = Instantiate<GameObject>(avatar, startLocationsParent.transform.position, startLocationsParent.transform.rotation, scaledEnvironment.transform);
        scaledPlayer.name = "ScaledPlayer";
        scaledPlayer.GetComponent<CharacterController>().enabled = true;
        //FIXME Figure out how to make the scaled up player move faster
        //scaledPlayer.GetComponent<CapsuleCollider>().attachedRigidbody.mass = scaledPlayer.GetComponent<CapsuleCollider>().attachedRigidbody.mass * scaledPlayerSpeed;
       

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
