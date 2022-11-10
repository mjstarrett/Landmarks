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
using VRStandardAssets.Utils;

public class HUD : MonoBehaviour
{

	private GameObject experiment;
	private dbLog log;
	private Experiment manager;

    // MJS - Identify components of the HUD
	public GameObject hudRig; // canvas over screen space for clean text presentation (parent)
	public GameObject hudPanel; // panel used to proved contrasting/anchoring background for hud text (child of hudRig)
	public GameObject Canvas; // canvas containing the text element for hud messages (child of hudRig)
	public float hudPanelON = 0.95f; // set the opacity when on
	public float hudPanelOFF = 0f; // set the opacity when off
	private string canvasName;
    public GameObject actionButton; // button that subjects use to interact with the game (if necessary);
    [HideInInspector] public bool actionButtonClicked = false;
    public GameObject debugButton; // button that can be used to force continue in debug mode;
    public GameObject confidenceSlider; // slider that can be used by any task for confidence judements

    public GameObject hudNonEssentials; // objects that can be turned off unless specifically needed if empty, a camera overlay hud will be assumed (as in desktop)

    public Camera[] cam;
	public int hudLayer = 13;
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

	private Text timeGui;
	private Text FPSgui;
	private Text statusGui;
	private Text statusGuiBack;
	private Text messageGui;
	private Text messageGuiBack;
	private Text scoreGui;
	private Text scoreGuiBack;
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
        //Debug.Log ("Starting HUD.cs");
        //canvasName = Canvas.name;
        //Debug.Log ("Canvas Name: " + canvasName);
        this.actionButton.GetComponent<Button>().onClick.AddListener(OnActionClick);
	}


	void Start()
	{
		lastInterval = Time.realtimeSinceStartup;
		frames = 0;
		intensity = 1.0f;
		score = 0;

		experiment = GameObject.FindWithTag("Experiment");
		manager = experiment.GetComponent("Experiment") as Experiment;
		log = manager.dblog;
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
			sgo.AddComponent<Text>();
			sgo.hideFlags = HideFlags.HideAndDontSave;
			sgo.transform.position = new Vector3(0, 0, 0);
			timeGui = sgo.GetComponent<Text>();
			//timeGui.pixelOffset = new Vector2(10,30);
			//timeGui.font = hudFont;
			timeGui.fontSize = 24;
			//timeGui.material.color = statusColor;
		}
		if (showTimestamp) timeGui.text = playback_time.ToString("f0");

	}


	// ---------------------------------------------------
	// Public Methods to manipulate the HUD message
	// ---------------------------------------------------

	public string GetMessage()
    {
		return message;
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


	// ---------------------------------------------------
	// Methods to manipulate rendering/culling
	// ---------------------------------------------------

	public void showEverything()
	{
		this.enabled = true;
		cam[0].cullingMask = 0 << hudLayer;
		cam[1].cullingMask = 0 << hudLayer;
		for (var i = 0; i < hudLayer; ++i)
		{
			cam[0].cullingMask = cam[0].cullingMask + (1 << i);
			cam[1].cullingMask = cam[1].cullingMask + (1 << i);
		}
		cam[0].clearFlags = CameraClearFlags.Skybox;
		cam[1].clearFlags = CameraClearFlags.Skybox;
	}

	public void showNothing()
	{
		cam[0].cullingMask = (0 << 30);
		cam[1].cullingMask = (0 << 30);
		cam[0].cullingMask = cam[0].cullingMask + (0 << 30);
		cam[1].cullingMask = cam[1].cullingMask + (0 << 30);

		cam[0].clearFlags = CameraClearFlags.SolidColor;
		cam[1].clearFlags = CameraClearFlags.SolidColor;
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

    public void showOnlyTargets()
    {
		cam[0].cullingMask = 1 << LayerMask.NameToLayer("Targets") | 1 << hudLayer;
        cam[1].cullingMask = 1 << LayerMask.NameToLayer("Targets") | 1 << hudLayer;
        cam[0].cullingMask = cam[0].cullingMask + (1 << 0);
        cam[1].cullingMask = cam[1].cullingMask + (1 << 0);

        cam[0].clearFlags = CameraClearFlags.SolidColor;
        cam[1].clearFlags = CameraClearFlags.SolidColor;
    }


	// ---------------------------------------------------
	// Other public methods
	// ---------------------------------------------------

	public void portHoleVertOn()
	{
		fullScreenFOV = Camera.main.fieldOfView;
		Camera.main.rect = new Rect(.35f, 0f, .3f, 1f);
		//		cam.depth = Camera.main.depth + 1;
	}

	public void portHoleHorzOn()
	{
		fullScreenFOV = Camera.main.fieldOfView;
		Camera.main.rect = new Rect(0f, .35f, 1f, .3f);
		Camera.main.fieldOfView = fullScreenFOV * Camera.main.rect.height;
	}

	public void portHoleOff()
	{
		Camera.main.rect = new Rect(0f, 0f, 1f, 1f);
		Camera.main.fieldOfView = fullScreenFOV;
	}

	// MJS - Moved from Experiment Task for VR functionality - March 2019
	public void OnActionClick()
	{
		actionButtonClicked = true;
	}


	// ---------------------------------------------------
	// Private Methods
	// ---------------------------------------------------

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

	

	void updateMessage()
	{
		if (!statusGui)
	    {
    		GameObject sgo = new GameObject("Message Display");
    		sgo.AddComponent<Text>();

			sgo.hideFlags = HideFlags.HideAndDontSave;
			sgo.transform.position = new Vector3(0,0,0);
			messageGui = sgo.GetComponent<Text>();
			//messageGui.pixelOffset = new Vector2( 20, Screen.height - 2);
   			messageGui.font = hudFont;
   			messageGui.material.color = statusColor;
	    }

	    if (!messageGuiBack)
	    {
    		GameObject sgo2 = new GameObject("Message Display Back");
    		sgo2.AddComponent<Text>();
			sgo2.hideFlags = HideFlags.HideAndDontSave;
			sgo2.transform.position = new Vector3(0,0,0);
   			messageGuiBack = sgo2.GetComponent<Text>();
			//messageGuiBack.pixelOffset = new Vector2( 20, Screen.height - 3);
   			messageGuiBack.font = hudFont;
   			messageGuiBack.material.color = Color.black;
	    }

        // if time is up, temporarily make the message an empty string
        var hidemessage = ((DateTime.Now - LastShown) > TimeSpan.FromSeconds(SecondsToShow));
        GameObject avatar = manager.player.GetComponent<HUD>().Canvas as GameObject;
        Text canvas = avatar.GetComponent<Text>();
        canvas.text = hidemessage ? string.Empty : message;

        // if this happens, dim the background panel too
        if (hidemessage)
		{
			Color panelTemp = hudPanel.GetComponent<Image> ().color;
			panelTemp.a = hudPanelOFF;
			hudPanel.GetComponent<Image> ().color = panelTemp;

            // if we're using an external wall or screen for the hud (fixed position), turn it off as well.
            if (hudNonEssentials != null)
            {
                foreach (Transform child in hudNonEssentials.transform)
                {
                    child.gameObject.layer = LayerMask.NameToLayer("Default");
                }
            }
        }
		else
		{
			Color panelTemp = hudPanel.GetComponent<Image>().color;
			panelTemp.a = hudPanelON;
			hudPanel.GetComponent<Image> ().color = panelTemp;

            // if we're using an external wall or screen for the hud (fixed position), Make sure it's active.
            if (hudNonEssentials != null)
            {
                foreach (Transform child in hudNonEssentials.transform)
                {
                    child.gameObject.layer = LayerMask.NameToLayer("HUD only");
                }
            }
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
    		sgo.AddComponent<Text>();
			sgo.hideFlags = HideFlags.HideAndDontSave;
			sgo.transform.position = new Vector3(0,0,0);
			statusGui = sgo.GetComponent<Text>();
			//statusGui.pixelOffset = new Vector2(5,75);
   			statusGui.font = hudFont;
   			statusGui.fontSize = 24;
   			statusGui.material.color = statusColor;
	    }

	    if (!statusGuiBack)
	    {
    		GameObject sgo2 = new GameObject("Status Display Back");
    		sgo2.AddComponent<Text>();
			sgo2.hideFlags = HideFlags.HideAndDontSave;
			sgo2.transform.position = new Vector3(0,0,0);
   			statusGuiBack = sgo2.GetComponent<Text>();
			//statusGuiBack.pixelOffset = new Vector2(5,74);
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
    		sgo.AddComponent<Text>();
			sgo.hideFlags = HideFlags.HideAndDontSave;
			sgo.transform.position = new Vector3(0,0,0);
			scoreGui = sgo.GetComponent<Text>();
			//scoreGui.pixelOffset = new Vector2(Screen.width - 100,Screen.height - 2);
   			scoreGui.font = hudFont;
   			scoreGui.material.color = statusColor;
	    }

	    if (!scoreGuiBack)
	    {
    		GameObject sgo2 = new GameObject("Score Display Back");
    		sgo2.AddComponent<Text>();
			sgo2.hideFlags = HideFlags.HideAndDontSave;
			sgo2.transform.position = new Vector3(0,0,0);
   			scoreGuiBack = sgo2.GetComponent<Text>();
			//scoreGuiBack.pixelOffset = new Vector2(Screen.width - 100,Screen.height - 3);
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
				go.AddComponent<Text>();
				go.hideFlags = HideFlags.HideAndDontSave;
				go.transform.position = new Vector3(0,0,0);
				FPSgui = go.GetComponent<Text>();
				//FPSgui.pixelOffset = new Vector2(Screen.width - 130,20);
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

	public void ReCenter(Transform t = default(Transform))
    {
		// Unless provided, recenter on whatever is colliding with things (the LM_Player collisionObject)
		// Useful for player controllers such as ViveRoomspace, where parent object tagged as player does not control movement
		if (t == default(Transform)) t = manager.player.GetComponent<LM_PlayerController>().collisionObject.transform;

		// Moves and orient the HUD
		hudRig.transform.position = new Vector3(t.position.x, hudRig.transform.position.y, t.position.z); // position
		hudRig.transform.eulerAngles = new Vector3(hudRig.transform.eulerAngles.x, t.eulerAngles.y, hudRig.transform.eulerAngles.z); // rotation

    }

}
