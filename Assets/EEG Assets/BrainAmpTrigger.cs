using System;
using System.Collections;
using System.IO.Ports;
using UnityEngine;


public class BrainAmpTrigger : MonoBehaviour {

    public int initialState = 0; // 0 in bits 0x00;
    public int state;
    public int port = 3; // COM port number
    public int nTriggers = 255; // how many unique combinations (e.g., 8 bits = 256 possibilities)

    private SerialPort triggerBox;
    private float delay = 0.05f; // how long to wait before turning off squarewave
    private Byte[] bit = { 0 };

    // Use this for initialization
    void Start () {
        //initiate trigger box
        initiatePort();
        state = initialState;
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Return))
    //    {
    //        Test(UnityEngine.Random.Range(0, nTriggers));
    //    }
    //}

    //public void Test(int trigger)
    //{
    //    Debug.Log(trigger);
    //    bit[0] = (byte)trigger; // set the trigger number 
    //    Debug.Log(bit[0]);
    //    triggerBox.Write(bit, 0, 1);
    //}

    // Coroutine to define square-wave pulse (with pause so we don't turn off too early)
    public IEnumerator Mark(int trigger)
    {
        if (trigger < nTriggers)
        {
            Debug.LogWarning("too many unique triggers");
        }
        bit[0] = (byte)trigger; // set the trigger number 
        triggerBox.Write(bit, 0, 1);
        // suspend execution 
        yield return new WaitForSeconds(delay);

        //reset to the initial state
        bit[0] = (byte)initialState;
        triggerBox.Write(bit, 0, 1);
        Debug.Log("port data written: " + triggerBox.ReadByte());

        state++;
    }


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
        bit[0] = 0xFF;
        triggerBox.Write(bit, 0, 1);
        // Close the serial port
        triggerBox.Close();
        Debug.Log("port closed!");
    }
}
