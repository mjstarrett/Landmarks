using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectOrder_MJS : MonoBehaviour {

	// Landmarks script 'ObjectOrder.cs' modified by Michael J. Starrett - 2018
	private List<GameObject> order = new List<GameObject> ();

	void awake()
	{
		Debug.Log ("Object order, y'all!");
		foreach (Transform child in transform) 
		{
			order.Add (new GameObject (child.name));
			print (child.name);
		}
	}
}
