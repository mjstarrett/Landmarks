using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColorSpace
{
    ColorAlphabet,
    CIELAB,
    HSV
}

public class LM_TargetColorAssignments : ExperimentTask
{
    [Header("Task-Specific Properties")]

    public ColorSpace colorSpace;


    // IF USING ALPHABET COLORS (Green-Armytage, 2010) -------------------------
    [Header("Color Alphabet Settings (or other ColorList)")]
    public ColorList alphabet;


    // IF USING LAB COLOR SPACE ------------------------------------------------
    [Header("CIELAB Settings")]

    [Tooltip("Minimum value for Luminance in CIELAB color space")]
    [Range(0f, 100f)]
    public float Lmin = 25f;

    [Tooltip("Maximum value for Luminance in CIELAB color space")]
    [Range(0f, 100f)]
    public float Lmax = 75f;

    [Min(1)]
    public int uniqueLuminances = 3; // repeat colors this many times at different luminances


    // IF USING HSV COLORSPACE -------------------------------------------------
    [Header("HSV Settings")]

    [Tooltip("Specify the HSV starting value - assignments will move clockwise through Hue-space" +
    ", keeping saturation and value constant")]
    public Color startingColor = Color.HSVToRGB(0f, 1f, 0.6f);


    private new void Awake()
    {
        base.Awake();
        if (Lmin >= Lmax)
        {
            Debug.LogError(this.name + ": Lmin must be less than Lmax");
        }
        
        
    }

    public override void startTask()    
    {
        TASK_START();

        // LEAVE BLANK
    }


    public override void TASK_START()
    {
        if (!manager) Start();
        base.startTask();

        // WRITE TASK STARTUP CODE HERE
        if (manager.config.levelNumber != 0)
        {
            skip = true;
        }

        if (skip)
        {
            log.log("INFO    skip task    " + name, 1);
            return;
        }


        // ---------------------------------------------------------------------
        // Get the targets to be colored
        // ---------------------------------------------------------------------

        // create an array of the target objects' transforms
        var targetTargets = manager.targetObjects.transform.GetComponentsInChildren<LM_Target>();
        var targetChildren = new Transform[targetTargets.Length];
        for (int i = 0; i < targetChildren.Length; i++)
        {
            targetChildren[i] = targetTargets[i].GetComponent<Transform>();
        }
        Debug.Log(targetChildren.Length + " target objects detected");

        // Shuffle target stores so we don't get the same assignment in each instance
        for (int i = targetChildren.Length - 1; i > 0; i--)
        {
            var r = Random.Range(0, i);
            var tmp = targetChildren[i];
            targetChildren[i] = targetChildren[r];
            targetChildren[r] = tmp;
        }
        
        switch (colorSpace)
        {
            case ColorSpace.ColorAlphabet:
                // ---------------------------------------------------------------------
                // Set Up and Apply Alphabet colors (list of 26 distinct colors
                // ---------------------------------------------------------------------
                alphabet.ShuffleColors();
                for (int i = 0; i < targetChildren.Length; i++)
                {
                    //Debug.Log("using alhpabet " + i + " - " + alphabet.materials[i]);
                    targetChildren[i].GetComponent<LM_TargetStore>().ChangeMaterial(alphabet.materials[i]);
                    
                }
                break;


            case ColorSpace.CIELAB:
                // ---------------------------------------------------------------------
                // Set Up and Apply CIE-LAB colors
                // ---------------------------------------------------------------------

                // how many total colors are needed?
                if (manager.targetObjects.transform.childCount % uniqueLuminances != 0)
                {
                    Debug.LogError("number of unique luminances (" + uniqueLuminances + ") " +
                        "must be a multiple of the number of target stores (" + manager.targetObjects.transform.childCount + ")");
                }
                var nDistinct = (manager.targetObjects.transform.childCount / uniqueLuminances);
                Debug.Log("there will be " + nDistinct + " distinct colors at " + uniqueLuminances + " luminance unique values");

                // list of luminances to be used
                var luminances = new List<float>();
                if (uniqueLuminances == 1)
                {
                    Debug.Log("All colors luminance matched; using Lmax");
                    luminances.Add(Lmax);
                }
                else if (uniqueLuminances == 2)
                {
                    Debug.Log("Using Lmin and Lmax as the two luminance values");
                    luminances.Add(Lmin);
                    luminances.Add(Lmax);
                }
                else
                {
                    Debug.Log(">2 luminances... calculating...");

                    var Linterval = (Lmax - Lmin);

                    for (int i = 0; i < uniqueLuminances; i++)
                    {
                        luminances.Add(Lmin + i * ((Lmax - Lmin) / (uniqueLuminances - 1)));
                    }
                }

                // List of colors (including luminance) to be used
                var colors = new List<Color>();

                foreach (var lum in luminances)
                {
                    var l = lum;
                    for (int k = 0; k < nDistinct; k++)
                    {
                        // calculate the red-green opponency value for this color
                        var a = 100 * Mathf.Cos(k * 2 * Mathf.PI / nDistinct);
                        // calculate the yellow-blue opponency value for this color
                        var b = 100 * Mathf.Sin(k * 2 * Mathf.PI / nDistinct);

                        // use the specified luminance to create a CIELAB color
                        LABColor lab = new LABColor(l, a, b);
                        // Convert to RGB and apply to the stores exterior
                        Debug.Log(lab.ToString());
                        colors.Add(lab.ToColor());
                    }
                }

                Debug.Log(colors.Count);

                for (int i = 0; i < targetChildren.Length; i++)
                {
                    var child = targetChildren[i];
                    child.GetComponent<LM_TargetStore>().ChangeColor(colors[i]);

                    // FIXME check conversion
                    //Debug.Log("Target " + (count + 1) + "will be CIELAB " + LABColor.FromColor(colors[count]));
                }
                break;


            case ColorSpace.HSV:
                // ---------------------------------------------------------------------
                // Set Up and Apply HSV colors
                // ---------------------------------------------------------------------

                var hueIncrement = 360.0f / manager.targetObjects.transform.childCount;

                // handle the start color
                float currentHue, S, V;
                Color.RGBToHSV(startingColor, out currentHue, out S, out V);
                Debug.Log("colors will be " + hueIncrement + " degrees apart");

                foreach (Transform child in targetChildren)
                {
                    Debug.Log("COLOR (" + currentHue + ", " + S + ", " + V + ")");
                    child.GetComponent<LM_TargetStore>().ChangeColor(Color.HSVToRGB(currentHue / 360.0f, S, V, true));
                    currentHue += hueIncrement;
                }
                break;

            default:
                Debug.LogWarning("defaulting to LAB colorspace");
                goto case ColorSpace.CIELAB;
        }
    }


    public override bool updateTask()
    {
        return true;

        // WRITE TASK UPDATE CODE HERE
    }


    public override void endTask()
    {
        TASK_END();

        // LEAVE BLANK
    }


    public override void TASK_END()
    {
        base.endTask();

        // WRITE TASK EXIT CODE HERE
    }
}
