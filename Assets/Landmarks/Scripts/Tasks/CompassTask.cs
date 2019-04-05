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

public class CompassTask : ExperimentTask {

	public GameObject compass;
	public Transform arrow;
	//public GameObject arrow;
	private bool viewable = false;
	public string question = "";
	public string answer = "";
	public ExperimentTask questionList;
	private string current;
	private int compassCount = 1;
	private bool inCooldown = false; //MJS-12/28/2015: bool to indicate cooldown timer for response button

	//View options (JDS)
	public int RotationSpeed; //MJS-11/18/2015: Added input to adjust arrow rotation speed
	public int cooldownTime; //MJS-12/29/2015: Added input for a JRD response cooldown timer
	public bool blackout;
	public bool portholeVert;
	public bool portholeHorz;
	
	
	public ObjectList objects;
	private GameObject currentObject;

	
	// Use this for initialization
	void Awake () {
		compass.SetActive(viewable);
		//arrow = compass.GetComponent("arrow").transform;
		arrow.gameObject.SetActive(viewable);
	}
	
	public override void TASK_ROTATE (GameObject go, Vector3 hpr) {
		//log.log("TASK_ROTATE\t" + name + "\t" + this.GetType().Name,1 );
		arrow.localEulerAngles = hpr;
		log.log(string.Format("TASK_ROTATE\t{0}\t{1}\t{2}", name, this.GetType().Name, arrow.localEulerAngles.ToString("f1")),1);

	}
		
	public override void startTask () {
		TASK_START();
	}
		
	public override void TASK_START() {

		if (!manager) Start();

		base.startTask();
		current = questionList.currentString();
		
		
		string[] parts = new string[1];		
        parts = current.Split(new char[] {'\t'},StringSplitOptions.RemoveEmptyEntries);				        	
			        	
		answer = parts[0];
		question = parts[0];
		
		/* remove
		if (objects) {
			currentObject = objects.currentObject();
			if (currentObject)
				question = string.Format(question, currentObject.name);
		}
		
		*/
		
		//question = question.Replace("    ", "\n");   //workaround for multi line questions
		
		hud.setMessageCompass(question.Replace("    ", "\n"));
		viewable = !viewable;
		compass.active = viewable;
		arrow.gameObject.SetActive(viewable);




		if (blackout) hud.showOnlyHUD();
		else hud.showEverything();
		
		if (portholeVert) hud.portHoleVertOn();
		else if (portholeHorz) hud.portHoleHorzOn();

	//	hud.showEverything();
		TASK_ROTATE (null, new Vector3(270.0F, 0.0F, 0.0F));
		log.log("INFO\tCOMPASS_START\t" + compassCount + "\t" + question + "\tAVATAR_LOCATION\tAVATAR_HEADING",1 );		

	}	
	/*
	      self.log.addType('COMPASS_START',[('COMPASS_NUM',int),('STORE',str),('AVATAR_LOCATION',Point3), ('AVATAR_HEADING',float)])
           self.log.addType('COMPASS_END',[('TRIAL_NUM',int),('AVATAR_LOCATION',Point3), ('COMPASS_HEADING',float),('QUESTION',str) ]) 
           */
		
	//Take key/arrow inputs
	public override bool updateTask () {
		if(!inCooldown && Input.GetButtonDown ("responseConfirm")) { ////MJS-12/28/2015: Add conditional to nullify if in cooldown
			log.log("INPUT_EVENT	dismiss compass	" + name,1 );
			StartCoroutine(responseCooldown()); //MJS-12/28/2015: Initiate the cooldown after the button is pressed
			return true;
		}
		float rotateInput = Input.GetAxis("responseTriggers"); //MJS-12/29/2015: Created new axis (edit>projectSettings>input) for Xbox triggers
		if (rotateInput > 0.0) {
			arrow.Rotate(Vector3.forward * RotationSpeed * Time.deltaTime*rotateInput); //MJS-12/29/2015: added ' * rotateInput' to account for trigger pressure in response
			//log.log("TASK_ROTATE\t" + name + "\t" + this.GetType().Name + "\t" + arrow.localEulerAngles.ToString("f1"),1);
		}
		if (rotateInput < 0.0) {
			arrow.Rotate(Vector3.forward * RotationSpeed * Time.deltaTime*rotateInput); //MJS-12/29/2015: added ' * rotateInput' to account for trigger pressure in response; Note- removed ' * -1' as Xbox trigger axis goes from -1(left) to +1 (right)
			//log.log("TASK_ROTATE\t" + name + "\t" + this.GetType().Name + "\t" + arrow.localEulerAngles.ToString("f1"),1);
		}
		log.log("TASK_ROTATE\t" + arrow.name + "\t" + this.GetType().Name + "\t" + arrow.localEulerAngles.ToString("f1"),1);
		//print ("TASK_ROTATE\t" + arrow.name + "\t" + this.GetType ().Name + "\t" + arrow.localEulerAngles.ToString ("f1"));
		return false;
	}
	
	
	public override void endTask() {
		TASK_END();
	}
	
	public override void TASK_END() {
		base.endTask();
		
		viewable = !viewable;
		compass.active = viewable;
		arrow.gameObject.SetActive(viewable);
			
		questionList.incrementCurrent();
		current = questionList.currentString();
		if (current == null) parentTask.skip = true;
		
		if (objects) {	
			objects.incrementCurrent();
			currentObject = objects.currentObject();	
			if (currentObject == null) parentTask.skip = true;
		}

		compassCount++;
		
		hud.setMessageCompass("");

		if (portholeVert) hud.portHoleOff();
		if (portholeHorz) hud.portHoleOff();




			
		//log.log("INFO\tCOMPASS_END\t" + avatarController.transform.position.ToString("f3") + "\t" + arrow.localEulerAngles.ToString("f1") + "\t" + answer + "\t" + question,1 );		
		log.log("INFO\tCOMPASS_END\t" + avatar.transform.position.ToString("f3") + "\t" +  avatar.transform.rotation.ToString("f3")+"\t" + arrow.localEulerAngles.ToString("f1") + "\t" + question,1 );		
		
	}
	
	//MJS-12/28/2015: Cooldown function to be called to turn off response button for 3 seconds
	IEnumerator responseCooldown() {
		inCooldown = true;
		yield return new WaitForSeconds(cooldownTime);
		inCooldown = false;
	}

}
