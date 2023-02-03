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
using System.IO;

public enum ConfigRunMode
{
    NEW,
    CONTINUE,
    RESUME,
    PLAYBACK,
    DEBUG
}


/// Config is a singleton.
/// To avoid having to manually link an instance to every class that needs it, it has a static property called
/// instance, so other objects that need to access it can just call:
///        Config.instance.DoSomeThing();
///
public class Config : MonoBehaviour
{

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
    [Tooltip("Treat the first n scenes in the build list as practice and run them first")]
    public int practiceTrialCount;
    public List<string> levelNames = new List<string>();
    public bool randomSceneOrder;
    [Tooltip("Read Only: Use as index for scence/condition")]
    public int levelNumber;
    public bool appendLogFiles = false;

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

    private void Awake()
    {

    }

    // This defines a static instance property that attempts to find the config object in the scene and
    // returns it to the caller.
    public static Config Instance
    {
        get
        {
            // Look for a preconfigured Config
            if (FindObjectOfType<Config>() != null)
            {
                s_Instance = FindObjectOfType<Config>();
                Debug.Log("Using an existing config object, " + s_Instance.gameObject.name);
            }
            // If there isn't one, create an instance
            else
            {
                GameObject obj = new GameObject("Config");
                s_Instance = obj.AddComponent(typeof(Config)) as Config;
                Debug.Log("Could not locate an Config object.  Config was Generated Automaticly.");
            }

            if (!s_Instance.initialized) s_Instance.Initialize(s_Instance);

            return s_Instance;
        }
    }

    // Ensure that the instance is destroyed when the game is stopped in the editor.
    void OnApplicationQuit()
    {
        s_Instance = null;
        PlayerPrefs.SetInt("Screenmanager Is Fullscreen mode", 0);
        PlayerPrefs.SetInt("Screenmanager Resolution Width", 968);
        PlayerPrefs.SetInt("Screenmanager Resolution Height", 768);
    }

    public void Initialize(Config config)
    {
        Debug.Log("Initializing the Config");

        var lvls = new List<string>();
        // If no levels are added/specified and we're in the editor, just add the open one
        if (config.levelNames.Count == 0 && Application.isEditor)
        {
            lvls.Add(SceneManager.GetActiveScene().name);
        }
        else
        {
            // Loop through scenes added to the build and cross-reference with config.levelnames
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                var lvl = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
                if (config.levelNames.Contains(lvl))
                {
                    lvls.Add(lvl);
                    config.levelNames.Remove(lvl);
                }
            }

            /* At this point, any extant levelnames should be in our lvls buffer and any names left in
             * config.levelnames is not in a scene in the build.
             * So, we'll reverse the process and for each item in config.levelNames check for an asterisk indicating
             * a wildcard search for levelnames. We'll remove it if it's just a bad level name with no asterisk
             * but search it using wildcard otherwise
             */
            if (config.levelNames.Count > 0)
            {
                
                foreach (var name in config.levelNames)
                {
                    if (name.Contains("*"))
                    {
                        var search = name.Replace("*", string.Empty);
                        Debug.Log("Searching for '" + search + "'");
                        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
                        {
                            var lvl = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
                            Debug.Log("\t ... in " + lvl);
                            if (lvl.Contains(search))
                            {
                                lvls.Add(lvl);
                            }
                        }

                    }
                    //config.levelNames.Remove(name);
                }
            }
        }

        config.levelNames.Clear();
        foreach (var lvl in lvls)
        {
            config.levelNames.Add(lvl);
        }

        // If we are running the scenes in a randomized order
        if (randomSceneOrder)
        {
            for (int i = 0; i < config.levelNames.Count; i++)
            {
                if (i >= practiceTrialCount)
                {
                    Debug.Log("Shuffling");
                    var temp = config.levelNames[i]; // grab the ith object
                    int randomIndex = UnityEngine.Random.Range(i, config.levelNames.Count); // random index between i and end of list
                    config.levelNames[i] = config.levelNames[randomIndex]; // replace ith element with the random element...
                    config.levelNames[randomIndex] = temp; // and swap the ith element into the random element's spot, continue on up
                }
                else Debug.Log("Not Shuffling");
            }
        }

        // if no conditions are specified, add a single entry called default
        if (conditions.Count == 0)
        {
            conditions.Add("default");
        }

        config.initialized = true;

        DontDestroyOnLoad(gameObject);
    }

    public void CheckConfig()
    {
        // make sure there are an equal number of conditions and levels (fill with "default" or trim)
        while (conditions.Count < levelNames.Count)
        {
            conditions.Add("default");
        }
        while (conditions.Count > levelNames.Count)
        {
            conditions.RemoveAt(conditions.Count - 1);
        }
    }

}

