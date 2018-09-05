using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Klasse, ist für die Kollisionen der Controller zuständig. 
/// </summary>
public class ControllerCollider : MonoBehaviour {

    private ObjectList _objList;              //Objektliste
    public InventoryHandler InventoryHandler; // Referenz zum InventoryHandler

    /// <summary>
    /// Funktion, wird von Unity aufgerufen, wenn das dazugehörige GameObject aktiviert wird.
    /// Fügt die entsprechenden Methoden dem Event-Handler-System hinzu.
    /// </summary>
    private void OnEnable()
    {
        EventManager.LevelIsLoadedEventMethods += SearchForObjctCtrl;
    }

    /// <summary>
    /// Funktion, wird von Unity aufgerufen, wenn das dazugehörige GameObject deaktiviert wird.
    /// Entfernt die entsprechenden Methoden aus dem Event-Handler-System.
    /// </summary>
    private void OnDisable()
    {
        EventManager.LevelIsLoadedEventMethods -= SearchForObjctCtrl;
    }

    /// <summary>
    /// Funktion, sucht in der Szene nach der Onjektliste und speichert eine Referenz auf der Instanzvariablen
    /// des Objektes. 
    /// </summary>
    private void SearchForObjctCtrl()
    {
		if (SceneManager.GetActiveScene().name == "Tutorial" || SceneManager.GetActiveScene().name == "HomeScene") return;
        _objList = GameObject.Find("ObjectList").GetComponent<ObjectList>();
    }

    /// <summary>
    /// Callback Funktion für die Erkennung einer Kollision des Objekts an dem das Script angefügt ist.
    /// </summary>
    /// <param name="other">Objekt mit dem "this" kollidiert ist.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (!GlobalDataHandler.GetCollisionState()) return;
        
        // Kollisionsbehandlung beim Einsammeln von Punkten
        if (other.gameObject.CompareTag("Points"))
        {
            EventManager.ScoreObjectCollected();
            
            //Fügt dem Inventar hinzu bis die Punkte oben am Score angekommen sind 
            other.transform.parent = InventoryHandler.transform;
            if (SceneManager.GetActiveScene().name != "Tutorial")
                _objList.RemoveFromList(other.gameObject);
        }
    }
    
}
