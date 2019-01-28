/*
    Copyright (C) 2010  Jason Laczko

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

    
#if UNITY_EDITOR 

using UnityEngine;
using UnityEditor;
using System.Collections;
 
public class TransformCopier : ScriptableObject
{
        private static Vector3 position;
        private static Quaternion rotation;
        private static Vector3 scale;
        private static string myName; 
    
    [MenuItem ("Custom/Transform Copier/Copy Transform")]
    static void DoRecord()
    {
       position = Selection.activeTransform.localPosition;
       rotation = Selection.activeTransform.localRotation;
       scale = Selection.activeTransform.localScale;
       myName = Selection.activeTransform.name;       
        
        EditorUtility.DisplayDialog("Transform Copy", "Local position, rotation, & scale of "+myName +" copied relative to parent.", "OK", "");
    }
 
    [MenuItem ("Custom/Transform Copier/Paste Transform")]
    static void DoApply()
    {
        Selection.activeTransform.localPosition = position;
        Selection.activeTransform.localRotation = rotation;
        Selection.activeTransform.localScale = scale;      
        
        EditorUtility.DisplayDialog("Transform Paste", "Local position, rotation, and scale of "+myName +"  pasted relative to parent of "+Selection.activeTransform.name+".", "OK", "");
    }
}


#endif