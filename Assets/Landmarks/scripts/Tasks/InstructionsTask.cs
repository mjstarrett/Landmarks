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
using UnityEngine.UI;

public class InstructionsTask : ExperimentTask {
	
	public TextAsset instruction;
	public TextAsset message;
	
	public StoreList objects;
	private GameObject currentObject;
	
	public TextList texts;
	private string currentText;
		
	public bool blackout = true;
	public Color text_color = Color.white;
	public Font instructionFont;
	public int instructionSize = 12;
		
	private GUIText gui;
	
	void OnDisable ()
	{
		if (gui)
			DestroyImmediate (gui.gameObject);
	}
	
	public override void startTask () {
		TASK_START();
		Debug.Log ("Starting an Instructions Task");
	}	

	public override void TASK_START()
	{
		
		if (!manager) Start();
		base.startTask();
		
		
		
		if (skip) {
			log.log("INFO	skip task	" + name,1 );
			return;
		}
       			
    	GameObject sgo = new GameObject("Instruction Display");
    		
		GameObject avatar = GameObject.FindWithTag("HUDtext");
		Text canvas = avatar.GetComponent<Text> ();
		hud.SecondsToShow = hud.InstructionDuration;

    		
		sgo.AddComponent<GUIText>();
		sgo.hideFlags = HideFlags.HideAndDontSave;
		sgo.transform.position = new Vector3(0,0,0);
		gui = sgo.GetComponent<GUIText>();
		gui.pixelOffset = new Vector2( 20, Screen.height - 20);
		gui.font = instructionFont;
		gui.fontSize = instructionSize;
		gui.material.color = text_color;
		gui.text = message.text;	   			

	    if (texts) currentText = texts.currentString().Trim();
	    if (objects) currentObject = objects.currentObject();
		if (instruction) canvas.text = instruction.text;
	    if (blackout) hud.showOnlyHUD();
		if (message) {
			string msg = message.text;
			if (currentText != null) msg = string.Format(msg, currentText);
			if (currentObject != null) msg = string.Format(msg, currentObject.name);
			hud.setMessage(msg);
		}
		hud.flashStatus("");		
		
	}
	// Update is called once per frame
	public override bool updateTask () {
		
		if (skip) {
			//log.log("INFO	skip task	" + name,1 );
			return true;
		}
		if ( interval > 0 && Experiment.Now() - task_start >= interval)  {
			return true;
	    }
	    
		if (Input.GetButtonDown("Return")) {
			log.log("INPUT_EVENT	clear text	1",1 );
	        return true;
	    }
	    return false;
	}
	
	public override void endTask() {
		Debug.Log ("Ending an instructions task");
		TASK_END();
	}
	
	public override void TASK_END() {
		base.endTask ();
		hud.setMessage ("");
		hud.SecondsToShow = hud.GeneralDuration; 
		
		if (canIncrementLists) {

			if (objects) {
				objects.incrementCurrent ();
				currentObject = objects.currentObject ();
			}
			if (texts) {
				texts.incrementCurrent ();		
				currentText = texts.currentString ();
			}

		}

		// MJS 05/02/2018 - commented out. No apparent function (throws error); see HUD line 222
		GameObject avatar = GameObject.FindWithTag("HUDtext");
		Text canvas = avatar.GetComponent<Text>();
		string nullstring = null;
		canvas.text = nullstring;
//			StartCoroutine(storesInactive());
		hud.showEverything();
	}

}
