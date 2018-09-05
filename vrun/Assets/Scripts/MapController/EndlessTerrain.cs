using UnityEngine;
using System.Collections.Generic;
using VRSF.Utils;

/// <summary>
/// EndlessTerrain generiert mehrere Meshes-Blöcke auf Basis der Position des Spielers
/// </summary>
public class EndlessTerrain : MonoBehaviour
{
    const float scale = 1f;
    const float viewerMoveThresholdForChunkUpdate = 25f;
    const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

    public LODInfo[] detailLevels;
    public static float maxViewZDst;
    public static float maxViewXDst;

    public Material mapMaterial;


    Vector2 viewerPositionOld;
    static MapGenerator mapGenerator;
    int chunkSize;
    int chunksVisibleInZViewDst;
    int chunksVisibleInXViewDst;

    public static Vector2 viewerPosition;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    static List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();
    private Transform viewer;

    /// <summary>
    /// Start wird am Anfang einmal aufgerufen, um alle Variablen zu initialisieren
    /// </summary>
    void Start()
    {
        viewer = VRSF_Components.CameraRig.transform;
        mapGenerator = FindObjectOfType<MapGenerator>();
        maxViewZDst = detailLevels[detailLevels.Length - 1].zSight;
        maxViewXDst = detailLevels[detailLevels.Length - 1].xSight;
        chunkSize = MapGenerator.mapChunkSize - 1;
        chunksVisibleInZViewDst = Mathf.RoundToInt(maxViewZDst / chunkSize);
        chunksVisibleInXViewDst = Mathf.RoundToInt(maxViewXDst / chunkSize);

        UpdateVisibleChunks();
    }

    /// <summary>
    /// Update wird in jedem Frame aufgerufen und es wird auf Basis eines Threshold
    /// überprüft ob die sichtbaren Terrain Blöcke aktualisiert werden sollen. Dies
    /// spart Rechenleistung
    /// </summary>
    void Update()
    {
        if (!viewer) return;
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z) / scale;

        if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
        {
            viewerPositionOld = viewerPosition;
            UpdateVisibleChunks();
        }
    }

    /// <summary>
    /// Führt die Neuberechnung der sichtbaren Blöcke aus und ist abhängig von der
    /// Spielerposition.
    /// </summary>
    void UpdateVisibleChunks()
    {
        for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++)
        {
            if (terrainChunksVisibleLastUpdate[i] == null) break;
            terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }

        terrainChunksVisibleLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordZ = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for (int zOffset = -chunksVisibleInZViewDst; zOffset <= chunksVisibleInZViewDst; zOffset++)
        {
            for (int xOffset = -chunksVisibleInXViewDst; xOffset <= chunksVisibleInXViewDst; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordZ + zOffset);

                if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                }
                else
                {
                    terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, detailLevels, transform, mapMaterial));
                }
            }
        }
    }

    /// <summary>
    /// Beschreibt einen Terrain Block bestehend aus einem Mesh von Vertices 	
    /// </summary>
    public class TerrainChunk
    {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        LODInfo[] detailLevels;
        LODMesh[] lodMeshes;

        MapData mapData;
        bool mapDataReceived;
        int previousLODIndex = -1;

        /// <summary>
        /// Konstruktor   	
        /// </summary>
        /// <param name="coord">Position der Terrain Blöcke </param>
        /// <param name="size">Größe der Terrain Blöcke </param>
        /// <param name="detailLevels">Detaillevel der Terrain Blöcke </param>
        /// <param name="parent">Referenz des übergeordneten Elements </param>
        /// <param name="material">Material der Terrain Blöcke </param>
        public TerrainChunk(Vector2 coord, int size, LODInfo[] detailLevels, Transform parent, Material material)
        {
            this.detailLevels = detailLevels;

            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer.material = material;

            meshObject.transform.position = positionV3 * scale;
            meshObject.transform.parent = parent;
            meshObject.transform.localScale = Vector3.one * scale;
            SetVisible(false);

            lodMeshes = new LODMesh[detailLevels.Length];
            for (int i = 0; i < detailLevels.Length; i++)
            {
                lodMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateTerrainChunk);
            }

            mapGenerator.RequestMapData(position, OnMapDataReceived);
        }

        /// <summary>
        /// Generiert die Textur nachdem auf Basis der Map Daten
        /// </summary>
        /// <param name="mapData">Map Daten zur Weiterleitung an den Texture Generator </param>
        void OnMapDataReceived(MapData mapData)
        {
            this.mapData = mapData;
            mapDataReceived = true;

            UpdateTerrainChunk();
        }


        /// <summary>
        /// Aktualisiere die Terrain Blöcke mit dem entsprechenden Detaillevel 	
        /// </summary>
        public void UpdateTerrainChunk()
        {
            if (mapDataReceived)
            {
                float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
                bool visible = viewerDstFromNearestEdge <= maxViewZDst;

                if (visible)
                {
                    int lodIndex = 0;

                    for (int i = 0; i < detailLevels.Length - 1; i++)
                    {
                        if (viewerDstFromNearestEdge > detailLevels[i].zSight)
                        {
                            lodIndex = i + 1;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (lodIndex != previousLODIndex)
                    {
                        LODMesh lodMesh = lodMeshes[lodIndex];
                        if (lodMesh.hasMesh)
                        {
                            previousLODIndex = lodIndex;
                            meshFilter.mesh = lodMesh.mesh;
                        }
                        else if (!lodMesh.hasRequestedMesh)
                        {
                            lodMesh.RequestMesh(mapData);
                        }
                    }

                    terrainChunksVisibleLastUpdate.Add(this);
                }

                SetVisible(visible);
            }
        }

        public void SetVisible(bool visible)
        {
            if (!meshObject) return;
            meshObject.SetActive(visible);
        }

        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }
    }

    /// <summary>
    /// Mesh in Abhängigkeit vom Detaillevel
    /// </summary>
    class LODMesh
    {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        int lod;
        System.Action updateCallback;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="lod">Level of Detail to be generated </param>
        /// <param name="updateCallback">Callback Reference</param>
        public LODMesh(int lod, System.Action updateCallback)
        {
            this.lod = lod;
            this.updateCallback = updateCallback;
        }

        void OnMeshDataReceived(MeshData meshData)
        {
            mesh = meshData.CreateMesh();
            hasMesh = true;

            updateCallback();
        }

        public void RequestMesh(MapData mapData)
        {
            hasRequestedMesh = true;
            mapGenerator.RequestMeshData(mapData, lod, OnMeshDataReceived);
        }
    }

    [System.Serializable]
    public struct LODInfo
    {
        public int lod;
        public float zSight;
        public float xSight;
    }
}