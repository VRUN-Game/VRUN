using UnityEngine;
using NewTypes;
using VRSF.Utils;
using UnityEngine.SceneManagement;

/// <summary>
/// Klasse, ist für die Steuerung und Bewegung des Spielers zuständig.
/// </summary>
public class PlayerCtrl : MonoBehaviour
{
	private static Transform _body;            // Transform des GameObjektes
    private Vector3 _inputs = Vector3.zero;    // Bewegungsvektor
    private bool _isGrounded = true;           // Flag, ob sich der Spieler auf dem Boden befindet
    private const int Gulf = -2;               // Grenzwert, der den Abgrund definiert
    private int _pastDistance = 0;             // zurückgelegte Strecke des Levels

    /// <summary>
    /// Funktion, wird von Unity aufgerufen, wenn das dazugehörige GameObject aktiviert wird.
    /// Fügt die entsprechenden Methoden dem Event-Handler-System hinzu.
    /// </summary>
    private void OnEnable()
    {
        EventManager.GameEndEventMethods += EndPlayerMovement;
        EventManager.GameResetEventMethods += GameReset;
        EventManager.LeaveCtrlPointEventMethods += StartPlayerMovement;
        EventManager.EnterCtrlPointEventMethods += EndPlayerMovement;
        EventManager.TutorialObstacleHitEventMethods += SetPlayerZPosition;
        EventManager.LevelIsLoadedEventMethods += ResetPlyrPositionAndPastDistanceForNextLevel;
    }

    /// <summary>
    /// Funktion, wird von Unity aufgerufen, wenn das dazugehörige GameObject deaktiviert wird.
    /// Entfernt die entsprechenden Methoden aus dem Event-Handler-System.
    /// </summary>
    private void OnDisable()
    {
        EventManager.GameEndEventMethods -= EndPlayerMovement;
        EventManager.GameResetEventMethods -= GameReset;
        EventManager.LeaveCtrlPointEventMethods -= StartPlayerMovement;
        EventManager.EnterCtrlPointEventMethods -= EndPlayerMovement;
        EventManager.TutorialObstacleHitEventMethods -= SetPlayerZPosition;
        EventManager.LevelIsLoadedEventMethods -= ResetPlyrPositionAndPastDistanceForNextLevel;
    }

    /// <summary>
    /// Funktion, wird von Unity ausgeführt, wenn das Skript aktiviert wird. Diese Funktion wird vor der 
    /// Update Methode ausgeführt.
    /// </summary>
    private void Start()
    {
        _body = VRSF_Components.CameraRig.transform;
    }

    /// <summary>
    /// Funktion, wird von Unity jeden Frame aufgerufen.
    /// </summary>
    private void Update()
    {
        if (_body != null) return;
        _body = VRSF_Components.CameraRig.transform;
    }

    /// <summary>
    /// Funktion, wird von Unity mehrmals pro Frame aufgerufen.
    /// </summary>
    private void FixedUpdate()
    {
        if (!GlobalDataHandler.PlayerAtCtrlPoint())
        {
            _inputs = Vector3.zero;
            _inputs.z = IsPlayerOnTrack() ? 1 : 0;
            
            // Erhöhe die Spielergeschwindigkeit beim Erreichen eines Meilensteins
            if (!GlobalDataHandler.GetActualSceneName().Contains("Tutorial") && 
                GlobalDataHandler.GetPlayerPosition() >= GlobalDataHandler.GetNextMilestone())
            {
                EventManager.IncreaseSpeedAndMilestone();
            }
            
            //Beende das Spiel, wenn der Spieler von der Strecke fällt
            if (!_isGrounded && _body.transform.position.y < Gulf)
            {
                EventManager.GameEnd();
            }

            //Aktualisiere die Punktzahl basierend auf der zurückgelegten Distanz
            int delta = (int) (_body.transform.position.z - _pastDistance);
            ScoreHandler.IncreasePlayerScore(delta);
            _pastDistance = (int) _body.transform.position.z;
        }
        
        if (!GlobalDataHandler.GetGameModus()) return;

        //Berechne die Geschwindigkeit des Spielers
        switch (GlobalDataHandler.GetRunMode())
        {
            case RunMode.Faster:
                GlobalDataHandler.SetPlayerspeed(GlobalDataHandler.GetSpeed() * 2);
                break;
            case RunMode.Normal:
                GlobalDataHandler.SetPlayerspeed(GlobalDataHandler.GetSpeed());
                break;
            case RunMode.Slower:
                GlobalDataHandler.SetPlayerspeed(GlobalDataHandler.GetSpeed() / 2);
                break;
            case RunMode.Idle:
                GlobalDataHandler.SetPlayerspeed(0);
                break;
        }

        //Berechne die neue Position des Spielers 
        _body.position += _inputs * GlobalDataHandler.GetPlayerspeed() * Time.fixedDeltaTime;
    }

    /// <summary>
    /// Funktion, gibt zurück ob sich der Spieler bezüglich der x-Achse noch auf der Strecke befindet.
    /// </summary>
    /// <returns>True, wenn der Spieler auf der Strecke ist. Sonst false.</returns>
    private bool IsPlayerOnTrack()
    {
        float trackBorder = GlobalDataHandler.TrackWidth / 2;
        return (_body.transform.position.x < trackBorder || _body.transform.position.x > -trackBorder);
    }

    /// <summary>
    /// Funktion, wird ausgeführt, wenn der Spieler nochmal spielt und das Spiel neugestartet wird.
    /// </summary>
    private void GameReset()
    {
        _body.position = Vector3.zero;
        _pastDistance = 0;
    }

    /// <summary>
    /// Funktion, setzt die Spielergewschwindigkeit auf normal.
    /// </summary>
    private void StartPlayerMovement()
    {
        GlobalDataHandler.SetRunMode(RunMode.Normal);
    }

    /// <summary>
    /// Funktion, setzt die Spielergeschwindigkeit auf "stehen".
    /// </summary>
    private void EndPlayerMovement()
    {
        GlobalDataHandler.SetRunMode(RunMode.Idle);
    }

    /// <summary>
    /// Funktion, setzt den z-Wert der Spielerposition auf den angegeben Wert.
    /// </summary>
    /// <param name="posZ">Z-Position auf die der Spieler gesetzt werden soll</param>
    public void SetPlayerZPosition(float posZ)
    {
        switch (VRSF_Components.DeviceLoaded)
        {
            case EDevice.OPENVR:
                _body.position = new Vector3(0, 0, posZ);
                break;
            default:
                _body.position = new Vector3(0, 3.3f, posZ);
                break;
        }
    }

    /// <summary>
    /// Gibt die aktuelle Position des Spielers wieder.
    /// </summary>
    /// <returns>Die aktuelle Position des Spielers</returns>
    public static float GetPlayerZPosition()
    {
        if (_body == null) return 0f;
        return _body.position.z;
    }

    /// <summary>
    /// Funktion, setzt den Spieler auf den z-Wert 0. Diese Funktion wird aufgerufen, wenn der Spieler
    /// das Level wechselt.
    /// </summary>
    private void ResetPlyrPositionAndPastDistanceForNextLevel()
    {
        if (_body == null || GlobalDataHandler.GetActualSceneName() == "HomeScene" 
            || GlobalDataHandler.GetActualSceneName() == "Tutorial") return;
        _body.position = new Vector3(_body.position.x, _body.position.y, 0);
        _pastDistance = 0;

    }
}