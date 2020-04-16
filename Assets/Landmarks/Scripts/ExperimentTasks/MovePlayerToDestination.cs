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

// MJS - Modified from MoveObject.cs to be specific to the player

using UnityEngine;
using System.Collections;

public class MovePlayerToDestination : ExperimentTask {

	[Header("Task-specific Properties")]
	public ObjectList destinations;
	public bool ignoreDestinationY;
	public bool swap;

	[HideInInspector] public GameObject start;
	[HideInInspector] public GameObject destination;
	
	private static Vector3 position;
	private static Quaternion rotation;


	public override void startTask () {
		TASK_START();
	}	

	public override void TASK_START()
	{
		base.startTask();
		
		if (!manager) Start();

        // move the correct player
        if (isScaled)
        {
            start = scaledAvatar;
        } else start = avatar;
		
		
		if (skip) {
			log.log("INFO	skip task	" + name,1 );
			return;
		}
		
		if ( destinations ) {
            if (destinations.currentObject().GetComponentInChildren<LM_SnapPoint>() != null)
            {
                destination = destinations.currentObject().GetComponentInChildren<LM_SnapPoint>().gameObject;
                Debug.Log(destination.name);
            } else destination = start;

        }

        position = start.transform.position;
        rotation = start.transform.rotation;

        // Handle if we don't want to change the moved objects height
        if (ignoreDestinationY)
        {
            Vector3 tmp = start.transform.position;
            tmp.x = destination.transform.position.x;
            tmp.z = destination.transform.position.z;
            start.transform.position = tmp;
        }
        else start.transform.position = destination.transform.position;

        // Log position
        log.log("TASK_ROTATE\t" + start.name + "\t" + this.GetType().Name + "\t" + start.transform.localEulerAngles.ToString("f1"), 1);

        // rotate the moved object to match the destination
        start.transform.localRotation = destination.transform.localRotation;
        // Log the rotation
        log.log("TASK_POSITION\t" + start.name + "\t" + this.GetType().Name + "\t" + start.transform.transform.position.ToString("f1"), 1);
        
		
		if (swap) {
			destination.transform.position = position;
			destination.transform.localRotation = rotation;
	
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
