using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationTask : ExperimentTask 
{

	public StoreList destinations;
	private GameObject current;

	private int score = 0;
	public int scoreIncrement = 50;
	public int penaltyRate = 2000;

	private float penaltyTimer = 0;

	public bool showScoring;
	public TextAsset NavigationInstruction;

    public bool blackoutOnMessage = false;
    
	public override void startTask () 
	{
		TASK_START();
		avatarLog.navLog = true;
        if (isScaled) scaledAvatarLog.navLog = true;
    }	
	public override void TASK_START() 
	{
		if (!manager) Start();
        base.startTask();

		hud.showEverything();
		hud.showScore = showScoring;
		current = destinations.currentObject();
		Debug.Log ("Find " + destinations.currentObject().name);


		if (NavigationInstruction) 
		{
			string msg = NavigationInstruction.text;
			if (destinations != null) msg = string.Format(msg, current.name);
			hud.setMessage(msg);
   		} 
		else 
		{
			hud.setMessage ("What are we looking for?");
			//hud.setMessage("Please find the " + current.name);
		}
	}	

	public override bool updateTask () 
	{
		base.updateTask();

		if (score > 0) penaltyTimer = penaltyTimer + (Time.deltaTime * 1000);


		if (penaltyTimer >= penaltyRate) 
		{
			penaltyTimer = penaltyTimer - penaltyRate;
			if (score > 0) 
			{
				score = score - 1;
				hud.setScore(score);
			}
		}

        // if we're gonna black out the screen when text is up... check and handle it
        if (blackoutOnMessage)
        {
            if (!hud.hidemessage) hud.showOnlyHUD();
            else if (hud.hidemessage) hud.showEverything();
        }

		if (killCurrent == true) 
		{
			return KillCurrent ();
		}

		return false;	
	}

	public override void endTask() 
	{
		TASK_END();
		//avatarController.handleInput = false;
	}

	public override void TASK_PAUSE() 
	{
		avatarLog.navLog = false;
        if (isScaled) scaledAvatarLog.navLog = false;
		//base.endTask();
		log.log("TASK_PAUSE\t" + name + "\t" + this.GetType().Name + "\t" ,1 );
		//avatarController.stop();

		hud.setMessage("");
		hud.showScore = false;

	}

	public override void TASK_END() 
	{
		base.endTask();
		//avatarController.stop();
		avatarLog.navLog = false;
        if (isScaled) scaledAvatarLog.navLog = false;

        if (canIncrementLists) 
		{
			destinations.incrementCurrent();
		}
		current = destinations.currentObject();
		hud.setMessage("");
		hud.showScore = false;

	}

	public override bool OnControllerColliderHit(GameObject hit)  
	{
		if (hit == current) 
		{
			if (showScoring) 
			{
				score = score + scoreIncrement;
				hud.setScore(score);
			}
			return true;
		}

		//		Debug.Log (hit.transform.parent.name + " = " + current.name);
		if (hit.transform.parent == current.transform) 
		{
			if (showScoring) 
			{
				score = score + scoreIncrement;
				hud.setScore(score);
			}
			return true;
		}
		return false;
	}
}
