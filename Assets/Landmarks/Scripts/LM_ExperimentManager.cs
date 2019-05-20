using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class LM_ExperimentManager : MonoBehaviour
{

    public TMP_InputField expID;
    public TMP_InputField subID;
    public TMP_Dropdown biosex;
    public TMP_InputField age;
    public TMP_Dropdown ui;
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
        if (!expidError && !subidError && !ageError)
        {
            start.gameObject.SetActive(true);
        }
        else start.gameObject.SetActive(false);

    }

    public void LoadExperiment()
    {
        ValidateBiosex();
        ValidateUI();

        if (!expidError && !subidError && !ageError && !biosexError && !uiError)
        {
            PlayerPrefs.SetString("expID", expID.text);
            PlayerPrefs.SetInt("subID", int.Parse(subID.text));
            PlayerPrefs.SetString("biosex", biosex.options[biosex.value].text);
            PlayerPrefs.SetInt("subAge", int.Parse(age.text));
            PlayerPrefs.SetString("ui", ui.options[ui.value].text);
            SceneManager.LoadScene("City01_StandardNavigation");
        }
    }

    //--------------------------------------------------------------------------
    // Validate Exp Id
    //--------------------------------------------------------------------------

    public void ValidateExpID()
    {
        TextMeshProUGUI _errorMessage = expID.transform.Find("Error").GetComponent<TextMeshProUGUI>();

        if (expID.text != "")
        {
            expidError = false;
            _errorMessage.gameObject.SetActive(false);

            if (!Directory.Exists(appDir + "/data/" + expID.text))
            {
                Directory.CreateDirectory(appDir + "/data/" + expID.text);
                expDirCreated = true;
            }
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

                // Even if the subID is an int, check if this ID has been used
                if (!Directory.Exists(appDir + "/data/" + expID.text + "/" + subID.text))
                {
                    Directory.CreateDirectory(appDir + "/data/" + expID.text + "/" + subID.text);
                    subDirCreated = true;
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
    // set up our config for the LM experiment
    //--------------------------------------------------------------------------
    void readyConfig()
    {
        config.runMode = ConfigRunMode.NEW;
        config.bootstrapped = true;

        config.expPath = appDir + "/data/" + expID.text;
        config.subjectPath = appDir + "/data/" + expID.text + "/" + subID.text;

        config.appPath = appDir;
        config.level = expID.text;
        config.subject = subID.text;

    }
}
