using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Helfeklasse zur einfacheren Darstellung der Strukturen im Unity Editor.
/// </summary>
[System.Serializable]
public class ObstaclesList
{
    public GameObject Obstcls;
    public bool IgnoreLanePosition;
    public bool IsStoneFigure;
}

/// <summary>
/// Helfeklasse zur einfacheren Darstellung der Strukturen im Unity Editor.
/// </summary>
[System.Serializable]
public class CollectiblesList
{
    public GameObject Cllbls;
    public bool CanGroup;
}

/// <summary>
/// Klasse, ist für das Spawnen der auftauchenden Objekte auf der Strecke zuständig.
/// </summary>
public class GeneralSpawner : MonoBehaviour
{
    //Obstacles
    public ObstaclesList[] Obstacles; //Liste der Hindernisse
    public float MinDistance = 20;    //maximale Spawndistanz
    public float MaxDistance = 30;    //minimale Spawndistanz

    //Portal
    public GameObject Portal;        //Portal
    public GameObject Particles;     //Partikel des Portaks
    public string NextLevelToLoad;   //Name der nächsten Szene
    public int PortalSpawnPosition;  //Position an der das Portal gespawnr werden soll

    //Collectibles
    public CollectiblesList[] Collectibles;

	private int _billowOffset;                            // Offset, der wabernden Coins
	private int[] _groupMembers = { 1, 3, 7 };            // Liste mit Anzahl der Gruppengrößen von Coins
	private ObjectList _objList;                          // Liste, welche die Objekte speichert 
    private float _lastSpawnPosZ;                         // z-Position des letzten gespawnten Objektes    
    private float _lastSpawnPosZCollectibles;             // z-Position des letzten Collectibles
    private const float InitSpawnOffset = 100f;           // Spawndistanz zu dem Spieler bei Beginn des Levels
    private static System.Random _rnd;                    // Zufallsvariable
    private const float OffsetBetweenCollectibles = 1.5f; //Abstand zwischen den Objekten.
    private const int AmountOfGroupItems = 6;             // Anzahl der Objekte in einer Gruppe.

    /// <summary>
    /// Funktion, wird von Unity ausgeführt, wenn das Skript aktiviert wird. Diese Funktion wird vor der 
    /// Update Methode ausgeführt.
    /// </summary>
    private void Start()
    {
		_billowOffset = _groupMembers[_groupMembers.Length - 1];
		_rnd = new System.Random();
        _objList = GetComponent<ObjectList>();
        _lastSpawnPosZ = GlobalDataHandler.GetPlayerPosition() + _objList.MaxSpawnDistance - InitSpawnOffset;
        SpawnPortal();
    }

    /// <summary>
    /// Funktion, wird von Unity jeden Frame aufgerufen.
    /// </summary>
    private void Update()
    {
        if (!GlobalDataHandler.GetGameModus()) return;
        if (!_objList.IsListFull())
        {
            SpawnObject();
            SpawnCollectibles();
        }

        DeleteObjects();
    }

    /// <summary>
    /// Funktion, ist für das Spawnen eines Objektes zuständig. Bestimmt zufällig Typ, Position und des
    /// jeweiligen Objektes und fügt das Objekt bei Erfolg der Objektliste hinzu.
    /// </summary>
    private void SpawnObject()
    {
        //Bestimme welches Objekt spawnen soll
        var index = DetermineNextObstacle();

        //Bestimme die z-Position
        var z = _lastSpawnPosZ + Random.Range(MinDistance, MaxDistance);
        if (z >= PortalSpawnPosition - MaxDistance) return;

        //Bestimme die x-Position
        var x = Obstacles[index].IgnoreLanePosition ? 0 : DetermineNextLanePos();

        //Ausnahme für die Skulpturen 
        if (Obstacles[index].IsStoneFigure)
        {
            x = _rnd.Next(0, 2) != 0 ? 0 : 2;
        }

        //Bestimme die Rotation des Objektes
        var rotation = Obstacles[index].IsStoneFigure
            ? new Quaternion(
                Obstacles[index].Obstcls.transform.localRotation.x * (x == 0 ? -1.0f : 1.0f),
                Obstacles[index].Obstcls.transform.localRotation.y,
                Obstacles[index].Obstcls.transform.localRotation.z,
                Obstacles[index].Obstcls.transform.localRotation.w * (x == 0 ? -1.0f : 1.0f)
            )
            : Obstacles[index].Obstcls.transform.rotation;

        //Setze die Position
        var position = new Vector3(
            Obstacles[index].IsStoneFigure ? Obstacles[index].Obstcls.transform.position.x - (x == 0 ? 10 : 0) : x,
            Obstacles[index].Obstcls.transform.position.y,
            z
        );

        //Instanziiere das Objekt
        var go = Instantiate(Obstacles[index].Obstcls, position, rotation);
        go.transform.SetParent(transform);

        //Füge das Objekt der Objektliste hinzu. Wenn dies nicht möglich ist,
        //lösche das Objekt
        if (_objList.AddItem(go, false))
        {
            _lastSpawnPosZ = z;
        }
        else
        {
            Destroy(go);
        }
    }

    /// <summary>
    /// Funktion, ist für das Spawnen des Portals zuständig. 
    /// </summary>
    private void SpawnPortal()
    {
        //Beende die Funktion, wenn es kein weiteres Level gibt
        if (NextLevelToLoad == "None") return;

        //Instanziiere das Portal
        var go = Instantiate(
            Portal,
            new Vector3(Portal.transform.position.x, Portal.transform.position.y, PortalSpawnPosition),
            Portal.transform.rotation
        );
        go.name = NextLevelToLoad;
        go.transform.SetParent(transform);

        //Instanziiere das dazugehörige Partikelsystem
        var go1 = Instantiate(
            Particles,
            new Vector3(Particles.transform.position.x, Particles.transform.position.y, PortalSpawnPosition),
            Particles.transform.rotation
        );
        go1.transform.SetParent(transform);
    }

    /// <summary>
    /// Funktion, ist für das Spawnen der Collectibles zuständig. 
    /// </summary>
    private void SpawnCollectibles()
    {
        //Bestimme welches Collectible gespawnt werden soll
        var index = DetermineNextCollectible();

        //Bestimme die z-Position
        var z = _lastSpawnPosZCollectibles + Random.Range(MinDistance, MaxDistance);
		if (z >= PortalSpawnPosition - MaxDistance) return;

        //Bestimme die x-Position
		var x = DetermineNextLanePos();

        //Liste zur Zwischenspeicherung von Objekten.
        var gos = new List<GameObject>();

        //Unterscheidung ob ein Objekt in einer Gruppe gespawnt werden kann.
		if (Collectibles[index].CanGroup && Random.Range(0, 2) == 1)
        {
			float lastPosZ = 0;

			for (int i = 0; i < AmountOfGroupItems; i++)
            {
                if (i == 0)
                {
                    lastPosZ = z;
                }
                else
                {
                    lastPosZ += OffsetBetweenCollectibles;
                }

                var go = Instantiate(
                    Collectibles[index].Cllbls,
                    new Vector3(x, Collectibles[index].Cllbls.transform.position.y, lastPosZ),
                    Collectibles[index].Cllbls.transform.rotation
                );

                //Prüfung ob Position des Objekts bereits in Verwendung ist.
                if (!GetComponent<ObjectList>().IsPositionUsed(go.transform, true))
                {
                    gos.Add(go);
		            go.GetComponent<PointController>().SetStartValue(_billowOffset);
					_billowOffset = (_billowOffset == 0) ? _groupMembers[_groupMembers.Length - 1] : _billowOffset - 1;
				}
				else
                {
                    Destroy(go);
                    break;
                }
            }

			z = lastPosZ;
        }
        else
        {
            gos.Add(Instantiate(
                Collectibles[index].Cllbls,
                new Vector3(x, Collectibles[index].Cllbls.transform.position.y, z),
                Collectibles[index].Cllbls.transform.rotation
            ));
        }

        //Füge generierte Objekte aus der gos Liste in die Liste der aktiven Elemente ein.
        foreach (var go in gos)
        {
            go.transform.SetParent(transform);
            //Add the object to the list
            if (_objList.AddItem(go, true))
            {
                _lastSpawnPosZCollectibles = z;
            }
            else
            {
                Destroy(go);
            }
        }

        gos.Clear();
    }

    /// <summary>
    /// Funktion, gibt einen zufälligen Index der Obstacles-Liste wieder und bestimmt damit das nächste
    /// Obstacle, welches gespawnt werden soll. Der zufällige Index orientiert sich an der Länge der Liste. 
    /// </summary>
    /// <returns>Zufälliger Listenindex</returns>
    private int DetermineNextObstacle()
    {
        return Random.Range(0, Obstacles.Length);
    }

    /// <summary>
    /// Funktion, bestimmt zufällig einen Collectible Typen für das nächste Spawning. 
    /// </summary>
    /// <returns>Index, der auf das nächste Collectible Element in der Liste verweist</returns>
    private int DetermineNextCollectible()
    {
		var index = Random.Range(1, Collectibles.Length);
		return Random.Range(0f, 1f) <= 0.7f ? 0 : index;

	}

    /// <summary>
    /// Funktion, gibt zufällig eine x-Position der Streckenspuren wieder. 
    /// </summary>
    /// <returns>x-Position, für das nächste Objektspawning</returns>
    private static int DetermineNextLanePos()
    {
        var randomLineIdx = Random.Range(0, GlobalDataHandler.GetLanePositions().Length);
        return GlobalDataHandler.GetLanePositions()[randomLineIdx];
    }

    /// <summary>
    /// Funktion, löscht das erste (und damit älteste) Elememt aus der Objektliste, wenn die Liste voll ist.
    /// </summary>
    private void DeleteObjects()
    {
        while (_objList.IsListFull())
        {
            _objList.RemoveLastItem();
        }
    }
}