using UnityEngine;
using System.Collections;

/// <summary>
/// MeshGenerator übersetzt die in Noise generierte HeightMap in ein Mesh aus 
/// Punkten. Dazu werden Vertices, IndexArrays (Triangles), Farben (Colors) und
/// Texturkoordinaten (UVs) berechnet und in einem MeshData Objekt zurück gegeben.
/// </summary>
public static class MeshGenerator
{
    /// <summary>
    /// Generierung des Meshes auf Basis der übergebenen Werte
    /// </summary>
    /// <param name="heightMap">DIe in Noise entstandene HeightMap </param>
    /// <param name="heightMultiplier"> Skalierungswert der Höhe der Hügel </param>
    /// <param name="_heightCurve"> Lässt eine nicht-lineare Umsetzung der Höhenwerte zu </param>
    /// <param name="levelOfDetail"> Detailgrad des zu generierenden Meshes </param>
    /// <param name="flatShaded"> True, wenn die Landschaft per FlatShading erschaffen werden soll </param>
    /// <param name="yOffset"> Verschiebt die Map auf der Y-Achse </param>
    /// <param name="valleyWidth"> Breite des Tals für den Lauftrack </param>
    /// <param name="valleyheight"> Höhe des Tals für den Lauftrack </param>
    /// <param name="gradient"> Der Farbverlauf auf der Landschaft </param>
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve _heightCurve, int levelOfDetail, bool flatShaded, float yOffset, float valleyWidth, float valleyHeight, Gradient gradient)
    {
        AnimationCurve heightCurve = new AnimationCurve(_heightCurve.keys);

        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        // Das MeshSimplificationIncrement steuert den Detailgrad der Vertices im Mesh
        int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
        int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;

        MeshData meshData = new MeshData(verticesPerLine, verticesPerLine, flatShaded);
        int vertexIndex = 0;

        for (int z = 0; z < height; z += meshSimplificationIncrement)
        {
            for (int x = 0; x < width; x += meshSimplificationIncrement)
            {
                float tmpYOffset = yOffset;

                if (x < (width / 2.0 + valleyWidth / 2.0) && x > (width / 2.0 - valleyWidth / 2.0))
                {
                    tmpYOffset += Mathf.Abs(x - width / 2) - valleyHeight;
                }

                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x, z]) * heightMultiplier + tmpYOffset, topLeftZ - z);
                meshData.uvs[vertexIndex] = new Vector2(x / (float) width, z / (float) height);
                // Mit Evaluate wird eine verzerrbare Skalierung der HeightMap umgesetzt
                meshData.colors[vertexIndex] = gradient.Evaluate(meshData.vertices[vertexIndex].y);

                if (x < width - 1 && z < height - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                    meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        if (flatShaded)
        {
            meshData.FlatShading();
        }

        return meshData;
    }
}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;
    public Color[] colors;
    int triangleIndex;

    // Instanziert die benötigten MeshData Variablen
    public MeshData(int meshWidth, int meshHeight, bool flatShaded)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
        colors = new Color[meshWidth * meshHeight];
    }

    /// Fügt ein Polygon auf Basis von 3 Koordinaten (a, b, c) in das IndexArray 
    /// der Vertices hinzu
    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    /// Für das FlatShading dürfen sich Polygone keine Vertices teilen.
    /// Die entsprechenden Änderungen werden hier berechnet.
    public void FlatShading()
    {
        Vector3[] flatShadedVertices = new Vector3[triangles.Length];
        Vector2[] flatShadedUVs = new Vector2[triangles.Length];
        Color[] flatShadedColors = new Color[triangles.Length];

        for (int i = 0; i < triangles.Length; i++)
        {
            flatShadedVertices[i] = vertices[triangles[i]];
            flatShadedUVs[i] = uvs[triangles[i]];
            flatShadedColors[i] = colors[triangles[i]];
            triangles[i] = i;
        }

        vertices = flatShadedVertices;
        uvs = flatShadedUVs;
        colors = flatShadedColors;
    }

    // Erschafft ein Mesh Object
    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.colors = colors;
        mesh.RecalculateNormals();
        return mesh;
    }
}