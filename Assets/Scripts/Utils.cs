using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static float[,] GaussianKernel(int size, float weight)
    {
        /// <summary> Creates a 2d gaussian kernal and returns it. Note: allocates memory</summary>
        /// <param> 
        /// size:   The dimensions of the output multi-array 
        /// weight: The weight/scale of the gaussian bell curve
        /// </param>
        /*
         * <summary> Creates a 2d gaussian kernel and returns it
         * <param> size:    The dimensions of of the output multi-array
         * <param> weight:  
         */

        // Allocate memory for kernel and initialize variables
        float[,] kernel = new float[size, size];
        float sum = 0f;
        int kernelRad = size / 2;
        float distance = 0f;

        // The formula for the gaussian contains a constant. It is precalculated here
        float calcEuler = 1f / (2f * Mathf.PI * Mathf.Pow(weight, 2));

        // Calculate each cell of kernel
        for(int filterY = -kernelRad; filterY <= kernelRad; filterY++)
        {
            for(int filterX = -kernelRad; filterX<= kernelRad; filterX++)
            {
                distance = ((filterX * filterX) + (filterY * filterY)) / (2 * (weight * weight));

                kernel[filterY + kernelRad, filterX + kernelRad] = calcEuler * Mathf.Exp(-distance);

                sum += kernel[filterY + kernelRad, filterX + kernelRad];
            }
        }

        /*
         * Ensures that the elements of the kernel sum up to 1
         * Especially handy if the size of the kernel is too small for the gaussian bell-curve
         */
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
