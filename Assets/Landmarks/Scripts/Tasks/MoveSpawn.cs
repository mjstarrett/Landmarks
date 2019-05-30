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
using UnityStandardAssets.Characters.FirstPerson;

public class MoveSpawn : ExperimentTask {

	[HideInInspector] public GameObject start;
	public GameObject destination;
	public ObjectList destinations;
	
	public bool swap;
	private static Vector3 position;
	private static Vector3 rotation;
    public bool ignoreY = false;


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
        rotation = start.transform.eulerAngles;


        // Set the position, but ignore the y-axis (just 2d position on map)
        Vector3 tempPos = start.transform.position; 
        tempPos.x = destination.transform.position.x;
        if (!ignoreY)
        {
            tempPos.y = destination.transform.position.y;
        }
        tempPos.z = destination.transform.position.z;
        avatar.transform.position = tempPos;
        log.log("TASK_POSITION\t" + start.name + "\t" + this.GetType().Name + "\t" + start.transform.transform.position.ToString("f1"), 1);


        // Set the rotation to random

        Vector3 tempRot = start.transform.eulerAngles;
        tempRot.y = Random.Range(0, 359.999f);
        start.transform.eulerAngles = tempRot;
        if (!isScaled)
        {
            /* MJS 2019
             * It is not possible to simply rotate the unity standard asset firstpersoncontroller manually, 
             * so we need to access a modified firstpersoncontroller.cs script
             * which will access a modified MouseLook.cs script to reset the mouselook 
             * which effectively forces it not to undo our manual rotation
            */           
            avatar.GetComponent<FirstPersonController>().ResetMouselook();
        }

        log.log("TASK_ROTATE\t" + start.name + "\t" + this.GetType().Name + "\t" + start.transform.localEulerAngles.ToString("f1"), 1);


        //Debug.Log("-------------------------------------------------");
        //Debug.Log(start.transform.position);
        //Debug.Log(destination.transform.position);
        //Debug.Log(avatar.transform.position);
        //Debug.Log("-------------------------------------------------");





        if (swap) {
			destination.transform.position = position;
			destination.transform.eulerAngles = rotation;
	
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
