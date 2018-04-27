using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class eventManager : MonoBehaviour {

	private Dictionary <string, UnityEvent> eventDictionary;

	private eventManager eventManager;

	public static eventManager instance{
		get{
		
		}
	}

}
