using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapTask : ExperimentTask
{

    public bool flattenMap = true; // take away 3d look for more map-like appearance
    public Vector3 hudTextOffset; // shift text from being directly over the center of the store
	protected bool targetSelected;
	protected GameObject selectedTarget;
    protected Vector3 baselineScaling; // record the scale at the beginning so we can put things back at the end

	public override void startTask()
	{
		TASK_START();
		avatarLog.navLog = false;
	}

	public override void TASK_START()
	{
		if (!manager) Start();
		base.startTask();

		// Modify the HUD display for the map task
		hud.setMessage("");
		hud.hudPanel.SetActive(true); // temporarily turn off the hud panel at task start (no empty message window)
		hud.ForceShowMessage();
		// move hud off screen if we aren't hitting a target shop
		hud.hudPanel.transform.position = new Vector3(99999, 99999, 99999);

		// make the cursor functional and visible
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		// Turn off Player movement
		avatar.GetComponent<CharacterController>().enabled = false;

		// Swap from 1st-person camera to overhead view
		firstPersonCamera.enabled = false;
		overheadCamera.enabled = true;

		if (flattenMap)
		{
			// Flatten out environment buildings so stores are clearly visible
			baselineScaling = GameObject.FindWithTag("Environment").transform.localScale;
			GameObject.FindWithTag("Environment").transform.localScale = new Vector3(baselineScaling.x, 0.01F, baselineScaling.z);
		}

		// Turn on action button and listen
		hud.actionButton.SetActive(true);
		hud.actionButton.GetComponent<Button>().onClick.AddListener(hud.OnActionClick);
	}


	public override bool updateTask()
	{
		base.updateTask();

		return false;
	}


	public override void endTask()
	{
		TASK_END();
	}


	public override void TASK_END()
	{
		base.endTask();

		// Set up hud for other tasks
		hud.hudPanel.SetActive(true); //hide the text background on HUD
									  // Change the anchor points to put the message back in center
		RectTransform hudposition = hud.hudPanel.GetComponent<RectTransform>() as RectTransform;
		hudposition.anchorMin = new Vector2(0, 0);
		hudposition.anchorMax = new Vector2(1, 1);
		hudposition.pivot = new Vector2(0.5f, 0.5f);
		hudposition.anchoredPosition3D = new Vector3(0, 0, 0);
		//hud.hudPanel.GetComponent<RectTransform> = hudposition;

		// make the cursor invisible
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = false;

		// Turn on Player movement
		avatar.GetComponent<CharacterController>().enabled = true;

		// Swap from overhead camera to first-person view
		firstPersonCamera.enabled = true;
		overheadCamera.enabled = false;

		if (flattenMap)
		{
			// un-Flatten out environment buildings so stores are clearly visible
			GameObject.FindWithTag("Environment").transform.localScale = baselineScaling;
		}

		// turn off the map action button
		hud.actionButton.GetComponent<Button>().onClick.RemoveListener(hud.OnActionClick);
		actionButton.GetComponentInChildren<Text>().text = actionButton.GetComponent<DefaultText>().defaultText;
		hud.actionButton.SetActive(false);
	}


	protected void MakeFlat(Transform stuff, bool flatStatus, bool setBaseline = false)
    {
		if (flatStatus)
		{
			if (setBaseline)
			{
				baselineScaling = stuff.localScale;
			}
			stuff.localScale = new Vector3(baselineScaling.x, 0.01f, baselineScaling.z);
		}
		else stuff.localScale = baselineScaling;

    }

	protected void ConfigureActionButton(string buttonText)
    {
		// Turn on action button and listen
		hud.actionButton.SetActive(true);
		hud.actionButton.GetComponent<Button>().onClick.AddListener(hud.OnActionClick);

		// Change text on the map action button
		actionButton.GetComponentInChildren<Text>().text = buttonText;
	}


	protected void CheckRayCastStatus()
    {
		//empty RaycastHit object which raycast puts the hit details into
		RaycastHit hit;
		//ray shooting out of the camera from where the mouse is
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


		if (Physics.Raycast(ray, out hit))
		{// & Input.GetMouseButtonDown(0)) 
			if (hit.transform.CompareTag("Target"))
			{
				//Debug.Log (objectHit.tag);
				hud.setMessage(hit.transform.name);
				hud.hudPanel.SetActive(true);
				hud.ForceShowMessage();

				// move hud text to the store being highlighted (coroutine to prevent Update framerate jitter)
				// jitterGuardOn is inherited from Experiment task so it can be used in multiple task scripts (e.g., MapStudy and MapTest) - MJS 2019
				if (!jitterGuardOn)
				{
					hud.hudPanel.transform.position = Camera.main.WorldToScreenPoint(hit.transform.position + hudTextOffset);
					StartCoroutine(HudJitterReduction());
				}

				selectedTarget = hit.transform.gameObject;
				targetSelected = true;

				log.log("Mouseover \t" + hit.transform.name, 1);
			}
			// Or if it is a sub-collider of a target (i.e., child collider)
			else if (hit.transform.parent.transform.CompareTag("Target"))
			{
				hud.setMessage(hit.transform.parent.transform.name);
				hud.hudPanel.SetActive(true);
				hud.ForceShowMessage();

				// move hud text to the store being highlighted (coroutine to prevent Update framerate jitter)
				// jitterGuardOn is inherited from Experiment task so it can be used in multiple task scripts (e.g., MapStudy and MapTest) - MJS 2019
				if (!jitterGuardOn)
				{
					hud.hudPanel.transform.position = Camera.main.WorldToScreenPoint(hit.transform.parent.transform.position + hudTextOffset);
					StartCoroutine(HudJitterReduction());
				}

				selectedTarget = hit.transform.parent.transform.gameObject;
				targetSelected = true;

				log.log("Mouseover \t" + hit.transform.parent.transform.name, 1);
			}
			else
			{
				DeselectStore();
			}
		}
		else
		{
			DeselectStore();
		}
	}



	protected void DeselectStore()
	{
		hud.setMessage("");
		hud.hudPanel.SetActive(true);
		hud.ForceShowMessage();
		// move hud off screen if we aren't hitting a target shop
		hud.hudPanel.transform.position = new Vector3(99999, 99999, 99999);

		targetSelected = false;
		selectedTarget = null;
	}

}
