using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM_VirtualHandle : MonoBehaviour
{
    public float scaleFactor;
    public float outputFloat;
    public int outputInt;


    // Update is called once per frame
    public int CalculateValue()
    {
        outputFloat = this.transform.localPosition.x / scaleFactor;
        outputInt = Mathf.RoundToInt(outputFloat);

        return outputInt;
    }
}
