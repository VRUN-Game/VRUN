using UnityEngine;
using System;
using System.Threading;
using System.Collections.Generic;

/// <summary>
/// MapGenerator generiert ein fertiges Teilstück der Map mit Unterstützung von Multithreading, um eine
/// stabile Performance zu garantieren.
/// </summary>
public class MapGenerator : MonoBehaviour
{
    public enum DrawMode
    {
        NoiseMap,
        ColourMap,
        Mesh
    };

    public DrawMode drawMode;

    public Noise.NormalizeMode normalizeMode;

    public const int mapChunkSize = 97;
    public bool flatShaded;

    [Range(0, 4)] public int editorPreviewLOD;
    public float noiseScale;

    public int octaves;
    [Range(0, 1)] public float persistance;
    public float lacunarity;

    public float yOffset;
    public float valleyWidth;
    public float valleyHeight;

    public int seed;
    public Vector2 offset;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public TerrainType[] regions;
    public Gradient gradient;

    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    /// <summary>
    /// Startet die Berechnung eines MapData Blocks in einem neuen Thread
    /// </summary>
    /// <param name="centre">Position des Blocks </param>
    /// <param name="callback"> Der zu delegierende Callback </param>
    public void RequestMapData(Vector2 centre, Action<MapData> callback)
    {
        ThreadStart threadStart = delegate { MapDataThread(centre, callback); };

        new Thread(threadStart).Start();
    }

    /// <summary>
    /// Fügt das MapData Objekt einem Queue hinzu, welcher Schritt für Schritt
    /// abgearbeitet wird
    /// </summary>
    /// <param name="centre">Position des Blocks </param>
    /// <param name="callback"> Der hinzuzufügende Callback </param>
    void MapDataThread(Vector2 centre, Action<MapData> callback)
    {
        MapData mapData = GenerateMapData(centre);
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    /// <summary>
    /// Startet die Berechnung eines MeshData Blocks in einem neuen Thread
    /// </summary>
    /// <param name="centre">Position des Blocks </param>
    /// <param name="callback"> Der zu delegierende Callback </param>
    public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate { MeshDataThread(mapData, lod, callback); };

        new Thread(threadStart).Start();
    }

    /// <summary>
    /// Fügt das MeshData Objekt einem Queue hinzu, welcher Schritt für Schritt
    /// abgearbeitet wird
    /// </summary>
    /// <param name="centre">Position des Blocks </param>
    /// <param name="callback"> Der hinzuzufügende Callback </param>
    void MeshDataThread(MapData mapData, int lod, Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, lod, flatShaded, yOffset, valleyWidth, valleyHeight, gradient);
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }

    /// <summary>
    /// Überprüft in jedem Frame ob sich ein zu renderndes MapData oder MeshData Object im Queue befindet
    /// </summary>
    void Update()
    {
        if (mapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    /// <summary>
    /// Generieren der Hügellandschaft und der entsprechenden Farbe
    /// </summary>
    /// <param name="centre">Position des Blocks </param>
    MapData GenerateMapData(Vector2 centre)
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, centre + offset, normalizeMode);
        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                colourMap[y * mapChunkSize + x] = regions[0].colour;
            }
        }

        return new MapData(noiseMap, colourMap);
    }

    /// <summary>
    /// Sicherstellen eines einzuhaltenden Wertebereiches
    /// </summary>
    void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }

        if (octaves < 0)
        {
            octaves = 0;
        }
    }

    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float start;
    public Color colour;
}

public struct MapData
{
    public readonly float[,] heightMap;
    public readonly Color[] colourMap;

    public MapData(float[,] heightMap, Color[] colourMap)
    {
        this.heightMap = heightMap;
        this.colourMap = colourMap;
    }
}