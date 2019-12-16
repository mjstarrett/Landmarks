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

    public string experiment = "default";
	public string home = "default";
	public string appPath = "default";
	public string expPath = "default";
	public string subjectPath = "default";
	public string subject = "default";
	public string session = "default";
	public string level = "default";
    public string condition = "default";
    public string ui = "default";

	public ConfigRunMode 	runMode = ConfigRunMode.NEW;
	[HideInInspector] public bool 	bootstrapped = false;
	

    // s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.	
	private static Config s_Instance = null;
	  
    // This defines a static instance property that attempts to find the config object in the scene and
    // returns it to the caller.
	public static Config instance {
        get {
            if (s_Instance == null) {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first Config object in the scene.
                s_Instance =  FindObjectOfType(typeof (Config)) as Config;
            }
            
            // If it is still null, create a new instance
            if (s_Instance == null) {
                GameObject obj = new GameObject("Config");
                s_Instance = obj.AddComponent(typeof (Config)) as Config;
                Debug.Log ("Could not locate an Config object.  Config was Generated Automaticly.");
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
    
    public void test() {
    	Debug.Log("test!!!");
    }
}

