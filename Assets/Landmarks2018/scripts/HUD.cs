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
using UnityEngine.UI;
using System;

public class HUD : MonoBehaviour 
{
	
	private GameObject experiment;
	private dbLog log;
	private Experiment manager;

	// MJS - Identify components of the HUD 
	public GameObject hudRig; // canvas over screen space for clean text presentation (parent)
	public GameObject Canvas; // canvas containing the text element for hud messages (child of hudRig)
	public GameObject hudPanel; // panel used to proved contrasting/anchoring background for hud text (child of hudRig)
	public Button actionButton; // button that subjects use to interact with the game (if necessary);
	public Button debugButton; // button that can be used to force continue in debug mode;
	public float hudPanelON = 0.95f; // set the opacity when on
	public float hudPanelOFF = 0f; // set the opacity when off
	private string canvasName;

	public Camera[] cam;
	public int hudLayer = 30;
	public Color statusColor;
	public Font hudFont;
	public bool showFPS;
	public bool showTimestamp = false;
	public bool showStatus;
	public bool showScore;
	private float intensity =  0.0f;
	
	private string message = "";
	private string compassmessage = "";
	
	private string status = "";
	private int score = 0;
	
	private GUIText timeGui;
	private GUIText FPSgui;
	private GUIText statusGui;
	private GUIText statusGuiBack;
	private GUIText messageGui;
	private GUIText messageGuiBack;
	private GUIText scoreGui;
	private GUIText scoreGuiBack;
	public float fadeSpeed = 0.05f;
	private Text canvan;
	private float updateInterval = 1.0f;
	private float lastInterval; // Last interval end time
	private int frames = 0; // Frames over current interval

	private float fullScreenFOV;
	
	DateTime LastShown = DateTime.Now;
	public int SecondsToShow = 0;
	public int GeneralDuration = 10;
	public int InstructionDuration = 99999; // MJS - allow different duration for instructions tasks

	[HideInInspector] public long playback_time = 0;
	
	public void Awake()
	{
		SecondsToShow = GeneralDuration;
		Debug.Log ("Starting HUD.cs");
		canvasName = Canvas.name;
		Debug.Log ("Canvas Name: " + canvasName);
	}
	public void setMessage(string newMessage)
	{
		message = newMessage;
	}
	
	public void setMessageCompass(string newMessage)
	{
	print ("setMessageCompass called!!!!");
		if (newMessage==""){
			Canvas.SetActive (false);
		}else{
			Canvas.SetActive (false);
		}
		message = newMessage;
		compassmessage = newMessage;
	}
	
	public void ForceShowMessage()
	{
		LastShown = DateTime.Now;
	}
	
	public void setScore(int newScore)
	{
		score = newScore;
		log.log("SET_SCORE	" + score,2 );
	}
		
	public void flashStatus( string newStatus)
	{
		status = newStatus;
		intensity = 1.0f;
	}
	
	public void showOnlyHUD()
	{
		cam[0].cullingMask = (1 << hudLayer);
		cam[1].cullingMask = (1 << hudLayer);
		cam[0].cullingMask = cam[0].cullingMask + (1 << 0);
		cam[1].cullingMask = cam[1].cullingMask + (1 << 0);

		cam[0].clearFlags = CameraClearFlags.SolidColor;
		cam[1].clearFlags = CameraClearFlags.SolidColor;
	}
	
	public void showEverything()
	{
	this.enabled = true;
		cam[0].cullingMask = 0 << hudLayer;
		cam[1].cullingMask = 0 << hudLayer;
		for (var i = 0; i < hudLayer; ++i) {
			cam[0].cullingMask = cam[0].cullingMask + (1 << i);
			cam[1].cullingMask = cam[1].cullingMask + (1 << i);
		}
		cam[0].clearFlags = CameraClearFlags.Skybox;
		cam[1].clearFlags = CameraClearFlags.Skybox;
	}
	
	void Start()
	{
	    lastInterval = Time.realtimeSinceStartup;
	    frames = 0;
	    intensity = 1.0f;  
	    score = 0; 
	    
		experiment = GameObject.FindWithTag ("Experiment");
	    manager = experiment.GetComponent("Experiment") as Experiment;
	    log = manager.dblog;
	
	
	}
	
	
	void OnDisable ()
	{
		if (FPSgui)
			DestroyImmediate (FPSgui.gameObject);
		if (statusGui)
			DestroyImmediate (statusGui.gameObject);
		if (statusGuiBack)
			DestroyImmediate (statusGuiBack.gameObject);	
		if (scoreGui)
			DestroyImmediate (scoreGui.gameObject);	
		if (scoreGuiBack)
			DestroyImmediate (scoreGuiBack.gameObject);	
	    if (messageGui)
			DestroyImmediate (messageGui.gameObject);	
		if (messageGuiBack)
			DestroyImmediate (messageGuiBack.gameObject);
		if (timeGui)
			DestroyImmediate (timeGui.gameObject);
		setMessage("");		
	}
	
	void Update()
	{
	    updateFPS();
		updateMessage();
		updateStatus();
		updateScore();
		
		if (Input.GetButtonDown("showHUD"))
			ForceShowMessage();
		
		if (!timeGui)
	    {	       			
    		GameObject sgo = new GameObject("Timecode Display");
    		sgo.AddComponent<GUIText>();
			sgo.hideFlags = HideFlags.HideAndDontSave;
			sgo.transform.position = new Vector3(0,0,0);
			timeGui = sgo.GetComponent<GUIText>();
			timeGui.pixelOffset = new Vector2(10,30);
   			//timeGui.font = hudFont;
   			timeGui.fontSize = 24;
   			//timeGui.material.color = statusColor;	   			
	    }
	    if (showTimestamp) timeGui.text = playback_time.ToString("f0");
	    
	}
	
	void updateMessage()
	{
		if (!statusGui)
	    {
    		GameObject sgo = new GameObject("Message Display");
    		sgo.AddComponent<GUIText>();
 
			sgo.hideFlags = HideFlags.HideAndDontSave;
			sgo.transform.position = new Vector3(0,0,0);
			messageGui = sgo.GetComponent<GUIText>();
			messageGui.pixelOffset = new Vector2( 20, Screen.height - 2);
   			messageGui.font = hudFont;
   			messageGui.material.color = statusColor;   			
	    }
	    
	    if (!messageGuiBack)
	    {    	
    		GameObject sgo2 = new GameObject("Message Display Back");
    		sgo2.AddComponent<GUIText>();
			sgo2.hideFlags = HideFlags.HideAndDontSave;
			sgo2.transform.position = new Vector3(0,0,0);
   			messageGuiBack = sgo2.GetComponent<GUIText>();
			messageGuiBack.pixelOffset = new Vector2( 20, Screen.height - 3);
   			messageGuiBack.font = hudFont;
   			messageGuiBack.material.color = Color.black;
	    }
	    
		// if time is up, temporarily make the message an empty string
	    var hidemessage = ((DateTime.Now - LastShown) > TimeSpan.FromSeconds(SecondsToShow));
	    GameObject avatar = GameObject.FindWithTag("HUDtext");
		Text canvas = avatar.GetComponent<Text>();
		canvas.text = hidemessage ? string.Empty : message; 
		// if this happens, dim the background panel too
		if (hidemessage) 
		{
			Color panelTemp = hudPanel.GetComponent<Image> ().color; 
			panelTemp.a = hudPanelOFF;
			hudPanel.GetComponent<Image> ().color = panelTemp;		}
		else
		{
			Color panelTemp = hudPanel.GetComponent<Image>().color; 
			panelTemp.a = hudPanelON;
			hudPanel.GetComponent<Image> ().color = panelTemp;
		}


		messageGuiBack.text = hidemessage ? string.Empty : message;

	    messageGui.enabled = showStatus;
	    messageGuiBack.enabled = showStatus;	
	}
	
	void updateStatus()
	{
		if (!statusGui)
	    {	       			
    		GameObject sgo = new GameObject("Status Display");
    		sgo.AddComponent<GUIText>();
			sgo.hideFlags = HideFlags.HideAndDontSave;
			sgo.transform.position = new Vector3(0,0,0);
			statusGui = sgo.GetComponent<GUIText>();
			statusGui.pixelOffset = new Vector2(5,75);
   			statusGui.font = hudFont;
   			statusGui.fontSize = 24;
   			statusGui.material.color = statusColor;	   			
	    }
	    
	    if (!statusGuiBack)
	    {    	
    		GameObject sgo2 = new GameObject("Status Display Back");
    		sgo2.AddComponent<GUIText>();
			sgo2.hideFlags = HideFlags.HideAndDontSave;
			sgo2.transform.position = new Vector3(0,0,0);
   			statusGuiBack = sgo2.GetComponent<GUIText>();
			statusGuiBack.pixelOffset = new Vector2(5,74);
   			statusGuiBack.font = hudFont;
   			statusGuiBack.fontSize = 24;
   			statusGuiBack.material.color = Color.black;
	    }
	    
	    statusGui.text = status;
	    statusGuiBack.text = status;
	    intensity -= fadeSpeed * Time.deltaTime;
	    if (intensity < 0.0 ) intensity = 0.0f;
	    
	    Color c = statusGui.material.color;
	    c.a = intensity;
	    statusGui.material.color = c;
	    statusGui.enabled = showStatus;
	    
	    c = statusGuiBack.material.color;
	    c.a = intensity;
	    statusGuiBack.material.color = c;
	    statusGuiBack.enabled = showStatus;	
	}	
	
	void updateScore()
	{
		if (!scoreGui)
	    {	       			
    		GameObject sgo = new GameObject("Score Display");
    		sgo.AddComponent<GUIText>();
			sgo.hideFlags = HideFlags.HideAndDontSave;
			sgo.transform.position = new Vector3(0,0,0);
			scoreGui = sgo.GetComponent<GUIText>();
			scoreGui.pixelOffset = new Vector2(Screen.width - 100,Screen.height - 2);
   			scoreGui.font = hudFont;
   			scoreGui.material.color = statusColor;	   			
	    }
	    
	    if (!scoreGuiBack)
	    {    	
    		GameObject sgo2 = new GameObject("Score Display Back");
    		sgo2.AddComponent<GUIText>();
			sgo2.hideFlags = HideFlags.HideAndDontSave;
			sgo2.transform.position = new Vector3(0,0,0);
   			scoreGuiBack = sgo2.GetComponent<GUIText>();
			scoreGuiBack.pixelOffset = new Vector2(Screen.width - 100,Screen.height - 3);
   			scoreGuiBack.font = hudFont;
   			scoreGuiBack.material.color = Color.black;
	    }
	    
	    scoreGui.text = score.ToString();
	    scoreGuiBack.text = score.ToString();

	    scoreGui.enabled = showScore;
	    scoreGuiBack.enabled = showScore;
	}
		
	void updateFPS()
	{
		++frames;
	    float timeNow = Time.realtimeSinceStartup;
	    if (timeNow > lastInterval + updateInterval)
	    {
			if (!FPSgui)
			{
				GameObject go = new GameObject("FPS Display");
				go.AddComponent<GUIText>();
				go.hideFlags = HideFlags.HideAndDontSave;
				go.transform.position = new Vector3(0,0,0);
				FPSgui = go.GetComponent<GUIText>();
				FPSgui.pixelOffset = new Vector2(Screen.width - 130,20);
				//gui.font = hudFont;
			}
	        float fps = frames / (timeNow - lastInterval);
			float ms = 1000.0f / Mathf.Max (fps, 0.00001f);
			FPSgui.text = ms.ToString("f1") + "ms " + fps.ToString("f2") + "FPS";
	        frames = 0;
	        lastInterval = timeNow;
	        FPSgui.enabled = showFPS;
	    }
	}




	
	public void portHoleVertOn()
	{
		fullScreenFOV=Camera.main.fieldOfView;
		Camera.main.rect = new Rect(.35f, 0f, .3f, 1f);
		//		cam.depth = Camera.main.depth + 1;
	}
	
	
	
	public void portHoleHorzOn()
	{
		
		fullScreenFOV=Camera.main.fieldOfView;
		Camera.main.rect = new Rect(0f, .35f, 1f, .3f);
		Camera.main.fieldOfView = fullScreenFOV  * Camera.main.rect.height;
	}
	
	public void portHoleOff()
	{
		Camera.main.rect = new Rect(0f, 0f, 1f, 1f);
		Camera.main.fieldOfView = fullScreenFOV;
		
	}
	
	
}