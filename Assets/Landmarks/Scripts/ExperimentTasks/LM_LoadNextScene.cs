//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using Valve.VR;
//using CybSDK;

//public class LM_LoadNextScene : ExperimentTask
//{

//    public override void startTask()
//    {
//        TASK_START();

//    }


//    public override void TASK_START()
//    {
//        if (!manager) Start();
//        base.startTask();

//        if (!skip)
//        {

//            // Grab any info about loading next scenes or conditions from PlayerPrefs(X)
//            var nextLevels = PlayerPrefsX.GetStringArray("NextLevels");
//            var nextConditions = PlayerPrefsX.GetStringArray("NextConditions");

//            // If we have a next task/condition, update it
//            if (nextIndex < nextLevels.Length)
//            {
//                manager.config.level = nextLevels[nextIndex];
//                var levelname = manager.config.level; // save from destruction
//                manager.config.condition = nextConditions[nextIndex];

//                manager.config.sceneIndex++;


//                // Handle the current Landmarks structure to avoid breaking the game on loading the next scene
//                if (avatar.GetComponent<CVirtHapticListener>() != null)
//                {
//                    Destroy(avatar.GetComponent<CVirtHapticListener>()); // There can only be one!
//                }
//                GameObject oldInstance = GameObject.Find("_Landmarks_");
//                oldInstance.name = "OldInstance";
//                GameObject.FindWithTag("Experiment").SetActive(false);
//                GameObject.FindWithTag("Environment").SetActive(false);
//                Destroy(avatar); // particularly important for SteamVR and interaction system; bugs on load

//                // End the timeline and close the log file so there is no data loss/clipping at the end
//                if (manager.config.runMode != ConfigRunMode.PLAYBACK)
//                {
//                    manager.tasks.endTask();
//                }
//                manager.dblog.close();

//                // avoid frame-drop during load forcing to SteamVr compositor by using SteamVR_LoadLevel for VR apps
//                if (vrEnabled)
//                {
//                    SteamVR_LoadLevel.Begin(levelname);
//                    Debug.Log("Loading new VR scene");
//                }
//                else SceneManager.LoadScene(levelname); // otherwise, just load the level like usual

//            }
//        }
//    }


//    public override bool updateTask()
//    {
        
//        return true;
//    }


//    public override void endTask()
//    {
//        TASK_END();
//    }


//    public override void TASK_END()
//    {
//        base.endTask();
//    }

//}

