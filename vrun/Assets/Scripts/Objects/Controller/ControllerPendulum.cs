using UnityEngine;

/// <summary>
/// Klasse, wird an jedes Pendel gebunden und kontrolliert die einzelne Iteminstanz. Zuständig für die
/// schwingende Bewegung des Pendels und das Abspielen des Sounds.
/// </summary>
public class ControllerPendulum : MonoBehaviour
{
    public const float SwingSpeed = 1.0f;
    private float _startTime;
    private Quaternion _start, _end;
    private const int MaxStartTime = 5;
    private const int MaxSwingAngle = 140;
    private const int MinSwingAngle = 280;
    private const int LeftSoundBorder = 160;
    private const int RightSoundBorder = 260;
    private AudioSource _audioSource;
    private bool _playedOncePerDirection;

    /// <summary>
    /// Initialisierungen zu Beginn des Spiels.
    /// </summary>
    private void Start()
    {
        _startTime = Random.Range(0, MaxStartTime);
        _start = Quaternion.Euler(0, 0, MaxSwingAngle);
        _end = Quaternion.Euler(0, 0, MinSwingAngle);
        _audioSource = GetComponent<AudioSource>();
    }


    /// <summary>
    /// Funktion, wird jeden Frame aufgerufen und lässt das Pendel schwingen.
    /// </summary>
    private void Update()
    {
        _startTime += Time.deltaTime;
        transform.rotation =
            Quaternion.Lerp(_start, _end, (Mathf.Sin(_startTime * SwingSpeed + Mathf.PI / 2) + 1.0f) / 2.0f);

        PlaySound();
    }

    /// <summary>
    /// Spielt den Sound für das Pendelschwingen ab in Abhängigkeit
    /// zur Pendelposition.
    /// </summary>
    private void PlaySound()
    {
        var pendulumAngle = transform.rotation.eulerAngles.z;

        if (pendulumAngle < RightSoundBorder && pendulumAngle > LeftSoundBorder && !_playedOncePerDirection)
        {
            _audioSource.Play();
            _playedOncePerDirection = true;
        }
        else
        {
            _playedOncePerDirection = false;
        }
    }
}