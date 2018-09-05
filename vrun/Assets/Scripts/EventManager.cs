using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventManager : MonoBehaviour
{
    /**
     * Game events handling the correct state order during a running instance of the game.
     */

    //GameExitEvent
    public delegate void GameExitEvent();
    public static event GameExitEvent GameExitEventMethods;

    //GameStartEvent
    public delegate void GameStartEvent();
    public static event GameStartEvent GameStartEventMethods;
    public static void GameStart()
    {
        if (GlobalDataHandler.GetGameModus()) return;
        if (GameStartEventMethods != null) GameStartEventMethods();
    }

    //GameEndEvent
    public delegate void GameEndEvent();
    public static event GameEndEvent GameEndEventMethods;

    public static void GameEnd()
    {
        if (GameEndEventMethods != null) GameEndEventMethods();
    }
    //GameResetEvent
    public delegate void GameResetEvent();
    public static event GameResetEvent GameResetEventMethods;
    public static void GameReset()
    {
        if (GameResetEventMethods != null) GameResetEventMethods();
    }
    //Load new scene
    public delegate void LoadNewSceneEvent(String sceneName);
    public static event LoadNewSceneEvent LoadNewSceneEventMethods;
    public static void SwitchScene(string sceneName)
    {
        if (LoadNewSceneEventMethods != null) LoadNewSceneEventMethods(sceneName);
    }

    public delegate void LevelIsLoadedEvent();
    public static event LevelIsLoadedEvent LevelIsLoadedEventMethods;
    public static void LevelIsLoaded()
    {
        PlayLevelSound(SceneManager.GetActiveScene().name);
        if (LevelIsLoadedEventMethods != null) LevelIsLoadedEventMethods();
    }
   
    public delegate void StartRunningEvent();

    public static event StartRunningEvent StartRunningEventMethods;

    public static void StartRunning()
    {
        if (StartRunningEventMethods != null) StartRunningEventMethods();
    }


    /**
     * Tutorial events handling the correct state order during the tutorial.
     */
    // Entering the Controllpoint
    public delegate void EnterCtrlPointEvent();

    public static event EnterCtrlPointEvent EnterCtrlPointEventMethods;
    public static void EnterCtrlPoint()
    {
        if (EnterCtrlPointEventMethods != null) EnterCtrlPointEventMethods();
    }

    // Leaving the Controllpoint
    public delegate void LeaveCtrlPointEvent();

    public static event LeaveCtrlPointEvent LeaveCtrlPointEventMethods;
    public static void LeaveCtrlPoint()
    {
        if (LeaveCtrlPointEventMethods != null) LeaveCtrlPointEventMethods();
    }
    
    // Event for resetting the player if he hits an obstacle in the tutorial
    public delegate void TutorialObstacleHitEvent(float posZ);

    public static event TutorialObstacleHitEvent TutorialObstacleHitEventMethods;

    public static void HitTutorialObstacle(float posZ)
    {
        if (TutorialObstacleHitEventMethods != null) TutorialObstacleHitEventMethods(posZ);
    }

	// Event for resetting the player if he hits an obstacle in the tutorial
	public delegate void ResetInventoryEvent();

	public static event ResetInventoryEvent ResetInventoryEventMethods;

	public static void ResetInventory()
	{
	    if (ResetInventoryEventMethods != null) ResetInventoryEventMethods();
	}

    /**
     * Game events handling the gameplay states while player is actually playing the game.
     */
    //Scoreobject Collected
    public delegate void LevelScoreObjCollectedEvent();

    public static event LevelScoreObjCollectedEvent LevelScoreObjCollectedEventMethods;

    //Obstaclesobject Hitted
    public delegate void LevelObstacleObjHitEvent();

    public static event LevelObstacleObjHitEvent LevelObstacleObjHitEventMethods;

    //Increase the speed and the next milestone of the player
    public delegate void LevelIncreaseSpeedAndMilestoneEvent();

    public static event LevelIncreaseSpeedAndMilestoneEvent LevelIncreaseSpeedAndMilestoneEventMethods;

    //Activate collected item from player inventory
    public delegate void LevelActivateItemEvent();

    public static event LevelActivateItemEvent LevelActivateItemEventMethods;

    //Play Timer Click Sound
    public delegate void LevelTimerTickSoundEvent();

    public static event LevelTimerTickSoundEvent LevelTimerTickSoundEventMethods;

    //Play Stone grind sound
    public delegate void StoneSpawnEvent();

    public static event StoneSpawnEvent StoneSpawnEventMethods;

	public delegate void PlayLevelSoundEvent(string name);
	public static event PlayLevelSoundEvent PlayLevelSoundEventMethods;

	public delegate void StopLevelSoundEvent();
	public static event StopLevelSoundEvent StopLevelSoundEventMethods;

    public static void ScoreObjectCollected()
    {
        if (LevelScoreObjCollectedEventMethods != null) LevelScoreObjCollectedEventMethods();
    }

    public static void HitObstacleObjectCollected()
    {
        if (LevelObstacleObjHitEventMethods != null) LevelObstacleObjHitEventMethods();
    }

    public static void IncreaseSpeedAndMilestone()
    {
        if (LevelIncreaseSpeedAndMilestoneEventMethods != null) LevelIncreaseSpeedAndMilestoneEventMethods();
    }

    public static void ActivatePlayersInventoryItem()
    {
        if (LevelActivateItemEventMethods != null) LevelActivateItemEventMethods();
    }

    public static void PlayTickSound()
    {
        if (LevelTimerTickSoundEventMethods != null) LevelTimerTickSoundEventMethods();
    }

    public static void PlayStoneGrindSound()
    {
        if (StoneSpawnEventMethods != null) StoneSpawnEventMethods();
    }

	public static void PlayLevelSound(string name)
	{
		if (PlayLevelSoundEventMethods != null) PlayLevelSoundEventMethods(name);
	}

	public static void StopLevelSound()
	{
		if (StopLevelSoundEventMethods != null) StopLevelSoundEventMethods();
	}

	//Update Timer Display ingame
	public delegate void UpdateTimerDisplayEvent();

    public static event UpdateTimerDisplayEvent UpdateTimerDisplayEventMethods;

    public static void UpdateTimerDisplay()
    {
        if (UpdateTimerDisplayEventMethods != null) UpdateTimerDisplayEventMethods();
    }

    public delegate void DeleteVrStuffEvent();

    public static event DeleteVrStuffEvent DeleteVrStuffEventMethods;

    public static void DeleteVrStuff()
    {
        if (DeleteVrStuffEventMethods != null) DeleteVrStuffEventMethods();
    }
    
    //Play Stone grind sound
    public delegate void ActivateScoreEvent();

    public static event ActivateScoreEvent ActivateScoreEventMethods;

    public static void ActivateScore()
    {
        if (ActivateScoreEventMethods != null) ActivateScoreEventMethods();
    }

    //Play Stone grind sound
    public delegate void ActivateTimerEvent();

    public static event ActivateTimerEvent ActivateTimerEventMethods;

    public static void ActivateTimer()
    {
        if (ActivateTimerEventMethods != null) ActivateTimerEventMethods();
    }

    // Kommentar
    public delegate void MuteAudioEvent();

    public static event MuteAudioEvent MuteAudioEventMethods;

    public static void MuteAudio()
    {
        if (MuteAudioEventMethods != null) MuteAudioEventMethods();
    }
}