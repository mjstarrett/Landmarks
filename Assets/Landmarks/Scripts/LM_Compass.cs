using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM_Compass : MonoBehaviour
{
    public GameObject pointer;
    public float rotationSpeedMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            pointer.transform.Rotate(transform.up, -1 * rotationSpeedMultiplier * Time.deltaTime);
            Debug.Log("Trying to rotate left!");
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            pointer.transform.Rotate(transform.up,rotationSpeedMultiplier * Time.deltaTime);
            Debug.Log("Trying to rotate left!");
        }
    }
}
