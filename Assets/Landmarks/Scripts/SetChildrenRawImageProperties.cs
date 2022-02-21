using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetChildrenRawImageProperties : MonoBehaviour
{
    // Properties that can be set by the user
    public bool replacement;
    public List<Texture> textures = new List<Texture>();
    public List<Color> colors = new List<Color>();
    public List<Material> materials = new List<Material>();
    
    // Images to set properties to
    private RawImage[] images;
    // Hidden replicants for altering
    private List<Texture> currentTextures;
    private List<Color> currentColors;
    private List<Material> currentMaterials;



    // Start is called before the first frame update
    void Awake()
    {
        // Grab all the images nested under this object
        images = transform.GetComponentsInChildren<RawImage>();

        // Log an error to console if we're going to run out of properties to assign
        if ((textures.Count != 0 & textures.Count < images.Length) |
            (colors.Count != 0 & colors.Count < images.Length) |
            (materials.Count != 0 & materials.Count < images.Length))
        {
            if (replacement)
            {
                Debug.LogWarning("There are more assignments than assignable values; values will be repeated.");
            }
            else
            {
                Debug.LogError("There are more assignments than assignable values; replacement must be set to true and note there will be repeats.");
            }
        }

        Reset();
    }


    public void Reset()
    {
        // set our hidden lists based on public list provided to inspector
        currentTextures = new List<Texture>(textures);
        currentColors = new List<Color>(colors);
        currentMaterials = new List<Material>(materials);


        foreach (var img in images)
        {
            // Change image properties
            if (currentTextures.Count != 0)
            {
                var thisTexture = currentTextures[Random.Range(0, currentTextures.Count)]; // get
                img.texture = thisTexture; // set
                if (!replacement) currentTextures.Remove(thisTexture); // remove if necessary
            }

            if (currentColors.Count != 0)
            {
                var thisColor = currentColors[Random.Range(0, currentColors.Count)]; // get
                img.color = thisColor; // set
                if (!replacement) currentColors.Remove(thisColor); // remove if necessary
            }
            if (currentMaterials.Count != 0)
            {

                var thisMaterial = currentMaterials[Random.Range(0, currentMaterials.Count)]; // get
                img.material = thisMaterial; // set
                if (!replacement) currentMaterials.Remove(thisMaterial); // remove if necessary
            }
        }
    }
}
