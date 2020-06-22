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
using System.Threading.Tasks;
using Valve.VR.InteractionSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using Valve.VR;

public enum EndListMode
{
	Loop,
	End
}

//[SerializeField]
public enum UserInterface
{
    KeyboardMouse,
    ViveRoomspace,
    ViveVirtualizer,
    ViveKatwalk
}

public class Experiment : MonoBehaviour {

    public GameObject availableControllers;
    public UserInterface userInterface = UserInterface.KeyboardMouse;
    public GameObject targetObjects;
    // public bool debugging = false;

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
    [HideInInspector]
    public LM_TrialLog trialLogger;
    [HideInInspector]
    public string logfile;
    [HideInInspector]
    public string dataPath;

    private bool playback = false;
	private bool pause = true;
	private bool done = false;
	private long now;
	private Event evt;
	private long playback_start;
	private long playback_offset;
	private long next_time;
	private string[] next_action;
    private string configfile = "";
    private LM_AzureStorage azureStorage;

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

        trialLogger = new LM_TrialLog();

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

        ////when in editor
        //if (Application.isEditor)
        //{
        //    Debug.Log("RUNNING IN THE EDITOR, SAVING IN THE PROJECT");
        //    if (!Directory.Exists(Directory.GetCurrentDirectory() + "/data/tmp"))
        //    {
        //        Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/data/tmp");
        //    }
        //    dataPath = Directory.GetCurrentDirectory() + "/data/tmp/";
        //    logfile = "test.log";
        //    configfile = dataPath + config.filename;
        //}
        //// Otherwise, save data in a true app data path
        //else
        {
            Debug.Log("THIS IS NOT THE EDITOR - SAVING IN PERSISTENTDATAPATH");
            Debug.Log(Application.persistentDataPath);
            dataPath = Application.persistentDataPath +
                        "/" + config.experiment + "/" + config.subject + "/";
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }
            logfile = config.experiment + "_" + config.subject + "_" + config.levelNames[config.levelNumber] + "_" + config.conditions[config.levelNumber] + ".log";


            configfile = dataPath + config.filename;
        }
        Debug.Log("data will be saved at " + dataPath);
        Debug.Log("data will be saved as " + logfile);


        if (config.runMode == ConfigRunMode.NEW) {
			dblog = new dbLog(dataPath + logfile);
		} else if (config.runMode == ConfigRunMode.RESUME) {
            dblog = new dbPlaybackLog(dataPath + logfile);
		} else if (config.runMode == ConfigRunMode.PLAYBACK) {
			CharacterController c = avatar.GetComponent<CharacterController>();
            c.detectCollisions = false;
			dblog = new dbPlaybackLog(dataPath + logfile);
		}

        dblog.log("EXPERIMENT:\t" + PlayerPrefs.GetString("expID") + "\tSUBJECT:\t" + config.subject +
                  "\tSTART_SCENE\t" + config.levelNames[config.levelNumber] + "\tSTART_CONDITION:\t" + config.conditions[config.levelNumber] + "\tUI:\t" + userInterface.ToString(), 1);
    }


    void Start()
    {

        ConfigOverrides.parse(configfile, dblog);
        hud.showFPS = config.showFPS;
        hud.showTimestamp = (config.runMode == ConfigRunMode.PLAYBACK);

        //start experiment
        if (config.runMode != ConfigRunMode.PLAYBACK)
        {
            //Debug.Log("Starting the Experiment");
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

        azureStorage = FindObjectOfType<LM_AzureStorage>();
    }


    async void Update()
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

                // THIS IS WHERE THE EXPERIMENT GET'S SHUT DOWN
                if (done)
                {
                    Cursor.visible = true;
                    await EndScene();
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
    public LM_PlayerController GetController()
    {

        LM_PlayerController lmPlayer;

        // Handle the selected UI enum from the inspector
        if (config.ui != "default")
        {
            switch (config.ui)
            {
                case "KeyboardMouse":
                    userInterface = UserInterface.KeyboardMouse;
                    break;
                case "ViveVirtualizer":
                    userInterface = UserInterface.ViveVirtualizer;
                    break;
                case "ViveRoomspace":
                    userInterface = UserInterface.ViveRoomspace;
                    break;
                case "ViveKatwalk":
                    userInterface = UserInterface.ViveKatwalk;
                    break;
                default:
                    userInterface = UserInterface.KeyboardMouse;
                    break;
            }
        }

        // Based on the UserInterface enum that was selected, get the player
        switch (userInterface)
        {
            case UserInterface.KeyboardMouse:
                lmPlayer = GameObject.Find("KeyboardMouseController").GetComponent<LM_PlayerController>();

                break;

            case UserInterface.ViveRoomspace:
                lmPlayer = GameObject.Find("ViveRoomspaceController").GetComponent<LM_PlayerController>();

                break;

            case UserInterface.ViveVirtualizer:

                try
                {
                    lmPlayer = GameObject.Find("ViveVirtualizerController").GetComponent<LM_PlayerController>();

                    break;
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("The proprietary ViveVirtualizerController asset cannot be found.\n" +
                    	"Are you missing the prefab in your Landmarks project or a reference to the prefab in your scene?");

                    goto default;
                }

            case UserInterface.ViveKatwalk:

								try
								{
										lmPlayer = GameObject.Find("ViveKatwalkController").GetComponent<LM_PlayerController>();

										break;
								}
								catch (Exception ex)
								{
										Debug.LogWarning("The proprietary ViveKatwalkController asset cannot be found.\n" +
											"Are you missing the prefab in your Landmarks project or a reference to the prefab in your scene?");

										goto default;
								}

            default:

                lmPlayer = GameObject.Find("KeyboardMouseController").GetComponent<LM_PlayerController>();
                Debug.LogWarning("Falling back to default controller (" + lmPlayer.name + ").");

                break;

        }

        return lmPlayer;

    }


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


    // MJS - Function to allow for flexible behavior at end of scene
    async Task EndScene()
    {
        // Wrap up any remaining tasks in the experiment
        if (config.runMode != ConfigRunMode.PLAYBACK)
        {
            tasks.endTask();
        }

        // Log out any EEG triggers (if available)
        var eeg = FindObjectOfType<BrainAmpManager>();
        if (eeg != null)
        {
            dblog.log(eeg.LogTriggerIndices(), 1);
        }

        // close the logfile
        dblog.close();

        // Upload data to remote storage if available and configured
        if (azureStorage != null)
        {
            //if (Application.isEditor)
            //{
            //    Debug.Log("Not saving to MICROSOFT AZURE because the experiment was run from the editor");
            //}
            //else
            {
                Debug.Log("trying to use MICROSOFT AZURE");
                await azureStorage.BasicStorageBlockBlobOperationsAsync();
            }
        }


        // ---------------------------------------------------------------------
        // Load the next level/scene/condition or quit
        // ---------------------------------------------------------------------

        //increment the level number (accounting for the zero-base compared to a count (starts with 1)
        config.levelNumber++;
        // If there is another level, load it
        if (config.levelNumber < config.levelNames.Count)
        {
            // Load the next Scene
            if (usingVR)
            {
                // Use steam functions to avoid issues w/ framerate drop
                SteamVR_LoadLevel.Begin(config.levelNames[config.levelNumber]);
                Debug.Log("Loading new VR scene");
            }
            else
            {
                SceneManager.LoadScene(config.levelNames[config.levelNumber]); // otherwise, just load the level like usual
            }
        }
        // Otherwise, close down; we're done
        else
        {
            Application.Quit();
        }
    }


    void OnApplicationQuit()
    {

        Cursor.visible = true;
    }

}
