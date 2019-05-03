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

public class MoveSpawn : ExperimentTask {

	[HideInInspector] public GameObject start;
	public GameObject destination;
	public SpawnList destinations;
	
	public bool swap;
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

        // determine what we're moving
        if (isScaled)
        {
            start = scaledAvatar;
        }
        else start = avatar; 


		if ( destinations ) {
			destination = destinations.currentObject();		
		}
		
		position = start.transform.position;
        rotation = start.transform.rotation;


        // Set the position, but ignore the y-axis (just 2d position on map)
        Vector3 tempPos = start.transform.position; 
        tempPos.x = destination.transform.position.x;
        tempPos.z = destination.transform.position.z;
        start.transform.position = tempPos;
        log.log("TASK_POSITION\t" + start.name + "\t" + this.GetType().Name + "\t" + start.transform.transform.position.ToString("f1"), 1);


        // Set the rotation to random
        start.transform.rotation = Random.rotation;
        log.log("TASK_ROTATE\t" + start.name + "\t" + this.GetType().Name + "\t" + start.transform.localEulerAngles.ToString("f1"), 1);
		

		if (swap) {
			destination.transform.position = position;
			destination.transform.rotation = rotation;
	
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
