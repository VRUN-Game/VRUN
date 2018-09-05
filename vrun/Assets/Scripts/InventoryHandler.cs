using NewTypes;
using UnityEngine;

/// <summary>
/// Klasse, ist für die Verwaltung und Funktionalität des Inventars zuständig. Ein Inventar kein ein Item speichern
/// und dieses aktivieren. Bei einer Aktivierung läuft eine festgelegte Zeit ab. 
/// </summary>
public class InventoryHandler : MonoBehaviour
{
    private static GameObject _item; //Item welches im Inventar gespeichert wird
    private static TimerRef _timer;  //Timerinstanz des Inventars
    private float _previousTimerVal; //Zwischenvariable für die Zeitberechnung
    private ItemType _activeItemName; //Name des aktiven Items

    private const int TimerInitVal = 6; //Initialer Wert für die Zwischenvariable
    private const int TimerDuration = 5;  //Laufzeit des Timers

    /// <summary>
    /// Funktion, wird von Unity aufgerufen, wenn das dazugehörige GameObject aktiviert wird.
    /// Fügt die entsprechenden Methoden dem Event-Handler-System hinzu.
    /// </summary>
    private void OnEnable()
    {
        EventManager.GameResetEventMethods += GameResetInventory;
        EventManager.LevelActivateItemEventMethods += ActivateItem;
        EventManager.UpdateTimerDisplayEventMethods += UpdateTimers;
        EventManager.ResetInventoryEventMethods += GameResetInventory;
    }

    /// <summary>
    /// Funktion, wird von Unity aufgerufen, wenn das dazugehörige GameObject deaktiviert wird.
    /// Entfernt die entsprechenden Methoden aus dem Event-Handler-System.
    /// </summary>
    private void OnDisable()
    {
        EventManager.GameResetEventMethods -= GameResetInventory;
        EventManager.LevelActivateItemEventMethods -= ActivateItem;
        EventManager.UpdateTimerDisplayEventMethods -= UpdateTimers;
        EventManager.ResetInventoryEventMethods -= GameResetInventory;
    }

    /// <summary>
    /// Funktion, wird von Unity jeden Frame nach dem Auufruf aller Update Methoden aufgerufen.
    /// </summary>
    private void LateUpdate()
    {
        EventManager.UpdateTimerDisplay();
        if (_timer != null)
        {
            if (GlobalDataHandler.GetGameModus())
            {
                if (!_timer.ElapsedCountdownTimer() || _activeItemName.Equals(ItemType.None)) return;
                if (_timer.ElapsedCountdownTimer()) _timer = null;
            }
            else
            {
                _timer = null;
            }
        }
        else
        {
            _previousTimerVal = TimerInitVal;
        }

        // Wird ausgeführt wenn der Timer des aktuellen Items ausgelaufen ist
        // Dabei werden alle Änderungen die durch die Items ausgelöst wurden zurückgeführt
        if ((_activeItemName.Equals(ItemType.Faster) || _activeItemName.Equals(ItemType.Slower)) && GlobalDataHandler.GetRunMode() != RunMode.Idle)
        {
            GlobalDataHandler.SetRunMode(RunMode.Normal);
        }
        else if (GlobalDataHandler.IsMultiplierActive())
        {
            GlobalDataHandler.SetMultiplierActive(!GlobalDataHandler.IsMultiplierActive());
        }

        _activeItemName = ItemType.None;
    }

    /// <summary>
    /// Funktion, fügt das übergebende Item dem Inventar hinzu, sofern das Inventar leer ist.
    /// Ist das Inventar belegt, passiert nichts.
    /// </summary>
    /// <param name="item">Item, welches hinzugefügt werden soll.</param>
    public void AddItem(GameObject item)
    {
        if (_item != null) return;
        _item = item;
        _item.transform.parent = transform;
    }

    /// <summary>
    /// Funktion, löscht das aktuelle Item aus dem Inventar und löscht zusätzlich die Objektinstanz, sofern ein Item 
    /// in dem Inventar vorhanden ist.
    /// </summary>
    private void RemoveItem()
    {
        if (_item == null) return;
        Destroy(_item);
        _item = null;
    }

    /// <summary>
    /// Funktion, aktiviert ein Item, welches sich im Inventar befindet. Falls kein Item im Inventar ist, passiert nichts.
    /// </summary>
    private void ActivateItem()
    {
        // Unterbreche die Funktion im Fehlerfall, dass sie aufgerufen wurde obwohl kein Item 
        // eingesammelt wurde.
        if (_item == null || !_activeItemName.Equals(ItemType.None)) return;

        /* Timer setzen */
        // Wenn der Timer bereits aktiv ist prüfen ob gleiches Item auch in der _itemBag vorhanden ist.
        if (_timer != null)
        {
            if (!_timer.ElapsedCountdownTimer() && _item.GetComponent<ControllerItem>().ItemType.Equals(_activeItemName))
            {
                _timer = new TimerRef(_timer.GetTime() + TimerDuration);
            }
        }
        else
            // Erstelle einen neuen Timer wenn noch kein Item aktiv ist.
        {
            _timer = new TimerRef(5);
        }

        // Item aktivieren
        if (_item.GetComponent<ControllerItem>().ItemType.Equals(ItemType.ScoreMultiplier))
        {
            GlobalDataHandler.SetMultiplierActive(!GlobalDataHandler.IsMultiplierActive());
        }
        else
        {
            GlobalDataHandler.SetRunMode((_item.GetComponent<ControllerItem>().ItemType.Equals(ItemType.Faster))
                ? RunMode.Faster
                : RunMode.Slower);
        }

        _activeItemName = _item.GetComponent<ControllerItem>().ItemType;
        RemoveItem();
    }

    /// <summary>
    /// Aktualisiert den Timer für das aktuell aktivierte Item und das dazugehörige Label im UI.
    /// </summary>
    private void UpdateTimers()
    {
        if (_timer == null) return;
        PlayTimerTickSound();
        _timer.SubTime();
    }

    /// <summary>
    /// Spielt den TickSound des Timers ab.
    /// </summary>
    private void PlayTimerTickSound()
    {
        if (!(_previousTimerVal - _timer.GetTime() >= 1.0)) return;
        EventManager.PlayTickSound();
        _previousTimerVal = _timer.GetTime();
    }

    /// <summary>
    /// Funktion, leert das Inventar und setzt den Timer zurück. 
    /// </summary>
    private void GameResetInventory()
    {
        RemoveItem();
        _activeItemName = ItemType.None;
        _timer = null;
        _previousTimerVal = 0;
    }

    /// <summary>
    /// Funktion, gibt den Timer zurück.
    /// </summary>
    /// <returns>Timer der Inventarinstanz</returns>
    public static TimerRef GetTimer()
    {
        return _timer;
    }

    /// <summary>
    /// Gibt das im Inventar befindliche Item zurück.
    /// </summary>
    /// <returns>Item, welchs im Inventar ist</returns>
    public static GameObject GetInventoryItem()
    {
        return _item;
    }
}