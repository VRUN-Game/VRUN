using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Klasse, ist für das Spawnen der Strecke, welche in Streckenabschnitte unterteilt ist, zuständig.
/// Die Streckenabschnitte werden dynamisch vor und hinter dem Spieler entfernt bzw. hinzugefügt. 
/// </summary>
public class TrackCtrl : MonoBehaviour
{
    [Header("Streckenabschnitt")] 
    public GameObject TrackTile;
    [Header("Abschnitte vor dem Spieler")] 
    public int NumberOfTilesInFrontDisplayed = 10;
    [Header("Abschnitte hinter dem Spieler")]
    public int NumberOfTilesBehindDisplayed = 10;

    //Offset, gibt an inwiefern sich die Streckenabschnitte überlappen
    public float TileOverlapOffset = 0;

    //Liste der Streckenabschnitte 
    [HideInInspector] public List<GameObject> ActiveTiles;

    private int _numberOfTilesDisplayed; //Anzahl aller existierender Streckenabschnitte
    private float _lastSpawnPosZ;        //Letzte Position an dem ein Streckenabschnitt gespawnt worden ist
    private bool _isInitialized = false; //Flag, ob die Strecke generiert worden ist.

    /// <summary>
    /// Funktion, wird von Unity aufgerufen, wenn das dazugehörige GameObject aktiviert wird.
    /// Fügt die entsprechenden Methoden dem Event-Handler-System hinzu.
    /// </summary>
    private void OnEnable()
    {
        EventManager.GameResetEventMethods += GameReset;
    }

    /// <summary>
    /// Funktion, wird von Unity aufgerufen, wenn das dazugehörige GameObject deaktiviert wird.
    /// Entfernt die entsprechenden Methoden aus dem Event-Handler-System.
    /// </summary>
    private void OnDisable()
    {
        EventManager.GameResetEventMethods -= GameReset;
    }

    /// <summary>
    /// Funktion, wird von Unity ausgeführt, wenn das Skript aktiviert wird. Diese Funktion wird vor der 
    /// Update Methode ausgeführt.
    /// </summary>
    private void Start()
    {
        InitializeTrackController();
    }

    /// <summary>
    /// Berechnung des nächsten Streckenabschnitts und Löschen der vorherigen Streckenabschnitte
    /// </summary>
    public void Update()
    {
        // Initialisiere den TrackController, wenn dies noch nicht passiert ist
        if (!_isInitialized)
        {
            InitializeTrackController();
            return;
        }

        int actualTracktileIndex = _numberOfTilesDisplayed / 2;
        float endOfTracktile = ActiveTiles[actualTracktileIndex].GetComponent<MeshRenderer>().bounds.max.z;

        // Wechselt der Spieler den aktuellen Streckenabschnitt wird ein neuer Streckenabschnitt gespawnt und der
        // erste gelöscht 
        if (GlobalDataHandler.GetPlayerPosition() > endOfTracktile)
        {
            SpawnTile(GetTrackTile());
            DestroyTile();
        }
    }

    /// <summary>
    /// Funktion, initialisiert den TrackController und spawnt die festgelegten Streckenabschnitte vor und hinter dem
    /// Spieler. 
    /// </summary>
    private void InitializeTrackController()
    {
            _numberOfTilesDisplayed = NumberOfTilesBehindDisplayed + NumberOfTilesInFrontDisplayed;

            float lengthOfTilesBehind = 0;

            List<GameObject> spawnTiles = new List<GameObject>();
            ActiveTiles = new List<GameObject>();

            //Initialisiere die ersten Streckenteile 
            for (int i = 0; i < _numberOfTilesDisplayed; i++)
            {
                GameObject newTrack = GetTrackTile();
                spawnTiles.Add(newTrack);

                if (i < NumberOfTilesBehindDisplayed)
                {
                    lengthOfTilesBehind += newTrack.GetComponent<MeshRenderer>().bounds.size.z;
                }
            }

            //Initiale Spawnposition
            _lastSpawnPosZ = GlobalDataHandler.GetPlayerPosition() - lengthOfTilesBehind
                             + (spawnTiles[0].GetComponent<MeshRenderer>().bounds.size.z / 2);

            // Fülle das Streckenarray 
            foreach (var tile in spawnTiles)
            {
                SpawnTile(tile);
            }


            spawnTiles.Clear();
            _isInitialized = true;
    }

    /// <summary>
    /// Gibt einen  Streckenabschnitt wieder. 
    /// </summary>
    /// <returns></returns>
    private GameObject GetTrackTile()
    {
        return TrackTile;
    }

    /// <summary>
    /// Funktion, instanziiert und platziert ein Streckenabschnitt.
    /// </summary>
    private void SpawnTile(GameObject newTrackTile)
    {
        GameObject go = Instantiate(newTrackTile);
        go.transform.SetParent(transform);

        float lastTileSize = 0;

        //Bestimme die neue Position 
        if (ActiveTiles.Count != 0)
        {
            lastTileSize = ActiveTiles[ActiveTiles.Count - 1].GetComponent<MeshRenderer>().bounds.size.z / 2;
            lastTileSize -= TileOverlapOffset;
        }        

        float nextTileSize = newTrackTile.GetComponent<MeshRenderer>().bounds.size.z / 2;
        nextTileSize -= TileOverlapOffset;


        _lastSpawnPosZ = (_lastSpawnPosZ + lastTileSize + nextTileSize);
        go.transform.localPosition = new Vector3(go.transform.position.x, go.transform.position.y, _lastSpawnPosZ);
        ActiveTiles.Add(go);
    }

    /// <summary>
    /// Funktion, löscht einen Streckenabschnitt und entfernt die Referenz aus der Liste.
    /// </summary>
    private void DestroyTile()
    {
        Destroy(ActiveTiles[0]);
        ActiveTiles.RemoveAt(0);
    }

    /// <summary>
    /// Funktion, wird bei einem GameRest ausgeführt. Leert die Liste der gespeicherten Streckenabschnitte und
    /// initialisiert den TrackController neu. 
    /// </summary>
    public void GameReset()
    {
        _lastSpawnPosZ = 0;
        ActiveTiles.Clear();
        InitializeTrackController();
    }
}