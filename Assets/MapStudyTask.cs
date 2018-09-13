using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapStudyTask : ExperimentTask {

	private GameObject tmp;

	public override void startTask () 
	{
		TASK_START();	
	}	

	public override void TASK_START() 
	{
		if (!manager) Start();
		base.startTask();

		hud.showEverything();
		hud.setMessage ("This is the framework for a map-learning task.");

		// make the cursor functional and visible
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		// Turn off Player movement
		avatar.GetComponent<CharacterController>().enabled = false;

		// Swap from 1st-person camera to overhead view
		firstPersonCamera.enabled = false;
		overheadCamera.enabled = true;

	}	


	public override bool updateTask () 
	{
		base.updateTask();

		if (Input.GetKeyDown (KeyCode.N)) {
			Debug.Log ("somebody's tring to escape!");
			return parentTask.endChild ();
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

		// Let the player move again.
		avatar.GetComponent<CharacterController>().enabled = true;
	}
}
