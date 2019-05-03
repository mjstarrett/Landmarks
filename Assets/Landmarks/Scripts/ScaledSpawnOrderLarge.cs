using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScaledSpawnOrderLarge : MonoBehaviour 
{

//	 SEE ALSO TransformOrder and ObjectOrder

	[HideInInspector]
	public List<GameObject> scaledSpawnOrderLarge; // accessed later (StoreList and subjsequent), but not visible

	void Awake()
	{

		foreach (Transform child in this.transform) 
		{
			scaledSpawnOrderLarge.Add (GameObject.Find(child.name));
			string tmp = child.name;
			Debug.Log ((child.name));
		}			

	}
}