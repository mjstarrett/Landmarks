using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM_TagChildren : MonoBehaviour
{
    public string TagToApply;

    void Awake()
    {
        foreach (Transform child in transform)
        {
            child.tag = "Target";
        }
    }

}
