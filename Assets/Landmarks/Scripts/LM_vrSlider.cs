using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class LM_vrSlider : MonoBehaviour
{
    public GameObject handle; // the object you will be manipulating to adjust the slider
    public GameObject start; // one endpoint of the sliders linear motion
    public GameObject end; // the other endpoint of the sliders linear motion
    public GameObject linearMapping; // the linear mapping object for SteamVR slider function
    public float minValue = 0; // This value is fixed at 0 as a base for calculations to work. Scale adjustments should be made at the output of 'value'
    public float maxValue = 3; // Max value, determines number of snaps and intervals thereof based on minValue
    public bool wholeNumbers = false;
    public float sliderValue;

    private float totalSliderRange;
    public float[] anchorPoints;

    // Start is called before the first frame update
    void Awake()
    {
        // Set up the anchor on the handle to be based on the far left of the slider (so it's displacement equals our value)
        handle.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.5f);
        handle.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0.5f);

        // figure out the total slider range, based on the anchored positions of our handle in the rect transform (which we just fixed to left aligned above)
        totalSliderRange = Mathf.Abs(end.GetComponent<RectTransform>().anchoredPosition.x - start.GetComponent<RectTransform>().anchoredPosition.x);

        // set the size of the anchorPoints array
        anchorPoints = new float[(int)maxValue + 1]; // handle the fact that zero is one of our snaps

        // Compute the value of each anchor point
        for (int iSnap = (int)minValue; iSnap < maxValue+1; iSnap++)
        {
            anchorPoints[iSnap] = iSnap * (totalSliderRange / (int)maxValue);
        }


        Vector3 position = handle.GetComponent<RectTransform>().anchoredPosition;
        position.x = anchorPoints[0];
        handle.GetComponent<RectTransform>().anchoredPosition = position;
    }

    private void Update()
    {
        sliderValue = handle.GetComponent<RectTransform>().anchoredPosition.x / (totalSliderRange / maxValue);
        linearMapping.GetComponent<LinearMapping>().value = sliderValue/maxValue;
    }


    public void SnapToPosition()
    {
        if (wholeNumbers)
        {
            sliderValue = Mathf.RoundToInt(sliderValue);

            //Cycle through each predefined anchor point
            for (int i = 0; i < anchorPoints.Length; i++)
            {
                //If lever is within "snapping distance" of that anchor point
                if (Mathf.Abs(handle.GetComponent<RectTransform>().anchoredPosition.x - anchorPoints[i]) < ((totalSliderRange / maxValue) / 2))
                {
                    //Get current lever position and update z pos to anchor point
                    Vector3 snapPosition = handle.GetComponent<RectTransform>().anchoredPosition;
                    snapPosition.x = anchorPoints[i];
                    handle.GetComponent<RectTransform>().anchoredPosition = snapPosition;
                    //Break so it stops checking other anchor points
                    break;
                }
            }
        }
    }

    public void ResetSliderPosition(bool random)
    {
        //// Allow other scripts to call and reset the slider value to zero or a random value
        Vector3 resetPosition = handle.GetComponent<RectTransform>().anchoredPosition;
        if (random)
        {
            resetPosition.x = anchorPoints[Random.Range((int)minValue, (int)maxValue)];
        }
        else
        {
            resetPosition.x = anchorPoints[0];
        }

        handle.GetComponent<RectTransform>().anchoredPosition = resetPosition;

    }
    
}
