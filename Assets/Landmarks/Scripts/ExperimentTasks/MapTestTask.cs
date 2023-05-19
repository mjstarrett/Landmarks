using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapTestTask : ExperimentTask {

    [Header("Task-specific Properties")]

    public ObjectList targetList;
	[Tooltip("In seconds; 0 = unlimited time")]
	public int timeLimit = 0;
	public bool flattenMap = true;
	//public bool highlightAssist = false; // MJS - Removing Target Highlights for ease of use (requires additional environment configuration)
	//public GameObject mapTestHighlights; // MJS - Removing Target Highlights for ease of use (requires additional environment configuration)
    public float snapToTargetProximity = 0.0f; // leave at 0.0f to have snapping off. Otherwise this will be the straight line distance within a target users must be to snap object to target position/location
	[TextArea]
	public string buttonText = "Get Score";

	private GameObject activeTarget; // This is the container we will use for whichever object is currently being clicked and dragged
	private bool targetActive = false; // Are we currently manipulating a targetobject?
	private Vector3 previousTargetPos; // save the position when a target was clicked so users can undo the current move
	private Vector3 previousTargetRot; // save the rotation when a target was clicked so users can undo the current rotate 
	// allow for user input to shift the store labels during the map task (to allow viewing store and text clearly); 
	public Vector3 hudTextOffset; // Text will be centered over an object. This allows users to move that to a desireable distance in order to keep the object visible when the name is shown

	private Vector3 baselineScaling;

	private float startTime;
	private float taskDuration;

	public override void startTask () 
	{
		TASK_START();	
		avatarLog.navLog = false;	
	}	


	public override void TASK_START() 
	{
		if (!manager) Start();
		base.startTask();

		startTime = Time.time;

		// Modify the HUD display for the map task
		hud.setMessage("");
		hud.hudPanel.SetActive (true); // temporarily turn off the hud panel at task start (no empty message window)
		hud.ForceShowMessage();
		// move hud off screen if we aren't hitting a target shop
		hud.hudPanel.transform.position = new Vector3(99999,99999,99999);
	
		// make the cursor functional and visible
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		// Turn off Player movement
		avatar.GetComponent<CharacterController>().enabled = false;

		// Swap from 1st-person camera to overhead view
		firstPersonCamera.enabled = false;
		overheadCamera.enabled = true;

		// Remove environment topography so tall things don't get in the way of dragging objects
		if (flattenMap)
		{
			baselineScaling = GameObject.FindWithTag("Environment").transform.localScale;
			// Flatten out environment buildings so stores are clearly visible
			GameObject.FindWithTag("Environment").transform.localScale = new Vector3(baselineScaling.x, 0.01F, baselineScaling.z);
			//Flatten out the copied target stores
			GameObject.Find("CopyObjects").transform.localScale = new Vector3(baselineScaling.x, 0.01f, baselineScaling.z);
		}

		// Change text and turn on the map action button
		actionButton.GetComponentInChildren<Text> ().text = buttonText;
		hud.actionButton.SetActive(true);
        hud.actionButton.GetComponent<Button>().onClick.AddListener(hud.OnActionClick);

	}	


	public override bool updateTask ()
	{
		base.updateTask ();

		taskDuration = Time.time - startTime;

		// If we're using a time limit; end the task when time is up
		if (timeLimit > 0 & taskDuration > timeLimit)
		{
			return true;
		}

		// ------------------------------------------------------------
		// Handle mouse input for hovering over and selecting objects
		// ------------------------------------------------------------

		// create a plane for our raycaster to hit
		// Note: make y high enough that store is visible over other envir. buildings
		Plane plane=new Plane(Vector3.up,new Vector3(0, 0.2f, 0));

		//empty RaycastHit object which raycast puts the hit details into
		RaycastHit hit;

		//ray shooting out of the camera from where the mouse is
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		// Register when our raycaster is hitting a gameobject...
		if (Physics.Raycast (ray, out hit)) 
		{
			// ... but only if that game object is one of our target stores ...
			if (hit.transform.CompareTag("Target"))
			{
				hud.setMessage(hit.transform.name);
				hud.hudPanel.SetActive(true);
				hud.ForceShowMessage();

				// move hud text to the store being highlighted (coroutine to prevent Update framerate jitter)
				// jitterGuardOn is inherited from Experiment task so it can be used in multiple task scripts (e.g., MapStudy and MapTest) - MJS 2019
				if (!jitterGuardOn)
				{
					hud.hudPanel.transform.position = Camera.main.WorldToScreenPoint(hit.transform.position + hudTextOffset);
					StartCoroutine(HudJitterReduction());
				}

				log.log("Mouseover \t" + hit.transform.name, 1);


				// BEHAVIOR Click Store to make it follow mouse
				if (Input.GetMouseButtonDown(0))
				{
					// Container for active store
					targetActive = true;
					activeTarget = hit.transform.gameObject;
					// Record previos position so the current move can be cancelled
					previousTargetPos = activeTarget.transform.position;
					previousTargetRot = activeTarget.transform.eulerAngles;
				}
			}
            else if (hit.transform.parent.transform.CompareTag("Target"))
            {
				hud.setMessage(hit.transform.parent.transform.name);
				hud.hudPanel.SetActive(true);
				hud.ForceShowMessage();

				// move hud text to the store being highlighted (coroutine to prevent Update framerate jitter)
				// jitterGuardOn is inherited from Experiment task so it can be used in multiple task scripts (e.g., MapStudy and MapTest) - MJS 2019
				if (!jitterGuardOn)
				{
					hud.hudPanel.transform.position = Camera.main.WorldToScreenPoint(hit.transform.parent.transform.position + hudTextOffset);
					StartCoroutine(HudJitterReduction());
				}

				log.log("Mouseover \t" + hit.transform.parent.transform.name, 1);

				// BEHAVIOR Click Store to make it follow mouse
				if (Input.GetMouseButtonDown(0))
				{
					// Container for active store
					targetActive = true;
					activeTarget = hit.transform.parent.transform.gameObject;
					// Record previos position so the current move can be cancelled
					previousTargetPos = activeTarget.transform.position;
					previousTargetRot = activeTarget.transform.eulerAngles;
				}
			}
			// ... Otherwise, clear the message and hide the gui 
			else
			{
				HideStoreName();
			}
		} 
		else 
		{
			HideStoreName ();
		}

		// -----------------------------------------
		// Manipulate the currently active store
		// -----------------------------------------

		if (targetActive) {

			// BEHAVIOR: active target (e.g. follow mouse, hide store name)
			float distance;
			if (plane.Raycast (ray, out distance)) {
				activeTarget.transform.position = ray.GetPoint (distance);
			}
			HideStoreName ();

            log.log("Moving :\t" + activeTarget.name +
                    "\tPosition (xyz): \t" + activeTarget.transform.position.x + "\t" + activeTarget.transform.position.y + "\t" + activeTarget.transform.position.z +
                    "\tRotation (xyz): \t" + activeTarget.transform.eulerAngles.x + "\t" + activeTarget.transform.eulerAngles.y + "\t" + activeTarget.transform.eulerAngles.z
                    , 1);

            // BEHAVIOR: left click released (e.g., drop the store where it is)
            if (Input.GetMouseButtonUp(0)){
                // Get position/rotation of nearest target location (if snapping assist is turned on)
                if (snapToTargetProximity > 0.0f)
                {
                    // determine which target is closest (see helper function at the end of this script)

                    Transform tMin = null; // initialize a container for the winner's transform and make it null to start
                    float minDist = Mathf.Infinity; // initialize a container for the current winning distance and set it to infinity to start
                    Vector3 currentPos = activeTarget.transform.position; // get the position of the object we're comparing 
                    foreach (Transform child in targetList.parentObject.transform)
                    {
                        float dist = vector2DDistance(child.position, currentPos); // get the distance between this child and the activeTarget we're comparing
                        if (dist < minDist) // if this is lower than the initial value of infinity or the current winner
                        {
                            tMin = child; // save this child's transform as the current winner
                            minDist = dist; // save the distance from the activeTarget to this child as the winning distance
                        }
                    }

                    if (minDist <= snapToTargetProximity)
                    {
                        activeTarget.transform.position = tMin.position;
                        activeTarget.transform.eulerAngles = tMin.eulerAngles;
                    }
                }

                targetActive = false;
                activeTarget = null;


            }
            // BEHAVIOR: Undo current move
            // INPUT NAME: Cancel
            else if (Input.GetButtonDown("Cancel")) {
				activeTarget.transform.position = previousTargetPos;
				activeTarget.transform.eulerAngles = previousTargetRot;
				targetActive = false;
				activeTarget = null;
			}

			// BEHAVIOR: Rotate active drag object 90 degrees CCW
			// INPUT NAME: MapTest_RotateCCW
			if (Input.GetButtonDown("MapTest_RotateCCW")) activeTarget.transform.Rotate(0.0f, -90.0f, 0.0f, Space.World);

			// BEHAVIOR: Rotate active drag object 90 degrees CW
			// INPUT NAME: MapTest_RotateCW
			if (Input.GetButtonDown("MapTest_RotateCW")) activeTarget.transform.Rotate(0.0f, 90.0f, 0.0f, Space.World);
        }

		// -----------------------------------------
		// Handle debug button behavior (kill task)
		// -----------------------------------------
		if (killCurrent == true) 
		{
			return KillCurrent ();
		}

		// -----------------------------------------
		// Handle action button behavior
		// -----------------------------------------
		if (Input.GetButtonDown("Return") | Input.GetKeyDown(KeyCode.Return))
		{
			return true;
		}
		else if (hud.actionButtonClicked == true)
		{
			hud.actionButtonClicked = false;
			return true;
		}

		return false;
	}
		

	public override void endTask() 
	{
		TASK_END();
	}


	public override void TASK_END() 
	{
		base.endTask();

		// log data
		// Log data
		taskLog.AddData(transform.name + "_testTime", taskDuration.ToString());

		// Set up hud for other tasks
		hud.hudPanel.SetActive(true); //hide the text background on HUD
		// Change the anchor points to put the message back in center
		RectTransform hudposition = hud.hudPanel.GetComponent<RectTransform>() as RectTransform;
		hudposition.anchorMin = new Vector2 (0, 0);
		hudposition.anchorMax = new Vector2 (1, 1);
		hudposition.pivot = new Vector2 (0.5f, 0.5f);
		hudposition.anchoredPosition3D= new Vector3 (0, 0, 0);
		//hud.hudPanel.GetComponent<RectTransform> = hudposition;


		// make the cursor invisible
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = false;

		// Turn on Player movement
		avatar.GetComponent<CharacterController>().enabled = true;

		// Swap from overhead camera to first-person view
		firstPersonCamera.enabled = true;
		overheadCamera.enabled = false;

		if (flattenMap) 
		{
			// un-Flatten out environment buildings so stores are clearly visible
			GameObject.FindWithTag ("Environment").transform.localScale = new Vector3 (1, 1, 1);
			// un-Flatten out the copied target stores
			GameObject.Find ("CopyObjects").transform.localScale = new Vector3 (1, 1, 1);

            // MJS - Removing Target Highlights for ease of use (requires additional environment configuration)
   //         // Return the highlights to their unflattened position
   //         if (highlightAssist) {
			//	Vector3 tmp = mapTestHighlights.transform.localPosition;
			//	tmp.y = mapTestHighlights.transform.localPosition.y - 10;
			//	mapTestHighlights.transform.localPosition = tmp;
			//}
		}

		// turn off the map action button
		hud.actionButton.GetComponent<Button>().onClick.RemoveListener (hud.OnActionClick);
        actionButton.GetComponentInChildren<Text>().text = actionButton.GetComponent<DefaultText>().defaultText;
        hud.actionButton.SetActive(false);

        // MJS - Removing Target Highlights for ease of use (requires additional environment configuration)
        //// Turn off the maptarget highlights (to show where stores should be located
        //if (highlightAssist == true) 
        //{
        //	mapTestHighlights.SetActive (false);
        //}
    }

    void HideStoreName()
	{
		hud.setMessage ("");
		hud.hudPanel.SetActive (true);
		hud.ForceShowMessage ();
		// move hud off screen if we aren't hitting a target shop
		hud.hudPanel.transform.position = new Vector3(99999,99999,99999);
	}

    // Calculate the planar distance between placement and targets (i.e., ignore the y-axis height of the copies)
    private float vector2DDistance(Vector3 v1, Vector3 v2)
    {
        return (Mathf.Sqrt(Mathf.Pow(Mathf.Abs(v1.x - v2.x), 2f) + Mathf.Pow(Mathf.Abs(v1.z - v2.z), 2f)));
    }

}