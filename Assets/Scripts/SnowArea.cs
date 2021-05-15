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
    public Texture2D mask;
    public LayerMask scanmask;
    public float initHeight = .5f;

    public float noiseScale = 5f;
    public float noiseHeight = 0.2f;

    public float distanceClearThreshold = 0.5f;

    public string displacementTextureID;

    private float counter;
    public float updateScoreEvery = 2f;

    private float initSnow;

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

        mask = new Texture2D(resolution, resolution);
        for(int x = 0; x < resolution; x++)
        {
            for(int y = 0; y < resolution; y++)
            {
                Collider c = GetComponent<Collider>();
                Vector3 pos = c.bounds.center - c.bounds.extents + new Vector3(c.bounds.size.x * x / resolution, c.bounds.size.y + 0.6f,c.bounds.size.z * y / resolution);
                if(Physics.CheckSphere(pos, 0.5f, scanmask)){
                    mask.SetPixel(x, y, Color.black);
                }
                else
                {
                    mask.SetPixel(x, y, Color.red);
                }
            }
        }
        mask.Apply();

        initSnow = GetRemainingSnow();
    }

    // Update is called once per frame
    void Update()
    {
        counter += Time.deltaTime;
        if(counter >= updateScoreEvery)
        {
            counter -= updateScoreEvery;
            print(GetRemainingSnow());
        }
    }

    public float ClearArea(Vector3 position, float[,] kernel, float capacity, ClearingMode mode)
    {
        if (Mathf.Abs(position.y - transform.position.y) > distanceClearThreshold)
            return 0f;

        Vector3 iPosition = PosToIndex(position);
        int rad = (int)Mathf.Sqrt(kernel.Length) / 2 + 1;

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

    public float GetRemainingSnow()
    {
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
        return initSnow;
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
