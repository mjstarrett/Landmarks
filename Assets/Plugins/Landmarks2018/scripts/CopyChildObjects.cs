using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyChildObjects : ExperimentTask {

	public GameObject parentObject;
	public bool setOriginalInactive = true;

	private string copiedParent;

	public override void startTask () 
	{
		TASK_START();

		//move the copy destination parent to the same place as the parentObject to be copied
		this.transform.position = parentObject.transform.position;
		this.transform.rotation = parentObject.transform.rotation;

		// instantiate a copy of each target store into the game object this script is attached to
		foreach (Transform child in parentObject.transform) {
			GameObject copy = Instantiate<GameObject> (child.gameObject, child.transform.position, child.transform.rotation, this.transform);
			copy.name = child.gameObject.name;
		}

		if (setOriginalInactive = true) {
			parentObject.SetActive (false);
		}
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
