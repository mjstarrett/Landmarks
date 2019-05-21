using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM_DontDestroy : MonoBehaviour
{
    void Awake()
    {
        transform.parent = null;
        DontDestroyOnLoad(gameObject);
    }
}
