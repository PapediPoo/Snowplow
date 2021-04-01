using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClearingMode
{
    Add,
    Subtract
}

public class SnowArea : MonoBehaviour
{

    [Header("System")]
    public int resolution = 128;
    public Texture2D tex;
    public float initHeight = .5f;

    public float noiseScale = 5f;
    public float noiseHeight = 0.2f;

    public string displacementTextureID;

    // Start is called before the first frame update
    void Start()
    {
        Color col = new Color(initHeight, 0f, 0f);
        Color[] cols = new Color[resolution * resolution];
        // Array.Fill(cols, Color.white); only supported for .net 5+
        for(int i = 0; i < cols.Length; i++)
        {
            //cols[i] = col;
            cols[i] = new Color(initHeight + Mathf.PerlinNoise(i % resolution / noiseScale, i / resolution / noiseScale) * noiseHeight, 0f, 0f);
        }

        tex = new Texture2D(resolution, resolution);
        tex.SetPixels(cols);
        tex.Apply();
        //GetComponent<Renderer>().material.mainTexture = tex;
        GetComponent<Renderer>().material.SetTexture(displacementTextureID, tex);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float ClearArea(Vector3 position, float[,] kernel, float capacity, ClearingMode mode)
    {
        Vector3 iPosition = PosToIndex(position);
        int rad = (int)Mathf.Sqrt(kernel.Length) / 2;

        float sum = 0f;
        float value;
        Color c = new Color(0f, 0f, 0f);
        for(int y=0;y<kernel.Length;y++)
        {
            for(int x =0; x <kernel.Length; x++)
            {
                try
                {
                    value = tex.GetPixel(Mathf.RoundToInt(iPosition.x) - rad + x, Mathf.RoundToInt(iPosition.z) - rad + y).r;
                    sum += Mathf.Clamp(kernel[y, x] * capacity, 0f, value);
                    if (mode == ClearingMode.Subtract)
                    {
                        c.r = Mathf.Max(0f, value - (kernel[y, x] * capacity));
                    }
                    else
                    {
                        c.r = value + (kernel[y, x] * capacity);
                    }
                    tex.SetPixel(Mathf.RoundToInt(iPosition.x) - rad + x, Mathf.RoundToInt(iPosition.z) - rad + y, c);

                }
                catch (IndexOutOfRangeException e)
                {

                }
            }
        }

        tex.Apply();
        return sum;
    }

    Vector4 BBoxToIndex(Bounds bbox)
    {
        Vector3 v1 = PosToIndex(bbox.center - bbox.extents);
        Vector3 v2 = PosToIndex(bbox.center + bbox.extents);
        return new Vector4(v1.x, v1.z, v2.x, v2.z);
    }

    Vector3 PosToIndex(Vector3 position)
    {
        Bounds bbox = GetComponent<Collider>().bounds;
        return InverseLerp(bbox.center - bbox.extents, bbox.center + bbox.extents, position) * resolution;
    }

    Vector3 InverseLerp(Vector3 a, Vector3 b, Vector3 value)
    {
        float x = Mathf.InverseLerp(a.x, b.x, value.x);
        float y = Mathf.InverseLerp(a.y, b.y, value.y);
        float z = Mathf.InverseLerp(a.z, b.z, value.z);
        return new Vector3(x, y, z);
    }
}
