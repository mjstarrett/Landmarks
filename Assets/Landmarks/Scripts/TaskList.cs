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
using UnityEngine.UI;


public class TaskList : ExperimentTask {
	
	public GameObject[] tasks; // no longer need to preset, shown for debugging - MJS
	public GameObject[] objectsList;

	public int repeat = 1;
	public TextList overideRepeat;
	private int repeatCount = 1;

	private int currentTaskIndex = 0;
	[HideInInspector] public ExperimentTask currentTask;


	public override void startTask() {
		// Debug.Log(this.GetType().Name);
		base.startTask();

		if (overideRepeat) {
			repeatCount = 1;
			repeat = Int32.Parse( overideRepeat.currentString().Trim() );
		}
        //Debug.Log("repeat: ");
        //Debug.Log( repeat);


        //----------------------------------------------------------------------
        // Automatically determine number of tasks based on children
        //----------------------------------------------------------------------
        tasks = new GameObject[transform.childCount];

        if (tasks.Length == 0) skip = true;
        else
        {
            for (int iTask = 0; iTask < tasks.Length; iTask++)
            {
                tasks[iTask] = transform.GetChild(iTask).gameObject;
            }

        }


        if (!skip) startNextTask();		
	}	
	public override void TASK_START () {
		repeatCount = 1;
	}	
	
	public void startNextTask() {
		Debug.Log("Starting " + tasks[currentTaskIndex].name);

		currentTask = tasks[currentTaskIndex].GetComponent<ExperimentTask>();
		currentTask.parentTask = this;
		currentTask.startTask();	
	}
	
	public override bool updateTask () {
		if (skip) return true;
		
		if ( currentTask.updateTask() ) {
			
			
			//cut
					
			if (pausedTasks) {
				//currentTask.endTask();
				//Debug.Log("pause");
				currentTask = pausedTasks;
				//endTask();
				pausedTasks.startTask();
				pausedTasks = null;		
				//return true;	
			} else {
				return endChild();
			}
		}

		return false;
	}

	public bool endChild() 
	{
		currentTask.endTask();
		currentTaskIndex = currentTaskIndex + 1;

		if (currentTaskIndex >= tasks.Length && repeatCount >= repeat) 
		{
			currentTaskIndex = 0;
			repeatCount = 1;
			return true;
		} 
		else 
		{
			if (currentTaskIndex >= tasks.Length) {
				repeatCount++;
				currentTaskIndex = 0;
			}
	 		startNextTask();	
		}

		return false;
	}
	
	
	public override void endTask() {
		base.endTask();
		
		if (overideRepeat) {
				overideRepeat.incrementCurrent();
		}
						
			//	if (pausedTasks) {
				//currentTask = pausedTasks;
				//endTask();
			//	pausedTasks.startTask();
		//if (!skip) currentTask.endTask();
	}
	
	public override bool OnControllerColliderHit(GameObject hit)  {
		if ( currentTask.OnControllerColliderHit(hit) ) {
			
			return endChild();
			
			//cut
			currentTask.endTask();
			currentTaskIndex = currentTaskIndex + 1;
			if (currentTaskIndex >= tasks.Length) {
				return true;
			} else {
		 		startNextTask();	
			}
			//
		}
		return false;
	}
	
	public string format(string str) {
		
		string[] names = new string[objectsList.Length];
		int i = 0;
		foreach( GameObject go in objectsList ) {
			names[i] = go.name;
			i++;
		}
		return string.Format(str, names);
	}
}

/*

   var enumerator = d.GetEnumerator();
    while (enumerator.MoveNext())
    {
	var pair = enumerator.Current;
	b += pair.Value;
    }
    
    */