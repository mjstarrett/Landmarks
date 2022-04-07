/*
    Copyright (C) 2010  Jason Laczko

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


public class ObjectList : ExperimentTask {

    [Header("Task-specific Properties")]

    public string parentName = "";
	public GameObject parentObject;
	public int current = 0;
	
	public List<GameObject> objects;
	public EndListMode EndListBehavior; 
	public bool shuffle;
	public GameObject order;
    	
	public override void startTask () {
        //ViewObject.startObjects.current = 0;
        //current = 0;

		// Temporary container to hold the items we decide to use
		var objs = new List<GameObject>();

        // If either parentObject or parentName are not empty
        if (parentObject != null || parentName != "") 
        {
			// predefined parentObject takes precedent; if empty parent name is used
            if (parentObject == null ) parentObject = GameObject.Find(parentName);		

			foreach (Transform child in parentObject.transform) objs.Add(child.gameObject);
        }
		// If both parentObject and parentName are empty, check if the user provided objects manually in inspector
		// Note parentobject and parentName take precedent and will otherwise erase anything manually added
		else if (objects.Count > 0) objs = objects;
        // At this point, if we have nothing, we need to throw an error
		else Debug.LogError("A value for either parentObject, parentName, or any number of objects are required on this component.");

		// Handle how the user want's the list (re)ordered
		if (order & !shuffle) {
			// Deal with specific ordering
			ObjectOrder ordered = order.GetComponent("ObjectOrder") as ObjectOrder;
		
			if (ordered) {
				Debug.Log("ordered");
				Debug.Log(ordered.order.Count);
				// MJS - Note to self - Object order should be deprecated;
				// Users should be able to just reorder the targetObjects and not check 'Shuffled'
				if (ordered.order.Count > 0) {
					objs = ordered.order;
				}
			}
		}
		else if (shuffle & !order) {
			Experiment.Shuffle(objs.ToArray());			
		}
		
		TASK_START();
		
		
			 
		foreach (GameObject obj in objs) {	             
        	objects.Add(obj);
			log.log("TASK_ADD	" + name  + "\t" + this.GetType().Name + "\t" + obj.name  + "\t" + "null",1 );
		}
		
	}	
	
	public override void TASK_ADD(GameObject go, string txt) {
		objects.Add(go);
	}
	
	public override void TASK_START()
	{
		base.startTask();		
		if (!manager) Start();

		objects = new List<GameObject>();
	}
	
	public override bool updateTask () {
	    return true;
	}
	public override void endTask() {
		//current = 0;
		TASK_END();
	}
	
	public override void TASK_END() {
		base.endTask();
	}
	
	public GameObject currentObject() {
		if (current >= objects.Count) {
			return null;
		} else {
			return objects[current];
		}
	}
	
	public new void incrementCurrent() {
		current++;
		if (current >= objects.Count && EndListBehavior == EndListMode.Loop) {
			current = 0;
		}
	}
}
