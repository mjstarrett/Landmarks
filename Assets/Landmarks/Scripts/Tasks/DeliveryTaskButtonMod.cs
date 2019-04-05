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

public class DeliveryTaskButtonMod : ExperimentTask {
	
	public ObjectList destinations;
	private GameObject current;
	
	private int score = 0;
	public int scoreIncrement = 50;
	public int penaltyRate = 2000;

	private float penaltyTimer = 0;

	public bool showScoring;
	public TextAsset deliveryInstruction;
	
	public override void startTask () {
		TASK_START();
		//avatarController.handleInput = true;		
	}	
	public override void TASK_START() {
		if (!manager) Start();
		base.startTask();
		
		hud.showEverything();
		hud.showScore = showScoring;
		current = destinations.currentObject();
		
		if (deliveryInstruction) {
			string msg = deliveryInstruction.text;
			if (destinations != null) msg = string.Format(msg, current.name);
			hud.setMessage(msg);
		} else {
			hud.setMessage("Please find the " + current.name);
		}
	}	
	
	public override bool updateTask () 
	{
		base.updateTask();
		if (Input.GetButton ("Compass" ))
		{


			if (showScoring) 
			{
				score = score + scoreIncrement;
				hud.setScore(score);
			}
			return true;

		
		//		Debug.Log (hit.transform.parent.name + " = " + current.name);

			if (showScoring) 
			{
				score = score + scoreIncrement;
				hud.setScore(score);
			}
			return true;

		return false;
		}
	

		return false;	
	}
	
	public override void endTask() {
		TASK_END();
		//avatarController.handleInput = false;	
	}
	
	public override void TASK_PAUSE() {
		//avatarController.handleInput = false;
		//base.endTask();
		log.log("TASK_PAUSE\t" + name + "\t" + this.GetType().Name + "\t" ,1 );
		//avatarController.stop();

		hud.setMessage("");
		hud.showScore = false;

	}
	
	public override void TASK_END() {
		base.endTask();
		//avatarController.stop();
		
		if (canIncrementLists) {
			destinations.incrementCurrent();
		}
		current = destinations.currentObject();
		hud.setMessage("");
		hud.showScore = false;

	}
	
	public override bool OnControllerColliderHit(GameObject hit)  {
		if (hit == current) {
			if (showScoring) {
				score = score + scoreIncrement;
				hud.setScore(score);
			}
			return true;
		}
		
		//		Debug.Log (hit.transform.parent.name + " = " + current.name);
		if (hit.transform.parent == current.transform) {
			if (showScoring) {
				score = score + scoreIncrement;
				hud.setScore(score);
			}
			return true;
		}
		return false;
	}

}