using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CVirtComment))]
public class CVirtCommentInspector : Editor
{
    private CVirtComment attachedComment
    {
        get
        {
            return target as CVirtComment;
        }
    }

    public override void OnInspectorGUI()
    {
        if (serializedObject == null) return;

        GUIStyle style = new GUIStyle();
        style.wordWrap = true;
        style.normal.textColor = Color.gray;

        serializedObject.Update();
        EditorGUILayout.Space();

        string text = EditorGUILayout.TextArea(attachedComment.comment, style);
        if (text != attachedComment.comment)
        {
            Undo.RecordObject(attachedComment, "Edit Comments");
            attachedComment.comment = text;
        }

        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();
    }
}
