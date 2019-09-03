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
using System.Collections.Generic;
using System;
using UnityEngine.UI;


public class TaskList : ExperimentTask {

    public string skipCondition = "";
	public GameObject[] tasks; // no longer need to preset, shown for debugging and visualization - MJS
	public GameObject[] objectsList;

	public int repeat = 1;
	public TextList overideRepeat;

    public int catchTrialCount = 0;
    public List<GameObject> skipOnCatch; // which task-components are we skipping on catch trials
    public List<GameObject> onlyOnCatch; // which task-components are we adding on catch trials
    public bool noCatchOnFirstTrial = true;
    [HideInInspector] public List<int> catchTrials; // list of catch trials
    [HideInInspector] public bool catchFlag = false;


    [HideInInspector] public int repeatCount = 1;

	private int currentTaskIndex = 0;
	[HideInInspector] public ExperimentTask currentTask;


	public override void startTask() {
        // Debug.Log(this.GetType().Name);

        TASK_START();


        if (!skip) startNextTask();		
	}	

	public override void TASK_START () {
		repeatCount = 1;

        base.startTask();

        if (skipCondition == manager.config.condition)
        {
            skip = true;
        }

        if (overideRepeat)
        {
            repeatCount = 1;
            repeat = Int32.Parse(overideRepeat.currentString().Trim());
        }

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

        //----------------------------------------------------------------------
        // create list of random trials to flag as catch trials
        //----------------------------------------------------------------------

        if (catchTrialCount > 0)
        {

            // Create a list of our trial numbers
            int[] catchCandidates;

            // if trial #1 can't be a catch trial
            if (noCatchOnFirstTrial)
            {
                catchCandidates = new int[repeat - 1];

                for (int i = 0; i < repeat - 1; i++) catchCandidates[i] = i + 2; // adjust because repeat count uses base-1 and to ignore trial 1
            }
            // Otherwise include all trials in our list
            else
            {
                catchCandidates = new int[repeat];
                for (int i = 0; i < repeat; i++) catchCandidates[i] = i + 1; // adjust because repeat count uses base-1
            }

            // Shuffle this list and use it to pick our catch trials
            Experiment.Shuffle(catchCandidates);

            // Pick our catch trials randomly from the shuffled trial order until
            // we have the specified number of catch trials
            catchTrials = new List<int>(); // must reset each time or we can get additive catch trials across blocks
            for (int i = 0; i < catchTrialCount; i++)
            {
                Debug.Log(i);
                Debug.Log(catchTrialCount);
                catchTrials.Add(catchCandidates[i]);
            }
            Debug.Log("HERE ARE OUR CATCH TRIALS (" + catchTrials.Count + ")");
            foreach (int item in catchTrials)
            {
                Debug.Log(item);
            }
        }
    }	
	
	public void startNextTask() {
		Debug.Log("Starting " + tasks[currentTaskIndex].name);

        if (currentTaskIndex == 0)
        {
            startNewRepeat();
        }

		currentTask = tasks[currentTaskIndex].GetComponent<ExperimentTask>();

        currentTask.parentTask = this;

        currentTask.startTask();
	}

    public void startNewRepeat()
    {
        // Mark if this repetition is a catch trial/block etc.
        if (catchTrials.Contains(repeatCount)) catchFlag = true;
        else catchFlag = false;


        // Send Block info to our log file at the start of each new block
        log.log("LM_Output\tTaskList\n" +
                "TaskListName\tRepetitionNumber\tCatchFlag\n" +
                this.name + repeatCount.ToString() + catchFlag.ToString()
                , 1);


        // Turn on/off items we are skipping/adding on catch trials
        foreach (GameObject item in tasks)
        {
            // Set the proper skip bool for tasks that are skipped on catch trials
            if (skipOnCatch.Contains(item))
            {
                item.GetComponent<ExperimentTask>().skip = catchFlag;
            }
            // Set the proper skip bool for tasks that are only on catch trials
            if (onlyOnCatch.Contains(item))
            {
                item.GetComponent<ExperimentTask>().skip = !catchFlag;
            }
        }
    }

	
	public override bool updateTask () {
		if (skip) return true;

        if (currentTask.updateTask())
        {


            //cut

            if (pausedTasks)
            {
                //currentTask.endTask();
                //Debug.Log("pause");
                currentTask = pausedTasks;
                //endTask();
                pausedTasks.startTask();
                pausedTasks = null;
                //return true;	
            }
            else
            {
                return endChild();
            }
        }

        return false;
	}

	public bool endChild() 
	{
        currentTask.endTask();
		currentTaskIndex++;


        // If we've finished all the tasks in all the cycles (repeats), end this tasklist
        if (currentTaskIndex >= tasks.Length && repeatCount >= repeat)
        {
            currentTaskIndex = 0;
            repeatCount = 1;
            return true;
        }
        else
        {
            // If we've reached the last task but have cycles (repeats) left -- reset task index, increment repeatcount and run startNextTask()
            if (currentTaskIndex >= tasks.Length)
            {
                repeatCount++; // increment the repeat count (i.e., update block/trial/repeat number)
                currentTaskIndex = 0; // reset the task index so the next task that starts is the first in the list
            }

            // Start the next task in the list
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