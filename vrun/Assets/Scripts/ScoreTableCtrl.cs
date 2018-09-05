using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Klasse, ist für die Verwaltung und Speicherung der Punketabelle zuständig. Stellt Funktionen zum Laden 
/// der Tabelle, Hinzufügen und Entfernen von Listeneinträgen zur Verfügung. 
/// </summary>
public class ScoreTableCtrl : MonoBehaviour
{
    private static List<int> _highScoreTable = new List<int>(); //Listeneinträge der Punktetabelle
    private const int _NoOfmaxEntries = 10;    //festgelegte maximale Anzahl an Einträgen

    /// <summary>
    /// Funktion, wird von Unity aufgerufen, wenn das dazugehörige GameObject aktiviert wird.
    /// Fügt die entsprechenden Methoden dem Event-Handler-System hinzu.
    /// </summary>
    private void OnEnable()
    {
        EventManager.GameExitEventMethods += SaveScoreTable;
        EventManager.GameEndEventMethods += PushNewScoreToTable;
    }

    /// <summary>
    /// Funktion, wird von Unity aufgerufen, wenn das dazugehörige GameObject deaktiviert wird.
    /// Entfernt die entsprechenden Methoden aus dem Event-Handler-System.
    /// </summary>
    private void OnDisable()
    {
        EventManager.GameExitEventMethods -= SaveScoreTable;
        EventManager.GameEndEventMethods -= PushNewScoreToTable;
    }

    /// <summary>
    /// Funktion, wird von Unity beim Start einmalig ausgeführt, wenn alle Objekte initialisiert worden sind. 
    /// </summary>
    void Awake()
    {
        LoadScoreTable();
    }

    /// <summary>
    /// Wird am Ende des Spiels aufgerufen, um den erspielten Score in den PlayerPrefs abzuspeichern.
    /// </summary>
    /// <param name="newScore">Abzuspeichernder Spielstand</param>
    public static void PushNewScoreToTable()
    {
        var newScore = ScoreHandler.GetPlayerScore();

        _highScoreTable.Sort();

        if (_highScoreTable[0] < newScore)
        {
            _highScoreTable.RemoveAt(0);
        }

        _highScoreTable.Add(newScore);
        _highScoreTable.Sort();
        _highScoreTable.Reverse();
    }

    /// <summary>
    /// Gibt die Punktetabelle zurück. 
    /// </summary>
    /// <returns>Die aktuelle Punktetabelle</returns>
    public static List<int> GetScoreTable()
    {
        return _highScoreTable;
    }

    /// <summary>
    /// Lädt die Speicherstände aus dem Speicher für die Punktetabelle.
    /// </summary>
    private void LoadScoreTable()
    {
        for (int i = 0; i < _NoOfmaxEntries; i++)
        {
            _highScoreTable.Add(PlayerPrefs.HasKey("highScore" + i) ? PlayerPrefs.GetInt("highScore" + i) : 0);
        }
    }

    /// <summary>
    /// Speichert die Punktetabelle wieder zusammen mit dem neuen Highscore ab. 
    /// </summary>
    /// <param name="scoreTable">Aktuelle Punktetabelle</param>
    private void SaveScoreTable()
    {
        for (int i = 0; i < _NoOfmaxEntries; i++)
        {
            PlayerPrefs.SetInt("highScore" + i, _highScoreTable[i]);
        }
    }
    
    /// <summary>
    /// Funktion, speichert bei Aufruf die Punktetabelle. 
    /// </summary>
    void OnApplicationQuit()
    {
        SaveScoreTable();
    }
}