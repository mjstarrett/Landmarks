/************************************************************************************

Filename    :   CVirtDeviceControllerEditor.cs
Content     :   ___SHORT_DISCRIPTION___
Created     :   August 8, 2014
Authors     :   Lukas Pfeifhofer

Copyright   :   Copyright 2014 Cyberith GmbH

Licensed under the ___LICENSE___

************************************************************************************/

using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CVirtDeviceController))]
public class CVirtDeviceControllerEditor : Editor {

    public override void OnInspectorGUI() {
        CVirtDeviceController targetScript = (CVirtDeviceController) target;

        targetScript.deviceTypeIndex = EditorGUILayout.Popup("Device", targetScript.deviceTypeIndex, 
        new string[]{
            "Auto Selection",
            //"Standard Virtualizer Device",
            //"Standard coupled Movement (Keyboard, Gamepad)"
        });
        targetScript.deviceMockupTypeIndex = EditorGUILayout.Popup("Device Mockup", targetScript.deviceMockupTypeIndex, 
        new string[]{
            "Disabled",
            "XInput Mockup (Xbox Controller)"
        });
        targetScript.activateHaptic = EditorGUILayout.Toggle("Haptic Feedback", targetScript.activateHaptic);
        EditorGUILayout.LabelField("", "(only active if supported by the device)");
    }

    public void OnSceneGUI(){

    }

}