using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TransformOrder : MonoBehaviour 
{

	// Landmarks script 'ObjectOrder.cs' modified by Michael J. Starrett - 2018
	public GameObject transformParent; // parent gameobject holding our target building prefabs

	[HideInInspector]
	public List<GameObject> order; // accessed later (transformList and subjsequent), but not visible

	void OrderTransforms()
	{
		foreach (Transform child in transformParent.transform) 
		{
			order.Add (new GameObject (child.name));
			string tmp = child.name;
			Debug.Log ((child.name));
		}			

	}
}