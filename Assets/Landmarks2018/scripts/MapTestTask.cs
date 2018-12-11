using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapTestTask : ExperimentTask {

	public GameObject mapTestLocations;
	public bool highlightAssist = false;
	public GameObject mapTestHighlights;
	public bool snapToTargetAssist = false;
	private GameObject activeTarget;
	private bool targetActive = false;
	private Vector3 previousTargetPos;
	// Allow adjsutment of the score required to continue advance the experiment (0%-100%)
	[Range(0,100)] public int CriterionPercentage = 100;
	// allow for user input to shift the store labels during the map task (to allow viewing store and text clearly); 
	public Vector3 hudTextOffset;


	public override void startTask () 
	{
		TASK_START();	
		avatarLog.navLog = false;	


	}	


	public override void TASK_START() 
	{
		if (!manager) Start();
		base.startTask();

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

		// Flatten out environment buildings so stores are clearly visible
		GameObject.FindWithTag("Environment").transform.localScale = new Vector3 (1, 0.01F, 1);
		//Flatten out the copied target stores
		GameObject.Find("CopyStores").transform.localScale = new Vector3 (1, 0.01f, 1);

		// Change text and turn on the map action button
		actionButton.GetComponentInChildren<Text> ().text = "Get Score";
		manager.actionButton.SetActive(true);
		actionButton.onClick.AddListener (OnActionClick);


		// Turn on the maptarget highlights (to show where stores should be located
		if (highlightAssist == true) 
		{
			mapTestHighlights.SetActive (true);
		}

	}	


	public override bool updateTask ()
	{
		base.updateTask ();

		// -----------------------------------------
		// Handle mouse input for dragging targets
		// -----------------------------------------

		// create a plane for our raycaster to hit
		Plane plane=new Plane(Vector3.up,new Vector3(0, 0, 0));

		//empty RaycastHit object which raycast puts the hit details into
		RaycastHit hit;

		//ray shooting out of the camera from where the mouse is
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		// Register when our raycaster is hitting a gameobject...
		if (Physics.Raycast (ray, out hit)) 
		{
			// ... but only if that game object is one of our target stores ...
			if (hit.transform.CompareTag ("store")) 
			{
				hud.setMessage (hit.transform.name);
				hud.hudPanel.SetActive (true);
				hud.ForceShowMessage ();
				// move hud text to the store being highlighted
				hud.hudPanel.transform.position = Camera.main.WorldToScreenPoint (hit.transform.position + hudTextOffset);

				// BEHAVIOR Click Store to make it follow mouse
				if (Input.GetMouseButtonDown (0)) {
					// Container for active store
					targetActive = true;
					activeTarget = hit.transform.gameObject;
					previousTargetPos = activeTarget.transform.position;
				}
			} 
			// ... Otherwise, clear the message and hide the gui 
			else 
			{
				HideStoreName ();
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

			// BEHAVIOR: left click released (e.g., drop the store where it is)
			if (Input.GetMouseButtonUp (0)) {
				targetActive = false;
				activeTarget = null;

				// Get position/rotation of nearest target location (if snapping assist is turned on)
				if (snapToTargetAssist) {

				}
			}
			// BEHAVIOR: Escape key pressed (e.g., undo current move)
			else if (Input.GetKeyDown (KeyCode.Escape)) {
				activeTarget.transform.position = previousTargetPos;
				targetActive = false;
				activeTarget = null;
			}
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
		if (actionButtonClicked == true) 
		{
			actionButtonClicked = false;
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

		// un-Flatten out environment buildings so stores are clearly visible
		GameObject.FindWithTag("Environment").transform.localScale = new Vector3 (1, 1, 1);
		// un-Flatten out the copied target stores
		GameObject.Find("CopyStores").transform.localScale = new Vector3 (1, 1, 1);

		// turn off the map action button
		actionButton.onClick.RemoveListener (OnActionClick);
		manager.actionButton.SetActive(false);

		// Turn off the maptarget highlights (to show where stores should be located
		if (highlightAssist == true) 
		{
			mapTestHighlights.SetActive (false);
		}
	}

	void HideStoreName()
	{
		hud.setMessage ("");
		hud.hudPanel.SetActive (true);
		hud.ForceShowMessage ();
		// move hud off screen if we aren't hitting a target shop
		hud.hudPanel.transform.position = new Vector3(99999,99999,99999);
	}
}


