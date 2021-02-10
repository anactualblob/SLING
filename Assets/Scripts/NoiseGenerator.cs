using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
    float y;


    void Awake()
    {
        // random offset stays the same for every game
        y = Random.Range(0.0f, 1000.0f);
    }

    public void GenerateNoiseChunk(ref float[] buffer, float noiseScale, float offset, int octaves = 1, float lacunarity = 1.0f, float persistence = 1.0f)
    {

        float startValue = 0;

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
                buffer[i] += Mathf.PerlinNoise(noiseScale * (i*f + startValue) / buffer.Length , offset) * a;
            }

            if (buffer[i] > max) max = buffer[i];
            if (buffer[i] < min) min = buffer[i];
        }

        // remap the buffer from 
        for (int i = 0; i < buffer.Length; i++)
        {
            // max should not be less than 1
            max = max <= 1 ? 1 : max;
            buffer[i] = Mathf.InverseLerp(min, max, buffer[i]) * 1.5f - 0.5f;
        }

    }


}
