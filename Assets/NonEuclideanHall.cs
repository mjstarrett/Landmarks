using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonEuclideanHall : MonoBehaviour
{
    private GameObject[] wallsToEnable, wallsToDisable;
    private int i, j;
    // Start is called before the first frame update
    private void Start()
    {
        wallsToEnable = GameObject.FindGameObjectsWithTag("enable");
        wallsToDisable = GameObject.FindGameObjectsWithTag("disable");
        i = 0;
        j = 0;
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Euclid"))
        {
            
            foreach (GameObject wall in wallsToEnable)
            {
                wallsToEnable[i].GetComponent<MeshRenderer>().enabled = true;
                i++;
            }

            
            foreach (GameObject wall in wallsToDisable)
            {
                wallsToDisable[j].GetComponent<MeshRenderer>().enabled = false;
                j++;
            }

        }

    }
}
