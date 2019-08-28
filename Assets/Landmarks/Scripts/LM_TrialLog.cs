/*
 * Class containing a dictionary and default dictionary entries that can be
 * modified by users. In Landmarks, an instance of this class is defined in
 * Experiment.cs, which allows ExperimentTasks from Landmarks to add a variety
 * of output data (in the form of {label, data} pairs that can then be formatted
 * into a tab delimited header and a corresponding tab delimited line of data values
 * This facilitates easy data importing and munging in R or other software
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM_TrialLog
{
    public Dictionary<string, string> trialData = new Dictionary<string, string>(); // the lists of header/data pairs
    public Dictionary<string, string> defaults = new Dictionary<string, string>(); // user-defined default header/data pairs to be reinititialized on subsequent resets
    public string current;

    // ------------------------
    // Manage default values
    // ------------------------

    public virtual void AddDefault(string key, string value)
    {
        // Allows any task with access to the class instance to add an entry for logging
        defaults.Add(key, value);

        FormatCurrent();
    }

    public virtual void RemoveDefault(string key, string value)
    {
        // Allows any task with access to the class instance to add an entry for logging
        defaults.Add(key, value);

        FormatCurrent();
    }


    // ------------------------
    // Manage trialData values
    // ------------------------

    public virtual void AddTrialData(string key, string value)
    {
        // Allows any task with access to the class instance to add an entry for logging
        trialData.Add(key, value);

        FormatCurrent();
    }


    public virtual void RemoveTrialData(string key)
    {
        // Allows values to be removed (not recommended; use Reset() )
        trialData.Remove(key);

        FormatCurrent();
    }


    // ------------------------------------
    // reset and reinitialize trialData
    // ------------------------------------

    public virtual void Reset()
    {
        // remove all entries from the trialData dictionary
        trialData.Clear(); // clean up and clear the trial data dictionary so we don't record this info again

        // IF defaults exist, add them into the now empty trialData
        if (defaults.Count > 0)
        {
            foreach (var entry in defaults)
            {
                trialData.Add(entry.Key, entry.Value);
            }
        }

        FormatCurrent();
    }


    // ------------------------------------
    // Format for printing as a single string
    // ------------------------------------

    public virtual string FormatCurrent()
    {
        string header = string.Empty;
        string data = string.Empty;

        // convert the list of label values into a formatted string for printing to the log
        foreach (var item in trialData)
        {
            header += item.Key + "\t"; // append and add a tab
            data += item.Value + "\t"; // append and add a tab
        }

        // Print it as an object that can be returned
        current = "LandmarksTrialData:\n--------------------\n" + header + "\n" + data + "\n--------------------\n";

        // return the string and pass it to a function such as dblog.log()
        return current;
    }
}
