using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightMapGenerator {

    public static HeightMap GenerateHeightMap(HeightMapSettings settings, Vector2 sampleCentre)
    {
        float[,] values = NoiseDemo.GenerateNoiseMap(settings.width, settings.height, settings.noiseSettings, sampleCentre);

        AnimationCurve heightCurve = new AnimationCurve(settings.heightCurve.keys);

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        for (int i = 0; i < settings.width; ++i)
        {
            for (int j = 0; j < settings.height; ++j)
            {
                values[i, j] *= settings.heightCurve.Evaluate(values[i, j]) * settings.heightMultiplier;

                if (values[i, j] > maxValue)
                    maxValue = values[i, j];
                if (values[i, j] < minValue)
                    minValue = values[i, j];
            }
        }

        return new HeightMap(values, minValue, maxValue);
    }
}

public struct HeightMap
{
    public readonly float[,] heightMap;
    public readonly float minValue;
    public readonly float maxValue;

    public HeightMap(float[,] heightMap, float minValue, float maxValue)
    {
        this.heightMap = heightMap;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }
}