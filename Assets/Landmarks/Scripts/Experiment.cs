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
    KeyboardMouse,
    ViveRoomspace,
    ViveVirtualizer,
}


public class Experiment : MonoBehaviour {

    public GameObject availableControllers;
    public UserInterface userInterface = UserInterface.KeyboardMouse;
    public bool debugging = false;

    [HideInInspector] 
    public TaskList tasks;
	[HideInInspector] 
    public Config config;	
    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public Camera playerCamera;
    [HideInInspector]
    public Camera overheadCamera;
    [HideInInspector] 
    public GameObject scaledPlayer;
    [HideInInspector] 
    public GameObject environment;
    [HideInInspector] 
    public GameObject scaledEnvironment;
    [HideInInspector] 
    public bool usingVR;
	[HideInInspector] 
    public dbLog dblog;
    [HideInInspector] 
    public long playback_time;

	private bool pause = true;
	private bool done = false;
	private long now;
	private Event evt;
	private long playback_start;
	private long playback_offset;
	private long next_time;
	private string[] next_action;
    //private long playback_start;
    private string logfile;
    private string configfile = "";

    protected GameObject avatar;
	protected AvatarController avatarController;
	protected HUD hud;


    // -------------------------------------------------------------------------
    // -------------------------- Builtin Methods ------------------------------
    // -------------------------------------------------------------------------


    void Awake() {

        // ------------------------------
        // Clean up & Initialize Scene
        // ------------------------------

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

        //since config is a singleton it will be the one created in scene 0 or this scene
        config = Config.instance;


        // ------------------------------
        // Set up the Experiment
        // ------------------------------

        // Assign experiment variables based on selected controller
        var lmPlayer = GetController();
        player = lmPlayer.controller;
        avatar = lmPlayer.gameObject;
        playerCamera = lmPlayer.cam;
        hud = lmPlayer.headsUpDisplay;
        usingVR = lmPlayer.usesVR;

        // Initialize controller properties
        playerCamera.gameObject.AddComponent<AudioListener>();
        playerCamera.enabled = true;
        hud.showOnlyHUD();
        player.tag = "Player";

        // Deactivate all other controllers
        foreach (Transform child in GameObject.Find("PlayerControllers").transform)
        {
            if (child.name != player.name)
            {
                child.gameObject.SetActive(false);
            }
        }

        // Assign other experiment variables
        tasks = GameObject.Find("LM_Timeline").GetComponent<TaskList>();
        overheadCamera = GameObject.Find("OverheadCamera").GetComponent<Camera>();
        environment = GameObject.FindGameObjectWithTag("Environment");
        scaledPlayer = GameObject.Find("SmallScalePlayerController");

        // Initialize other experiment variables
        if (usingVR)
        {
            overheadCamera.stereoTargetEye = StereoTargetEyeMask.Both;
        }
        else
        {
            overheadCamera.stereoTargetEye = StereoTargetEyeMask.None;
        }
        overheadCamera.enabled = false;
        Cursor.visible = false;
        scaledPlayer.SetActive(false);


        // ------------------------------
        // Handle Config file
        // ------------------------------

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

    }


    void Start()
    {

        ConfigOverrides.parse(configfile, dblog);
        hud.showFPS = config.showFPS;
        hud.showTimestamp = (config.runMode == ConfigRunMode.PLAYBACK);

        //start experiment
        if (config.runMode != ConfigRunMode.PLAYBACK)
        {
            Debug.Log("Trying to start a task");
            tasks.startTask();
        }
        else
        {
            hud.flashStatus("Playback Paused");
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


    void Update()
    {

        if (!done)
        {
            if (config.runMode != ConfigRunMode.PLAYBACK)
            {

                if (Input.GetKeyDown(KeyCode.T))
                {
                    dblog.log("BOOKMARK t-trigger", 1);
                }

                done = tasks.updateTask();
                if (done)
                {
                    Cursor.visible = true;
                    Application.Quit();
                }
            }
            else
            {
                updatePlayback();
            }
        }
    }


    // -------------------------------------------------------------------------
    // ------------------------ LM-Specific Methods ----------------------------
    // -------------------------------------------------------------------------

    public void StartPlaying() {		
		long tick = DateTime.Now.Ticks;
        playback_start = tick / TimeSpan.TicksPerMillisecond;
        playback_offset = 0;
    }

	
	public void OnControllerColliderHit(GameObject hit)  {
		if (config.runMode != ConfigRunMode.PLAYBACK) {
			tasks.OnControllerColliderHit(hit);
		}
	}
	

    public static long Now()
    {

        long tick = DateTime.Now.Ticks;
        return tick / TimeSpan.TicksPerMillisecond;
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

        while ((!pause || (Mathf.Abs(seek) > Mathf.Epsilon)) && !done && dblog.PlaybackTime() <= playback_time)
        {
            Debug.Log(next_action[2]);

            //try {							
                if (next_action[2] == "AVATAR_HPR" || next_action[2] == "AVATAR_POS" || next_action[2] == "AVATAR_STOP")
                {
                    vec = next_action[3].Split(new char[] { ',', '(', ')', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    vec3 = new Vector3(float.Parse(vec[0]), float.Parse(vec[1]), +float.Parse(vec[2]));
                    Type t = typeof(AvatarController);
                    t.InvokeMember(next_action[2], BindingFlags.Default | BindingFlags.InvokeMethod, null, avatarController, new System.Object[] { vec3 });
                }
                else if (next_action[2] == "TASK_ROTATE" || next_action[2] == "TASK_POSITION" || next_action[2] == "TASK_SCALE")
                {

                    vec = next_action[5].Split(new char[] { ',', '(', ')', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    vec3 = new Vector3(float.Parse(vec[0]), float.Parse(vec[1]), +float.Parse(vec[2]));

                    GameObject taskObject = GameObject.Find(next_action[3]);
                    Component script = taskObject.GetComponent(next_action[4]) as Component;
                    //Type t = typeof(AvatarController); 
                    this.GetType().InvokeMember(next_action[2], BindingFlags.Default | BindingFlags.InvokeMethod, null, this, new System.Object[] { taskObject, vec3 });
                }
                else if (next_action[2] == "TASK_ADD")
                {
                    GameObject taskObject = GameObject.Find(next_action[3]);
                    Component script = taskObject.GetComponent(next_action[4]) as Component;
                    //Type t = typeof(AvatarController); 
                    GameObject secondObject = GameObject.Find(next_action[5]);
                    script.GetType().InvokeMember(next_action[2], BindingFlags.Default | BindingFlags.InvokeMethod, null, script, new System.Object[] { secondObject, next_action[6] });
                }
                else if (next_action[2] == "INPUT_EVENT")
                {
                    hud.flashStatus("Input: " + next_action[3] + " " + next_action[4]);
                }
                else if (next_action[2] == "SET_SCORE")
                {
                    hud.setScore(int.Parse(next_action[3]));

                }
                else if (next_action[2] == "INFO")
                {
                    //skip
                }
                else if (next_action[2] == "DATA")
                {
                    //skip
                }
                else if (next_action[2] == "BOOKMARK")
                {
                    //skip
                }
                else if (next_action[2] == "CONFIG_SET")
                {
                    //Debug.Log("CONFIG_SET" );
                    ConfigOverrides.set_keyvalue(next_action[3] + "=" + next_action[4], "Config: ", dblog);
                }
                else
                {
                    //	Debug.Log("else" );
                    GameObject taskObject = GameObject.Find(next_action[3]);
                    Component script = taskObject.GetComponent(next_action[4]) as Component;

                    //Type t = typeof(AvatarController); 
                    script.GetType().InvokeMember(next_action[2], BindingFlags.Default | BindingFlags.InvokeMethod, null, script, null);
                }
        	//} 
            //catch (FormatException)
            //{

            //}
            //catch (IndexOutOfRangeException)
            //{

            //}

            //next_action = dblog.NextAction();
            //if (next_action == null)
            //{
            //    hud.setMessage("Playback Done");
            //    done = true;
            //}

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


    void OnApplicationQuit()
    {
        if (config.runMode != ConfigRunMode.PLAYBACK)
        {
            tasks.endTask();
        }
        dblog.close();
        Cursor.visible = true;
    }


    public  LM_PlayerController GetController()
    {

        var lmPlayer = new LM_PlayerController();

        // Handle the selected UI enum from the inspector
        if (config.ui != "default")
        {
            switch (config.ui)
            {
                case "Desktop":
                    userInterface = UserInterface.KeyboardMouse;
                    break;
                case "Vive Virt.":
                    userInterface = UserInterface.ViveVirtualizer;
                    break;
                case "Vive Std.":
                    userInterface = UserInterface.ViveRoomspace;
                    break;
                default:
                    userInterface = UserInterface.KeyboardMouse;
                    break;
            }
        }

        // Based on the UserInterface enum that was selected, get the player
        switch (userInterface)
        {
            case UserInterface.KeyboardMouse :
                lmPlayer = GameObject.Find("DesktopDefaultController").GetComponent<LM_PlayerController>();

                break;

            case UserInterface.ViveRoomspace :
                lmPlayer = GameObject.Find("ViveRoomspaceController").GetComponent<LM_PlayerController>();

                break;

            case UserInterface.ViveVirtualizer:
            
                lmPlayer = GameObject.Find("ViveVirtualizerController").GetComponent<LM_PlayerController>();

                if (lmPlayer = null)
                {
                    Debug.LogWarning("The proprietary ViveVirtualizerController asset cannot be found");
                    goto default;
                }

                break;

            default:
                lmPlayer = GameObject.Find("DesktopDefaultController").GetComponent<LM_PlayerController>();

                break;

        }

        return lmPlayer;

    }

}
