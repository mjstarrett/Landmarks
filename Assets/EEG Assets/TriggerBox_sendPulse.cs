using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO.Ports;
using UnityEngine;

public class Pulses : MonoBehaviour {
    private SerialPort sPort;
    private float delay = 0.05f;
    public Byte[] data = { (byte)0 };

    // Use this for initialization
    void Start () {
        //initiate trigger box
        initiatePort();
    }

    // Update is called once per frame
    void Update () {
		
	}
    // ************************************

    public IEnumerator pulse(int trigger)
    {
        data[0] = (byte)trigger;
        sPort.Write(data, 0, 1);
        // suspend execution 
        yield return new WaitForSeconds(delay);
        //reset to the initial state
        data[0] = 0x00;
        sPort.Write(data, 0, 1);
        Debug.Log("port data written: " + trigger);
    }

    public void initiatePort()
    {
        var config = Config.instance;
        sPort = new SerialPort(config.name, 19200, Parity.None, 8, StopBits.One); // had to change config.portName to config.Name (no definition for portName in Config)
        sPort.Open();
        // Set the port to an initial state
        data[0] = 0x00;
        sPort.Write(data, 0, 1);
        Debug.Log("port open! " + config.name); // had to change config.portName to config.Name (no definition for portName in Config)
    }

    public void closePort()
    {
        // Reset the port to its default state
        data[0] = 0xFF;
        sPort.Write(data, 0, 1);
        // Close the serial port
        sPort.Close();
        Debug.Log("port closed!");
    }
}
