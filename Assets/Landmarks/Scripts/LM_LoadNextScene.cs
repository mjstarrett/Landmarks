using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LM_LoadNextScene : ExperimentTask
{

    public override void startTask()
    {
        TASK_START();

    }


    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();


        // Grab any info about loading next scenes or conditions from PlayerPrefs(X)
        var nextIndex = PlayerPrefs.GetInt("NextIndex");
        var nextLevels = PlayerPrefsX.GetStringArray("NextLevels");
        var nextConditions = PlayerPrefsX.GetStringArray("NextConditions");

        // If we have a next task/condition, update it
        if (nextIndex < nextLevels.Length)
        {
            manager.config.level = nextLevels[nextIndex];
            var levelname = manager.config.level; // save from destruction
            manager.config.condition = nextConditions[nextIndex];

            nextIndex++;
            PlayerPrefs.SetInt("NextIndex", nextIndex);

            //destroy the current landmarks structure
            Destroy(GameObject.Find("_Landmarks_"));

            SceneManager.LoadScene(levelname);

        }
        else
        {
            this.skip = true;
        }
    }


    public override bool updateTask()
    {
        if (skip)
        {
            //log.log("INFO    skip task    " + name,1 );
            return true;
        }
        return true;
    }


    public override void endTask()
    {
        TASK_END();
    }


    public override void TASK_END()
    {
        base.endTask();
    }

}

