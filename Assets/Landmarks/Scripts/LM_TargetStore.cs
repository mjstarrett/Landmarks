using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LM_TargetStore : LM_Target
{
    //[Header("Store Identity")]
    //public Sprite storeIcon;
    [Header("Store-Specific Properties")]
    public GameObject[] exteriorElements;
    public Text[] signTextElements;
    public Image[] iconImageElements;
    [Header("Door Properties")]
    public GameObject door;
    public float doorMaxOpenAngle = -115;
    public float doorSpeedMulitplier = 1;
    private bool doorOpen;
    private bool doorInMotion;

    //private Color storeColor;


    private void Awake()
    {
        //if (!useExisting)
        //{
        //    // Assign any text elements
        //    foreach (var textitem in signTextElements)
        //    {
        //        textitem.text = gameObject.name;
        //    }
        //    // Assign any icon elements
        //    foreach (var iconitem in iconImageElements)
        //    {
        //        iconitem.sprite = storeIcon;
        //    }

        //    storeColor = color;
        //}
        //else storeColor = exteriorElements[0].GetComponent<Renderer>().material.color;
    }

    // Start is called before the first frame update
    void Start()
    {
        door.transform.localEulerAngles = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {

        
    }

    public void ChangeMaterial(Material mat)
    {
        foreach (var elem in exteriorElements)
        {
            elem.GetComponent<Renderer>().material = mat;
        }
    }

    public void ChangeColor(Color col)
    {
        foreach (var elem in exteriorElements)
        {
            elem.GetComponent<Renderer>().material.color = col;
            Debug.Log("Element " + elem.name.ToString() + " changed to color " + col);
        }
    }

    public void OpenDoor()
    {
        if (!doorInMotion) StartCoroutine(Open());
    }

    public void CloseDoor()
    {
        if (!doorInMotion) StartCoroutine(Close());
    }

    IEnumerator Open()
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

    IEnumerator Close()
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
