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
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public enum ConfigRunMode
{
	NEW,
	RESUME,
	PLAYBACK,
	DEBUG
}


 /// Config is a singleton.
 /// To avoid having to manually link an instance to every class that needs it, it has a static property called
 /// instance, so other objects that need to access it can just call:
 ///        Config.instance.DoSomeThing();
 ///
public class Config : MonoBehaviour{

	public float version;
	public int width = 1024;
	public int height = 768;
	public float volume = 1.0F;
	public bool nofullscreen = false;
	public bool showFPS = false;
	public string filename = "config.txt";
    [Tooltip("Experiment names can contain only lowercase letters, numbers, and hyphens, and must begin and end with a letter or a number. The name can't contain two consecutive hyphens.")]
    public string experiment = "default";
    public string ui = "default";
    public ConfigRunMode runMode = ConfigRunMode.NEW;
    public List<string> conditions = new List<string>();
    [Tooltip("Must be scene objects with the .unity file extension")]
    //public List<Object> levelObjects = new List<Object>();
    public List<string> levelNames = new List<string>();
    [Tooltip("Read Only: Use as index for scence/condition")]
    public int levelNumber;

	[HideInInspector]
    public bool bootstrapped = false;
    [HideInInspector]
    public string home = "default";
    [HideInInspector]
    public string appPath = "default";
    [HideInInspector]
    public string expPath = "default";
    [HideInInspector]
    public string subjectPath = "default";
    public string subject = "default";
    [HideInInspector]
    public string session = "default";
    [HideInInspector]
    public string level = "default";
    [HideInInspector]
    public string condition = "default";

    // s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.	
    private static Config s_Instance = null;
    public bool initialized;
	  
    // This defines a static instance property that attempts to find the config object in the scene and
    // returns it to the caller.
	public static Config instance {
        get {
            //  FindObjectOfType(...) returns the first Config object in the scene.
            if (s_Instance == null)
            {
                s_Instance =  FindObjectOfType(typeof (Config)) as Config;
                Debug.Log("Using an existing config object");
            }
            
            // If it is still null, create a new instance
            if (s_Instance == null) {
                GameObject obj = new GameObject("Config");
                s_Instance = obj.AddComponent(typeof (Config)) as Config;
                Debug.Log ("Could not locate an Config object.  Config was Generated Automaticly.");
            }

            if (!s_Instance.initialized)
            {
                s_Instance.Initialize(s_Instance);
            }

            return s_Instance;
        }
    }
    
    // Ensure that the instance is destroyed when the game is stopped in the editor.
    void OnApplicationQuit() {
        s_Instance = null;
        PlayerPrefs.SetInt("Screenmanager Is Fullscreen mode", 0);
        PlayerPrefs.SetInt("Screenmanager Resolution Width", 968);
        PlayerPrefs.SetInt("Screenmanager Resolution Height", 768);
    }
	
    void Awake() {
        DontDestroyOnLoad(transform.gameObject);
    }


    public void Initialize(Config config) {
        //Debug.Log("Initializing the Config");

        //// If no levels are specified, just add the current level to the config
        if (config.levelNames.Count == 0)
        {
            Debug.Log("No scenes provided; using current scene only.");
            config.levelNames.Add(SceneManager.GetActiveScene().name);
        }

        // if no conditions are specified, add a single entry called default
        if (config.conditions.Count == 0)
        {
            config.conditions.Add("default");
        }

        //// make sure there are an equal number of conditions and levels (fill with "default" or trim)
        //while (config.conditions.Count < config.levelNames.Count)
        //{
        //    config.conditions.Add("default");
        //}
        //while (config.conditions.Count > config.levelNames.Count)
        //{
        //    config.conditions.RemoveAt(config.conditions.Count - 1);
        //}

        config.initialized = true;
    }
}

