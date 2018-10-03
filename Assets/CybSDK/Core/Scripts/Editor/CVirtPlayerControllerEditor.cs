using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CVirtPlayerController))]
public class CVirtPlayerControllerEditor : Editor {

    public override void OnInspectorGUI() {
        CVirtPlayerController targetScript = (CVirtPlayerController)target;
        targetScript.movementSpeedMultiplier = EditorGUILayout.Slider("Speed Multiplier", targetScript.movementSpeedMultiplier, 0.0f, 10.0f);
    }

    public void OnSceneGUI(){

    }

}