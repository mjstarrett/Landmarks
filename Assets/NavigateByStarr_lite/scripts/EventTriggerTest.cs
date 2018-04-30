using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTriggerTest : MonoBehaviour 
{

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("q")) 
		{
			EventManager.TriggerEvent("test");
		}
	}
}
