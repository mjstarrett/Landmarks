using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoreOrder : MonoBehaviour 
{

	// SEE ALSO TransformOrder and ObjectOrder
//	public GameObject storesParent; // parent gameobject holding our target building prefabs
//
//	[HideInInspector]
	public List<GameObject> order; // accessed later (StoreList and subjsequent), but not visible
//
//	void OrderStores()
//	{
//		foreach (Transform child in storesParent.transform) 
//		{
//			order.Add (new GameObject (child.name));
//			string tmp = child.name;
//			Debug.Log ((child.name));
//		}			
//
//	}
}