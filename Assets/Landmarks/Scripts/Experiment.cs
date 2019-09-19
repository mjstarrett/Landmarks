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
using System.IO;
using System;
using System.Reflection;
using Valve.VR.InteractionSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public enum EndListMode
{
	Loop,
	End
}

public enum UserInterface
{
    DesktopDefault,
    ViveRoomspace,
    ViveVirtualizer,
    ViveKatwalk
}


public class Experiment : MonoBehaviour {

    public GameObject uiParent;
    public UserInterface userInterface = UserInterface.DesktopDefault;
    [HideInInspector] public TaskList tasks;
	[HideInInspector] public Config config;
	private long microseconds = 1;
	private string logfile;
	private string configfile = "";
    [HideInInspector] public GameObject player;
    [HideInInspector] public Camera playerCamera;
    [HideInInspector] public Camera overheadCamera;
    [HideInInspector] public GameObject scaledPlayer;
    [HideInInspector] public GameObject environment;
    [HideInInspector] public GameObject scaledEnvironment;

    public bool debugging = false;

    [HideInInspector] public bool usingVR;

	[HideInInspector] public dbLog dblog;
	
	
	private bool playback = false;
	private bool pause = true;
	private bool done = false;
	private long now;
	
	private Event evt;
	private long playback_start;
	private long playback_offset;
	[HideInInspector] public long playback_time;

	private long next_time;
	private string[] next_action;
	//private long playback_start;
	
	protected GameObject avatar;
	protected AvatarController avatarController;
	protected HUD hud;

	public static long Now() {
		
		long tick = DateTime.Now.Ticks;
        return tick / TimeSpan.TicksPerMillisecond;
	}
	
	void Awake() {

        // check if we have any old Landmarks instances from LoadScene.cs and handle them
        GameObject oldInstance = GameObject.Find("OldInstance");
        if (oldInstance != null)
        {
            foreach (var item in oldInstance.transform)
            {
                //Destroy(item); // this tends to break the steamvr skeleton buttons and hand rendermodels
                oldInstance.SetActive(false);
            }
        }

        Debug.Log ("Starting Experiment.cs");

        //since config is a singleton it will be the one created in scene 0 or this scene
        config = Config.instance;

        // ------------------------------------------
        // Grab the Landmarks items that are not controller dependent
        // ------------------------------------------

        // Grab the tasklist that controls the timeline of the experiment
        tasks = GameObject.Find("LM_Timeline").GetComponent<TaskList>();

        // grab the overhead camera in the scene
        overheadCamera = GameObject.Find("OverheadCamera").GetComponent<Camera>();

        // Assign the scaled player if it's in the scene
        scaledPlayer = GameObject.Find("SmallScalePlayerController");
        scaledPlayer.SetActive(false);

        // find Environment
        environment = GameObject.FindGameObjectWithTag("Environment");
        

        // Handle the selected UI enum from the inspector
        if (PlayerPrefs.GetString("UserInterface") != "default")
        {
            Debug.Log("Getting user interface from config file.");

            switch (config.ui)
            {
                case "Desktop":
                    userInterface = UserInterface.DesktopDefault;
                    break;
                case "Vive Virt.":
                    userInterface = UserInterface.ViveVirtualizer;
                    break;
                case "Vive Std.":
                    userInterface = UserInterface.ViveRoomspace;
                    break;
                default:
                    break;
            }
        }

        // ------------------------------------------
        // Assign Player and Camera based on UI enum
        // ------------------------------------------


        switch (userInterface)
        {
            case UserInterface.DesktopDefault:
                // Standard Desktop with Keyboard/mouse controller
                player = GameObject.Find("DesktopDefaultController");
                playerCamera = GameObject.Find("DesktopDefaultCamera").GetComponent<Camera>();

                // Render the overhead camera on the main display (none)
                overheadCamera.stereoTargetEye = StereoTargetEyeMask.None;

                // create variable to indicate if using VR
                usingVR = false;

                break;

            case UserInterface.ViveRoomspace:

                // HTC Vive and Cyberith Virtualizer
                player = GameObject.Find("ViveRoomspaceController");
                playerCamera = GameObject.Find("VRCamera").GetComponent<Camera>();

                // Render the overhead camera to each lense of the HMD
                overheadCamera.stereoTargetEye = StereoTargetEyeMask.Both;

                // create variable to indicate if using VR
                usingVR = true;

                break;

            case UserInterface.ViveVirtualizer:

                // This is a proprietary asset that must be added to the _Landmarks_ prefab to work
                // If it is not added (either for lack of need or lack of the proprietary SDK), use the default
                if (GameObject.Find("ViveVirtualizerController") == null)
                {
                    Debug.Log("UserInterface player controller not found. Falling back to Default");
                    goto default;
                }

                // HTC Vive and Cyberith Virtualizer
                player = GameObject.Find("ViveVirtualizerController");
                playerCamera = GameObject.Find("ViveVirtualizerCamera").GetComponent<Camera>();

                // Render the overhead camera to each lense of the HMD
                overheadCamera.stereoTargetEye = StereoTargetEyeMask.Both;

                // create variable to indicate if using VR
                usingVR = true;

                break;

            default:
                // Standard Desktop with Keyboard/mouse controller
                player = GameObject.Find("DesktopDefaultController");
                playerCamera = GameObject.Find("DesktopDefaultCamera").GetComponent<Camera>();

                // Render the overhead camera on the main display (none)
                overheadCamera.stereoTargetEye = StereoTargetEyeMask.None;

                // create variable to indicate if using VR
                usingVR = false;

                break;
        }

        Debug.Log (player.name);
		// Tag the selected playerController
		player.tag = "Player";
		// Deactivate all other controllers
		foreach (Transform child in GameObject.Find("PlayerControllers").transform) {
			if (child.name != player.name) {
				child.gameObject.SetActive (false);
			}
		}

        // Add audiolistener to camera (default one should be removed on on all LM_playerControllers)
        // This ensures there will only be one in the scene, attached to the active camera
        playerCamera.gameObject.AddComponent<AudioListener>();

        // Set up Overhead Camera (for map task or any other top-down viewed tasks)
		overheadCamera = GameObject.Find("OverheadCamera").GetComponent<Camera> ();

		// ------------------------------------------
		// Configure Player properties
		// ------------------------------------------

		playerCamera.enabled = true;
		overheadCamera.enabled = false;
		Cursor.visible = false;
		

		// Set the avatar and hud
		avatar = player;
		hud = avatar.GetComponent("HUD") as HUD;
        hud.showOnlyHUD();


        // ------------------------------------------
        // Handle the config file
        // ------------------------------------------

        logfile = config.subjectPath + "/" + PlayerPrefs.GetString("expID") + "_" + config.subject + "_" + config.level + ".log";
		configfile = config.expPath + "/" + config.filename;
		
		//when in editor
		if (!config.bootstrapped) {
			logfile = Directory.GetCurrentDirectory() + "/data/tmp/" + "test.log";
			configfile = Directory.GetCurrentDirectory() + "/data/tmp/" + config.filename;
		}

        if (config.runMode == ConfigRunMode.NEW) {
			dblog = new dbLog(logfile);
		} else if (config.runMode == ConfigRunMode.RESUME) {
            dblog = new dbPlaybackLog(logfile);
		} else if (config.runMode == ConfigRunMode.PLAYBACK) {
			CharacterController c = avatar.GetComponent<CharacterController>();
            c.detectCollisions = false;
			dblog = new dbPlaybackLog(logfile);
		}

        //start session

        dblog.log("EXPERIMENT:\t" + PlayerPrefs.GetString("expID") + "\tSUBJECT:\t" + config.subject + 
                  "\tSTART_SCENE\t" + config.level + "\tSTART_CONDITION:\t" + config.condition + "\tUI:\t" + userInterface.ToString(), 1);

        Debug.Log(XRSettings.loadedDeviceName);
    }


    public void StartPlaying() {		
		long tick = DateTime.Now.Ticks;
        playback_start = tick / TimeSpan.TicksPerMillisecond;
        playback_offset = 0;
    }

	void Start () {

        ConfigOverrides.parse(configfile,dblog);
		hud.showFPS = config.showFPS;
		hud.showTimestamp = (config.runMode == ConfigRunMode.PLAYBACK);

		
		
		//start experiment
		if (config.runMode != ConfigRunMode.PLAYBACK) {
			Debug.Log ("Trying to start a task");
			tasks.startTask();	
		} else {
			hud.flashStatus( "Playback Paused" );
			next_action = dblog.NextAction();
			next_time = Int64.Parse(next_action[0]);
			long tick = DateTime.Now.Ticks;
        	playback_start = tick / TimeSpan.TicksPerMillisecond;
        	playback_offset = 0;
        	now = playback_start;
   		}

        // find ScaledEnvironment
        try
        {
            scaledEnvironment = FindObjectOfType<LM_ScaledEnvironment>().transform.gameObject;
            scaledEnvironment.gameObject.SetActive(false);
        }
        catch
        {
            scaledEnvironment = null; 
        }

    }
	
	public void OnControllerColliderHit(GameObject hit)  {
		if (config.runMode != ConfigRunMode.PLAYBACK) {
			tasks.OnControllerColliderHit(hit);
		}
	}
	
	void Update () {
		
		if ( !done) {
			if (config.runMode != ConfigRunMode.PLAYBACK) {
				
				if (Input.GetKeyDown (KeyCode.T)) {
					dblog.log("BOOKMARK	t-trigger",1 );
				}
				
				done = tasks.updateTask();	
				if (done) {
					Cursor.visible = true;
					Application.Quit();
				}		
			} else {
				updatePlayback();
			}
		}
	}
	
	void updatePlayback() {
		
		long last_now = now;
		long tick = DateTime.Now.Ticks;
        now = tick / TimeSpan.TicksPerMillisecond;
		
		if (Input.GetButtonDown("PlayPause")) {
			pause = !pause;
			hud.flashStatus( "Playback Paused" );
		}

		if (pause) {
			playback_offset -= now - last_now;
		}
		
		float seek = Input.GetAxis("Horizontal");
		//if (seek != 0.0) {
		if (Input.GetButton("Horizontal")) {
			playback_offset += 250;// * Convert.ToInt64(seek);	
		}
        playback_time = now - playback_start + playback_offset;
		hud.playback_time = playback_time;
		
		string[] vec;
		Vector3 vec3;
		
		while ( (!pause || (seek != 0.0) ) && !done && dblog.PlaybackTime() <= playback_time  ) {
//					 Debug.Log(dblog.PlaybackTime() + " : " + playback_time);
								Debug.Log(next_action[2] );
								
								
	//try {							
			if(next_action[2] == "AVATAR_HPR" || next_action[2] == "AVATAR_POS" || next_action[2] == "AVATAR_STOP") {
				vec = next_action[3].Split(new char[] {',', '(', ')', ' '},StringSplitOptions.RemoveEmptyEntries);				 
				vec3 = new Vector3(float.Parse(vec[0]), float.Parse(vec[1]), +float.Parse(vec[2]) );
				Type t = typeof(AvatarController); 
				t.InvokeMember(next_action[2], BindingFlags.Default | BindingFlags.InvokeMethod, null, avatarController, new System.Object[] {vec3});
			} else if(next_action[2] == "TASK_ROTATE" || next_action[2] == "TASK_POSITION"  || next_action[2] == "TASK_SCALE") {

				vec = next_action[5].Split(new char[] {',', '(', ')', ' '},StringSplitOptions.RemoveEmptyEntries);				 
				vec3 = new Vector3(float.Parse(vec[0]), float.Parse(vec[1]), +float.Parse(vec[2]) );
				
				GameObject taskObject = GameObject.Find(next_action[3]);
				Component script = taskObject.GetComponent(next_action[4]) as Component;
				//Type t = typeof(AvatarController); 
				this.GetType().InvokeMember(next_action[2], BindingFlags.Default | BindingFlags.InvokeMethod, null, this, new System.Object[] {taskObject, vec3});
			} else if ( next_action[2] == "TASK_ADD" ) {				
				GameObject taskObject = GameObject.Find(next_action[3]);
				Component script = taskObject.GetComponent(next_action[4]) as Component;
				//Type t = typeof(AvatarController); 
				GameObject secondObject = GameObject.Find(next_action[5]);
				script.GetType().InvokeMember(next_action[2], BindingFlags.Default | BindingFlags.InvokeMethod, null, script, new System.Object[] {secondObject,next_action[6]});								
			} else if ( next_action[2] == "INPUT_EVENT" ) {
				hud.flashStatus( "Input: " + next_action[3] + " " + next_action[4]);
			} else if ( next_action[2] == "SET_SCORE" ) {
				hud.setScore( int.Parse(next_action[3] ));

			} else if ( next_action[2] == "INFO" ) {
				//skip
			} else if ( next_action[2] == "DATA" ) {
				//skip
			} else if ( next_action[2] == "BOOKMARK" ) {
				//skip
			} else if ( next_action[2] == "CONFIG_SET" ) {
				//Debug.Log("CONFIG_SET" );
				ConfigOverrides.set_keyvalue(next_action[3]+"="+next_action[4],"Config: ", dblog);
			} else {
							//	Debug.Log("else" );
				GameObject taskObject = GameObject.Find(next_action[3]);
				Component script = taskObject.GetComponent(next_action[4]) as Component;

				//Type t = typeof(AvatarController); 
				script.GetType().InvokeMember(next_action[2], BindingFlags.Default | BindingFlags.InvokeMethod, null, script, null);
			}
			
	//	} catch (FormatException){
	//	} catch (IndexOutOfRangeException){ }
					
			next_action = dblog.NextAction();
			if (next_action == null) {
				hud.setMessage( "Playback Done" );
				done = true;	
			}

		}
	}
	public  void TASK_ROTATE (GameObject go, Vector3 hpr) {
		go.transform.localEulerAngles = hpr;
	}	
	public  void TASK_POSITION (GameObject go, Vector3 pos) {
		go.transform.position = pos;
	}	
	public  void TASK_SCALE (GameObject go, Vector3 scale) {
		go.transform.localScale = scale;
	}	

//log.log("TASK_POSITION\t" + current.name  + "\t" + this.GetType().Name + "\t" + current.transform.position.ToString("f4"),1);
//		log.log("TASK_ROTATE\t" + current.name  + "\t" + this.GetType().Name + "\t" + current.transform.localRotation.ToString("f3"),1);
//		log.log("TASK_SCALE\t" + current.name  + "\t" + this.GetType().Name + "\t" + current.transform.localScale.ToString("f3"),1);
				
	void OnApplicationQuit() 
	{
		if (config.runMode != ConfigRunMode.PLAYBACK) {
			tasks.endTask();
		}
		dblog.close();
		Cursor.visible = true;
	}
	
	void OnGUI () 
	{
		
		//GUILayout.Label(config.home);
		//if (config.bootstrapped)
		//	GUILayout.Label("BOOTSTRAPPED");
			
		/*	this captures too many events - maybe just KeyUp?
		evt = Event.current;

		if(evt.isKey){
		   if(evt.type == EventType.KeyDown){
		      Debug.Log(evt.keyCode);
		   }
		   if(ev	t.type == EventType.KeyUp){
		      Debug.Log(evt.keyCode);
		   }

		}
		*/
	}
	
	public static void Shuffle<T>(T[] array)
    {
		var random = new System.Random();
		for (int i = array.Length; i > 1; i--)
		{
		    // Pick random element to swap.
		    int j = random.Next(i); // 0 <= j <= i-1
		    // Swap.
		    T tmp = array[j];
		    array[j] = array[i - 1];
		    array[i - 1] = tmp;
		}
    }

	public static void Shuffle_168<T>(T[] array )
	{
		var random = new System.Random();

		for (int i = 168; i > 1; i--)
		{
			// Pick random element to swap.
			int j = random.Next(i); // 0 <= j <= i-1
			// Swap.
			T tmp = array[j];
			array[j] = array[i - 1];
			array[i - 1] = tmp;
		}
	}
	
}
