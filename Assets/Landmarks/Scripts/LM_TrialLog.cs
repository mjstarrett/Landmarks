using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM_TrialLog : MonoBehaviour
{
    public Dictionary<string, string> trialData;
    private Dictionary<string, string> defaults; 

    public void Init()
    {
        // restore the defaults (if any)
        foreach (var entry in defaults)
        {
            trialData.Add(entry.Key, entry.Value);
        }
    }

    public virtual string Format(string msg)
    {
        string header = string.Empty;
        string data = string.Empty;

        // convert the list of label values into a formatted string for printing to the log
        foreach (var item in trialData)
        {
            header += item.Key + "\t"; // append and add a tab
            data += item.Value + "\t"; // append and add a tab
        }

        // Print to our log file
        msg = "LandmarksTrialData:\n--------------------\n" + header + "\n" + data + "\n--------------------\n";

        return msg;
    }

    public virtual void Add(string key, string value)
    {
        trialData.Add(key, value);
    }

    public virtual void Remove(string key)
    {
        trialData.Remove(key);
    }

    public virtual void ClearData()
    {
        trialData.Clear(); // clean up and clear the trial data dictionary so we don't record this info again


    }
}
