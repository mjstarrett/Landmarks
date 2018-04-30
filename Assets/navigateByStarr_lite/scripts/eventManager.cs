using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
 
public class EventManager : MonoBehaviour 
{

	// create a new dictionary for events
	private Dictionary <string, UnityEvent> EventDictionary;

	// create static instance of our dictionary
	private static EventManager eventManager;

	// Grab our event manager object 
	public static EventManager instance
	{
		get
		{
			// If we don't have it, find it
			if (!eventManager) 
			{
				eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

				// if we still don't have it, print to console that we have a problem
				if (!eventManager) {
					Debug.LogError("There needs to be one active EventManager script on a GameObject in your scene.");
				}
			} 
			// if we do have it, initialize it
			else 
			{
				eventManager.Init();
			}

			return eventManager;
		}
	}

	// if our dictionary is null, create it as a new dictionary
	void Init()
	{
		if (EventDictionary == null) 
		{
			EventDictionary = new Dictionary<string, UnityEvent>();
		}
	}

	// Take an event and an action, add a listener, and add it to the dictionary
	public static void StartListening(string eventName, UnityAction listener)
	{
		UnityEvent thisEvent = null;
		if (instance.EventDictionary.TryGetValue(eventName, out thisEvent)) 
		{
			thisEvent.AddListener(listener);
		} 
		// if it doesn't exist, create the entry
		else 
		{
			thisEvent = new UnityEvent();
			thisEvent.AddListener(listener);
			instance.EventDictionary.Add(eventName, thisEvent);
		}
	}

	// find the entry, if it exists, and remove it
	public static void StopListening(string eventName, UnityAction listener)
	{
		if (eventManager == null) return;
		UnityEvent thisEvent = null;
		if (instance.EventDictionary.TryGetValue(eventName, out thisEvent)) 
		{
			thisEvent.RemoveListener(listener);
		}
	}

	// find the entry in the dictionary and invoke the event
	public static void TriggerEvent(string eventName)
	{
		UnityEvent thisEvent = null;
		if (instance.EventDictionary.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.Invoke();
		}
	}
}
