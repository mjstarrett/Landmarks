using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(Experiment))]
[CanEditMultipleObjects]


public class ExperimentEditor : Editor
{
    //SerializedProperty experiment;
    SerializedProperty cOpts;
    SerializedProperty ui;
    [SerializeField] List<string> uiList;
    public int index;

    private void OnEnable()
    {
        cOpts = serializedObject.FindProperty("controllerOptions");
        ui = serializedObject.FindProperty("userInterface");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Experiment exp = (Experiment)target;

        EditorGUILayout.PropertyField(cOpts, new GUIContent("enum"));
        EditorGUILayout.PropertyField(ui, new GUIContent("string"));

        EditorGUILayout.EnumPopup("My UI", exp.controllerOptions);

        // list of available UIs as a Dropdown
        uiList = new List<string>();
        foreach (Transform child in exp.uiParent.transform)
        {
            uiList.Add(child.name);
        }
        exp.userInterface = uiList[EditorGUILayout.Popup(label: "User Interface", (int)Enum.Parse(typeof(UserInterface), exp.userInterface), uiList.ToArray<string>())];
        exp.userInterface = EditorGUILayout.PropertyField(exp.userInterface, new GUIContent("hello"))

        //var selection = uiList[index];

        //exp.userInterface = Enum.Parse(typeof(UserInterface), Enum.GetName(typeof(UserInterface), index));


        
        serializedObject.ApplyModifiedProperties();
    }
}
