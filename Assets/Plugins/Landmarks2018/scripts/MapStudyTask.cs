using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapStudyTask : ExperimentTask {

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
		// Change the anchor points to put the message in the botton left
		hud.hudPanel.GetComponent<RectTransform>().anchorMin = new Vector2 (0, 0);
		hud.hudPanel.GetComponent<RectTransform>().anchorMax = new Vector2 (0, 0);
		hud.hudPanel.GetComponent<RectTransform>().pivot = new Vector2 (0, 0);
		hud.hudPanel.GetComponent<RectTransform>().anchoredPosition3D = new Vector3 (0, 0, 0);
	
		// make the cursor functional and visible
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		// Turn off Player movement
		avatar.GetComponent<CharacterController>().enabled = false;

		// Swap from 1st-person camera to overhead view
		firstPersonCamera.enabled = false;
		overheadCamera.enabled = true;

		// Flatten out environment buildings so stores are clearly visible
		GameObject.FindWithTag("Environment").transform.localScale = new Vector3 (1, 0.1F, 1);



		// turn on the map action button
		manager.actionButton.SetActive(true);
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
				hud.setMessage (hit.transform.parent.name);
				hud.hudPanel.SetActive (true);
				hud.ForceShowMessage ();
			} else {
				hud.setMessage ("");
				hud.hudPanel.SetActive (true);
				hud.ForceShowMessage ();
			}
		} else {
			hud.setMessage ("");
			hud.hudPanel.SetActive (true);
			hud.ForceShowMessage ();
		}
			
		if (killCurrent == true) 
		{
			return KillCurrent ();
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

		// turn off the map action button
		manager.actionButton.SetActive(false);
	}
}


