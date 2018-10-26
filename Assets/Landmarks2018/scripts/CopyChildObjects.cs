using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyChildObjects : ExperimentTask {

	public GameObject sourcesParent;
	public GameObject destinationsParent;

	public bool setOriginalInactive = true;

	private string copiedParent;

	public override void startTask () 
	{
		TASK_START();

		//move the copy destination parent to the same place as the sourcesParent to be copied
		this.transform.position = destinationsParent.transform.position;
		this.transform.rotation = destinationsParent.transform.rotation;

		if (sourcesParent.transform.childCount != destinationsParent.transform.childCount) {
			Debug.Log ("NUMBER OF MapTestPlaceholders AND TARGETS DO NOT MATCH! CHECK YOUR ENVIRONMENT!!!!");
		}

		// instantiate a copy of each target store into the game object this script is attached to
		for (int i = 0; i < sourcesParent.transform.childCount; i++)
		{
			GameObject sourceChild = sourcesParent.transform.GetChild(i).gameObject;
			GameObject destinationChild = destinationsParent.transform.GetChild (i).gameObject;

			GameObject copy = Instantiate<GameObject> (sourceChild, destinationChild.transform.position, destinationChild.transform.rotation, this.transform);
			copy.name = sourceChild.name;
		}

		if (setOriginalInactive = true) {
			sourcesParent.SetActive (false);
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
