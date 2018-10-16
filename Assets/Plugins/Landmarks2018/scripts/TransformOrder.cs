using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TransformOrder : MonoBehaviour 
{

	// Landmarks script 'ObjectOrder.cs' modified by Michael J. Starrett - 2018

	[HideInInspector]
	public List<GameObject> order; // accessed later (transformList and subsequent)

	void Awake()
	{
		foreach (Transform child in this.transform) 
		{
			order.Add (GameObject.Find(child.name));
			string tmp = child.name;
			Debug.Log ((child.name));
		}			

	}
}