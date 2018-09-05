using UnityEngine;

/// <summary>
/// Klasse, wird an jeden gebunden und lässt Zacken, die auf den Boden platziert sind, hochfahren.
/// </summary>
public class ControllerSpike : MonoBehaviour
{
    private float _upSpeed;
    private bool _grounded;
    private int _differenceToPlayer;
    private float _newHeight;
    private bool _soundPlayed;

    /// <summary>
    /// Initialisierungen zu Beginn des Spiels.
    /// </summary>
    void Start()
    {
        _upSpeed = 0.003f;
        _grounded = transform.position.y < 0.0f;
        _differenceToPlayer = 40;
        _newHeight = 0;
        _soundPlayed = false;
    }

    /// <summary>
    /// Funktion, wird jeden Frame aufgerufen und lässt die Zacken bei Bedarf hochfliegen.
    /// </summary>
    void Update()
    {
        //Überprüft, ob sich der Spieler nahe genug am Zacken befindet
        if (_grounded && ((transform.position.z - GlobalDataHandler.GetPlayerPosition()) < _differenceToPlayer))
        {
            //Spielt den Sound ab
            if (!_soundPlayed)
            {
                EventManager.PlayStoneGrindSound();
                _soundPlayed = true;
            }

            //Interpoliert die Position = Hochfahren des Zackens
            transform.localPosition = Vector3.Slerp(transform.localPosition,
                new Vector3(transform.localPosition.x, _newHeight, transform.localPosition.z), Time.time * _upSpeed);
        }
    }
}