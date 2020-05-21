using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM_TargetStore : LM_Target
{
    [Header("Store-specific Target Properties")]
    public GameObject door;
    public float doorMaxOpenAngle = -115;
    public float doorSpeedMulitplier = 1;
    private bool doorOpen;
    private bool doorInMotion;
    // Start is called before the first frame update
    void Start()
    {
        door.transform.localEulerAngles = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (!doorInMotion)
        {
            if (Input.GetKeyDown(KeyCode.O) & !doorOpen)
            {
                StartCoroutine(OpenDoor());
            }
            else if (Input.GetKeyDown(KeyCode.O) & doorOpen)
            {
                StartCoroutine(CloseDoor());
            }
        }
        
    }

    IEnumerator OpenDoor()
    {
        doorInMotion = true;
        for (float ft = 0; ft > doorMaxOpenAngle; ft--)
        {
            door.transform.localEulerAngles = new Vector3(0f, ft, 0f);
            yield return null;
        }
        door.transform.localEulerAngles = new Vector3(0f, doorMaxOpenAngle, 0f);
        doorInMotion = false;
        doorOpen = true;
    }

    IEnumerator CloseDoor()
    {
        doorInMotion = true;
        for (float ft = doorMaxOpenAngle; ft < 0; ft++)
        {
            door.transform.localEulerAngles = new Vector3(0f, ft, 0f);
            yield return null;
        }
        door.transform.localEulerAngles = Vector3.zero;
        doorInMotion = false;
        doorOpen = false;
    }
}
