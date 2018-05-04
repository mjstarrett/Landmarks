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

public enum EndListMode
{
	Loop,
	End
}


public class Experiment : MonoBehaviour {

	public TaskList tasks;
	private Config config;
	private long microseconds = 1;
	private string logfile;
	private string configfile = "";
	public GameObject player;


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
		
		Cursor.visible = false;
		//since config is a singleton it will be the one created in scene 0 or this scene
		config = Config.instance;

		// MJS - 7/8/2016 - added if loop to check for COVR then use regular if missing or off
		avatar = player;
//		if (VR == true){
//			avatar = GameObject.Find("COVRPlayerController"); // MJS - this is the original line of code
//		} else {
//			avatar = GameObject.Find ("First Person Controller");
//		}
		////////////////////////////////////////////////
		
		hud = avatar.GetComponent("HUD") as HUD;

		logfile = config.subjectPath + "/test.log";
		configfile = config.expPath + "/" + config.filename;

		hud.showOnlyHUD();
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
		} else if (config.runMode == ConfigRunMode.NEW) {
			//dblog = new dbMockLog(logfile);
		}
		
		//start session


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
				
	void OnApplicationQuit() {
		if (config.runMode != ConfigRunMode.PLAYBACK) {
			tasks.endTask();
		}
		dblog.close();
		Cursor.visible = true;
	}
	
		void OnGUI () {
		
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
