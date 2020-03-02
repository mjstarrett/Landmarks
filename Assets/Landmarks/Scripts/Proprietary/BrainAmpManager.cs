using System;
using System.Collections;
using System.IO.Ports;
using UnityEngine;
using System.Collections.Generic;


public class BrainAmpManager : MonoBehaviour {

    public int initialState; // default state (Hex: 0xFF, Dec.: 0, 8-bit: 0000 0000)
    public int port = 3; // COM port number
    public int nTriggers = 255; // how many unique combinations (e.g., 8 bits = 0-255, 256 permutations)

    private SerialPort triggerBox; // container for our communication with triggerbox virtual port
    private float delay = 5f; // how long to wait before turning off squarewave
    private Byte[] bit = { 0 }; // container for our trigger configuration info in 8-bit format
    public Dictionary<string, int> triggers = new Dictionary<string, int>(); // Store trigger name/number pairs
    private int nextTriggerValue;

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
        Debug.Log(trigger);
        bit[0] = (byte)trigger; // set the trigger number 
        Debug.Log(bit[0]);
        triggerBox.Write(bit, 0, 1);
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
        // Configure the triggerbox info
        var portName = "COM" + port.ToString();
        Debug.Log("Looking for TriggerBox on port " + portName);
        triggerBox = new SerialPort(portName);

        // Open the virtual serial port
        triggerBox.Open(); // open the port
        Debug.Log("Successfully opened " + portName);

        // Set the port to an initial state
        bit[0] = (byte)initialState;
        triggerBox.Write(bit, 0, 1);
        Debug.Log("port data written: " + triggerBox.ReadByte());
    }

    public void closePort()
    {
        // Reset the port to its default state
        bit[0] = 0xFF; // HEX: 0xFF, DEC: 255, BIN: 1111 1111
        triggerBox.Write(bit, 0, 1);
        // Close the serial port
        triggerBox.Close();
        Debug.Log("port closed!");
    }
}
