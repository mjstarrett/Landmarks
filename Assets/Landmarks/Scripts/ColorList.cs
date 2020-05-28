using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorList : MonoBehaviour
{
    public Material[] materials;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ShuffleColors()
    {
        for (int i = materials.Length - 1; i > 0; i--)
        {
            var r = Random.Range(0, i);
            var tmp = materials[i];
            materials[i] = materials[r];
            materials[r] = tmp;
        }
    }
}
