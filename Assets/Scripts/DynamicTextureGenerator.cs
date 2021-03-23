using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicTextureGenerator : MonoBehaviour
{
    public int resolution = 32;
    public Texture2D tex;
    public GameObject obj;


    // Start is called before the first frame update
    void Start()
    {
        tex = new Texture2D(resolution, resolution);
        GetComponent<Renderer>().material.mainTexture = tex;
    }

    // Update is called once per frame
    void Update()
    {
        Vector4 index = BBoxToIndex(obj.GetComponent<Collider>().bounds);
        for(int x = Mathf.RoundToInt(index.x); x < Mathf.RoundToInt(index.z); x++)
        {
            for (int y = Mathf.RoundToInt(index.y); y < Mathf.RoundToInt(index.w); y++)
            {
                tex.SetPixel(x, y, Color.black);
            }
        }
        tex.Apply();


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
