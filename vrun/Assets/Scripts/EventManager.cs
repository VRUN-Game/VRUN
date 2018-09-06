using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Klasse, die verschiedene Events zum strukturierten Ablauf des Spiels bereitstellt.
/// </summary>
public class EventManager : MonoBehaviour
{
    /// <summary>
    /// Event, welches beim Start einer Spielsession ausgeführt wird.
    /// </summary>
    public delegate void GameStartEvent();
    public static event GameStartEvent GameStartEventMethods;
    public static void GameStart()
    {
        if (GlobalDataHandler.GetGameModus()) return; //Fürs debugging, beeinträchtigt aber nicht das normale Spiel.
        if (GameStartEventMethods != null) GameStartEventMethods();
    }

    /// <summary>
    /// Event, welches nach dem kollidieren des Spielers mit einem Hindernis ausgeführt wird. 
    /// </summary>
    public delegate void GameEndEvent();
    public static event GameEndEvent GameEndEventMethods;
    public static void GameEnd()
    {
        if (GameEndEventMethods != null) GameEndEventMethods();
    }
    
    /// <summary>
    /// Event, welches beim Wiedereintritt aus dem Spiel ins Hauptmenü ausgeführt wird.
    /// Es setzt alle variablen des Spiels zurück.
    /// </summary>
    public delegate void GameResetEvent();
    public static event GameResetEvent GameResetEventMethods;
    public static void GameReset()
    {
        if (GameResetEventMethods != null) GameResetEventMethods();
    }
    
    /// <summary>
    /// Event, welches den Übergang in ein nächstes Level.
    /// </summary>
    /// <param name="sceneName">Name der Scene die als nächstes geladen werden soll.</param>
    public delegate void LoadNewSceneEvent(String sceneName);
    public static event LoadNewSceneEvent LoadNewSceneEventMethods;
    public static void SwitchScene(string sceneName)
    {
        if (LoadNewSceneEventMethods != null) LoadNewSceneEventMethods(sceneName);
    }

    /// <summary>
    /// Event, welches nach dem erfolgreichen Laden der Szene aufgerufen wird.
    /// </summary>
    public delegate void LevelIsLoadedEvent();
    public static event LevelIsLoadedEvent LevelIsLoadedEventMethods;
    public static void LevelIsLoaded()
    {
        PlayLevelSound(GlobalDataHandler.GetActualSceneName());
        if (LevelIsLoadedEventMethods != null) LevelIsLoadedEventMethods();
    }
   
    /// <summary>
    /// Event, welches nach dem Laden eines Levels aufgerufen wird.
    /// </summary>
    public delegate void StartRunningEvent();
    public static event StartRunningEvent StartRunningEventMethods;
    public static void StartRunning()
    {
        if (StartRunningEventMethods != null) StartRunningEventMethods();
    }

    /// <summary>
    /// Event, welches beim eintreten in einen Kontrollpunkt aufgerufen wird.
    /// </summary>
    public delegate void EnterCtrlPointEvent();
    public static event EnterCtrlPointEvent EnterCtrlPointEventMethods;
    public static void EnterCtrlPoint()
    {
        if (EnterCtrlPointEventMethods != null) EnterCtrlPointEventMethods();
    }

    /// <summary>
    /// Event, welches beim austreten aus einem Kontrollpunkt aufgerufen wird.
    /// </summary>
    public delegate void LeaveCtrlPointEvent();
    public static event LeaveCtrlPointEvent LeaveCtrlPointEventMethods;
    public static void LeaveCtrlPoint()
    {
        if (LeaveCtrlPointEventMethods != null) LeaveCtrlPointEventMethods();
    }
    
    /// <summary>
    /// Event, welches beim treffen eines Hindernisses im Tutorial ausgeführt wird.
    /// </summary>
    /// <param name="posZ">Z-Position</param>
    public delegate void TutorialObstacleHitEvent(float posZ);
    public static event TutorialObstacleHitEvent TutorialObstacleHitEventMethods;
    public static void HitTutorialObstacle(float posZ)
    {
        if (TutorialObstacleHitEventMethods != null) TutorialObstacleHitEventMethods(posZ);
    }

	/// <summary>
	/// Event, welches das Inventar des Spielers zurücksetzt.
	/// </summary>
	public delegate void ResetInventoryEvent();
	public static event ResetInventoryEvent ResetInventoryEventMethods;
	public static void ResetInventory()
	{
	    if (ResetInventoryEventMethods != null) ResetInventoryEventMethods();
	}

    /// <summary>
    /// Event, welches beim einsammeln eines Punktes im Level ausgeführt wird.
    /// </summary>
    public delegate void LevelScoreObjCollectedEvent();
    public static event LevelScoreObjCollectedEvent LevelScoreObjCollectedEventMethods;
    public static void ScoreObjectCollected()
    {
        if (LevelScoreObjCollectedEventMethods != null) LevelScoreObjCollectedEventMethods();
    }
    
    /// <summary>
    /// Event, welches beim kollidieren mit einem Hinderniss im Level ausgeführt wird.
    /// </summary>
    public delegate void LevelObstacleObjHitEvent();
    public static event LevelObstacleObjHitEvent LevelObstacleObjHitEventMethods;
    public static void HitObstacleObjectCollected()
    {
        if (LevelObstacleObjHitEventMethods != null) LevelObstacleObjHitEventMethods();
    }
    
    /// <summary>
    /// Event, welches beim erreichen eines Meilensteins aufgerufen wird.
    /// </summary>
    public delegate void LevelIncreaseSpeedAndMilestoneEvent();
    public static event LevelIncreaseSpeedAndMilestoneEvent LevelIncreaseSpeedAndMilestoneEventMethods;
    public static void IncreaseSpeedAndMilestone()
    {
        if (LevelIncreaseSpeedAndMilestoneEventMethods != null) LevelIncreaseSpeedAndMilestoneEventMethods();
    }
    
    /// <summary>
    /// Event, welches beim aktivieren eines Items im Level ausgeführt wird.
    /// </summary>
    public delegate void LevelActivateItemEvent();
    public static event LevelActivateItemEvent LevelActivateItemEventMethods;
    public static void ActivatePlayersInventoryItem()
    {
        if (LevelActivateItemEventMethods != null) LevelActivateItemEventMethods();
    }
    
    /// <summary>
    /// Event, welches beim herunterzählen des Timers nach jeder Sekunde ausgeführt wird.
    /// </summary>
    public delegate void LevelTimerTickSoundEvent();
    public static event LevelTimerTickSoundEvent LevelTimerTickSoundEventMethods;
    public static void PlayTickSound()
    {
        if (LevelTimerTickSoundEventMethods != null) LevelTimerTickSoundEventMethods();
    }
    
    /// <summary>
    /// Event, welches im Level beim spawnen der Zacken einen Sound abspielt.
    /// </summary>
    public delegate void StoneSpawnEvent();
    public static event StoneSpawnEvent StoneSpawnEventMethods;
    public static void PlayStoneGrindSound()
    {
        if (StoneSpawnEventMethods != null) StoneSpawnEventMethods();
    }
    
    /// <summary>
    /// Event, welches in einem jeweiligen Level die Hintergrundmusik abspielt.
    /// </summary>
    /// <param name="name">Name des Sounds der abgespielt wird.</param>
	public delegate void PlayLevelSoundEvent(string name);
	public static event PlayLevelSoundEvent PlayLevelSoundEventMethods;
    public static void PlayLevelSound(string name)
    {
        if (PlayLevelSoundEventMethods != null) PlayLevelSoundEventMethods(name);
    }

    /// <summary>
    /// Event, welches die wiedergabe des Sounds in einem Level stoppt. 
    /// </summary>
	public delegate void StopLevelSoundEvent();
	public static event StopLevelSoundEvent StopLevelSoundEventMethods;
    public static void StopLevelSound()
    {
        if (StopLevelSoundEventMethods != null) StopLevelSoundEventMethods();
    }

	/// <summary>
	/// Event, welches die Timeranzeige in der Szene aktualisiert.
	/// </summary>
	public delegate void UpdateTimerDisplayEvent();
    public static event UpdateTimerDisplayEvent UpdateTimerDisplayEventMethods;
    public static void UpdateTimerDisplay()
    {
        if (UpdateTimerDisplayEventMethods != null) UpdateTimerDisplayEventMethods();
    }

    /// <summary>
    /// Event, welches alle VR Komponenten löscht.
    /// </summary>
    public delegate void DeleteVrStuffEvent();
    public static event DeleteVrStuffEvent DeleteVrStuffEventMethods;
    public static void DeleteVrStuff()
    {
        if (DeleteVrStuffEventMethods != null) DeleteVrStuffEventMethods();
    }
    
    /// <summary>
    /// Event, welches beim Level die Punktedarstellung übernimmt.
    /// </summary>
    public delegate void ActivateScoreEvent();
    public static event ActivateScoreEvent ActivateScoreEventMethods;
    public static void ActivateScore()
    {
        if (ActivateScoreEventMethods != null) ActivateScoreEventMethods();
    }

    /// <summary>
    /// Event, welches beim Level den Timer aktiviert.
    /// </summary>
    public delegate void ActivateTimerEvent();
    public static event ActivateTimerEvent ActivateTimerEventMethods;
    public static void ActivateTimer()
    {
        if (ActivateTimerEventMethods != null) ActivateTimerEventMethods();
    }

    /// <summary>
    /// Event, welches die Audioausgabe stumm schaltet.
    /// </summary>
    public delegate void MuteAudioEvent();
    public static event MuteAudioEvent MuteAudioEventMethods;
    public static void MuteAudio()
    {
        if (MuteAudioEventMethods != null) MuteAudioEventMethods();
    }
}