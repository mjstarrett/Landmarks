using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreMapTest : ExperimentTask {

	public GameObject copiedStoresParent;
	//public GameObject copiedPlaceholderParent;

	public override void startTask () 
	{
		TASK_START();
	}

	public override void TASK_START ()
	{
		base.startTask ();


		// Destroy the copies we created when initializing the map test task
		foreach (Transform child in copiedStoresParent.transform) 
		{
			Destroy (child.gameObject);
		}
		//		foreach (Transform child in copiedPlaceholderParent.transform) 
		//		{
		//			Destroy (child.gameObject);
		//		}

		// make the originals from our copies visible again
		copiedStoresParent.GetComponent<CopyChildObjects>().sourcesParent.SetActive(true);
		//copiedPlaceholderParent.GetComponent<CopyChildObjects> ().parentObject.SetActive (true);
	}

	public override bool updateTask ()
	{
		return true;
	}

	public override void endTask() {
		TASK_END();
	}

	public override void TASK_END() {
		base.endTask();
	}
}
