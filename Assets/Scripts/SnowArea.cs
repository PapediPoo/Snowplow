using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Robin Schmidiger
// April 2021
// Version: 1.05

// Controls if snow should be added or removed
public enum ClearingMode
{
    Add,
    Subtract
}

public class SnowArea : MonoBehaviour
{
    /// <summary>
    /// Creates an area where snow is displayed. The snow can be deformed using the ClearArea() method.
    /// Uses an underlying texture to store the state of the snow in the SnowArea. Communicates with a shader to actually display the snow displacement
    /// </summary>

    [Header("System")]
    public int resolution = 128;    // The resolution of the underlying texture that stores the snow height
    public Texture2D tex;           // Reference to the underlying snow texture
    public Texture2D mask;          // A texture that indicates which are should not count towards the total amount of remaining snow
    public LayerMask scanmask;      // A layermask that indicates which objects should be counted towards the mask texture
    public float initHeight = .5f;  // The initial hieght of the snow

    // all snow at the same height would be boring. So I add a bit of perlin noise
    public float noiseScale = 5f;       // Controls the scale of the noise
    public float noiseHeight = 0.2f;    // Controls the amplitude of the noise

    public float distanceClearThreshold = 0.5f; // Controls how close the clearing object has to be to the SnowArea in order to register the clearing

    public string displacementTextureID;    // Sends the underlying texture to the gameobject-renderer's material using this ID. This is used by the shader to find the right texture for displacement

    private float initSnow;             // Stores how much snow there was at the start

    // Start is called before the first frame update
    public void Start()
    {
        // Initializes the snow to initHeight + noise and creates a color array from it
        Color[] colorarray = new Color[resolution * resolution];
        for(int i = 0; i < colorarray.Length; i++)
        {
            colorarray[i] = new Color(initHeight + Mathf.PerlinNoise(i % resolution / noiseScale, i / resolution / noiseScale) * noiseHeight, 0f, 0f);
        }

        // Creates a texture from the color array
        tex = new Texture2D(resolution, resolution);
        tex.SetPixels(colorarray);
        tex.Apply();

        // Sends the texture to the renderer's material
        GetComponent<Renderer>().material.SetTexture(displacementTextureID, tex);

        // Create the mask texture by sequentially scanning the terrain and checking if there is an object blocking the area
        // Note: technically mask & tex could be combined to a single texture using different channels. Keeping them separate is just easier to debug and performance isn't a huge issue, so I just leave them as 2 individual textures.
        mask = new Texture2D(resolution, resolution);
        Collider collider = GetComponent<Collider>();
        for (int x = 0; x < resolution; x++)
        {
            for(int y = 0; y < resolution; y++)
            {
                Vector3 pos = collider.bounds.center - collider.bounds.extents + new Vector3(collider.bounds.size.x * x / resolution, collider.bounds.size.y + 0.6f,collider.bounds.size.z * y / resolution);
                if(Physics.CheckSphere(pos, 0.5f, scanmask)){
                    mask.SetPixel(x, y, Color.black);
                }
                else
                {
                    mask.SetPixel(x, y, Color.red); // I only use the R channel. No particular reason, I just like red
                }
            }
        }
        mask.Apply();

        // Calculate the initial snow
        initSnow = GetRemainingSnow();
    }

    public float ClearArea(Vector3 position, float[,] kernel, float capacity, ClearingMode mode)
    {
        /// <summary>
        /// Either adds or removes snow at the indicated position depeinding on the clearingmode.
        /// Note: Adding/removing snow isn't a lossless operation. But for the purposes of this game that's fine since the snow "simulation" isn't meant to be physically accurate
        /// </summary>
        /// <params>
        /// position:   global position where the snow should be added/removed
        /// kernel:     shape of the snow manipulation area
        /// capacity:   amount of snow to be modified at max
        /// mode:       whether snow should be added or removed
        /// </params>

        // If the position isn't close to the snow plane, return
        if (Mathf.Abs(position.y - transform.position.y) > distanceClearThreshold)
            return 0f;

        // Turn the position into an index for the texture
        Vector3 iPosition = PosToIndex(position);

        // extract the kernel radius from the kernel size
        int rad = (int)Mathf.Sqrt(kernel.Length) / 2 + 1;

        float sum = 0f;                     // accumulator to sum up the snow in the kernel area
        float value;                        // temp var for holding the current pixel value
        Color c = new Color(0f, 0f, 0f);    // Temp var for holding the color of the current pixel
        for(int y=0;y<kernel.Length;y++)
        {
            for(int x =0; x <kernel.Length; x++)
            {
                try
                {
                    value = tex.GetPixel(Mathf.RoundToInt(iPosition.x) - rad + x, Mathf.RoundToInt(iPosition.z) - rad + y).r;
                    sum += Mathf.Clamp(kernel[y, x] * capacity, 0f, value);

                    // depending on the mode, either add or remove "value" from the pixel at the current position
                    if (mode == ClearingMode.Subtract)
                    {
                        // Since the amount of snow can't be lower than 0 at a given position, avoid negative values.
                        c.r = Mathf.Max(0f, value - (kernel[y, x] * capacity));
                    }
                    else
                    {
                        c.r = value + (kernel[y, x] * capacity);
                    }

                    // write new value to the current pixel
                    tex.SetPixel(Mathf.RoundToInt(iPosition.x) - rad + x, Mathf.RoundToInt(iPosition.z) - rad + y, c);

                }
                catch (IndexOutOfRangeException)
                {
                    // Ignore if indexed outside of the texture
                }
            }
        }

        tex.Apply();
        return sum;
    }

    public float GetRemainingSnow()
    {
        ///<summary>
        /// returns the total remaining snow
        /// iterates over every pixel of the underlying texture and sums everything up (if not masked)
        /// </summary>
        float result = 0f;
        for(int x = 0; x < resolution; x++)
        {
            for(int y = 0; y < resolution; y++)
            {
                result += tex.GetPixel(x, y).r * mask.GetPixel(x, y).r;
            }
        }
        return result;
    }

    public float GetInitSnow()
    {
        /// <summary>
        /// returns the amount of snow that existed when the game started
        /// </summary>
        return initSnow;
    }

    Vector3 PosToIndex(Vector3 position)
    {
        /// <summary>
        /// turns a global position into an index for the underlying texture.
        /// Does no boundary checks
        /// </summary>
        /// <params>
        /// position:   global position to be turned into an index
        /// </params>
        /// <returns>
        /// the resulting index from the input position
        /// </returns>
        Bounds bbox = GetComponent<Collider>().bounds;
        return InverseLerp(bbox.center - bbox.extents, bbox.center + bbox.extents, position) * resolution;
    }

    Vector3 InverseLerp(Vector3 a, Vector3 b, Vector3 value)
    {
        /// <summary>
        /// wrapper function Mathf.InverseLerp applied to components of a Vector3
        /// </summary>
        float x = Mathf.InverseLerp(a.x, b.x, value.x);
        float y = Mathf.InverseLerp(a.y, b.y, value.y);
        float z = Mathf.InverseLerp(a.z, b.z, value.z);
        return new Vector3(x, y, z);
    }
}
