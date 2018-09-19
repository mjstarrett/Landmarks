using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapStudyTask : ExperimentTask {

	public override void startTask () 
	{
		TASK_START();	
	}	

	public override void TASK_START() 
	{
		if (!manager) Start();
		base.startTask();

		// Set up hud for task
		hud.hudPanel.SetActive(false); //hide the text background on HUD

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
				hud.hudPanel.SetActive (false);
			}
		}
			
		if (Input.GetKeyDown (KeyCode.N)) 
		{
			Debug.Log ("DEBUG FORCE NEXT TASK");
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

		// switch the cameras back
		firstPersonCamera.enabled = true;
		overheadCamera.enabled = false;

		// Put the hud panel back on
		hud.hudPanel.SetActive(true);

		// Let the player move again.
		avatar.GetComponent<CharacterController>().enabled = true;

		// Scale the city back up
		GameObject.FindWithTag("Environment").transform.localScale = new Vector3 (1, 1, 1);

		hud.SecondsToShow = hud.GeneralDuration;
	}
}
