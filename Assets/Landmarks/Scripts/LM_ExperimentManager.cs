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
    public TMP_Dropdown biosex;
    public TMP_InputField age;
    public TMP_Dropdown ui;
    public TMP_Dropdown conds;
    public Button start;

    private string appDir = "";
    private bool expDirCreated = false;
    private bool subDirCreated = false;
    private Config config;

    private bool expidError = true;
    private bool subidError = true;
    private bool biosexError = true;
    private bool ageError = true;
    private bool uiError = true;
    private bool condError = false;

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

    }

    public void LoadExperiment()
    {
        TextMeshProUGUI _errorMessage = start.transform.Find("Error").GetComponent<TextMeshProUGUI>();

        Debug.Log("starting to load experiment");

        ValidateExpID();
        ValidateSubjectID();
        ValidateAge();
        ValidateBiosex();
        ValidateUI();
        if (conds.gameObject.activeSelf) ValidateCondition();


        if (!expidError && !subidError && !ageError && !biosexError && uiError)
        {
            //if (conds.gameObject.activeSelf && condError) ;

            // Create the directories if they don't exist
            if (!Directory.Exists(appDir + "/data/" + expID.options[expID.value].text))
            {
                Directory.CreateDirectory(appDir + "/data/" + expID.options[expID.value].text);
                expDirCreated = true;
            }
            Directory.CreateDirectory(appDir + "/data/" + expID.options[expID.value].text + "/" + subID.text);
            subDirCreated = true;

            Debug.Log("All error flags removed; proceeding");
            _errorMessage.gameObject.SetActive(false);


            PlayerPrefs.SetString("expID", expID.options[expID.value].text);
            PlayerPrefs.SetInt("subID", int.Parse(subID.text));
            PlayerPrefs.SetString("biosex", biosex.options[biosex.value].text);
            PlayerPrefs.SetInt("subAge", int.Parse(age.text));
            PlayerPrefs.SetString("ui", ui.options[ui.value].text);
            SceneManager.LoadScene("City01_StandardNavigation");
            
        }
        else
        {
            _errorMessage.gameObject.SetActive(true);
        }

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


        // Show special options for individual experiments 

        ////////////
        /// ESC  ///
        ////////////

        if (expID.options[expID.value].text == "esc")
        {
            List<string> options = new List<string> { "-Select-", "1:1", "1:150" }; // List the starting options for ESC

            conds.ClearOptions(); // clear any old options 
            conds.AddOptions(options); // populate the options in the dropdown

            conds.gameObject.SetActive(true); // activate this field

            condError = true;
        }
        else
        {
            conds.gameObject.SetActive(false);
            condError = false;
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

                // Even if the subID is an int, check if this ID has been used
                if (!Directory.Exists(appDir + "/data/" + expID.options[expID.value].text + "/" + subID.text))
                {
                    subidError = false;
                    _errorMessage.gameObject.SetActive(false); // then and only then, will we release the flag
                }
                // otherwise, flag an error and for in-use id;
                else
                {
                    subidError = true;
                    _errorMessage.text = "That SubjectID is already in use.";
                    _errorMessage.gameObject.SetActive(true);
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
    // Validate Biosex Input
    //--------------------------------------------------------------------------

    public void ValidateBiosex()
    {
        TextMeshProUGUI _errorMessage = biosex.transform.Find("Error").GetComponent<TextMeshProUGUI>();

        if (biosex.value != 0)
        {
            biosexError = false;
            _errorMessage.gameObject.SetActive(false);
        }
        else
        {
            biosexError = true;
            _errorMessage.text = "Please select one of the options provided.";
            _errorMessage.gameObject.SetActive(true);

        }
    }


    //--------------------------------------------------------------------------
    // Validate Age Input
    //--------------------------------------------------------------------------

    public void ValidateAge()
    {
        TextMeshProUGUI _errorMessage = age.transform.Find("Error").GetComponent<TextMeshProUGUI>();

        if (age.text != "")
        {
            if (int.TryParse(age.text, out int _age))
            {
                ageError = false;
                _errorMessage.gameObject.SetActive(false);
            }
            else
            {
                ageError = true;
                _errorMessage.text = "Age must be an integer.";
                _errorMessage.gameObject.SetActive(true);
            }
        }
        else
        {
            ageError = true;
            _errorMessage.text = "You must provide an Age.";
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
            biosexError = false;
            _errorMessage.gameObject.SetActive(false);
        }
        else
        {
            biosexError = true;
            _errorMessage.text = "Please select a UI from the dropdown.";
            _errorMessage.gameObject.SetActive(true);
        }
    }


    //--------------------------------------------------------------------------
    // Validate Condition Input
    //--------------------------------------------------------------------------

    public void ValidateCondition()
    {
        TextMeshProUGUI _errorMessage = conds.transform.Find("Error").GetComponent<TextMeshProUGUI>();

        if (conds.value != 0)
        {
            condError = false;
            _errorMessage.gameObject.SetActive(false);
        }
        else
        {
            condError = true;
            _errorMessage.text = "You must select a condition for experiment " + expID.options[expID.value].text + ".";
            _errorMessage.gameObject.SetActive(true);

        }
    }



    //--------------------------------------------------------------------------
    // set up our config for the LM experiment
    //--------------------------------------------------------------------------
    void readyConfig()
    {
        config.runMode = ConfigRunMode.NEW;
        config.bootstrapped = true;

        config.expPath = appDir + "/data/" + expID.options[expID.value].text;
        config.subjectPath = appDir + "/data/" + expID.options[expID.value].text + "/" + subID.text;

        config.appPath = appDir;
        config.level = expID.options[expID.value].text;
        config.subject = subID.text;

        DontDestroyOnLoad(config);

    }
}
