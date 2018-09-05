using System;
using System.Collections;
using NewTypes;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRSF.Utils;

namespace NewTypes
{
    public enum RunMode
    {
        Idle,
        Slower,
        Normal,
        Faster
    };

    public enum ItemType
    {
        Faster,
        Slower,
        ScoreMultiplier,
        None
    }
}

/// <summary>
/// Klasse,  legt alle Spielinformationen und Spielparameter fest, die das Spielgeschehen definieren und beeinflussen.
/// </summary>
public class GlobalDataHandler : MonoBehaviour
{
    private static GlobalDataHandler _instance; // Instanz des GlobalDataHandlers
    public const int Milestone = 800; // Ab dem Score wird der Spieler schneller
    public const float SpeedAdditionPerMileStone = 10f; //Geschwindigkeitszusatz nach Erreichen eines Meilensteins
    public static float StartSpeed = 20; // Anfangsgeschwindigkeit des Spielers
    public static int TrackWidth = 6; // Gesamtbreite der Strecke
    public static int CoinAddition = 10; //Erhöhung beim Einsammeln eines Coins


    private static int _nextMilestone; // z-Position des nächsten Meilensteins
    private static bool _gameModus; // Game-Modus 
    private static bool _isMultiplierActive; // Punkte-Verdoppler
    private static float _playerSpeed; // aktuelle Spielergeschwindigkeit
    private static float _baseSpeed; // Grundgeschwindigkeit
    private static bool _collision; // Kollisionsstatus des Spielers

    private static RunMode _runMode; // Renn-Modus des Spielers 

    public static int[] LanePositions = { -2, 0, 2 }; // x-Positionen der Streckenspuren
    
    /// <summary>
    /// Funktion, wird von Unity aufgerufen, wenn das dazugehörige GameObject aktiviert wird.
    /// Fügt die entsprechenden Methoden dem Event-Handler-System hinzu.
    /// </summary>
    private void OnEnable()
    {
        EventManager.GameResetEventMethods += ResetGlobalData;
        EventManager.LevelIncreaseSpeedAndMilestoneEventMethods += IncreasePlayerSpeedAndMilestone;
        EventManager.GameStartEventMethods += StartGameAndChangeMode;
        EventManager.GameEndEventMethods += StopGameAndChangeMode;
        EventManager.TutorialObstacleHitEventMethods += PlayerHitTutorialObstacle;
        EventManager.StartRunningEventMethods += StartGameAndChangeMode;
        EventManager.StartRunningEventMethods += StartPlayer;
        EventManager.DeleteVrStuffEventMethods += StartDeleteVrStuff;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Funktion, wird von Unity aufgerufen, wenn das dazugehörige GameObject deaktiviert wird.
    /// Entfernt die entsprechenden Methoden aus dem Event-Handler-System.
    /// </summary>
    private void OnDisable()
    {
        EventManager.GameResetEventMethods -= ResetGlobalData;
        EventManager.LevelIncreaseSpeedAndMilestoneEventMethods -= IncreasePlayerSpeedAndMilestone;
        EventManager.GameStartEventMethods -= StartGameAndChangeMode;
        EventManager.GameEndEventMethods -= StopGameAndChangeMode;
        EventManager.TutorialObstacleHitEventMethods -= PlayerHitTutorialObstacle;
        EventManager.StartRunningEventMethods -= StartGameAndChangeMode;
        EventManager.StartRunningEventMethods -= StartPlayer;
        EventManager.DeleteVrStuffEventMethods -= StartDeleteVrStuff;
        SceneManager.sceneLoaded -= OnSceneLoaded;

    }

    /// <summary>
    /// Funktion, wird von Unity beim Start einmalig ausgeführt, wenn alle Objekte initialisiert worden sind. 
    /// </summary>    
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (_instance != this) {
            Destroy(gameObject);
            _instance.gameObject.SetActive(false);
            _instance.gameObject.SetActive(true);
        }
        ResetGlobalData();
    }

    /// <summary>
    /// Funktion, sendet das Event, dass der Spieler losrennen soll, wenn die geladene Szene das Tutorial
    /// ist. Überladene Funktion von Unity.
    /// </summary>
    /// <param name="scene">Name der geladenen Szene</param>
    /// <param name="mode">Modus in dem die Szene geladen wird</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    { 
        if (scene.name == "Tutorial")
            EventManager.StartRunning();
    }

    /// <summary>
    /// Funktion, erhöht die Grundgeschwindigkeit des Spielers und setzt den Meilenstein um.
    /// </summary>
    private static void IncreasePlayerSpeedAndMilestone()
    {
        _baseSpeed += SpeedAdditionPerMileStone;
        _nextMilestone += Milestone;
    }

    /// <summary>
    /// 
    /// </summary>
    private static void ResetGlobalData()
    {
        _gameModus = false;
        _nextMilestone = Milestone;
        _isMultiplierActive = false;
        _playerSpeed = _baseSpeed = StartSpeed;
        _runMode = RunMode.Normal;
        _collision = true;
    }

    /// <summary>
    /// Gibt die x-Positionen der Strecken wieder.
    /// </summary>
    /// <returns>Array mit dem Streckenwerten</returns>
    public static int[] GetLanePositions()
    {
        return LanePositions;
    }

    public static int GetNextMilestone()
    {
        return _nextMilestone;
    }

    public static RunMode GetRunMode()
    {
        return _runMode;
    }

    public static void SetRunMode(RunMode mode)
    {
        _runMode = mode;
    }

    public static float GetSpeed()
    {
        return _baseSpeed;
    }

    public static void SetPlayerspeed(float speed)
    {
        _playerSpeed = speed;
    }

    public static float GetPlayerspeed()
    {
        return _playerSpeed;
    }

    /**
     * Returns the Z value of the player position.
     */
    public static float GetPlayerPosition()
    {
        return PlayerCtrl.GetPlayerZPosition();
    }

    public static bool IsMultiplierActive()
    {
        return _isMultiplierActive;
    }

    public static void SetMultiplierActive(bool value)
    {
        _isMultiplierActive = value;
    }

    public static bool GetGameModus()
    {
        return _gameModus;
    }

    private static void SetGameModus(bool value)
    {
        _gameModus = value;
    }

    public static void ToggleCollisionState()
    {
        _collision = !_collision;
    }

    public static bool GetCollisionState()
    {
        return _collision;
    }

    public static void StartPlayer()
    {
        SetRunMode(RunMode.Normal);
    }
    
    public static bool PlayerAtCtrlPoint()
    {
        return _runMode == RunMode.Idle && _gameModus;
    }

    public static GameObject GetInventoryItem()
    {
        return InventoryHandler.GetInventoryItem();
    }

    public static String GetActualSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    private static void StartGameAndChangeMode()
    {
        SetGameModus(true);
    }

    private static void StopGameAndChangeMode()
    {
        SetGameModus(false);
    }

    private static void PlayerHitTutorialObstacle(float pos)
    {
        SetRunMode(RunMode.Idle);
    }

    private void StartDeleteVrStuff()
    {
        StartCoroutine(DeleteVrStuff());
    }
    
    private IEnumerator DeleteVrStuff()
    {
        yield return new WaitForSeconds(0.7f);

        Destroy(VRSF_Components.CameraRig.gameObject);
        Destroy(FindObjectOfType<SetupVR>().gameObject);
    }
}