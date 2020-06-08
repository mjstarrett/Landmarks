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



public class MoveObjects : ExperimentTask {

    [Header("Task-specific Properties")]

    public ObjectList sources;
	private GameObject source;

	private GameObject destination;
	public ObjectList destinations;
    public bool hideDestinations = true;
	
	public bool swap;

    public bool ignoreYPos = false;
    public bool ignoreRotation = false;

	private static Vector3 position;
	private static Quaternion rotation;
    private List<GameObject> usedTargets = new List<GameObject>();
    private List<GameObject> unusedTargets = new List<GameObject>();
    private GameObject spent; // container for parent object of UsedObjects
    private GameObject stored; // container for parent object of UnusedObjects

	public override void startTask () {
		TASK_START();
	}	

	public override void TASK_START()
	{
		base.startTask();


		if (!manager) Start();

		
		if (skip) {
			log.log("INFO	skip task	" + name,1 );
			return;
		}


        // ---------------------------------------------------------------------
        // Store a list of Used and Unused Targets to use in subsequent scenes
        // ---------------------------------------------------------------------

        for (int i = 0; i < sources.objects.Count; i++)
        {
            if (i < destinations.objects.Count)
            {
                // What got used?
                usedTargets.Add(sources.objects[i]);
            }
            else 
            {
                // What didn't get used?
                unusedTargets.Add(sources.objects[i]);
            }
        }

        // If this is the first scene, create new containers for used and unused
        if (manager.config.levelNumber == 0)
        {
            spent = new GameObject();
            spent.name = "UsedTargets";
            DontDestroyOnLoad(spent);

            stored = new GameObject();
            stored.name = "UnusedTargets";
            DontDestroyOnLoad(stored);
        }
        else
        {
            spent = GameObject.Find("UsedTargets");
            stored = GameObject.Find("UnusedTargets");
        }


        // Update the objects in each parent container
        foreach (var target in usedTargets)
        {
            GameObject childcopy = Instantiate(target.gameObject);
            childcopy.transform.SetParent(spent.transform);
            childcopy.name = target.gameObject.name;
            childcopy.SetActive(false);
        }
        foreach (var target in unusedTargets)
        {
            GameObject childcopy = Instantiate(target.gameObject);
            childcopy.transform.SetParent(stored.transform);
            childcopy.name = target.gameObject.name;
            childcopy.SetActive(false);
        }


        // ---------------------------------------------------------------------
        // Remove excess stores from the Target Objects parent object in this
        // scene (so they don't get used)
        // ---------------------------------------------------------------------

        for (int i = 0; i < sources.objects.Count; i++)
        {
            if (i > destinations.objects.Count - 1) // needs to be one less or it won't clip the first store on the copping block.
            {
                Destroy(sources.objects[i]); // delete them from the list of target objects
            }
        }


        // If we are moving our target objects, list their info ----------------
        if (sources.objects[0].CompareTag("Target"))
        {
            log.log("TARGET INFORMATION ----------------------------------------", 1);
        }

        destination = destinations.currentObject();		
		source = sources.currentObject();	
		
		while (source != null && destination != null ) {	
			position = source.transform.position;
	        rotation = source.transform.rotation;


            if (ignoreYPos)
            {
                Vector3 temp = source.transform.position;
                temp.x = destination.transform.position.x;
                temp.z = destination.transform.position.z;
                source.transform.position = temp;
            }
            else source.transform.position = destination.transform.position;


            if (!ignoreRotation)
            {
                source.transform.localRotation = destination.transform.localRotation;
            }

            // Log the target info
            log.log(destination.name + ": \t" + source.name + 
                    "\tPosition: \t" + source.transform.position.x + "\t" + source.transform.position.y + "\t" + source.transform.position.z +
                    "\tRotation: \t" + source.transform.eulerAngles.x + "\t" + source.transform.eulerAngles.y + "\t" + source.transform.eulerAngles.z
                    , 1);

			
			if (swap) {
				destination.transform.position = position;
				destination.transform.localRotation = rotation;
		
			}
			
			destinations.incrementCurrent();
			destination = destinations.currentObject();
			
			sources.incrementCurrent();
			source = sources.currentObject();
		}

        // Clearly mark the log file with the end of the target info so it's easy to find, visually
        if (sources.objects[0].CompareTag("Target"))
        {
            log.log("-----------------------------------------------------------/", 1);
        }

    }

    public override bool updateTask () {
	    return true;
	}
	public override void endTask() {
		TASK_END();
	}
	
	public override void TASK_END() {
		base.endTask();
		
		if ( destinations ) {
			destinations.incrementCurrent();
			destination = destinations.currentObject();
		}

        if (hideDestinations)
        {
            foreach (var destination in destinations.objects)
            {
                destination.SetActive(false);
            }
        }
	}
}
