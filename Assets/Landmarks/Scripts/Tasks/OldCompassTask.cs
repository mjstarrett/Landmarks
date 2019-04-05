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

public class OldCompassTask : ExperimentTask {

	public GameObject compass;
	public Transform arrow;
	private bool viewable = false;
	public string question = "";
	public TextList questionList;
	private string current;
	private int compassCount = 1;
	
	
	public ObjectList objects;
	private GameObject currentObject;

	
	// Use this for initialization
	void Awake () {
		compass.active = viewable;
		//arrow.active = viewable;
	}
	
	public override void TASK_ROTATE (GameObject go, Vector3 hpr) {
		//log.log("TASK_ROTATE\t" + name + "\t" + this.GetType().Name,1 );
		arrow.localEulerAngles = hpr;
		log.log("TASK_ROTATE\t" + name + "\t" + this.GetType().Name + "\t" + arrow.localEulerAngles.ToString("f1"),1);

	}
		
	public override void startTask () {
		TASK_START();
	}
		
	public override void TASK_START() {
		
		if (!manager) Start();

		base.startTask();
		current = questionList.currentString();
		
		question = current;
		if (objects) {
			currentObject = objects.currentObject();
			if (currentObject)
				question = string.Format(question, currentObject.name);
		}
		
		question = question.Replace("    ", "\n");   //workaround for multi line questions
		
		hud.setMessage(question);
		viewable = !viewable;
		compass.active = viewable;
		//arrow.active = viewable;
	//	hud.showEverything();
		TASK_ROTATE (null, new Vector3(270.0F, 0.0F, 0.0F));
		log.log("INFO\tCOMPASS_START\t" + compassCount + "\t" + question + "\tAVATAR_LOCATION\tAVATAR_HEADING",1 );		

	}	
	/*
	      self.log.addType('COMPASS_START',[('COMPASS_NUM',int),('STORE',str),('AVATAR_LOCATION',Point3), ('AVATAR_HEADING',float)])
           self.log.addType('COMPASS_END',[('TRIAL_NUM',int),('AVATAR_LOCATION',Point3), ('COMPASS_HEADING',float),('QUESTION',str) ]) 
           */
		

	public override bool updateTask () {
		if(Input.GetButtonDown ("Return")) {
			log.log("INPUT_EVENT	dismiss compass	" + name,1 );
			return true;
		}
		float rotateInput = Input.GetAxis("Horizontal");
		if (rotateInput > 0.0) {
			arrow.Rotate(Vector3.forward * 20 * Time.deltaTime);
			//log.log("TASK_ROTATE\t" + name + "\t" + this.GetType().Name + "\t" + arrow.localEulerAngles.ToString("f1"),1);
			log.log("TASK_ROTATE\t" + arrow.name + "\t" + this.GetType().Name + "\t" + arrow.localEulerAngles.ToString("f1"),1);


		}
		if (rotateInput < 0.0) {
			arrow.Rotate(Vector3.forward * -20 * Time.deltaTime);
			//log.log("TASK_ROTATE\t" + name + "\t" + this.GetType().Name + "\t" + arrow.localEulerAngles.ToString("f1"),1);
			log.log("TASK_ROTATE\t" + arrow.name + "\t" + this.GetType().Name + "\t" + arrow.localEulerAngles.ToString("f1"),1);

		}
		
		return false;
	}
	
	
	public override void endTask() {
		TASK_END();
	}
	
	public override void TASK_END() {
		base.endTask();
		
		viewable = !viewable;
		compass.active = viewable;
		//arrow.active = viewable;
			
		questionList.incrementCurrent();
		current = questionList.currentString();
		if (current == null) parentTask.skip = true;
		
		if (objects) {	
			objects.incrementCurrent();
			currentObject = objects.currentObject();	
			if (currentObject == null) parentTask.skip = true;
		}

		compassCount++;
		
		hud.setMessage("");
			
		//log.log("INFO\tCOMPASS_END\t" + avatarController.transform.position.ToString("f3") + "\t" + arrow.localEulerAngles.ToString("f1") + "\t" + question,1 );		

	}
	
	
	
	
}
