using UnityEngine;
using System.Collections;


/// <summary>
/// Noise generiert ein 2D Array von float Werten, wobei jeder Wert die Höhe
/// des Terrains an einer bestimmten Koordinate repräsentiert. Es wird
/// PerlinNoise benutzt, um eine zufällige aber trotzdem kontinuierliche 
/// Umgebung zu generieren. Viele der Parameter in diesem Skript sind über
/// das MapGenerator Skript in Unity zu erreichen.
/// </summary>
public static class Noise
{
    public enum NormalizeMode
    {
        Local,
        Global
    };

    /// <summary>
    /// Generierung der HeightMap auf Basis der übergebenen Werte
    /// </summary>
    /// <param name="mapWidth"> X Ausbreitung</param>
    /// <param name="mapHeight"> Z Ausbreitung </param>
    /// <param name="seed"> Zufallswert </param>
    /// <param name="octaves"> Anzahl der Hügelstufen zwischen großen Bergen und feinem Rauschen </param>
    /// <param name="persistence"> Wert um den die Amplitude bei steigenden Octaves abnimmt </param>
    /// <param name="lacunarity"> Sprunggröße zwischen Frequenzunterschieden der Octaves</param>
    /// <param name="offset"> Statisches Offset in X und Z Richtung </param>
    /// <param name="normalizeMode"> Passiert die Normalisierung lokal per Block oder global auf Basis aller Blöcke </param>
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, NormalizeMode normalizeMode)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }

        // Sicherstellen dass scale > 0 ist
        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;


        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (normalizeMode == NormalizeMode.Local)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                }
                else
                {
                    float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }

        return noiseMap;
    }
}