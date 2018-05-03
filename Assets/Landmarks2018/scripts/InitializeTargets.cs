using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetClass : ExperimentTask
{
	void Start()
	{
		GameObject optionsParent = GameObject.Find ("TargetOptions");
		GameObject locationsParent = GameObject.Find ("TargetLocations");
		List<NavigationTarget> targetOptions = new List<NavigationTarget> ();

		foreach (Transform child in optionsParent.transform) 
		{
			targetOptions.Add (new NavigationTarget (child.name, 0));
		}
		print (targetOptions);
	}
}
