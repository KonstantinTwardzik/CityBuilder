using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {
    
    public static float fBM (float x, float y, int octaves, float persistence, float frequency)
    {
        float total = 0;
        float frequence = 1;
        float amplitude = 1;
        float maxValue = 0;
        for (int i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise(x * frequency, y * frequency) * amplitude;
            maxValue += amplitude;
            amplitude *= persistence;
            frequence *= frequency;
        }
        return total / maxValue;
    }
}
