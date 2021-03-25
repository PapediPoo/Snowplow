using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowArea : MonoBehaviour
{
    [Header("System")]
    public int resolution = 128;
    public Texture2D tex;
    public int initHeight = 255;


    [Header("Snow Properties")]
    public float density = 150;

    // Start is called before the first frame update
    void Start()
    {
        tex = new Texture2D(resolution, resolution);
        GetComponent<Renderer>().material.mainTexture = tex;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float ClearArea(Collider area, float capacity)
    {
        float coeff = transform.lossyScale.x * transform.lossyScale.y * transform.lossyScale.z / resolution / resolution;
        Vector4 index = BBoxToIndex(area.bounds);
        float sum = 0;
        int count = 0;
        for (int x = Mathf.RoundToInt(index.x); x < Mathf.RoundToInt(index.z); x++)
        {
            for (int y = Mathf.RoundToInt(index.y); y < Mathf.RoundToInt(index.w); y++)
            {
                sum += tex.GetPixel(x, y).grayscale;
                count++;
                // ex.SetPixel(x, y, Color.black);
            }
        }
        float volume = sum * coeff;

        float clearedCapacity = Mathf.Max(0f, capacity - volume);
        float adjvolume = (volume - clearedCapacity) / coeff / count;
        print(adjvolume);
        Color adjcolor = new Color(adjvolume, adjvolume, adjvolume);
        for (int x = Mathf.RoundToInt(index.x); x < Mathf.RoundToInt(index.z); x++)
        {
            for (int y = Mathf.RoundToInt(index.y); y < Mathf.RoundToInt(index.w); y++)
            {
                tex.SetPixel(x, y, adjcolor);
            }
        }

        tex.Apply();
        return clearedCapacity;
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
