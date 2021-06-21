using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyChildObjects : ExperimentTask {

    [Header("Task-specific Properties")]

    public ObjectList sourcesParent;
	public ObjectList destinationsParent;

	public bool setOriginalInactive = true;
    public bool randomlyRotateCopy = false;

	private string copiedParent;

	public override void startTask ()
	{
		TASK_START ();
	}

	public override void TASK_START () 
	{
		base.startTask ();

		//move the copy destination parent to the same place as the sourcesParent to be copied
		this.transform.position = destinationsParent.gameObject.transform.position;
		this.transform.rotation = destinationsParent.gameObject.transform.rotation;

		if (destinationsParent.objects.Count < sourcesParent.objects.Count) {
			Debug.Log ("The number of MapTestPlaceholders must be equal to or greater than the number of TargetObjects in LM_Environment!!!!!");
		}

		for (int i = 0; i < sourcesParent.objects.Count; i++)
		{
            GameObject sourceChild = sourcesParent.objects[i];
            GameObject destinationChild = destinationsParent.objects[i];

			GameObject copy = Instantiate<GameObject> (sourceChild, destinationChild.transform.position, sourceChild.transform.rotation, this.transform);

            if (randomlyRotateCopy)
            {
                // Randomly rotate the copied object 0, 90, 180, or 270 degrees
                List<float> rotateOptions = new List<float> { 0.0f, 90.0f, 180.0f, 270.0f };
                copy.transform.localEulerAngles = new Vector3(copy.transform.localEulerAngles.x, rotateOptions[Random.Range(0, rotateOptions.Count)], copy.transform.localEulerAngles.z);
            }
			else
            {
				// set local rotation to zero
				copy.transform.localEulerAngles = new Vector3(copy.transform.localEulerAngles.x, 0f, copy.transform.localEulerAngles.z);
			}
           
            copy.name = sourceChild.name;
		}

		if (setOriginalInactive == true) {
			sourcesParent.parentObject.SetActive (false);
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
