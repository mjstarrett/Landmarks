using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeTargets : ExperimentTask
{


    [Header("Task-specific Properties")]

    public GameObject targetLocationsParent; // MJS - added 4/30/2018
	public GameObject targetOptionsParent; 	// MJS - added 4/30/2018


	new void Start()
	{
		base.Start();
		GameObject optionsParent = GameObject.Find ("TargetOptions");
		GameObject locationsParent = GameObject.Find ("TargetLocations");
		List<NavigationTarget> targetOptions = new List<NavigationTarget> ();

		foreach (Transform child in optionsParent.transform) 
		{
			targetOptions.Add (new NavigationTarget (child.name, 0));
		}
		print (targetOptions);
	}
//
//	// Landmarks-specific functions
//	public override void startTask () 
//	{
//
//		TASK_START();
//	}
//
//	public override bool updateTask () 
//	{
//		
//		return true;	
//	}
//		
//	public override void endTask() {
//
//		TASK_END();
//	}
//
//	public override void TASK_END() {
//		base.endTask();
//	}
//
//	public GameObject currentObject() {
//		if (current >= objects.Count) {
//			return null;
//			current = 0;
//		} else {
//			return objects[current];
//		}
//	}
//
//	public  void incrementCurrent() {
//		current++;
//		if (current >= objects.Count && EndListBehavior == EndListMode.Loop) {
//			current = 0;
//		}
//	}
}
