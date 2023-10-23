using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Valve.VR;
using Valve.VR.InteractionSystem;

public enum HideTargetOnStart
{
    Off,
    SetInactive,
    SetInvisible,
    DisableCompletely,
    Mask,
    SetProbeTrial
}

public class NavigationTask : ExperimentTask
{
    [Header("Task-specific Properties")]
    [Tooltip("Leave blank for free exploration")]
    public ObjectList destinations;
	public GameObject currentTarget;

    public TextAsset NavigationInstruction;

    // Manipulate trial/task termination criteria
    [Tooltip("in meters")]
    public float distanceAllotted = Mathf.Infinity;
    [Tooltip("in seconds")]
    public float timeAllotted = Mathf.Infinity;
    [Tooltip("Do we want time or distance remaining to be broadcast somewhere?")]
    public TextMeshProUGUI printRemainingTimeTo;
    private string baseText;

    // Use a scoring/points system (not currently configured)
    [HideInInspector] private int score = 0;
    [HideInInspector] public int scoreIncrement = 50;
    [HideInInspector] public int penaltyRate = 2000;
    [HideInInspector] private float penaltyTimer = 0;
    [HideInInspector] public bool showScoring;

    // Handle the rendering of the target objects (default: always show)
    public HideTargetOnStart hideTargetOnStart;
    [Tooltip("negative values denote time before targets are hidden; 0 is always on; set very high for no targets")]
    public float showTargetAfterSeconds;
    //public TextMeshProUGUI overlayTargetObject;

    // Manipulate the rendering of the non-target environment objects (default: always show)
    public bool hideNonTargets;

    // for compass assist
    public LM_Compass assistCompass;
    [Tooltip("negative values denote time before compass is hidden; 0 is always on; set very high for no compass")]
    public float SecondsUntilAssist = Mathf.Infinity;
    public Vector3 compassPosOffset; // where is the compass relative to the active player snappoint
    public Vector3 compassRotOffset; // compass rotation relative to the active player snap point


    // For logging output
    private float startTime;
    private Vector3 playerLastPosition;
    private float playerDistance = 0;
    private Vector3 scaledPlayerLastPosition;
    private float scaledPlayerDistance = 0;
    private float optimalDistance;
    private LM_DecisionPoint[] decisionPoints;
    private bool exploration;


    // 4/27/2022 Added for Loop Closure Task
    public float allowContinueAfter = Mathf.Infinity; // flag to let participants press a button to continue without necessarily arriving
    public bool haptics;
    private float clockwiseTravel = 0; // relative to the origin (0,0,0) in world space
    public bool logStartEnd;
    private Vector3 startXYZ;
    private Vector3 endXYZ;

    public override void startTask ()
	{
		TASK_START();
		avatarLog.navLog = true;
        if (isScaled) scaledAvatarLog.navLog = true;
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

        if (!destinations)
        {
            Debug.LogWarning("No target objects specified; task will run as" +
                " free exploration with specified time Alloted or distance alloted" +
                " (whichever is less)");

            exploration = true;

            //// Make a dummy placeholder for exploration task to avoid throwing errors
            //var tmp = new List<GameObject>();
            //tmp.Add(gameObject);
            //gameObject.AddComponent<ObjectList>();
            //gameObject.GetComponent<ObjectList>().objects = tmp;
            //destinations = gameObject.GetComponent<ObjectList>();
        }

        hud.showEverything();
		hud.showScore = showScoring;

        if (exploration) currentTarget = gameObject;
        else currentTarget = destinations.currentObject();

        // update the trial count on the overlay
        //if (overlayTargetObject != null & currentTarget != null) overlayTargetObject.text = string.Format("{0}", currentTarget.name);

        // Debug.Log ("Find " + current.name);

        // if it's a target, open the door to show it's active
        if (currentTarget.GetComponentInChildren<LM_TargetStore>() != null)
        {
            currentTarget.GetComponentInChildren<LM_TargetStore>().OpenDoor();
        }

		if (NavigationInstruction)
		{
			string msg = NavigationInstruction.text;
			if (destinations != null) msg = string.Format(msg, currentTarget.name);
			hud.setMessage(msg);
   		}
		else
		{
            hud.SecondsToShow = 0;
            //hud.setMessage("Please find the " + current.name);
		}

        // Handle if we're hiding all the non-targets
        if (hideNonTargets)
        {
            foreach (GameObject item in destinations.objects)
            {
                if (item.name != currentTarget.name)
                {
                    item.SetActive(false);
                }
                else item.SetActive(true);
            }
        }


        // Handle if we're hiding the target object
        if (hideTargetOnStart != HideTargetOnStart.Off && !exploration)
        {
            if (hideTargetOnStart == HideTargetOnStart.SetInactive)
            {
                foreach (var c in currentTarget.GetComponents<Collider>()) c.enabled = false;
            }
            else if (hideTargetOnStart == HideTargetOnStart.SetInvisible)
            {
                manager.HideRecursive(currentTarget);
            }
            else if (hideTargetOnStart == HideTargetOnStart.DisableCompletely)
            {
                manager.HideRecursive(currentTarget);
                foreach (var c in currentTarget.GetComponents<Collider>()) c.enabled = false;
                foreach (var child in currentTarget.GetComponentsInChildren<Transform>())
                {
                    var halo = (Behaviour)child.GetComponent("Halo");
                    if (halo != null) halo.enabled = false;
                }
            }
            else if (hideTargetOnStart == HideTargetOnStart.SetProbeTrial)
            {
                manager.HideRecursive(currentTarget);
                foreach (var c in currentTarget.GetComponents<Collider>()) c.enabled = false;

            }
            
        }
        else if (!exploration)
        {
            currentTarget.SetActive(true); // make sure the target is visible unless the bool to hide was checked
            try
            {
                manager.HideRecursive(currentTarget, true);
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        // save the original string so we can reformat each frame
        if (printRemainingTimeTo != null) baseText = printRemainingTimeTo.text;

        // startTime = Current time in seconds
        startTime = Time.time;

        // Get the avatar start location (distance = 0)
        playerDistance = 0.0f;
        clockwiseTravel = 0.0f;
        playerLastPosition = avatar.GetComponent<LM_PlayerController>().collisionObject.transform.position;
        if (isScaled)
        {
            scaledPlayerDistance = 0.0f;
            scaledPlayerLastPosition = scaledAvatar.transform.position;
        }

        // Calculate optimal distance to travel (straight line)
        if (isScaled)
        {
            optimalDistance = Vector3.Distance(scaledAvatar.transform.position, currentTarget.transform.position);
        }
        else optimalDistance = Vector3.Distance(avatar.GetComponent<LM_PlayerController>().collisionObject.transform.position, currentTarget.transform.position);


        // Grab our LM_Compass object and move it to the player snapPoint
        if (assistCompass != null)
        {
            assistCompass.transform.parent = avatar.GetComponentInChildren<LM_SnapPoint>().transform;
            assistCompass.transform.localPosition = compassPosOffset;
            assistCompass.transform.localEulerAngles = compassRotOffset;
            assistCompass.gameObject.SetActive(false);
        }

        //// MJS 2019 - Move HUD to top left corner
        //hud.hudPanel.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
        //hud.hudPanel.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.9f);


        // Look for any LM_Decsion Points we will want to track
        if (FindObjectsOfType<LM_DecisionPoint>().Length > 0)
        {
            decisionPoints = FindObjectsOfType<LM_DecisionPoint>();

            // Clear any decisions on LM_DecisionPoints
            foreach (var pt in decisionPoints)
            {
                pt.ResetDecisionPoint();
            }
        }

        if (logStartEnd) startXYZ = avatar.GetComponent<LM_PlayerController>().collisionObject.transform.position;
            
        if (vrEnabled & haptics) SteamVR_Actions.default_Haptic.Execute(0f, 2.0f, 65f, 1f, SteamVR_Input_Sources.Any);
    }

    public override bool updateTask ()
	{
		base.updateTask();

        if (skip)
        {
            //log.log("INFO    skip task    " + name,1 );
            return true;
        }

        if (score > 0) penaltyTimer = penaltyTimer + (Time.deltaTime * 1000);

		if (penaltyTimer >= penaltyRate)
		{
			penaltyTimer = penaltyTimer - penaltyRate;
			if (score > 0)
			{
				score = score - 1;
				hud.setScore(score);
			}
		}

        //show target after set time
        if (hideTargetOnStart != HideTargetOnStart.Off && Time.time - startTime > showTargetAfterSeconds && !exploration)
        {

            switch (hideTargetOnStart)
            {
                case HideTargetOnStart.SetInactive:
                    foreach (var c in currentTarget.GetComponents<Collider>()) c.enabled = true;
                    break;
                case HideTargetOnStart.SetInvisible:
                    manager.HideRecursive(currentTarget, true);
                    break;
                case HideTargetOnStart.DisableCompletely:
                    manager.HideRecursive(currentTarget, true);
                    foreach (var c in currentTarget.GetComponents<Collider>()) c.enabled = true;
                    foreach (var child in currentTarget.GetComponentsInChildren<Transform>())
                    {
                        var halo = (Behaviour)child.GetComponent("Halo");
                        if (halo != null) halo.enabled = true;
                    }
                    break;
                case HideTargetOnStart.SetProbeTrial:
                    manager.HideRecursive(currentTarget, true);
                    foreach (var c in currentTarget.GetComponents<Collider>()) c.enabled = true;
                    break;
                default:
                    Debug.Log("No hidden targets identified");
                    break;
            }
        }

        // Keep updating the distance traveled and kill task if they reach max
        playerDistance += Vector3.Distance(avatar.GetComponent<LM_PlayerController>().collisionObject.transform.position, playerLastPosition);
        // Subtract the counter-clockwise angle since the last frame to get clockwise movement
        clockwiseTravel -= Vector3Angle2D(playerLastPosition, avatar.GetComponent<LM_PlayerController>().collisionObject.transform.position);
        playerLastPosition = avatar.GetComponent<LM_PlayerController>().collisionObject.transform.position;
        
        if (isScaled)
        {
            scaledPlayerDistance += Vector3.Distance(scaledAvatar.transform.position, scaledPlayerLastPosition);
            scaledPlayerLastPosition = scaledAvatar.transform.position;
        }
        
        // handle the compass objects render (visible or not)
        if (assistCompass != null)
        {
            // Keep the assist compass pointing at the target (even if it isn't visible)
            var targetDirection = 2 * assistCompass.transform.position - currentTarget.transform.position;
            targetDirection = new Vector3(targetDirection.x, assistCompass.pointer.transform.position.y, targetDirection.z);
            assistCompass.pointer.transform.LookAt(targetDirection, Vector3.up);
            // Show assist compass if and when it is needed
            if (assistCompass.gameObject.activeSelf == false & SecondsUntilAssist >= 0 & (Time.time - startTime > SecondsUntilAssist))
            {
                assistCompass.gameObject.SetActive(true);
            }
        }


        float distanceRemaining = distanceAllotted - playerDistance;
        float timeRemaining = timeAllotted - (Time.time - startTime);
        // If we have a place to output ongoing trial info (time/dist remaining), use it
        if (printRemainingTimeTo != null) 
        {
            printRemainingTimeTo.text = string.Format(baseText, Mathf.Round(distanceRemaining), Mathf.Round(timeRemaining));
        }

        // End the trial if they reach the max distance allotted
        if (!isScaled & playerDistance >= distanceAllotted) return true;
        else if (isScaled & scaledPlayerDistance >= distanceAllotted) return true;
        // End the trial if they reach the max time allotted
        if (Time.time - startTime >= timeAllotted)
        {
            Debug.LogWarning("WTF.. END!");
            return true;
        }

        if (killCurrent == true)
		{
			return KillCurrent ();
		}

        // if we're letting them say when they think they've arrived
        if (Time.time - startTime > allowContinueAfter)
        {
            if (vrEnabled)
            {
                if (vrInput.TriggerButton.GetStateDown(SteamVR_Input_Sources.Any))
                {
                    Debug.Log("Participant ended the trial");
                    log.log("INPUT_EVENT    Player Arrived at Destination    1", 1);
                    hud.hudPanel.SetActive(false);
                    hud.setMessage("");

                    if (haptics) SteamVR_Actions.default_Haptic.Execute(0f, 2.0f, 65f, 1f, SteamVR_Input_Sources.Any);

                    return true;
                }
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("Participant ended the trial");
                log.log("INPUT_EVENT    Player Arrived at Destination    1", 1);
                hud.hudPanel.SetActive(false);
                hud.setMessage("");
                return true;
            }
        }

		return false;
	}

	public override void endTask()
	{
		TASK_END();
		//avatarController.handleInput = false;
	}

	public override void TASK_PAUSE()
	{
		avatarLog.navLog = false;
        if (isScaled) scaledAvatarLog.navLog = false;
		//base.endTask();
		log.log("TASK_PAUSE\t" + name + "\t" + this.GetType().Name + "\t" ,1 );
		//avatarController.stop();

		hud.setMessage("");
		hud.showScore = false;

	}

	public override void TASK_END()
	{
		base.endTask();
        if (printRemainingTimeTo != null) printRemainingTimeTo.text = baseText;
        var navTime = Time.time - startTime;

        if (logStartEnd) endXYZ = avatar.GetComponent<LM_PlayerController>().collisionObject.transform.position;

        //avatarController.stop();
        avatarLog.navLog = false;
        if (isScaled) scaledAvatarLog.navLog = false;

        // close the door if the target was a store and it is open
        // if it's a target, open the door to show it's active
        if (currentTarget.GetComponentInChildren<LM_TargetStore>() != null)
        {
            currentTarget.GetComponentInChildren<LM_TargetStore>().CloseDoor();
        }

        // re-enable everything on the gameobject we just finished finding
        manager.HideRecursive(currentTarget, true);
        foreach (var c in currentTarget.GetComponents<Collider>()) c.enabled = true;
        foreach (var child in currentTarget.GetComponentsInChildren<Transform>())
        {
            var halo = (Behaviour)child.GetComponent("Halo");
            if (halo != null) halo.enabled = true;
        }

        hud.setMessage("");
		hud.showScore = false;

        hud.SecondsToShow = hud.GeneralDuration;

        if (assistCompass != null)
        {
            // Hide the assist compass
            assistCompass.gameObject.SetActive(false);
        }
        
        // Move hud back to center and reset
        hud.hudPanel.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        hud.hudPanel.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);

        float perfDistance;
        if (isScaled)
        {
            perfDistance = scaledPlayerDistance;
        }
        else perfDistance = playerDistance;

        var excessPath = perfDistance - optimalDistance;
        
        // set impossible values if the nav task was skipped
        if (skip)
        {
            navTime = float.NaN;
            perfDistance = float.NaN;
            optimalDistance = float.NaN;
            excessPath = float.NaN;
        }


        // log.log("LM_OUTPUT\tNavigationTask.cs\t" + masterTask.name + "\t" + this.name + "\n" +
        // 	"Task\tBlock\tTrial\tTargetName\tOptimalPath\tActualPath\tExcessPath\tRouteDuration\n" +
        // 	masterTask.name + "\t" + masterTask.repeatCount + "\t" + parent.repeatCount + "\t" + currentTarget.name + "\t" + optimalDistance + "\t"+ perfDistance + "\t" + excessPath + "\t" + navTime
        //     , 1);

        // More concise LM_TrialLog logging
        if (!exploration) taskLog.AddData(transform.name + "_target", currentTarget.name);
        taskLog.AddData(transform.name + "_actualPath", perfDistance.ToString());
        if (!exploration) taskLog.AddData(transform.name + "_optimalPath", optimalDistance.ToString());
        if (!exploration) taskLog.AddData(transform.name + "_excessPath", excessPath.ToString());
        taskLog.AddData(transform.name + "_clockwiseTravel", clockwiseTravel.ToString());
        taskLog.AddData(transform.name + "_duration", navTime.ToString());

        if (logStartEnd)
        {

            taskLog.AddData(transform.name + "_startX", startXYZ.x.ToString());
            taskLog.AddData(transform.name + "_startZ", startXYZ.z.ToString());
            taskLog.AddData(transform.name + "_endX", endXYZ.x.ToString());
            taskLog.AddData(transform.name + "_endZ", endXYZ.z.ToString());

        }

        // Record any decisions made along the way
        if (decisionPoints != null)
        {
            foreach (LM_DecisionPoint nexus in decisionPoints)
            {
                taskLog.AddData(nexus.name + "_initialChoice", nexus.initialChoice);
                taskLog.AddData(nexus.name + "_finalChoice", nexus.currentChoice);
                taskLog.AddData(nexus.name + "_totalChoices", nexus.totalChoices.ToString());

                nexus.ResetDecisionPoint();
            }
        }
        
        // Hide the overlay by setting back to empty string
        //if (overlayTargetObject != null) overlayTargetObject.text = "";

        //// If we created a dummy Objectlist for exploration, destroy it
        //Destroy(GetComponent<ObjectList>());

        if (canIncrementLists) destinations.incrementCurrent();
        if (!exploration) currentTarget = destinations.currentObject();
    }

	public override bool OnControllerColliderHit(GameObject hit)
	{
		if ((hit == currentTarget | hit.transform.parent.gameObject == currentTarget) & 
            hideTargetOnStart != HideTargetOnStart.DisableCompletely & hideTargetOnStart != HideTargetOnStart.SetInactive)
		{
			if (showScoring)
			{
				score = score + scoreIncrement;
				hud.setScore(score);
			}
			return true;
		}

		return false;
	}
}

