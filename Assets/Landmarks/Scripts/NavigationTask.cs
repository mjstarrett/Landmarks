using System.Collections;
using System.Collections.Generic;
using System.Management;
using UnityEngine;
using TMPro;

public enum HideTargetOnStart
{
    Off,
    SetInactive,
    SetInvisible,
    SetProbeTrial
}

public class NavigationTask : ExperimentTask 
{

	public ObjectList destinations;
	private GameObject current;

	private int score = 0;
	public int scoreIncrement = 50;
	public int penaltyRate = 2000;


    private float penaltyTimer = 0;

	public bool showScoring;
	public TextAsset NavigationInstruction;

    public HideTargetOnStart hideTargetOnStart;
    [Range(0,60)] public float showTargetAfterSeconds;
    public bool hideNonTargets;

    private float startTime;

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

        if (skip)
        {
            log.log("INFO    skip task    " + name, 1);
            return;
        }

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
            hud.SecondsToShow = 0;
            hud.setMessage("Please find the " + current.name);
		}

        // Handle if we're hiding all the non-targets
        if (hideNonTargets)
        {
            foreach (GameObject item in destinations.objects)
            {
                if (item.name != destinations.currentObject().name)
                {
                    item.SetActive(false);
                }
                else item.SetActive(true);
            }
        }


        // Handle if we're hiding the target object
        if (hideTargetOnStart != HideTargetOnStart.Off)
        {
            if (hideTargetOnStart == HideTargetOnStart.SetInactive)
            {
                destinations.currentObject().SetActive(false);
            }
            else if (hideTargetOnStart == HideTargetOnStart.SetInvisible)
            {
                destinations.currentObject().GetComponent<MeshRenderer>().enabled = false;
            }
            else if (hideTargetOnStart == HideTargetOnStart.SetProbeTrial)
            {
                destinations.currentObject().SetActive(false);
                destinations.currentObject().GetComponent<MeshRenderer>().enabled = false;
            }
        }
        else
        {
            destinations.currentObject().SetActive(true); // make sure the target is visible unless the bool to hide was checked
            destinations.currentObject().GetComponent<MeshRenderer>().enabled = true;
        }

        // startTime = Current time in seconds
        startTime = Time.time;
        Debug.Log(startTime);

        //// MJS 2019 - Move HUD to top left corner
        //hud.hudPanel.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
        //hud.hudPanel.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.9f);
    }	

	public override bool updateTask () 
	{
		base.updateTask();

        if (skip)
        {
            //log.log("INFO    skip task    " + name,1 );
            return true;
        }

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

        //VR capabiity with showing target
        if (vrEnabled)
        {
            if (hideTargetOnStart != HideTargetOnStart.Off && hideTargetOnStart != HideTargetOnStart.SetProbeTrial && ((Time.time - startTime > (showTargetAfterSeconds) || vrInput.TouchpadButton.GetStateDown(Valve.VR.SteamVR_Input_Sources.Any))))
            {
                destinations.currentObject().SetActive(true);
            }

            if (hideTargetOnStart == HideTargetOnStart.SetProbeTrial && vrInput.TouchpadButton.GetStateDown(Valve.VR.SteamVR_Input_Sources.Any))
            {
                //get current location and then log it

                destinations.currentObject().SetActive(true);
                destinations.currentObject().GetComponent<MeshRenderer>().enabled = true;
            }
        }

        //show target on button click or after set time
        if (hideTargetOnStart != HideTargetOnStart.Off && hideTargetOnStart != HideTargetOnStart.SetProbeTrial && ((Time.time - startTime > (showTargetAfterSeconds) || Input.GetButtonDown("Return"))))
        {
            destinations.currentObject().SetActive(true);
        }

        if (hideTargetOnStart == HideTargetOnStart.SetProbeTrial && Input.GetButtonDown("Return"))
        {
            //get current location and then log it

            destinations.currentObject().SetActive(true);
            destinations.currentObject().GetComponent<MeshRenderer>().enabled = true;
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

        hud.SecondsToShow = hud.GeneralDuration;

        // Move hud back to center and reset
        hud.hudPanel.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        hud.hudPanel.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);

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
