using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class LM_ExperimentManager : MonoBehaviour
{

    public TMP_Dropdown expID;
    public TMP_InputField subID;
    public TMP_Dropdown ui;
    public Button start;
    public Toggle practice;

    private string appDir = "";
    private bool expDirCreated = false;
    private bool subDirCreated = false;
    private Config config;

    private bool expidError = true;
    private bool subidError = true;
    private bool uiError = true;

    void Start()
    {

        config = Config.instance;
        appDir = Directory.GetCurrentDirectory();

        // check if there is a /data/ folder in our project; create if necessary
        if (!Directory.Exists(appDir + "/data/"))
        {
            Directory.CreateDirectory(appDir + "/data/");
        }

        start.onClick.AddListener(LoadExperiment);

    }

    private void Update()
    {
        practice.onValueChanged.AddListener(delegate { TogglePracticeState(practice); });

    }


    //--------------------------------------------------------------------------
    // Validate Exp Id
    //--------------------------------------------------------------------------

    public void ValidateExpID()
    {
        TextMeshProUGUI _errorMessage = expID.transform.Find("Error").GetComponent<TextMeshProUGUI>();

        if (expID.value != 0)
        {
            expidError = false;
            _errorMessage.gameObject.SetActive(false);
        }
        else
        {
            expidError = true;
            _errorMessage.text = "You must provide an Experiment ID.";
            _errorMessage.gameObject.SetActive(true);
        }
    }


    //--------------------------------------------------------------------------
    // Validate Subject ID
    //--------------------------------------------------------------------------

    public void ValidateSubjectID()
    {
        TextMeshProUGUI _errorMessage = subID.transform.Find("Error").GetComponent<TextMeshProUGUI>();

        // check if a subID was even provided
        if (subID.text != "")
        {
            // if so, make sure it's an int
            if (int.TryParse(subID.text, out int _subID))
            {

                // If this id has already been used to save data, flag an error
                if (Directory.Exists(appDir + "/data/" + expID.options[expID.value].text + "/" + subID.text) && !practice.isOn)
                {
                    subidError = true;
                    _errorMessage.text = "That SubjectID is already in use.";
                    _errorMessage.gameObject.SetActive(true);
                }
                else
                {
                    subidError = false;
                    _errorMessage.gameObject.SetActive(false); // then and only then, will we release the flag
                }
            }
            // if the subID is not an int, throw the message to fix
            else
            {
                subidError = true;
                _errorMessage.text = "Subject ID must be an integer.";
                _errorMessage.gameObject.SetActive(true);
            }
        }
        else
        {
            subidError = true;
            _errorMessage.text = "You must provide a Subject ID.";
            _errorMessage.gameObject.SetActive(true);
        }
    }


    //--------------------------------------------------------------------------
    // Validate UI selected (make sure they selected one)
    //--------------------------------------------------------------------------

    public void ValidateUI()
    {
        TextMeshProUGUI _errorMessage = ui.transform.Find("Error").GetComponent<TextMeshProUGUI>();

        if (ui.value != 0)
        {
            uiError = false;
            _errorMessage.gameObject.SetActive(false);
        }
        else
        {
            uiError = true;
            _errorMessage.text = "Please select a UI from the dropdown.";
            _errorMessage.gameObject.SetActive(true);
        }
    }

    public void LoadExperiment()
    {
        TextMeshProUGUI startErrorMessage = start.transform.Find("Error").GetComponent<TextMeshProUGUI>();

        Debug.Log("trying to load experiment");

        ValidateExpID();
        ValidateSubjectID();
        ValidateUI();

        if (!expidError && !subidError && !uiError)
        {
            startErrorMessage.gameObject.SetActive(false);

            // Create the directories if they don't exist
            if (!Directory.Exists(appDir + "/data/" + expID.options[expID.value].text))
            {
                Directory.CreateDirectory(appDir + "/data/" + expID.options[expID.value].text);
                expDirCreated = true;
            }

            if (practice.isOn)
            {
                if (!Directory.Exists(appDir + "/data/" + expID.options[expID.value].text + "/practice"))
                {
                    Directory.CreateDirectory(appDir + "/data/" + expID.options[expID.value].text + "/practice");
                }
            }
            else
            {
                Directory.CreateDirectory(appDir + "/data/" + expID.options[expID.value].text + "/" + subID.text);
                subDirCreated = true;
            }

            readyConfig();
            ReadyExp();
            SceneManager.LoadScene(config.level);
        }
        else
        {
            startErrorMessage.gameObject.SetActive(true);
        }
    }

    // Specific paramerters for loading experiments

    void ReadyExp()
    {

        // ---------------------------------------------------------------------
        // Experiment: ESC return name of city to load based on id number
        // ---------------------------------------------------------------------
        if (expID.options[expID.value].text == "esc")
        {
            int id = int.Parse(subID.text); // parse the id input string to int

            if (practice.isOn)
            {
                config.level = "esc_practice";
            }
            else
            {
                // if it's in the 100 range, load city 01
                if (id > 100 && id < 200)
                {
                    config.level = "esc_city01";
                    PlayerPrefsX.SetStringArray("NextLevels", new string[1] { "esc_city02" }); // using arrayprefs2 allows for multiple 'next' levels
                }
                else if (id > 200 && id < 300)
                {
                    config.level = "esc_city02";
                    PlayerPrefsX.SetStringArray("NextLevels", new string[1] { "esc_city01" }); // using arrayprefs2 allows for multiple 'next' levels
                }
            }

            // if it's odd, start with standard navigation
            if (id % 2 != 0)
            {
                config.condition = "Standard";
                PlayerPrefsX.SetStringArray("NextConditions", new string[1] { "Scaled" });
            }
            else
            {
                config.condition = "Scaled";
                PlayerPrefsX.SetStringArray("NextConditions", new string[1] { "Standard" });
            }

            PlayerPrefs.SetInt("NextIndex", 0);
        }
        // ---------------------------------------------------------------------




        else config.level = "simpleSample_gettingStarted";

    }



    //--------------------------------------------------------------------------
    // set up our config for the LM experiment
    //--------------------------------------------------------------------------
    void readyConfig()
    {
        config.runMode = ConfigRunMode.NEW;
        config.bootstrapped = true;

        config.expPath = appDir + "/data/" + expID.options[expID.value].text;

        if (practice.isOn)
        {
            config.subjectPath = appDir + "/data/" + expID.options[expID.value].text + "/practice";
        }
        else
        {
            config.subjectPath = appDir + "/data/" + expID.options[expID.value].text + "/" + subID.text;
        }

        config.experiment = expID.options[expID.value].text;
        config.appPath = appDir;
        config.subject = subID.text;
        config.ui = ui.options[ui.value].text;

        DontDestroyOnLoad(config);

    }

    void TogglePracticeState(Toggle toggle)
    {
        if (toggle.isOn)
        {
            // ValidateSubjectID();
        }

    }
}
