using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LM_vrSlider : MonoBehaviour
{
    public GameObject handle;
    public GameObject start;
    public GameObject end;
    private float minZ = 0;
    public float maxZ = 3;
    public bool wholeNumbers = false;

    private float totalZ;
    public float[] anchorPoints;
    public float snapDistance = 0.05f;

    // Start is called before the first frame update
    void Start()
    {

        totalZ = Mathf.Abs(end.GetComponent<RectTransform>().anchoredPosition.x - start.GetComponent<RectTransform>().anchoredPosition.x);

        anchorPoints = new float[(int)maxZ + 1]; // handle the fact that zero is one of our snaps

        for (int iSnap = (int)minZ; iSnap < maxZ+1; iSnap++)
        {
            anchorPoints[iSnap] = iSnap * (totalZ / (int)maxZ);
        }

        Vector3 position = handle.GetComponent<RectTransform>().anchoredPosition;
        position.x = anchorPoints[0];
        handle.GetComponent<RectTransform>().anchoredPosition = position;
    }

    public void GetSnap()
{
		SnapToPosition();
}


    public void SnapToPosition()
    {

        //Cycle through each predefined anchor point
        for (int i = 0; i < anchorPoints.Length; i++)
        {
            //If lever is within "snapping distance" of that anchor point
            if (Mathf.Abs(handle.GetComponent<RectTransform>().anchoredPosition.x - anchorPoints[i]) < snapDistance)
            {
                //Get current lever position and update z pos to anchor point
                float snapPosition = handle.GetComponent<RectTransform>().anchoredPosition.x;
                //Break so it stops checking other anchor points
                break;
            }
        }
    }
}
