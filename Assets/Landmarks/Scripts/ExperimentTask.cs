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
using System.Reflection;
using UnityEngine.UI;
using Valve.VR;
using UnityEditor;

public class ExperimentTask : MonoBehaviour{

	protected GameObject avatar;
	protected HUD hud;
	protected GameObject experiment;
	protected dbLog log;
	protected Experiment manager;
	protected avatarLog avatarLog;
	
    protected GameObject scaledAvatar; // MJS 2019 - track scaled avatar in scaled nav task
    protected avatarLog scaledAvatarLog; // MJS 2019 - track scaled avatar in scaled nav task
    
	protected long task_start;

    protected SteamVR_Input_ActionSet_landmarks vrInput;

    protected ArrayList trialHeader;
    protected ArrayList trialData;
	
	public bool skip = false;
	public bool canIncrementLists = true;

	public int interval = 0;

	public TaskList interruptTasks;
	public int interruptInterval = 0;
	public int repeatInterrupts = 9999;
	public int currentInterrupt = 0;
	[HideInInspector] public ExperimentTask pausedTasks;
	[HideInInspector] public TaskList parentTask;

	[HideInInspector] public Camera firstPersonCamera;
	[HideInInspector] public Camera overheadCamera;

	[HideInInspector] public Button debugButton;
	[HideInInspector] public Button actionButton;

    [HideInInspector] public bool vrEnabled; // use hidden variable to communicate if we're using VR based on input interface

    public static bool killCurrent = false;
    protected static bool isScaled = false; // allows scaled nav task components to inherit this bool - MJS 2019
    protected static bool jitterGuardOn = false; // prevent raycast jitter when using a moving HUD such as in the map task
    public LM_TaskLog taskLog; // The logging destination assigned to this object

    [Header("EEG Settings (if available)")]
    public string triggerLabel; // name prefix for unique triggers
    public bool triggerOnStart; // mark a unique trigger at TASK_START
    public bool triggerOnEnd; // mark a unique trigger at TASK_END
    private BrainAmpManager eegManager;


    public void Awake () 
	{
		// Look for a BrainAmp EEG manager in the eperiment
        eegManager = FindObjectOfType<BrainAmpManager>();
	}

	public void Start () 
    {

	}
	
	public virtual void startTask() {
		avatar = GameObject.FindWithTag ("Player");
		avatarLog = avatar.GetComponentInChildren<avatarLog>() as avatarLog; //jdstokes 2015
		hud = avatar.GetComponent("HUD") as HUD;
		experiment = GameObject.FindWithTag ("Experiment");
		manager = experiment.GetComponent("Experiment") as Experiment;
		firstPersonCamera = manager.playerCamera;
		overheadCamera = manager.overheadCamera;
        log = manager.dblog;
        vrEnabled = manager.usingVR;

        // set up vrInput if we're using VR
        if (vrEnabled) vrInput = SteamVR_Input.GetActionSet<SteamVR_Input_ActionSet_landmarks>(default);

        // Grab the scaled nav task/player and log it - MJS 2019
        scaledAvatar = manager.scaledPlayer;
        scaledAvatarLog = scaledAvatar.GetComponent("avatarLog") as avatarLog;

        //debugButton = hud.debugButton.GetComponent<Button>();
        actionButton = hud.actionButton.GetComponent<Button>();

        //// Start listening for debug skips
        //if (manager.debugging)
        //{
        //    debugButton.gameObject.SetActive(true);
        //    debugButton.onClick.AddListener(onDebugClick);
        //}

		task_start = Experiment.Now();
		ResetHud();
		hud.ForceShowMessage ();
		//currentInterrupt = 0;        Not here since after an interuupt we will restart
		
		log.log("TASK_START\t" + name + "\t" + this.GetType().Name,1 );

        if (eegManager != null & triggerOnStart)
        {
            var startLabel = triggerLabel;
            if (startLabel == "")
            {
                startLabel = transform.name + "_start";
            }
            else startLabel += "_start";

            eegManager.EEGTrigger(startLabel);
            log.log("EEG_TRIGGER\tName\t" + startLabel + "\tValue\t" + eegManager.triggers[startLabel].ToString(), 1);
        }
    }
	
	public virtual void TASK_START () {

	}	
	
	public virtual bool updateTask () {

		bool attemptInterupt = false;
		
		if ( interruptInterval > 0 && Experiment.Now() - task_start >= interruptInterval)  {
	        attemptInterupt = true;
	    }
	    
		if( Input.GetButtonDown ("Compass") ) {
			attemptInterupt = true;	
		}    
	    
		if(attemptInterupt && interruptTasks && currentInterrupt < repeatInterrupts && !interruptTasks.skip) 
		{	
			if (interruptTasks.skip) {
				log.log("INFO	skip interrupt	" + interruptTasks.name,1 );
			} 
			else 
			{
				Debug.Log(currentInterrupt);
	    		Debug.Log(repeatInterrupts);
	    
				log.log("INPUT_EVENT	interrupt	" + name,1 );
				//interruptTasks.pausedTasks = this;
				parentTask.pausedTasks = this;
				TASK_PAUSE();
				//endTask();
				currentInterrupt = currentInterrupt + 1;
				interruptTasks.startTask();
				parentTask.currentTask = interruptTasks;
	
			}
		}


		
		return false;
	}


	public virtual void endTask()
    {

        if (eegManager != null & triggerOnEnd)
        {
            var endLabel = triggerLabel;
            if (endLabel == "")
            {
                endLabel = transform.name + "_end";
            }
            else endLabel += "_end";

            eegManager.EEGTrigger(endLabel);
            log.log("EEG_TRIGGER\tName\t" + endLabel + "\tValue\t" + eegManager.triggers[endLabel].ToString(), 1);
        }

        long duration = Experiment.Now() - task_start;
		currentInterrupt = 0;    //put here because of interrupts
		log.log("TASK_END\t" + name + "\t" + this.GetType().Name + "\t" + duration,1 );
        hud.showNothing();
	}


	public virtual void TASK_END ()
    {

	}


	public virtual void TASK_PAUSE ()
    {

	}


	public virtual bool OnControllerColliderHit(GameObject hit)
    {
		return false;
	}


	public virtual void TASK_ROTATE (GameObject go, Vector3 hpr)
    {

	}


	public virtual void TASK_POSITION (GameObject go, Vector3 hpr)
    {

	}


	public virtual void TASK_SCALE (GameObject go, Vector3 scale)
    {

	}


	public virtual void TASK_ADD(GameObject go, string txt)
    {

	}

	
	public virtual string currentString()
    {
		return "";
	}


	public virtual void incrementCurrent()
    {

	}


	// http://www.haslo.ch/blog/setproperty-and-getproperty-with-c-reflection/
	private object getProperty(object containingObject, string propertyName)
	{
	    return containingObject.GetType().InvokeMember(propertyName, BindingFlags.GetProperty, null, containingObject, null);
	}


	private void setProperty(object containingObject, string propertyName, object newValue)
	{
	    containingObject.GetType().InvokeMember(propertyName, BindingFlags.SetProperty, null, containingObject, new object[] { newValue });
	}


	public void onDebugClick ()
	{
		killCurrent = true;
	}


    public bool KillCurrent () 
	{
		killCurrent = false;
		Debug.Log ("ForceKilling " + this.name);
		return true;
	}


	// Reset hud position to the forward direction (for world space Canvas UI)
	public void ResetHud()
	{
		/*ViveRoomspaceController and any other controller where the player "body" is not
		the parent object of the player controller (tagged "Player at runtime") must have
		the HUD position AND rotation updated.*/
		var cam = avatar.GetComponent<LM_PlayerController>().cam.transform;
		hud.hudRig.transform.localPosition = new Vector3(cam.localPosition.x, 
														 hud.hudRig.transform.localPosition.y, 
														 cam.localPosition.z);
		hud.hudRig.transform.localEulerAngles = new Vector3(0f, cam.localEulerAngles.y, 0f);
	}


    // Move the hud, but don't move it again for a time period to avoid jitter
    public IEnumerator HudJitterReduction()
    {
        jitterGuardOn = true;
        yield return new WaitForSeconds(0.5f);
        jitterGuardOn = false;
    }


    //recursive calls
    public void MoveToLayer(Transform root, int layer)
    {
        root.gameObject.layer = layer;
        foreach (Transform child in root) MoveToLayer(child, layer);
    }


	// Calculate the planar distance between placement and targets (i.e., ignore the y-axis height of the copies)
	public float Vector3Distance2D(Vector3 v1, Vector3 v2)
	{
		return (Mathf.Sqrt(Mathf.Pow(Mathf.Abs(v1.x - v2.x), 2f) + Mathf.Pow(Mathf.Abs(v1.z - v2.z), 2f)));
	}

	public float Vector3Angle2D(Vector3 v1, Vector3 v2) 
	{
		return Vector2.SignedAngle(new Vector2(v1.x, v1.z), new Vector2(v2.x, v2.z));
	}
}
