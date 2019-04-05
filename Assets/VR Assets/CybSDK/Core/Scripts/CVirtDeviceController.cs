/************************************************************************************

Filename    :   CVirtDeviceController.cs
Content     :   ___SHORT_DISCRIPTION___
Created     :   August 8, 2014
Authors     :   Lukas Pfeifhofer

Copyright   :   Copyright 2014 Cyberith GmbH

Licensed under the ___LICENSE___

************************************************************************************/

using UnityEngine;
using System.Collections;
using CybSDK;

public class CVirtDeviceController : MonoBehaviour
{
	private CVirtDevice virtDevice;
    //
	public int deviceTypeIndex = 0;
	public int deviceMockupTypeIndex = 0;
	public bool activateHaptic = true;
    //
    public enum CVirtDeviceControllerCallbackType
    {
        Connect,
        Disconnect
    }
    public delegate void CVirtDeviceControllerCallback(CVirtDevice virtDevice, CVirtDeviceController.CVirtDeviceControllerCallbackType callbackType);
    public CVirtDeviceControllerCallback OnCVirtDeviceControllerCallback = null;

    // Use this for initialization
    void Start ()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE_WIN
        switch (deviceTypeIndex)
        {
            //Auto selection
            case 0:
                virtDevice = getStandardVirtualizerDevice();
                if (virtDevice == null)
                {
                    fallbackToStandardCoupled();
                    return;
                }
                break;

            //Standard de-coupled virtualizer input
            case 1:
                virtDevice = getStandardVirtualizerDevice();
                break;

            //TODO: Standard coupled movement input 
            case 2:
                fallbackToStandardCoupled();
                return;

            //Failed to find VirtDevice
            default:
                break;
        }
        #endif
        
        #if UNITY_ANDROID
            virtDevice = new CVirtDeviceBluetooth();
        #endif

        if (virtDevice != null)
        {
            CLogger.Log("Virtualizer device found, connecting...");

            if (virtDevice.Open())
            {
                CLogger.Log("Successfully connected to Virtualizer device.");

                //Reset ResetPlayerOrientation and PlayerHeight
                virtDevice.ResetPlayerOrientation();
                virtDevice.GetPlayerHeight();

                // Callback
                if (this.OnCVirtDeviceControllerCallback != null)
                    this.OnCVirtDeviceControllerCallback(virtDevice, CVirtDeviceControllerCallbackType.Connect);
            }
            else
            {
                CLogger.LogError("Failed to connect to Virtualizer device.");
            }

        }
        else
        {
            CLogger.LogError("Virtualizer device not found...");
        }

    }

    private void fallbackToStandardCoupled()
    {
        CVirtPlayerControllerCoupled coupledController = GetComponent<CVirtPlayerControllerCoupled>();
        if (coupledController != null)
        {
            CLogger.Log("Fallback to CVirtPlayerControllerCoupled implementation.");
            coupledController.activate();
        }
        else
        {
            CLogger.LogError("No CVirtPlayerControllerCoupled implementation available, fallback not possible.");
        }
    }

    private CVirtDevice getStandardVirtualizerDevice()
    {
        CVirtDevice device = CVirt.FindDevice();

        if (device == null)
        {
            //If mockup is enabled and device null, find mockup input
            switch (deviceMockupTypeIndex)
            {
                case 1:
                    device = CVirt.CreateDeviceMockupXInput();
                    break;

                default:
                    break;
            }
        }

        return device;
    }

    public CVirtDevice GetDevice(){
        return this.virtDevice;
    }

    //Cleanup if the game gets terminated. This is important to 
    //prevend connection failures.
    void OnDisable()
    {
        if (virtDevice != null)
        {
            virtDevice.Close ();
            CLogger.Log("Automatically disconnected from Virtualizer device.");

            // Callback
            if (this.OnCVirtDeviceControllerCallback != null)
                this.OnCVirtDeviceControllerCallback(virtDevice, CVirtDeviceControllerCallbackType.Disconnect);
        }
    }

}
