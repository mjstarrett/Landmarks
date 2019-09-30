using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryDetection : MonoBehaviour
/*
 | The purpose of this script is to check a player's head, hands, etc. location
 | against preestablished boundaries, and warn them if they get too close (this is done by fading warning objects in and out)
 | Make sure the warnings are in the props layer
 |
 | Position tracking used instead of collision detection to save on resources
 | Useful for when the VR chaperone is forced off
 */
{   //empty game object that represents the "warning" popup
    public GameObject popupSign, popupText;
    //bufferRadius set in the inspector
    public float bufferRadius;
    //boundaries of the room set by preestablished wall objects in Environment
    private float xPosBound, xNegBound, zPosBound, zNegBound;
    //material array for the warning object;
    private Material[] signMats, textMats;
    //visibility boolean for toggling the fade in and fade out
    private bool visible;
    

    // Start is called before the first frame update
    void Start()
    {
        xPosBound = GameObject.FindWithTag("X+Bound").transform.position.x - bufferRadius;
        xNegBound = GameObject.FindWithTag("X-Bound").transform.position.x + bufferRadius;
        zPosBound = GameObject.FindWithTag("Z+Bound").transform.position.z - bufferRadius;
        zNegBound = GameObject.FindWithTag("Z-Bound").transform.position.z + bufferRadius;

        //sets popup to be the child object whose name is "warning"
        //popupSign = this.transform.Find("warning/Sign").gameObject;
        //popupText = this.transform.Find("warning/Text").gameObject;
        //gets the materials in popup and sets them to an array
        Debug.Log("The object is + " + popupSign.name);
        signMats = popupSign.GetComponent<MeshRenderer>().materials;
        textMats = popupText.GetComponent<MeshRenderer>().materials;
        //sets visibility to false
        visible = false;
        //sets the renderer to false at start
        

        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (this.transform.position.x >= xPosBound || this.transform.position.x <= xNegBound || this.transform.position.z >= zPosBound || this.transform.position.z <= zNegBound)
        {
            fadeIn();
            //Debug.Log("Your " + this.name + " is getting close to a boundary");
        }
        else
        {
            fadeOut();
            
        }
    }

    public void fadeOut()
    {
        Color color0 = signMats[0].color;
        Color color1 = signMats[1].color;
        Color color2 = textMats[0].color;

        if (signMats[0].color.a >= 0 && signMats[1].color.a >= 0 && textMats[0].color.a >=0)
        {
            signMats[0].color = new Color(color0.r, color0.g, color0.b, (color0.a - 0.01f));
            signMats[1].color = new Color(color1.r, color1.g, color1.b, (color1.a - 0.01f));
            textMats[0].color = new Color(color2.r, color2.g, color2.b, (color2.a - 0.01f));

        }
        else
        {
            visible = false;
        }

    }
    public void fadeIn()
    {
        Color color0 = signMats[0].color;
        Color color1 = signMats[1].color;
        Color color2 = textMats[0].color;

        if (signMats[0].color.a <= 1 && signMats[1].color.a <= 1)
        {
            signMats[0].color = new Color(color0.r, color0.g, color0.b, (color0.a + 0.01f));
            signMats[1].color = new Color(color1.r, color1.g, color1.b, (color1.a + 0.01f));
            textMats[0].color = new Color(color2.r, color2.g, color2.b, (color2.a + 0.01f));

        }
        else
        {
            visible = true;
        }
    }
}
