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

public class ViewObject : ExperimentTask {

	public ObjectList startObjects;
	private GameObject current;
	public GameObject destination;

	private static Vector3 position;
	private static Quaternion rotation;
	private static Vector3 scale;
	private int saveLayer;
	private int viewLayer = 0;
	public bool blackout = true;
	public bool rotate = true;
	public long rotation_start;
	public float rotation_start_float;
	public override void startTask () {
		TASK_START();
		current = startObjects.currentObject();
		
		initCurrent();	
		rotation_start = Experiment.Now();
		rotation_start_float = rotation_start/1f;
		print(rotation_start);
		
	}	

	public override void TASK_START()
	{


		if (!manager) Start();
		base.startTask();
		if (skip) {
			log.log("INFO	skip task	" + name,1 );
			return;
		}
		

	    if (blackout) hud.showOnlyHUD();
	    else hud.showEverything();
		

	}
	
	public override bool updateTask () {
		
		if (skip) {
			//log.log("INFO	skip task	" + name,1 );
			return true;
		}
		
		if (current) {
			
			if (rotate) {
				current.transform.Rotate( Vector3.up, 30 * Time.deltaTime);
				log.log("TASK_ROTATE\t" + current.name  + "\t" + this.GetType().Name + "\t" + current.transform.localEulerAngles.ToString("f1"),2);
			}

			if ( Experiment.Now() - rotation_start >= interval)  {
				rotation_start = Experiment.Now();
				rotation_start_float = rotation_start/1f;
				startObjects.incrementCurrent();
				returnCurrent();
				current = startObjects.currentObject();	
				if (current) initCurrent();
			} else {
		    	return false;
			}
		} else {
			return true;
		}
		return false;
	}
	public void initCurrent() {
		position = current.transform.position;
        rotation = current.transform.rotation;
		scale = current.transform.localScale;
		
		current.transform.position = destination.transform.position;
		current.transform.localRotation = destination.transform.localRotation;
		current.transform.localScale = destination.transform.localScale;
				
		saveLayer = current.layer;
		setLayer(current.transform,viewLayer);
		
		log.log("TASK_ADD\t" + name  + "\t" + this.GetType().Name + "\t" + current.name + "\tadd",1);		
		log.log("TASK_POSITION\t" + current.name  + "\t" + this.GetType().Name + "\t" + current.transform.position.ToString("f4"),1);
		log.log("TASK_ROTATE\t" + current.name  + "\t" + this.GetType().Name + "\t" + current.transform.localEulerAngles.ToString("f3"),1);
		log.log("TASK_SCALE\t" + current.name  + "\t" + this.GetType().Name + "\t" + current.transform.localScale.ToString("f3"),1);


	}
	
	public override void TASK_ADD(GameObject go, string txt) {
		if (txt == "add") {
			saveLayer = go.layer;
			setLayer(go.transform,viewLayer);
		} else if (txt == "remove") {
			setLayer(go.transform,saveLayer);
		}

	}
	
	public void returnCurrent() {
		current.transform.position = position;
		current.transform.localRotation = rotation;
		current.transform.localScale = scale;
		setLayer(current.transform,saveLayer);
		
		log.log("TASK_ADD\t" + name  + "\t" + this.GetType().Name + "\t" + current.name + "\tremove",1);		
		log.log("TASK_POSITION\t" + current.name  + "\t" + this.GetType().Name + "\t" + current.transform.position.ToString("f4"),1);
		log.log("TASK_ROTATE\t" + current.name  + "\t" + this.GetType().Name + "\t" + current.transform.localEulerAngles.ToString("f3"),1);
		log.log("TASK_SCALE\t" + current.name  + "\t" + this.GetType().Name + "\t" + current.transform.localScale.ToString("f3"),1);
		
	}
	public override void endTask() {
		//returnCurrent();
		startObjects.current = 0;
		TASK_END();
	}
	
	public override void TASK_END() {

		base.endTask();
		
		if (blackout) hud.showEverything();
		else hud.showOnlyHUD();
		
	}
	
	public void setLayer(Transform t, int l) {
		t.gameObject.layer = l;
		foreach (Transform child in t) {
			setLayer(child,l);
		}
	}
}
