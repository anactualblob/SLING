using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
    float y;
    float x = 0;


    void Awake()
    {
    }

    public void GenerateNoiseChunk(ref float[] buffer, float noiseScale, float offset, int octaves = 1, float lacunarity = 1.0f, float persistence = 1.0f)
    {


        float max = float.MinValue;
        float min = float.MaxValue;

        float a, f;

        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i] = 0;

            for (int j = 0; j < octaves; j++)
            {
                f = Mathf.Pow(lacunarity, j);
                a = Mathf.Pow(persistence, j);
                buffer[i] += Mathf.PerlinNoise(noiseScale * (float)i*f / (float)buffer.Length + x, offset) * a;
            }

            if (buffer[i] > max) max = buffer[i];
            if (buffer[i] < min) min = buffer[i];
        }

        // increment the x offset
        x++; 

        
        for (int i = 0; i < buffer.Length; i++)
        {
            // max should not be less than 1
            max = max <= 1 ? 1 : max;
            // min shoud not be more than 0
            min = min >= 0 ? 0 : min;
        
            // normalize values between min and max
            buffer[i] = Mathf.InverseLerp(min, max, buffer[i]);
        
            // remap the range (0,1) to (-0.5,1)
            //buffer[i] = buffer[i] * 1.5f - 0.5f;
        }

    }


}
