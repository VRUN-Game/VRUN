using UnityEngine;

/// <summary>
/// Klasse, ist für die Verwaltung der Punkte des Spielers innerhalb eines Spiels zuständig.
/// Stellt Funktionen zum Wiedergeben, Erhöhen und Löschen des Punktestandes. 
/// </summary>
public class ScoreHandler : MonoBehaviour
{
    private static int _score; //Score Wert 

    /// <summary>
    /// Funktion, wird von Unity aufgerufen, wenn das dazugehörige GameObject aktiviert wird.
    /// Fügt die entsprechenden Methoden dem Event-Handler-System hinzu.
    /// </summary>
    private void OnEnable()
    {
        EventManager.GameStartEventMethods += ResetScore;
        EventManager.LevelScoreObjCollectedEventMethods += CoinCollected;
        EventManager.ActivateScoreEventMethods += ResetScore;
    }

    /// <summary>
    /// Funktion, wird von Unity aufgerufen, wenn das dazugehörige GameObject deaktiviert wird.
    /// Entfernt die entsprechenden Methoden aus dem Event-Handler-System.
    /// </summary>
    private void OnDisable()
    {
        EventManager.GameStartEventMethods -= ResetScore;
        EventManager.LevelScoreObjCollectedEventMethods -= CoinCollected;
        EventManager.ActivateScoreEventMethods -= ResetScore;
    }

    /// <summary>
    /// Funktion, gibt den Punktestand des Spielers zurück. 
    /// </summary>
    /// <returns>Punktestand des Spielers</returns>
    public static int GetPlayerScore()
    {
        return _score;
    }

    /// <summary>
    /// Funktion, erhöht den Punktestand des Spielers um den angegebenen Wert.
    /// </summary>
    /// <param name="val">Wert, um den der Punktestand erhöht werden soll</param>
    public static void IncreasePlayerScore(int val)
    {
        _score += val;
    }

    /// <summary>
    /// Funktion, wird ausgeführt, wenn ein Punkt eingesammelt wurde und erhöht den Punktestand um den
    /// festgelegten Wert. Ist der Punkteverdoppler aktiv, wird der doppelte Wert hinzugefügt. 
    /// </summary>
    private static void CoinCollected()
    {
        IncreasePlayerScore(GlobalDataHandler.IsMultiplierActive() ? 2 * GlobalDataHandler.CoinAddition: GlobalDataHandler.CoinAddition);
    }

    /// <summary>
    /// Funktion, setzt den Punktestand zurück auf 0. 
    /// </summary>
    private void ResetScore()
    {
        _score = 0;
    }
}