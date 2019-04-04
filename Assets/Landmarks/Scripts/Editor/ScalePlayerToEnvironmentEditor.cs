/*
    ScalePlayerToEnvironment Editor
       
    This allows for custom behavior for the ScalePlayerToEnvironment.cs script
    in the Unity Inspector

    Copyright (C) 2019 Michael J. Starrett

    Navigate by StarrLite (Powered by LandMarks)
    Human Spatial Cognition Laboratory
    Department of Psychology - University of Arizona   
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Custom Inspector to handle sufficient conditions
[CustomEditor(typeof(ScalePlayerToEnvironment))]
[CanEditMultipleObjects]
public class ScalePlayerToEnvironmentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // set our custom inspector up to communicate with the main script and alias that script for brevity
        ScalePlayerToEnvironment spte = (ScalePlayerToEnvironment)target;

        // Gameobject field for Scaled environment
        spte.scaledEnvironment = (GameObject)EditorGUILayout.ObjectField("Scaled Enviornment", spte.scaledEnvironment, typeof(GameObject), true);

        // Gameobject field for StartLocationsParent
        spte.startLocationsParent = (GameObject)EditorGUILayout.ObjectField("Start Locations Parent", spte.startLocationsParent, typeof(GameObject), true);

        // bool for randomizing start location
        spte.randomStartLocation = EditorGUILayout.Toggle("Random Start Location", spte.randomStartLocation);

        // publish our autoscale option
        spte.autoscale = EditorGUILayout.Toggle("AutoScale", spte.autoscale);
        // tell inspector to hide the scale ratio option if they are autoscaling
        using (new EditorGUI.DisabledScope(spte.autoscale))
        {
            spte.scaleRatio = EditorGUILayout.FloatField("Scale Ratio", spte.scaleRatio);
        }
    }
}
