using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM_vrSlider : MonoBehaviour
{
    public GameObject handle;
    public GameObject start;
    public GameObject end;
    private float min = 0;
    public float max = 3;
    public bool wholeNumbers = false;

    private float totalZ;
    public float[] anchorPoints;
    public float snapDistance = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        // Set up the anchor on the handle to be based on the far left of the slider (so it's displacement equals our value)
        handle.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.5f);
        handle.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0.5f);


        totalZ = Mathf.Abs(end.GetComponent<RectTransform>().anchoredPosition.x - start.GetComponent<RectTransform>().anchoredPosition.x);

        anchorPoints = new float[(int)max + 1]; // handle the fact that zero is one of our snaps

        for (int iSnap = (int)min; iSnap < max+1; iSnap++)
        {
            anchorPoints[iSnap] = iSnap * (totalZ / (int)max);
        }

        Vector3 position = handle.GetComponent<RectTransform>().anchoredPosition;
        position.x = anchorPoints[0];
        handle.GetComponent<RectTransform>().anchoredPosition = position;
    }

    public void GetSnap()
{
		SnapToPosition();
        Debug.Log("Checking Snap Position");
}


    public void SnapToPosition()
    {
        Debug.Log("Roger that... checking now");
        //Cycle through each predefined anchor point
        for (int i = 0; i < anchorPoints.Length; i++)
        {
            //If lever is within "snapping distance" of that anchor point
            if (Mathf.Abs(handle.GetComponent<RectTransform>().anchoredPosition.x - anchorPoints[i]) < totalZ/(anchorPoints.Length + 1))
            {
                //Get current lever position and update z pos to anchor point
                Vector3 snapPosition = handle.GetComponent<RectTransform>().anchoredPosition;
                snapPosition.x = anchorPoints[i];
                handle.GetComponent<RectTransform>().anchoredPosition = snapPosition;
                Debug.Log("Did we snap to " + snapPosition.x);
                //Break so it stops checking other anchor points
                break;
            }
        }

       
    }
}
