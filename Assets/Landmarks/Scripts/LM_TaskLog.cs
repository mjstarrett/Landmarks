/*
 * Class containing a dictionary and default dictionary entries that can be
 * modified by users. This allows ExperimentTasks from Landmarks to add a variety
 * of output data (in the form of {label, data} pairs that can then be formatted
 * into a tab delimited header and a corresponding tab delimited line of data values
 * This facilitates easy data importing and munging in R or other software
 *
 */

using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class LM_TaskLog : MonoBehaviour 
{
    public Dictionary<string, string> trialData = new Dictionary<string, string>(); // the lists of header/data pairs
    public Dictionary<string, string> defaults; // what should get added every time
    public StreamWriter output;

    private Experiment exp;


    // Manage trialData values
    public virtual void AddData(string key, string value)
    {
        trialData.Add(key, value);
    }


    public virtual void LogTrial()
    {
        Debug.LogWarning("LM_TaskLog is logging trial data!!!!!");
        exp = FindObjectOfType<Experiment>();
        output = new StreamWriter(exp.dataPath +
                                    "/sub-" + exp.config.subject +
                                    "_task-" + name +
                                    "_behav.csv", append:true);
        Debug.Log("Logging to: " + ((FileStream)(output.BaseStream)).Name);

        string header = string.Empty;
        string data = string.Empty;

        // convert the list of label values into a formatted string for printing to the log
        foreach (var item in trialData)
        {
            header += item.Key + ","; // append and add a tab
            data += item.Value + ","; // append and add a tab
        }

        // Log the header (if empty file) and data
        if (output.BaseStream.Length == 0) output.WriteLine(header);
        output.WriteLine(data);

        // Clean up from this trial
        trialData.Clear();
        output.Close();
    }

    // Format to feed to the dbLog class for the main log file
    public virtual string FormatCurrent()
    {
        string header = string.Empty;
        string data = string.Empty;

        // convert, print, and return the list of label values into a formatted string
        foreach (var item in trialData)
        {
            header += item.Key + "\t"; 
            data += item.Value + "\t"; 
        }
        var current = "LandmarksTrialData:\n--------------------\n" + header + "\n" + data + "\n--------------------\n";
        return current;
    }
}
