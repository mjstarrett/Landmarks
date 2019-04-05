﻿using UnityEngine;
using System.Collections;
using UnityEditor;

namespace CybSDK
{

    [CustomEditor(typeof(CVirtHapticEmitter))]
    public class CVirtHapticEmitterEditor : Editor
    {

        private static Color orange = new Color(1f, 0.549f, 0f);

        public override void OnInspectorGUI()
        {
            CVirtHapticEmitter targetScript = (CVirtHapticEmitter)target;

            targetScript.autoStart = EditorGUILayout.Toggle("AutoStart Playing", targetScript.autoStart);
            targetScript.loop = EditorGUILayout.Toggle("Loop", targetScript.loop);
            targetScript.duration = EditorGUILayout.FloatField("Timespan", targetScript.duration);
            targetScript.distance = EditorGUILayout.FloatField("Distance", targetScript.distance);

            targetScript.forceOverTime = EditorGUILayout.CurveField("Volume over Time", targetScript.forceOverTime, Color.green, Rect.MinMaxRect(0, 0, 1, 1), GUILayout.Height(80));
            targetScript.forceOverDistance = EditorGUILayout.CurveField("Force over Distance", targetScript.forceOverDistance, orange, Rect.MinMaxRect(0, 0, 1, 1), GUILayout.Height(80));
        }

        public void OnSceneGUI()
        {
            CVirtHapticEmitter targetScript = (CVirtHapticEmitter)target;

            Handles.color = orange;
            Handles.DrawWireDisc(targetScript.transform.position, targetScript.transform.up, targetScript.distance);
        }

    }

}
