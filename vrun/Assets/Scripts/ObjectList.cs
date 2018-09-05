using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Klasse, hält eine Liste mit allen Objekten, die auf der Strecke gespawnt werden. Stellt Funktionen zum Verwalten
/// dieser Liste zur Verfügung. 
/// </summary>
public class ObjectList : MonoBehaviour
{
    public int MaxObjectCount;            // maximale Anzahl der Objekte 
    public float Offset = 5f;             // genereller Abstand zwischen den Objekten
    public float CollectiblesOffset = 5f; // Abstand zwischen den Collectibles
    public float MaxSpawnDistance = 300;  // Distanz in der die Objekte vor dem Spieler gespwant werden

    private List<GameObject> _objectList; //Liste in der die Objekte gespeichert werden

    /// <summary>
    /// Funktion, wird von Unity ausgeführt, wenn das Skript aktiviert wird. Diese Funktion wird vor der 
    /// Update Methode ausgeführt.
    /// </summary>
    private void Start()
    {
        _objectList = new List<GameObject>();
    }

    /// <summary>
    /// Funktion, überprüft, ob das eingegebene Objekt an der angegebenen Stelle gespawnt werden kann.
    /// </summary>
    /// <param name="t">Transform des Objektes, welches gespawnt werden soll</param>
    /// <param name="isCollectible">Gibt an, ob das Objekt ein Collectible ist</param>
    /// <returns>True, wenn die Position nicht geeignet ist zum Spawnen des Objektes</returns>
    public bool IsPositionUsed(Transform t, bool isCollectible)
    {
        foreach (var child in _objectList)
        {
            if ((System.Math.Abs(child.transform.position.z - t.position.z) < (isCollectible && 
                (child.CompareTag("Obstacle") || child.CompareTag("Points")) ? 0.5f : Offset)) &&
                (child.transform.position.x > t.position.x - Mathf.Epsilon &&
                 child.transform.position.x < t.position.x + Mathf.Epsilon))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Funktion, gibt zurück, ob die Objektliste voll ist.
    /// </summary>
    /// <returns>True, wenn die Objektliste voll ist. Ansonsten false</returns>
    public bool IsListFull()
    {
        return _objectList.Count >= MaxObjectCount;
    }

    /// <summary>
    /// Funktion, versucht der Objektliste das angegebene Item hinzuzufügen. Hierbei wird überprüft, ob die Liste 
    /// voll ist, die Position des Objektes schon vergeben ist und die Spawnentfernung bzgl. des Spielers ausreicht.
    /// </summary>
    /// <param name="go">Das Objekt, welches der Liste hinzugefügt werden soll</param>
    /// <param name="isCollectible">Gibt an, ob das Objekt ein Collectible ist</param>
    /// <returns>True, wenn das Objekt der Liste hinzugefügt wurde. Ansonsten false</returns>
    public bool AddItem(GameObject go, bool isCollectible)
    {
        if (!IsListFull() && !IsPositionUsed(go.transform, isCollectible) && ((GlobalDataHandler.GetPlayerPosition() + MaxSpawnDistance) - go.transform.position.z > 0))
        {
            _objectList.Add(go);
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Funktion, entfernt das erste Objekt aus der Objektliste.
    /// </summary>
    public void RemoveLastItem()
    {
        Destroy(_objectList[0]);
        _objectList.RemoveAt(0);
    }

    /// <summary>
    /// Funktion, entfernt das angegebene Objekt anhand der Objektinstanz aus der Objektliste.
    /// </summary>
    /// <param name="go">Objekt, welches aus der Liste entfernt werden soll</param>
    public void RemoveFromList(GameObject go)
    {
        _objectList.Remove(go);
    }

    /// <summary>
    /// Funktion, entfernt das angegebene Objekt anhand der Objektinstanz aus der Objektliste und zerstört zusätzlich
    /// die Objektinstanz.
    /// </summary>
    /// <param name="go">Objekt, welches gelöscht werden soll.</param>
    public void RemoveItem(GameObject go)
    {
        _objectList.Remove(go);
        Destroy(go);
    }

    /// <summary>
    /// Funktion, gibt das erste Element der Liste wieder, sofern eins vorhanden ist.
    /// </summary>
    /// <returns>Das erste Element der Liste, sonst null</returns>
    public GameObject GetFirstObject()
    {
        return _objectList.Count == 0 ? null : _objectList[0];
    }
}