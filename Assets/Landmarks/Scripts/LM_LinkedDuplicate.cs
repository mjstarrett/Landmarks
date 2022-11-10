using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM_LinkedDuplicate : MonoBehaviour
{
    public ExperimentTask original;
    // Start is called before the first frame update
    void Start()
    {
        System.Type type = original.GetType();
        Component copy = gameObject.AddComponent(type);
        // Copied fields can be restricted with BindingFlags
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
    }

}
