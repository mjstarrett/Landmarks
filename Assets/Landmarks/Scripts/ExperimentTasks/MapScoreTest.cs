using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapScoreTest : ExperimentTask {

    [Header("Task-specific Properties")]

    public GameObject copyObjects;

	private GameObject targetObjects; // should be the game object called TargetObjects under Environment game object

	public float distanceErrorTolerance = 30; // world units (suggest meters)
	[Range(0,100)] public int percentCorrectCriterion = 100; // Allow adjustment of the score required to continue advance the experiment (0%-100%)
	private int numberTargets;
	private float percentCorrect;
	private string progressionText;	// Modifier for our message telling them whether they can continue or must try again

	// FROM INSTRUCTIONS TASK
	public TextAsset message;
	public bool blackout = true;
	private Text gui;

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

        // make the cursor functional and visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // --------------------------------------------------------------------
        // Set up Lists of the copies and originals to compare and score
        // --------------------------------------------------------------------

        // Automatically select the answers and targets based on LandMarks structure (can be changed)
        targetObjects = copyObjects.GetComponent<CopyChildObjects>().sourcesParent.parentObject; // should be the game object called TargetObjects under Environment game object

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
			float tempErrorAngleX = Mathf.DeltaAngle( copies [itarget].transform.localRotation.eulerAngles.x, targets [itarget].transform.localRotation.eulerAngles.x); 
			float tempErrorAngleY = Mathf.DeltaAngle( copies [itarget].transform.localRotation.eulerAngles.y, targets [itarget].transform.localRotation.eulerAngles.y); // note this seems to correspond to z rotation axis in inspector
			float tempErrorAngleZ = Mathf.DeltaAngle( copies [itarget].transform.localRotation.eulerAngles.z, targets [itarget].transform.localRotation.eulerAngles.z);
			float tempErrorDistance =   Vector3Distance2D(copies [itarget].transform.position, targets [itarget].transform.position);

            log.log("Store: \t" + targets[itarget].name + "\tError Distance: \t" + tempErrorDistance + "\tError Rotation (xyz): \t" + tempErrorAngleX + "\t" + tempErrorAngleY + "\t" + tempErrorAngleZ, 2);

			// Check if rotation or distance are wrong, otherwise mark it correct and move on
			if (Mathf.Abs(tempErrorAngleX) > Mathf.Epsilon) {
				Debug.Log ("The store was oriented incorrectly on the x-axis");
			} else if (Mathf.Abs(tempErrorAngleY) > Mathf.Epsilon) {
				Debug.Log ("The store was oriented incorrectly on the y-axis");
			} else if (Mathf.Abs(tempErrorAngleZ) > Mathf.Epsilon) {
				Debug.Log ("The store was oriented incorrectly on the z-axis");
			} else if (tempErrorDistance > distanceErrorTolerance) {
				Debug.Log (tempErrorDistance + " meters from the correct position.");
			} else {
				numberCorrect++;
				Debug.Log ("Correct! " + tempErrorDistance + " meters from the correct position.");
			}
		}
			
		// calculate a percentage to report
		percentCorrect = ((float)numberCorrect/numberTargets)*100; // when dividing two integers, must cast one as float to avoid unity rounding unneccesarily
		Debug.Log ("Map Score = " + percentCorrect + "%");
        log.log("Score: \t" + numberCorrect + "\t/\t" + numberTargets + "\tPercentage: \t" + percentCorrect, 0);

        // New Landmarks Logging
		taskLog.AddData(transform.name + "_correct", numberCorrect.ToString());
		taskLog.AddData(transform.name + "_possible", numberTargets.ToString());
		taskLog.AddData(transform.name + "_percentage", percentCorrect.ToString());
		taskLog.AddData(transform.name + "_criterion", percentCorrectCriterion.ToString());

		// ----------------------------------------------------
		// React to Score based on Performance Criterion
		// ----------------------------------------------------

		// Store the selected, default number of reps so we can reset it after sub-criterion scores
		if (parentTask.GetComponent<TaskList>().repeatCount == 1) // the repeat count variable starts at 1 (not 0)
        {
            PlayerPrefs.SetInt("BaseMapRepeats", parentTask.GetComponent<TaskList>().repeat); // store the users repeat value in player prefs
        }

        // Based on performance, update the instruction text and add another repeat to the parent task, if below criterion
        if (percentCorrect >= percentCorrectCriterion)
        {
            progressionText = actionButton.GetComponent<DefaultText>().defaultText;
            parentTask.GetComponent<TaskList>().repeat = PlayerPrefs.GetInt("BaseMapRepeats"); // if criterion is met, reset the repeat value of the parent
        }
        else if (percentCorrect < percentCorrectCriterion)
        {
            progressionText = "Try Again";
            parentTask.GetComponent<TaskList>().repeat++; // if criterion is not met, add another repetition
        }
        else
        {
            progressionText = "CHECK WHAT'S WRONG WITH THE CODE";
        }


        // ---------------------------------
        // Create the GUI object
        // ---------------------------------
        GameObject sgo = new GameObject("Instruction Display");

        GameObject avatar = manager.player.GetComponent<HUD>().Canvas as GameObject;
        Text canvas = avatar.GetComponent<Text>();
        hud.SecondsToShow = hud.InstructionDuration;

		sgo.AddComponent<Text>();
		sgo.hideFlags = HideFlags.HideAndDontSave;
		sgo.transform.position = new Vector3(0,0,0);
		gui = sgo.GetComponent<Text>();
		//gui.pixelOffset = new Vector2( 20, Screen.height - 20);
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
		hud.actionButton.SetActive(true);
        hud.actionButton.GetComponent<Button>().onClick.AddListener(hud.OnActionClick);
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
		if (hud.actionButtonClicked == true) 
		{
			hud.actionButtonClicked = false;
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

        // make the cursor invisible
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false; 

        GameObject avatar = manager.player.GetComponent<HUD>().Canvas as GameObject;
        Text canvas = avatar.GetComponent<Text>();
        string nullstring = null;
		canvas.text = nullstring;
		//			StartCoroutine(storesInactive());

        // turn off the map action button
        actionButton.GetComponentInChildren<Text>().text = actionButton.GetComponent<DefaultText>().defaultText;
        hud.actionButton.GetComponent<Button>().onClick.RemoveListener(hud.OnActionClick);
        hud.actionButton.SetActive(false);

		// -------------------------------
		// Prep the Target Object States
		// -------------------------------

		// Destroy the copies we created when initializing the map test task
		foreach (Transform child in copyObjects.transform) 
		{
			Destroy (child.gameObject);
		}

	}

	
}
