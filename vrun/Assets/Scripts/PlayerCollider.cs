using NewTypes;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Klasse, ist für die Kollisionsbehandlung des Spielers mit den auf der Strecke befindlichen Objekten zuständig.
/// </summary>
public class PlayerCollider : MonoBehaviour
{
    private ObjectList _objList; //Objektliste 
    public InventoryHandler InventoryHandler; //Referenz zu dem InventoryHandler
    public CtrlPointHandler CtrlPointHandler; //Referenz zu dem CtrlPointHandler

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
    /// Funktion, sucht die Objektliste in der Szene und speichert diese auf der dazugehörigen Instanzvariablen.
    /// </summary>
    private void SearchForObjctCtrl()
    {
        if (SceneManager.GetActiveScene().name == "Tutorial" || SceneManager.GetActiveScene().name == "HomeScene") return;
        _objList = GameObject.Find("ObjectList").GetComponent<ObjectList>();
    }

    /// <summary>
    /// Funktion, wird von Unity jeden Frame aufgerufen.
    /// </summary>
    private void Update()
    {
        if (_objList == null)
        {
            SearchForObjctCtrl();
        }
    }

    /// <summary>
    /// Callback Funktion für die Erkennung einer Kollision des Objekts an dem das Script angefügt ist.
    /// </summary>
    /// <param name="other">Objekt mit dem "this" kollidiert ist.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Wenn der Kollisionsmodus deaktiviert ist, beende die Methode
        if (!GlobalDataHandler.GetCollisionState()) return;

        switch (other.gameObject.tag)
        {
            // Einsammeln eines Collectibles 
            case "Collectible":
                InventoryHandler.AddItem(other.gameObject);
                if (SceneManager.GetActiveScene().name != "Tutorial")
                    _objList.RemoveFromList(other.gameObject);
                break;
            // Kollisonsbehandlung mit einem Hindernis der Level
            case "Obstacle":
                EventManager.HitObstacleObjectCollected();
                EventManager.GameEnd();
                if (SceneManager.GetActiveScene().name != "Tutorial")
                    _objList.RemoveItem(other.gameObject);
                break;
            // Einsammeln eines Coins
            case "Points":
                EventManager.ScoreObjectCollected();
                //Fügt dem Inventar hinzu bis die Punkte oben am Score angekommen sind 
                other.transform.parent = InventoryHandler.transform;
                if (SceneManager.GetActiveScene().name != "Tutorial")
                    _objList.RemoveFromList(other.gameObject);
                break;
            // Kollisonsbehandlung mit einem Hindernis in dem Tutorial
            case "TutorialObstacle":
                GlobalDataHandler.SetRunMode(RunMode.Idle);
                EventManager.HitTutorialObstacle(CtrlPointHandler.GetActualCtrlPointPosZ());
                break;
            // Durchlaufen eines Portals
            case "Portal":
                if (SceneManager.GetActiveScene().name == "Tutorial")
                {
                    EventManager.DeleteVrStuff();
                }

                EventManager.SwitchScene(other.name);
                break;
            // Durchlaufen eines Kontrollpunktes
            case "Checkpoint":
                if (GlobalDataHandler.GetRunMode() != RunMode.Idle && GlobalDataHandler.GetCollisionState())
                {
                    EventManager.EnterCtrlPoint();
                }

                break;
        }
    }
}