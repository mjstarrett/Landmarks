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

public class MoveObjects : ExperimentTask {

	public ObjectList sources;
	private GameObject source;

	private GameObject destination;
	public ObjectList destinations;
	
	public bool swap;

    public bool destroyExtraObjects; // get rid of any gameobjects in sources that aren't used here (anything over the lenght of destinations)

	private static Vector3 position;
	private static Quaternion rotation;


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


        // Destroy any objects that don't get moved (say if we're only randomly using 8 stores out of a possible selection of 16)
        if (destroyExtraObjects)
        {
            Debug.Log("Trimming ObjectList " + sources.name + " from " + sources.objects.Count + " to " + destinations.objects.Count);
            for (int i = 0; i < sources.objects.Count; i++)
            {
                Debug.Log(i);
                if (i > destinations.objects.Count - 1) // needs to be one less or it won't clip the first store on the copping block.
                {
                    Destroy(sources.objects[i]); // delete them from the list of target objects
                }
            }
        }


        destination = destinations.currentObject();		
		source = sources.currentObject();	
		
		while (source != null && destination != null ) {	
			position = source.transform.position;
	        rotation = source.transform.rotation;
	
			
			source.transform.position = destination.transform.position;
			source.transform.localRotation = destination.transform.localRotation;

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
	}
}
