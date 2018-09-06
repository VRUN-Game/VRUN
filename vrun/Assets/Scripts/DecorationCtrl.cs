using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Generiert die Objekte die neben der Spur im Level 3 platziert werden. 
/// </summary>
public class DecorationCtrl : MonoBehaviour
{
    /// <summary>
    /// Klasse, die alle Dekorationsobjekte hält und im Kontext des Dekorationskontrollers
    /// einfacher Zugänglich macht. 
    /// </summary>
    [Serializable]
    public class DecorationItem
    {
        public GameObject Go; //GameObject welches in der Szene dargestellt wird.
        public CustomVector Height; //Bereich auf der Y-Achse in dem das GameObject instanziiert wird.
    }

    /// <summary>
    /// Klasse, die eine einfachere Darstellung des Spawnbereichs definiert. 
    /// </summary>
    [Serializable]
    public class CustomVector
    {
        public float Min; //Minimaler Wert
        public float Max; //Maximaler Wert
    }

    /// <summary>
    /// Breite des Bereichs in dem die Objekte platziert werden. Jeweils Links und Rechts von der Strecke.
    /// </summary>
    [Header("Bereich nach Links/Rechts in dem Objekte gespawnt werden")]
    public int DefineSegmentWidth = 20;

    /// <summary>
    /// Angabe ob Objekte auf beiden Seiten der Strecke platziert werden sollen.
    /// </summary>
    [Header("Beide Seiten (Sonst nur rechts)")]
    public bool BothSides = false;

    public GameObject ParentGameObject; //Elternelement in der Szene, für eine Strukturiertere instanziierung innerhalb der Laufzeit.
    public float ZDistanceToNextObj = 10; //Abstand auf der Z-Achse zwischen zwei Objekten.
    public int MaxPartsToSpawn = 150; //Maximale Anzahl an Objekten die instanziiert werden.
    public int OffsetToPlayer = 100; //Abstand zum Spieler.

    /// <summary>
    /// Liste, die alle Objekte aus dem Editor hält und übersichtlich darstellt.
    /// </summary>
    [Header("Elemente")] public List<DecorationItem> DecorationParts;

    private List<GameObject> _activeParts = new List<GameObject>(); //Aktive Objekte die bereits instanziiert wurden.
    private float _segmentBoundsZMax = 0; //Maximaler Z-Wert für den Spawnbereich
    private float _segmentBoundsZMin = 0; //Minimaler Z-Wert für den Spawnbereich
    private float _segmentBoundsXMax = 0; //Maximaler X-Wert für den Spawnbereich
    private float _segmentBoundsXMin = 0; //Minimaler X-Wert für den Spawnbereich

    /// <summary>
    /// Gibt eine Rotation in einem definierten Bereich zurück.
    /// </summary>
    /// <returns></returns>
    private Quaternion GetRotationInRange()
    {
        return Quaternion.Euler(
            Random.Range(-70.0f, -108.0f),
            Random.Range(-40.0f, 40.0f),
            Random.Range(160.0f, 250.0f)
        );
    }

    /// <summary>
    /// Gibt eine Zufällige Position innerhalb eines Bereichs zurück.
    /// </summary>
    /// <param name="minZ">Minimaler Z-Wert des Bereichs</param>
    /// <param name="maxZ">Maximaler Z-Wert des Bereichs</param>
    /// <param name="decorationIndex">Indedx des Objekts in der Objektliste, welches gespawnt wird.</param>
    /// <returns>Position als Vektor</returns>
    private Vector3 GetPositionInRange(float minZ, float maxZ, int decorationIndex)
    {
        var tmpPosition = new Vector3(
            GetXValueInRange(),
            Random.Range(DecorationParts[decorationIndex].Height.Min, DecorationParts[decorationIndex].Height.Max), 
            Random.Range(minZ, maxZ)
        );

        //Prüft ob bereits instanziierte Objekte die Position blockieren
        _activeParts.ForEach(part =>
        {
            while (Math.Abs(Vector3.Distance(part.transform.position, tmpPosition)) <= 5)
            {
                tmpPosition = GetPositionInRange(minZ, maxZ, decorationIndex);
            }
        });

        return tmpPosition;
    }

    /// <summary>
    /// Gibt einen X-Wert innerhalb des global definierten Bereichs zurück.
    /// </summary>
    /// <returns>X-Wert als float Datentyp</returns>
    private float GetXValueInRange()
    {
        float tmpXValue = 0;

        while (tmpXValue <= GlobalDataHandler.TrackWidth && tmpXValue >= -GlobalDataHandler.TrackWidth)
        {
            tmpXValue = Random.Range(_segmentBoundsXMin, _segmentBoundsXMax);
        }

        return tmpXValue;
    }

    /// <summary>
    /// Funktion, wird von Unity ausgeführt, wenn das Skript aktiviert wird. Diese Funktion wird vor der 
    /// Update Methode ausgeführt.
    /// </summary>
    private void Start()
    {
        CalculateSegmentBounds();
        SpawnDecorationParts();
    }

    /// <summary>
    /// Funktion, wird von Unity mehrmals pro Frame aufgerufen.
    /// </summary>
    private void FixedUpdate()
    {
        CalculateSegmentBounds();
        CheckForDecorationsOutOfBoundsAndRemove();
        SpawnDecorationParts();
    }

    /// <summary>
    /// Insstanziiert Objekte und platziert diese in der Szene. Zusätzlich werden die Objekte in einer
    /// Liste gespeichert.
    /// </summary>
    private void SpawnDecorationParts()
    {
        if (_activeParts.Count >= MaxPartsToSpawn) return;

        float zTmpMin;

        //Bestimmung des minimalen Z-Werts. 
        if (_activeParts.Count == 0)
        {
            zTmpMin = _segmentBoundsZMin;
        }
        else
        {
            zTmpMin = _activeParts[_activeParts.Count - 1].transform.position.z + GlobalDataHandler.TrackWidth;
        }
        
        //Maximaler Z-Wert in dem ein Objekt instanziiert werden darf.
        float zTmpMax = zTmpMin + ZDistanceToNextObj;

        if (zTmpMax >= _segmentBoundsZMax) return;

        var decorationIndex = Random.Range(0, DecorationParts.Count);
        var go = Instantiate(DecorationParts[decorationIndex].Go, GetPositionInRange(zTmpMin, zTmpMax , decorationIndex), GetRotationInRange());
        go.transform.SetParent(ParentGameObject.transform);
        _activeParts.Add(go);
    }

    /// <summary>
    /// Berechnet die Grenzen in denen die Objekte platziert werden dürfen.
    /// </summary>
    private void CalculateSegmentBounds()
    {
        var playerPos = GlobalDataHandler.GetPlayerPosition();
        _segmentBoundsXMin = BothSides ? -GlobalDataHandler.TrackWidth - DefineSegmentWidth : GlobalDataHandler.TrackWidth;
        _segmentBoundsXMax = GlobalDataHandler.TrackWidth + DefineSegmentWidth;
        _segmentBoundsZMin = playerPos - OffsetToPlayer;
        _segmentBoundsZMax = playerPos + OffsetToPlayer;
    }

    /// <summary>
    /// Prüft auf Objekte die außerhalb des definierten Bereichs liegen und löscht diese.
    /// </summary>
    private void CheckForDecorationsOutOfBoundsAndRemove()
    {
        _activeParts.RemoveAll(part => part.transform.position.z <= _segmentBoundsZMin);
    }
    
}