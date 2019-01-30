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
		
		
		destination = destinations.currentObject();		
		source = sources.currentObject();	
		
		while (source != null && destination != null ) {	
			position = source.transform.position;
	        rotation = source.transform.rotation;
			Debug.Log (destination.name.ToString());
			Debug.Log (source.name);
	
			
			source.transform.position = destination.transform.position;
			log.log("TASK_ROTATE\t" + source.name + "\t" + this.GetType().Name + "\t" + source.transform.localEulerAngles.ToString("f1"),1);

			source.transform.localRotation = destination.transform.localRotation;
			log.log("TASK_POSITION\t" + source.name + "\t" + this.GetType().Name + "\t" + source.transform.position.ToString("f1"),1);

			
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
