/************************************************************************************

Filename    :   CVirt.cs
Content     :   Native CybSDK plugin interface
Created     :   August 8, 2014
Authors     :   Lukas Pfeifhofer

Copyright   :   Copyright 2014 Cyberith GmbH

Licensed under the ___LICENSE___

************************************************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CybSDK
{

    public class CVirt
    {

        // Import from plugin
        public const string CybSDKLib = "CybSDK";
        //#if WIN64
        //        public const string CybSDKLib = "Plugins/x86_64/CybSDK";
        //#else
        //        public const string CybSDKLib = "Plugins/x86/CybSDK";
        //#endif

        [DllImport(CybSDKLib)]
        public static extern IntPtr CybSDK_VirtDevice_FindDevice();

        [DllImport(CybSDKLib)]
        public static extern IntPtr CybSDK_VirtDevice_CreateDeviceMockupXInput();

        [DllImport(CybSDKLib)]
        public static extern IntPtr CybSDK_VirtDevice_CreateDeviceSimulation();


        [DllImport(CybSDKLib)]
        public static extern bool CybSDK_VirtDevice_Open(IntPtr device);

        [DllImport(CybSDKLib)]
        public static extern bool CybSDK_VirtDevice_IsOpen(IntPtr device);

        [DllImport(CybSDKLib)]
        public static extern bool CybSDK_VirtDevice_Close(IntPtr device);


        [DllImport(CybSDKLib)]
        public static extern float CybSDK_VirtDevice_GetPlayerHeight(IntPtr device);

        [DllImport(CybSDKLib)]
        public static extern void CybSDK_VirtDevice_ResetPlayerHeight(IntPtr device);

        [DllImport(CybSDKLib)]
        public static extern float CybSDK_VirtDevice_GetPlayerOrientation(IntPtr device);

        [DllImport(CybSDKLib)]
        public static extern float CybSDK_VirtDevice_GetMovementSpeed(IntPtr device);

        [DllImport(CybSDKLib)]
        public static extern float CybSDK_VirtDevice_GetMovementDirection(IntPtr device);

        [DllImport(CybSDKLib)]
        public static extern void CybSDK_VirtDevice_ResetPlayerOrientation(IntPtr device);


        [DllImport(CybSDKLib)]
        public static extern void CybSDK_VirtDevice_StartDFU(IntPtr device);


        [DllImport(CybSDKLib)]
        public static extern bool CybSDK_VirtDevice_HasHaptic(IntPtr device);

        [DllImport(CybSDKLib)]
        public static extern void CybSDK_VirtDevice_HapticPlay(IntPtr device);

        [DllImport(CybSDKLib)]
        public static extern void CybSDK_VirtDevice_HapticStop(IntPtr device);

        [DllImport(CybSDKLib)]
        public static extern void CybSDK_VirtDevice_HapticSetGain(IntPtr device, int gain);

        [DllImport(CybSDKLib)]
        public static extern void CybSDK_VirtDevice_HapticSetFrequency(IntPtr device, int frequency);

        [DllImport(CybSDKLib)]
        public static extern void CybSDK_VirtDevice_HapticSetVolume(IntPtr device, int volume);

        public static CVirtDevice FindDevice()
        {
            IntPtr dev = CVirt.CybSDK_VirtDevice_FindDevice();
            if (dev.ToInt32() != 0)
            {
                return new CVirtDeviceNative(dev);
            }
            else
            {
                return null;
            }
        }

        public static CVirtDevice CreateDeviceMockupXInput()
        {
            IntPtr dev = CVirt.CybSDK_VirtDevice_CreateDeviceMockupXInput();
            if (dev.ToInt32() != 0)
            {
                return new CVirtDeviceNative(dev);
            }
            else
            {
                return null;
            }
        }
    }
}