using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class LM_ExperimentManager_OnlineStudy : MonoBehaviour
{
    [Min(1001)]
    public int firstSubjectId = 1001;
    public string azureConnectionString = string.Empty;
    public bool shuffleSceneOrder = true;

    private Config config;


    void Start()
    {
        // Get the config (dont use Config.Instance() as we need a preconfigured one)
        config = FindObjectOfType<Config>();

        // Don't continue unless a config is found (even in editor)
        if (config == null)
        {
            Debug.LogError("No Config found to autmatically configure");
            return;
        }

        // FIXME
        // ---------------------------------------------------------------------
        // Compute Subject ID (without overwriting data on the Azure storage client
        // ---------------------------------------------------------------------

        //- Get the Azure Storage client and blob information


        //- Access the folder where the data would be stored


        //- if(while) the folder for the current subjectID exists,

                //- increment the subjectID up one integer


        // Once we have a new, unused id...


        // create a folder in the storage and continue


        // Put the subject ID into the config.subject field
        config.subject = firstSubjectId.ToString();


        // ---------------------------------------------------------------------
        // Counterbalance Conditions 
        // ---------------------------------------------------------------------

        // create list of permutaitons (use permuation fucntions from LM_PermutedList.cs)
        var conditionList = LM_PermutedList.Permute(config.conditions, config.conditions.Count);
        List<string> theseConditions = new List<string>();

        int subCode;
        int.TryParse(config.subject, out subCode);
        subCode -= 1000;
        Debug.Log("subcode = " + subCode.ToString());
        // use the subject id multiples to determine condition order
        Debug.Log(conditionList.Count.ToString());
        for (int i = conditionList.Count; i > 0; i--)
        {
            // determine the highest multiple
            if (subCode % i == 0)
            {
                Debug.Log(i.ToString());
                // take this set of conditions based on the multiple used
                foreach (var item in conditionList[i-1])
                {
                    Debug.Log(item.ToString());
                    theseConditions.Add(item);
                }
                break; // return control from this for loop
            }
        }

        config.condition = theseConditions[0];
        var tmp = theseConditions;
        tmp.Remove(theseConditions[0]);
        config.nextConditions = tmp;


        // ---------------------------------------------------------------------
        // Pseudo-Randomize the level/scene order
        // ---------------------------------------------------------------------

        var theseScenes = config.scenes; // temporary variable
        LM_PermutedList.FisherYatesShuffle(theseScenes); // shuffle using function from LM_PermutedList.cs
        config.level = theseScenes[0].ToString();
        theseScenes.Remove(theseScenes[0]);
        foreach (var item in theseScenes)
        {
            config.nextLevels.Add(item.ToString());
        }


        //----------------------------------------------------------------------
        // set up our config for the LM experiment
        //----------------------------------------------------------------------

        config.runMode = ConfigRunMode.NEW;
        config.bootstrapped = true;
        config.appPath = Application.persistentDataPath;
        DontDestroyOnLoad(config);



        //----------------------------------------------------------------------
        // Load the first level
        //----------------------------------------------------------------------
        SceneManager.LoadScene(config.level);

    }

}
