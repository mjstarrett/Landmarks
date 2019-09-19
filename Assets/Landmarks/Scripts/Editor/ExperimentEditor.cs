//using System;
using UnityEditor;

//[CustomEditor(typeof(Experiment))]

public class ExperimentEditor : Editor
{
    private SerializedProperty userInterface;
    private SerializedProperty UserInterface;


    void OnEnable()
    {
        

        userInterface = serializedObject.FindProperty("userInterface");

        UserInterface = serializedObject.FindProperty("UserInterface");
    }
    
    public override void OnInspectorGUI()
    {
        Experiment exp = (Experiment)target;



        //serializedObject.Update();
        //EditorGUILayout.PropertyField(experiment);
        //serializedObject.ApplyModifiedProperties();

        
    }
}
