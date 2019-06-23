using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseDemo : MonoBehaviour {

    public static float[,] GenerateNoiseMap(int width, int height, NoiseSettings settings, Vector2 sampleCentre)
    {
        float[,] noiseMap = new float[width, height];

        System.Random prng = new System.Random(settings.seed);
        Vector2[] octaveOffsets = new Vector2[settings.octaves];
        // sample octaves from different locations
        for (int i = 0; i < settings.octaves; ++i)
        {
            float offsetX = prng.Next(-100000, 100000) + settings.offset.x + sampleCentre.x;
            float offsetY = prng.Next(-100000, 100000) + settings.offset.y - sampleCentre.y;

            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        // normaliing values in the range of 0:1
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = width / 2f;
        float halfHeight = height / 2f;

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                float amplitude = 1f;
                float frequency = 1f;
                float noiseHeight = 0f;
                for (int i = 0; i < settings.octaves; ++i)
                {
                    float sampleX = (x - halfWidth) / settings.scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / settings.scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Noise.Perlin3D(new Vector3(sampleX, sampleY, 1f), 1f);
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= settings.persistance;
                    frequency *= settings.lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
}

[System.Serializable]
public class NoiseSettings
{
    public float scale = 50;

    public int octaves = 5;
    [Range(0, 1)]
    public float persistance = 0.6f;
    public float lacunarity = 2;

    public int seed;
    public Vector2 offset;

    public void ValidateValues()
    {
        scale = Mathf.Max(scale, 0.01f);
        octaves = Mathf.Max(octaves, 1);
        lacunarity = Mathf.Max(lacunarity, 1);
        persistance = Mathf.Clamp01(persistance);
    }
}
