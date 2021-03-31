using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static float[,] GaussianKernel(int size, float weight)
    {
        float[,] kernel = new float[size, size];
        float sum = 0f;

        int kernelRad = size / 2;
        float distance = 0f;

        float calcEuler = 1f / (2f * Mathf.PI * Mathf.Pow(weight, 2));

        for(int filterY = -kernelRad; filterY <= kernelRad; filterY++)
        {
            for(int filterX = -kernelRad; filterX<= kernelRad; filterX++)
            {
                distance = ((filterX * filterX) + (filterY * filterY)) / (2 * (weight * weight));

                kernel[filterY + kernelRad, filterX + kernelRad] = calcEuler * Mathf.Exp(-distance);

                sum += kernel[filterY + kernelRad, filterX + kernelRad];
            }
        }

        for(int y=0; y < size; y++)
        {
            for(int x = 0; x < size; x++)
            {
                kernel[y, x] = kernel[y, x] * (1f / sum);
            }
        }

        return kernel;
    }
}
