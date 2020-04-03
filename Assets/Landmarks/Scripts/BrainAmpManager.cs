using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_Editor_OSX
using System.IO.Ports;
#endif


public class BrainAmpManager : MonoBehaviour {

    public int initialState; // default state (Hex: 0xFF, Dec.: 0, 8-bit: 0000 0000)
    public int port = 3; // COM port number
    public int nTriggers = 255; // how many unique combinations (e.g., 8 bits = 0-255, 256 permutations)

    private float delay = 5f; // how long to wait before turning off squarewave
    private Byte[] bit = { 0 }; // container for our trigger configuration info in 8-bit format
    public Dictionary<string, int> triggers = new Dictionary<string, int>(); // Store trigger name/number pairs
    private int nextTriggerValue;
    public bool disabled;

#if UNITY_Editor_OSX
    private SerialPort triggerBox; // container for our communication with triggerbox virtual port
#endif

    // Use this for initialization
    void Awake () {
        //initiate trigger box
        initiatePort();
        nextTriggerValue = initialState+1;
    }

    //// -----------------------------------------------------------------------
    //// MJS - use for debugging to send random trigger values with keypress
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Return))
    //    {
    //        Test(UnityEngine.Random.Range(0, nTriggers));
    //    }
    //}

    //// -----------------------------------------------------------------------

    public void EEGTrigger(string triggerName)
    {
        // if the trigger doesn't exist, add it
        if (!triggers.ContainsKey(triggerName))
        {
            triggers.Add(triggerName, nextTriggerValue);
            nextTriggerValue++;
        }
        Debug.Log("EEG trigger\t" + triggerName + ": " + triggers[triggerName].ToString());

        var triggerNumber = triggers[triggerName];
        Test(triggerNumber);
    }


    public void Test(int trigger)
    {
#if UNITY_Editor_OSX
        if (disabled)
        {
            return;
        }
        Debug.Log(trigger);
        bit[0] = (byte)trigger; // set the trigger number 
        Debug.Log(bit[0]);
        triggerBox.Write(bit, 0, 1);
#endif
    }

    //// Coroutine to define square-wave pulse (with pause so we don't turn off too early)
    //public IEnumerator Mark(int triggerNumber)
    //{
    //    // Warn if this trigger is outside the feasible bit range
    //    if (triggerNumber > nTriggers)
    //    {
    //        Debug.LogWarning("too many unique triggers");
    //    }

    //    Debug.Log("Inside Marking method for EEG!");

    //    // Set the trigger value and write
    //    bit[0] = (byte)triggerNumber; 
    //    triggerBox.Write(bit, 0, 1);
    //    yield return new WaitForSeconds(delay);

    //    //reset Trigger to the initial state
    //    bit[0] = 0x00;
    //    triggerBox.Write(bit, 0, 1);
    //    Debug.Log("Reset trigger state" + triggerBox.ReadByte());
    //}



    public void initiatePort()
    {
#if UNITY_Editor_OSX
        // Configure the triggerbox info
        var portName = "COM" + port.ToString();
        Debug.Log("Looking for TriggerBox on port " + portName);
        try
        {
            triggerBox = new SerialPort(portName);

            // Open the virtual serial port
            triggerBox.Open(); // open the port
            Debug.Log("Successfully opened " + portName);

            // Set the port to an initial state
            bit[0] = (byte)initialState;
            triggerBox.Write(bit, 0, 1);
            Debug.Log("port data written: " + triggerBox.ReadByte());
        }
        catch (Exception ex)
        {
            disabled = true;
            Debug.LogWarning("Initialization failed - disabling BrainAmpManager and EEG triggers");
            return;
        }
#endif
    }

    public void closePort()
    {
#if UNITY_Editor_OSX
        if (disabled)
        {
            return;
        }
        // Reset the port to its default state
        bit[0] = 0xFF; // HEX: 0xFF, DEC: 255, BIN: 1111 1111
        triggerBox.Write(bit, 0, 1);
        // Close the serial port
        triggerBox.Close();
        Debug.Log("port closed!");
#endif
    }

    public string LogTriggerIndices()
    {
        var message = "\n================ Index of EEG Triggers ================\n";
        foreach (var entry in triggers)
        {
            message += entry.Value.ToString() + ": " + entry.Key + "\n";
        }
        return message;
    }
}
