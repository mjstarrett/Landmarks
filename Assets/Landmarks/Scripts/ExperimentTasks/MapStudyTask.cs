using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapStudyTask : ExperimentTask {

    [Header("Task-specific Properties")]

    // allow for user input to shift the store labels during the map task (to allow viewing store and text clearly); 
    public bool flattenMap = true;
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

		if (flattenMap) {
			// Flatten out environment buildings so stores are clearly visible
			GameObject.FindWithTag ("Environment").transform.localScale = new Vector3 (1, 0.01F, 1);
		}



		// Change text and turn on the map action button
		actionButton.GetComponentInChildren<Text> ().text = "Start Test";
		hud.actionButton.SetActive(true);
	    hud.actionButton.GetComponent<Button>().onClick.AddListener (hud.OnActionClick);
	}	


	public override bool updateTask ()
	{
		base.updateTask ();

		//empty RaycastHit object which raycast puts the hit details into
		RaycastHit hit;
		//ray shooting out of the camera from where the mouse is
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);


		if (Physics.Raycast (ray, out hit)) {// & Input.GetMouseButtonDown(0)) 
			if (hit.transform.CompareTag ("Target")) {
				//Debug.Log (objectHit.tag);
				hud.setMessage (hit.transform.name);
				hud.hudPanel.SetActive (true);
				hud.ForceShowMessage ();

                // move hud text to the store being highlighted (coroutine to prevent Update framerate jitter)
                // jitterGuardOn is inherited from Experiment task so it can be used in multiple task scripts (e.g., MapStudy and MapTest) - MJS 2019
                if (!jitterGuardOn)
                {
                    hud.hudPanel.transform.position = Camera.main.WorldToScreenPoint(hit.transform.position + hudTextOffset);
                    StartCoroutine(HudJitterReduction());
                }

                log.log("Mouseover \t" + hit.transform.name, 1);
			} else {
				hud.setMessage ("");
				hud.hudPanel.SetActive (true);
				hud.ForceShowMessage ();

				// move hud off screen if we aren't hitting a target shop
				hud.hudPanel.transform.position = new Vector3(99999,99999,99999);
			}
		} else {
			hud.setMessage ("");
			hud.hudPanel.SetActive (true);
			hud.ForceShowMessage ();

			// move hud off screen if we aren't hitting a target shop
			hud.hudPanel.transform.position = new Vector3(99999,99999,99999);
		}
			
		if (killCurrent == true) 
		{
			return KillCurrent ();
		}

		if (hud.actionButtonClicked == true) 
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

		if (flattenMap){
			// un-Flatten out environment buildings so stores are clearly visible
			GameObject.FindWithTag("Environment").transform.localScale = new Vector3 (1, 1, 1);
		}

        // turn off the map action button
        hud.actionButton.GetComponent<Button>().onClick.RemoveListener(hud.OnActionClick);
        actionButton.GetComponentInChildren<Text>().text = actionButton.GetComponent<DefaultText>().defaultText;
        hud.actionButton.SetActive(false);
	}
}


