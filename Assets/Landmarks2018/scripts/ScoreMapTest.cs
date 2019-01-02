using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreMapTest : ExperimentTask {

	private GameObject copyObjects; // should be the game object called copyObjects in the MapTask game object
	private GameObject targetObjects; // should be the game object called TargetObjects under Environment game object
	public float distanceErrorTolerance = 30; // world units (suggest meters)
	[Range(0,100)] public int percentCorrectCriterion = 100; // Allow adjustment of the score required to continue advance the experiment (0%-100%)
	private int numberTargets;
	private float percentCorrect;
	private string progressionText;	// Modifier for our message telling them whether they can continue or must try again

	// FROM INSTRUCTIONS TASK
	public TextAsset message;
	public bool blackout = true;
	private GUIText gui;

	void OnDisable ()
	{
		if (gui)
			DestroyImmediate (gui.gameObject);
	}

	public override void startTask () 
	{
		TASK_START();
		Debug.Log ("Scoring the map test");

		if (skip) {
			log.log("INFO	skip task	" + name,1 );
			return;
		}

		// --------------------------------------------------------------------
		// Set up Lists of the copies and originals to compare and score
		// --------------------------------------------------------------------

		// Automatically select the answers and targets based on LandMarks structure (can be changed)
		copyObjects = GameObject.Find("CopyObjects"); // should be the game object called copyObjects in the MapTask game object
		targetObjects = copyObjects.GetComponent<CopyChildObjects>().sourcesParent; // should be the game object called TargetObjects under Environment game object

		// Get the total possible
		numberTargets = copyObjects.transform.childCount;
		Debug.Log (numberTargets);

		// reactivate the original objects
		targetObjects.SetActive (true);

		// Populate our list of copies (participant answers) to score
		List<GameObject> copies = new List<GameObject>();
		foreach (Transform copy in copyObjects.transform) {
			copies.Add (copy.gameObject);
		}
		// Populate our list of targets (answer key) to compare
		List<GameObject> targets = new List<GameObject>();
		foreach (Transform target in targetObjects.transform) {
			targets.Add (target.gameObject);
		} 

		// Compare responses to answer key
		int numberCorrect = 0;
		for (int itarget = 0; itarget < (numberTargets); itarget++) {
			Debug.Log ("Checking score for the " + targets [itarget].name);

			// check if the store is rotated correctly. If not, we don't even need to check distance error... it's wrong
			float tempErrorAngleX = copies [itarget].transform.eulerAngles.x - targets [itarget].transform.eulerAngles.x; 
			float tempErrorAngleZ = copies [itarget].transform.eulerAngles.z - targets [itarget].transform.eulerAngles.z;

			// If the absolute value of this is larger than 1 degree (in case of unity rounding errors it is not set to 0) it's wrong
			if (Mathf.Abs (tempErrorAngleX) > 0) {
				Debug.Log ("The store was oriented incorrectly on the x-axis");
			} 

			else if (Mathf.Abs (tempErrorAngleZ) > 0) {
				Debug.Log ("The store was oriented incorrectly on the z-axis");
			}

			// If the angle isn't wrong, let's go ahead and check the straight line distance from placement to actual position
			else {
				float tempErrorDistance =  vector2DDistance (copies [itarget].transform.position, targets [itarget].transform.position);

				Debug.Log (tempErrorDistance + " meters from the correct position.");
				// if the error is within our public tolerance variable level, mark it correct
				if (tempErrorDistance <= distanceErrorTolerance) {
					numberCorrect++;
				} 
			}
		}
			
		// calculate a percentage to report
		percentCorrect = ((float)numberCorrect/numberTargets)*100; // when dividing two integers, must cast one as float to avoid unity rounding unneccesarily
		Debug.Log ("Map Score = " + percentCorrect + "%");


		// ----------------------------------------------------
		// React to Score based on Performance Criterion
		// ----------------------------------------------------

		if (percentCorrect >= percentCorrectCriterion) {
			progressionText = "Continue";

		} else if (percentCorrect < percentCorrectCriterion) {
			progressionText = "Try Again";
			parentTask.GetComponent<TaskList> ().repeat++;
		} else {
			progressionText = "CHECK WHAT'S WRONG WITH THE CODE";
		}


		// ---------------------------------
		// Create the GUI object
		// ---------------------------------
		GameObject sgo = new GameObject("Instruction Display");

		GameObject avatar = GameObject.FindWithTag("HUDtext");
		Text canvas = avatar.GetComponent<Text> ();
		hud.SecondsToShow = hud.InstructionDuration;

		sgo.AddComponent<GUIText>();
		sgo.hideFlags = HideFlags.HideAndDontSave;
		sgo.transform.position = new Vector3(0,0,0);
		gui = sgo.GetComponent<GUIText>();
		gui.pixelOffset = new Vector2( 20, Screen.height - 20);
		gui.text = message.text;	   			

		if (blackout) hud.showOnlyHUD();

		if (message) {
			string msg = message.text;
			msg = string.Format(msg, numberCorrect, numberTargets, progressionText);
			hud.setMessage(msg);
		}
		hud.flashStatus("");

		// Change text and turn on the map action button
		actionButton.GetComponentInChildren<Text> ().text = progressionText;
		manager.actionButton.SetActive(true);
		actionButton.onClick.AddListener (OnActionClick);
	}


	public override void TASK_START ()
	{
		if (!manager)
			Start ();
		
		base.startTask ();
	}


	public override bool updateTask ()
	{
		base.updateTask ();

		// Handle if the task is set to skip
		if (skip) {
			//log.log("INFO	skip task	" + name,1 );
			return true;
		}

		// Handle Timeout 
		if ( interval > 0 && Experiment.Now() - task_start >= interval)  {
			return true;
		}

		// Move on if they click enter
		if (Input.GetButtonDown("Return")) {
			log.log("INPUT_EVENT	clear text	1",1 );
			return true;
		}

		// -----------------------------------------
		// Handle Debug button behavior
		// -----------------------------------------
		if (killCurrent == true) 
		{
			return KillCurrent ();
		}

		// -----------------------------------------
		// Handle action button behavior
		// -----------------------------------------
		if (actionButtonClicked == true) 
		{
			actionButtonClicked = false;
			return true;
		}

		return false;
	}

	public override void endTask() {
		TASK_END();
	}

	public override void TASK_END() {

		base.endTask ();

		hud.setMessage ("");
		hud.SecondsToShow = hud.GeneralDuration; 

		GameObject avatar = GameObject.FindWithTag("HUDtext");
		Text canvas = avatar.GetComponent<Text>();
		string nullstring = null;
		canvas.text = nullstring;
		//			StartCoroutine(storesInactive());
		hud.showEverything();

		// turn off the map action button
		actionButton.onClick.RemoveListener (OnActionClick);
		manager.actionButton.SetActive(false);

		// -------------------------------
		// Prep the Target Object States
		// -------------------------------

		// Destroy the copies we created when initializing the map test task
		foreach (Transform child in copyObjects.transform) 
		{
			Destroy (child.gameObject);
		}

	}

	// Calculate the planar distance between placement and targets (i.e., ignore the y-axis height of the copies)
	private float vector2DDistance (Vector3 v1, Vector3 v2)
	{
		return (Mathf.Sqrt (Mathf.Pow (Mathf.Abs (v1.x - v2.x), 2f) + Mathf.Pow (Mathf.Abs (v1.z - v2.z), 2f)));
	}
}
