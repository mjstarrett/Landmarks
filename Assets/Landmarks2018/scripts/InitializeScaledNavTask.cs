using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class InitializeScaledNavTask : ExperimentTask
{

    public GameObject scaledEnvironment;
    public GameObject startLocationsParent; // must have at least 1 child
    public bool randomStartLocation = false;
    public float scaleRatio = 1;

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

        // Player Movement?
        avatar.GetComponent<CharacterController>().enabled = true;

        // Make the scaled environment visible
        scaledEnvironment.SetActive(true);

        // Grab the scale of the scaled environment from it's gameobject
        scaleRatio = scaledEnvironment.transform.localScale.x;


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

        // Make the player big
        manager.player.transform.localScale = new Vector3(scaleRatio, scaleRatio, scaleRatio);
       
        //Move the player to the starting position and appropriate rotation;
        manager.player.transform.localPosition = startLocationsParent.transform.localPosition;
       
        // Rotate the player
        manager.player.transform.localEulerAngles = startLocationsParent.transform.localEulerAngles;

        // Scale up the movement speed as well
        manager.player.GetComponent<FirstPersonController>().m_WalkSpeed = scaleRatio * manager.player.GetComponent<FirstPersonController>().m_WalkSpeed;

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
