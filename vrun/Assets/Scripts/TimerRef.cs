using UnityEngine;

/// <summary>
/// Klasse zur Repräsentationen von Timerinstanzen. Ein Timer kann entweder runter- oder hochzählen.
/// </summary>
public class TimerRef
{
    private float _spawnTime; //Grenze, die bestimmt wird
    private float _time; //Aktuelle Zeit des Timers
    private int _minSpawnTime; // Minimaler möglicher Timerwert  
    private int _maxSpawnTime; // Maximaler möglicher Timerwert

    /// <summary>
    /// Konstruktor, erstellt einen Timer. Der Timer wird auf einen zufälligen Wert zwischen
    /// Min- und MaxTime gesetzt. 
    /// </summary>
    /// <param name="minTime">kleinst möglicher Wert</param>
    /// <param name="maxTime">größt möglicher Wert</param>
    public TimerRef(int minTime, int maxTime)
    {
        _minSpawnTime = minTime;
        _maxSpawnTime = maxTime;
        _time = 0;
        SetRandomTime();
    }

    /// <summary>
    /// Konstruktor, erstellt einen Timer und setzt die Timerzeit.
    /// </summary>
    /// <param name="time">Zeit auf die der Timer gesetzt wird</param>
    public TimerRef(float time)
    {
        _time = time;
    }

    /// <summary>
    /// Setzt die Spawntime auf einen zufälligen Wert zwischen min- und maxSpawntime.
    /// </summary>
    private void SetRandomTime()
    {
        _spawnTime = Random.Range(_minSpawnTime, _maxSpawnTime);
    }

    /// <summary>
    /// Setzt die Timerzeit zurück und erstellt einen neuen zufälligen Timerwert.
    /// </summary>
    public void ResetTime()
    {
        _time = 0;
        SetRandomTime();
    }

    /// <summary>
    /// Addiert die Timerzeit. Lässt den Timer laufen. Für einen Timer der hochzählt.
    /// </summary>
    public void AddTime()
    {
        _time += Time.deltaTime;
    }

    /// <summary>
    /// Subtrahiert die Timerzeit für einen Timer der runterzählt.
    /// </summary>
    public void SubTime()
    {
        _time -= Time.deltaTime;
    }

    /// <summary>
    /// Setzt die minimalen möglichen Timerwert.
    /// </summary>
    /// <param name="time">Zeit die gesetzt wird</param>
    public void SetMinSpawnTime(int time)
    {
        _minSpawnTime = time;
    }

    /// <summary>
    /// Setzt den maximalen möglichen Timerwert.
    /// </summary>
    /// <param name="time">Zeit die gesetzt wird</param>
    public void SetMaxSpawnTime(int time)
    {
        _maxSpawnTime = time;
    }

    /// <summary>
    /// Gibt die Zeit des Timers wieder.
    /// </summary>
    /// <returns>Zeit des Timers</returns>
    public float GetTime()
    {
        return _time;
    }

    /// <summary>
    /// Setzt die Grenzwerte des Timers.
    /// </summary>
    /// <param name="minTime">minimale Grenze</param>
    /// <param name="maxTime">maximale Grenze</param>
    public void SetSpawnTimes(int minTime, int maxTime)
    {
        _minSpawnTime = minTime;
        _maxSpawnTime = maxTime;
    }

    /// <summary>
    /// Gibt zurück, ob der Timer, der hochzählt, abgelaufen ist.
    /// </summary>
    /// <returns>true, wenn der Timer abgelaufen ist</returns>
    public bool ElapsedCountUpTimer()
    {
        return _time >= _spawnTime;
    }

    /// <summary>
    /// Gibt zurück, ob der Timer, der runterzählt, abgelaufen ist.
    /// </summary>
    /// <returns>true, wenn der Timer abgelaufen ist</returns>
    public bool ElapsedCountdownTimer()
    {
        return _time <= 0;
    }

    /// <summary>
    /// Fasst die Funktionalität des Timers in einer Methode zusammen.
    /// Bei jedem Aufruf wird der Timer, wenn das Spiel läuft, hochgezählt und bei Erreichen der
    /// Grenze zurückgesetzt.
    /// </summary>
    /// <param name="activeGame">Spielmodus</param>
    /// <returns>true, wenn der Timer abgelaufen ist</returns>
    public bool ProcessCountUpTimer(bool activeGame)
    {
        if (activeGame)
        {
            AddTime();

            if (ElapsedCountUpTimer())
            {
                ResetTime();
                return true;
            }
        }

        return false;
    }
}