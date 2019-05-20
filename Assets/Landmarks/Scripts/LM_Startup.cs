using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class LM_Startup : MonoBehaviour
{
    public GUISkin skin;
    private string subID = "";
    private string errID;
    private string subAge = "";
    public string levelName = "error";
    private Config config;
    private string appDir = "";
    private bool dirCreated = false;
    private bool dirError = false;
    private bool nameError = false;
    public bool reorient;

    void OnGUI()
    {
        GUI.skin = skin;

        GUILayout.BeginArea(new Rect(0f, 0f, Screen.width, Screen.height));
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Please Enter Subject ID:");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        subID = GUILayout.TextField(subID, 25, GUILayout.Width(150));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Please Enter Subject Age:");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        subAge = GUILayout.TextField(subAge, 25, GUILayout.Width(150));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Begin Experiment") || Input.GetKeyDown(KeyCode.Return))
        {
            errID = subID;
            StartLevel();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        if (nameError == true)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Question list does not exist or is out of range");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Try adding 's' before the subID");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        else if (dirError == true)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("The ID " + errID + " is already in use. Try again.");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    void Start()
    {

        config = Config.instance;
        appDir = Directory.GetCurrentDirectory();
        if (!Directory.Exists(appDir + "/data/" + levelName))
        {
            Directory.CreateDirectory(appDir + "/data/" + levelName);
        }
    }

    public void StartLevel()
    {
        readyConfig();
        if (dirError != true && nameError != true)
        {
            if (reorient == true)
            {
                PlayerPrefs.SetString("levelName", levelName);
                PlayerPrefs.SetString("subID", subID);
                //Application.LoadLevel("reorient");
            }
            else
            {
                //Application.LoadLevel(levelName);
            }
        }
    }

    void readyConfig()
    {
        config.runMode = ConfigRunMode.NEW;
        config.bootstrapped = true;

        if (subID.Substring(1, 1) == "3" || subID.Substring(1, 1) == "4")
        {
            levelName = "videOS_v5";
        }
        else if (subID.Substring(1, 1) == "1")
        {
            levelName = "videOS_desktopVR";
        }
        else if (subID.Substring(1, 1) == "5")
        {
            levelName = "sheltonville";
        }
        else
        {
            print("There is an error finding which level to load");
        }

        config.expPath = appDir + "/data/" + levelName;
        config.subjectPath = appDir + "/data/" + levelName + "/" + subID;

        config.appPath = appDir;
        config.level = levelName;
        config.subject = subID;



        if (!File.Exists(appDir + "/Assets/Resources/list_bigCityCyberith_" + subID + ".txt"))
        {
            nameError = true;
        }
        else
        {
            nameError = false;
            if (!Directory.Exists(appDir + "/data/" + levelName + "/" + subID))
            {
                dirError = false;
                Directory.CreateDirectory(appDir + "/data/" + levelName + "/" + subID);
                dirCreated = true;
            }
            else
            {
                dirError = true;
                Start();
            }
        }
    }
}
