using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
    float y;


    void Awake()
    {
        // random offset stays the same for every game
        
    }

    public void GenerateNoiseChunk(ref float[] buffer, float startValue, float noiseScale, int octaves = 1, float lacunarity = 1.0f, float persistence = 1.0f)
    {

        

        y = Random.Range(0.0f, 1000.0f);
        for (int j = 0; j < octaves; j++)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = Mathf.PerlinNoise(noiseScale * (i + startValue) / buffer.Length , y);
            }
        }

    }


}
