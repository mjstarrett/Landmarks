using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM_Compass : MonoBehaviour
{
    public GameObject pointer;
    public float rotationSpeedMultiplier;
    public bool interactable;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (interactable)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                pointer.transform.Rotate(new Vector3(0f, -1 * rotationSpeedMultiplier * Time.deltaTime, 0f), Space.Self);
                
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                pointer.transform.Rotate(new Vector3(0f, rotationSpeedMultiplier * Time.deltaTime, 0f), Space.Self);
                
            }
        }
    }


    // move the pointer back to zero degrees unless random is specified
    public void ResetPointer(bool random = false)
    {
        // store the vector3 rotation of the pointer in a temporary variable
        var temp = pointer.transform.localEulerAngles;

        // set the Y value of the desired Vector3 rotation (eulers)
        if (random)
        {
            temp.y = Random.Range(0f, 360f - Mathf.Epsilon); // random from 0 to 359.999999999
        }
        else temp.y = 0; // or zero

        // set the actual pointer transform's rotation to the temporary variable
        pointer.transform.localEulerAngles = temp;
    }
}
