using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoreOrder : MonoBehaviour 
{

//	 SEE ALSO TransformOrder and ObjectOrder

	[HideInInspector]
	public List<GameObject> storeOrder; // accessed later (StoreList and subjsequent), but not visible

	void Awake()
	{

		foreach (Transform child in this.transform) 
		{
			storeOrder.Add (GameObject.Find(child.name));
			string tmp = child.name;
			Debug.Log ((child.name));
		}			

	}
}