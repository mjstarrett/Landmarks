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

		hud.showEverything();
		hud.setMessage ("This is the framework for a map-learning task.");
		Cursor.visible = true;

		// Turn off Player movement


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

		firstPersonCamera.enabled = true;
		overheadCamera.enabled = false;
	}
}
