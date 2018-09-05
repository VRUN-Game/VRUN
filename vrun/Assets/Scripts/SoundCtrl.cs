using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Klasse, verwaltet und steuert die Sounds des Spiels. Stellt Funktionen zum Laden, Abspielen, Stoppen von
/// Sound zur Verfügung. 
/// </summary>
public class SoundCtrl : MonoBehaviour
{
    /// <summary>
    /// Das Struct dient der Darstellung im Inspector, damit das Dictionary befüllt werden kann.
    /// </summary>
    [Serializable]
    public class SoundSources
    {
        public string Name;
        public AudioClip AudioClip;
    };

    /// <summary>
    /// Liste von Structs die beim initialisieren in ein Dictionary kopiert werden.
    /// </summary>
    public SoundSources[] SoundSourcesList;

    /// <summary>
    /// Dictionary welches die SoundClips hält.
    /// </summary>
    private Dictionary<string, AudioSource> _soundSources;

    // Flag, wird gesetzt, wenn kein Sound abgespielt werden soll. 
    private bool _muteAudio;

    /// <summary>
    /// Funktion, wird von Unity aufgerufen, wenn das dazugehörige GameObject aktiviert wird.
    /// Fügt die entsprechenden Methoden dem Event-Handler-System hinzu.
    /// </summary>
    private void OnEnable()
    {
        EventManager.PlayLevelSoundEventMethods += PlayBackgroundSound;
        EventManager.GameEndEventMethods += StopInGameSound;
        EventManager.StopLevelSoundEventMethods += StopInGameSound;
        EventManager.LevelObstacleObjHitEventMethods += PlayHitSound;
        EventManager.LevelScoreObjCollectedEventMethods += PlayScoreCollectibleSound;
        EventManager.LevelTimerTickSoundEventMethods += PlayTickSound;
        EventManager.LevelIncreaseSpeedAndMilestoneEventMethods += PlayFasterSound;
        EventManager.StoneSpawnEventMethods += PlayStoneGrind;
        EventManager.MuteAudioEventMethods += MuteAudio;
    }

    /// <summary>
    /// Funktion, wird von Unity aufgerufen, wenn das dazugehörige GameObject deaktiviert wird.
    /// Entfernt die entsprechenden Methoden aus dem Event-Handler-System.
    /// </summary>
    private void OnDisable()
    {
        EventManager.PlayLevelSoundEventMethods -= PlayBackgroundSound;
        EventManager.GameEndEventMethods -= StopInGameSound;
        EventManager.StopLevelSoundEventMethods -= StopInGameSound;
        EventManager.LevelObstacleObjHitEventMethods -= PlayHitSound;
        EventManager.LevelScoreObjCollectedEventMethods -= PlayScoreCollectibleSound;
        EventManager.LevelTimerTickSoundEventMethods -= PlayTickSound;
        EventManager.LevelIncreaseSpeedAndMilestoneEventMethods -= PlayFasterSound;
        EventManager.StoneSpawnEventMethods -= PlayStoneGrind;
        EventManager.MuteAudioEventMethods -= MuteAudio;
    }

    /// <summary>
    /// Funktion, wird von Unity beim Start einmalig ausgeführt, wenn alle Objekte initialisiert worden sind. 
    /// </summary>
    private void Awake()
    {
        _soundSources = new Dictionary<string, AudioSource>();
        FillDictionary(SoundSourcesList, ref _soundSources);
    }

    /// <summary>
    /// Funktion, wird von Unity jeden Frame ausgeführt.
    /// </summary>
    private void Update()
    {
        // Change the audio pitch based on the current time scale
        foreach (var soundSource in _soundSources)
        {
            if (soundSource.Value.isPlaying)
            {
                soundSource.Value.pitch = Time.timeScale;
            }
        }
    }

    /// <summary>
    /// Setzt die Flag um, die den Sound de-/ aktviert. 
    /// </summary>
    private void MuteAudio()
    {
        _muteAudio = !_muteAudio;
    }

    /// <summary>
    /// Befüllt das Dictionary mit dem übergebenen Array.
    /// </summary>
    /// <param name="ss">Array mit Name, AudioClip kombinationen</param>
    /// <param name="d">Dictionary welches befüllt wird</param>
    /// <exception cref="ArgumentException">Wird ausgegeben wenn ein Key doppelt erscheint</exception>
    private void FillDictionary(SoundSources[] ss, ref Dictionary<string, AudioSource> d)
    {
        foreach (var sounds in ss)
        {
            var tmp = gameObject.AddComponent<AudioSource>();
            tmp.clip = sounds.AudioClip;
            tmp.playOnAwake = false;
            d.Add(sounds.Name, tmp);
        }

        if (d.Count != ss.Length)
        {
            throw new ArgumentException("Key duplication present.");
        }
    }

    /// <summary>
    /// Fügt zur Laufzeit der Instanz eine weitere AudioSource hinzu die abgespielt werden kann.
    /// </summary>
    /// <param name="name">Name des Keys.</param>
    /// <param name="ac">AudioClip der in der AudioSource ist.</param>
    /// <exception cref="ArgumentNullException">Wird geworfen wenn der AudioClip null ist.</exception>
    /// <exception cref="ArgumentException">Wird geworfen wenn der zu setzende Key leer oder null ist.</exception>
    public void AddSound([NotNull] string name, [NotNull] AudioClip ac)
    {
        if (ac == null) throw new ArgumentNullException("ac", "Value cannot be null.");
        if (string.IsNullOrEmpty(name)) throw new ArgumentException("name", "Value cannot be null or empty.");

        var tmp = gameObject.AddComponent<AudioSource>();
        tmp.clip = ac;

        _soundSources.Add(name, tmp);
    }

    /// <summary>
    /// Spielt einen eingespeicherten Sound ab.
    /// </summary>
    /// <param name="name">Name der AudioSource der abgespielt werden soll.</param>
    /// <exception cref="ArgumentException">Wird geworfen, wenn der Name leer oder null ist.</exception>
    private void PlaySound([NotNull] string name)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentException("name", "Value cannot be null or empty.");
        if (_muteAudio) return;
        _soundSources[name].Play();
    }

    /// <summary>
    /// Stoppt einen eingespeicherten Sound.
    /// </summary>
    /// <param name="name">Name der AudioSource die gestoppt werden soll.</param>
    /// <exception cref="ArgumentException">Wird geworfen, wenn der Name leer oder null ist.</exception>
    private void StopSound([NotNull] string name)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentException("name", "Value cannot be null or empty.");
        _soundSources[name].Stop();
    }

    /// <summary>
    /// Gibt zurück ob die AudioSource gerade abgespielt wird.
    /// </summary>
    /// <param name="name">Name der AudioSource die überprüft wird.</param>
    /// <returns>Boolean über den Status der AudioSource ob sie gerade abspielt.</returns>
    /// <exception cref="ArgumentException">Wird geworfen, wenn der Name null oder leer ist.</exception>
    private bool IsSoundPlaying([NotNull] string name)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentException("name", "Value cannot be null or empty.");
        return _soundSources[name].isPlaying;
    }

    /// <summary>
    /// Setzt das Loop Attribut der AudioSource. Bewirkt, dass die AudioSource im Loop abgespielt wird.
    /// </summary>
    /// <param name="name">Name der AudioSource die umgestellt werden soll.</param>
    /// <param name="loop">Boolean der den Status der AudioSource setzt.</param>
    /// <exception cref="ArgumentException">Wird geworfen, wenn der Name der AudioSource leer oder null ist.</exception>
    private void SetLoop([NotNull] string name, bool loop)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentException("name", "Value cannot be null or empty.");
        _soundSources[name].loop = loop;
    }

    /// <summary>
    /// Spielt den Hintergrundsound ab. Der Hintergrundsound wird dabei durch ein Fade-In eingespielt. 
    /// </summary>
    /// <param name="name">Name der AudioSource, die abgespielt werden soll</param>
    /// <exception cref="ArgumentException">Wird geworfen, wenn der Name der AudioSource leer oder null ist.</exception>
    private void PlayBackgroundSound(string name)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentException("name", "Value cannot be null or empty.");
        StartCoroutine(FadeIn(_soundSources[name], .5f));
        SetLoop(name, true);
    }

    private void StopInGameSound()
    {
        if (string.IsNullOrEmpty(SceneManager.GetActiveScene().name)) throw new ArgumentException("name", "Value cannot be null or empty.");
        StartCoroutine(FadeOut(_soundSources[SceneManager.GetActiveScene().name], .5f));
    }

    /// <summary>
    /// Funktion, blendet den eingegebenen Sound in einer angegebenen Zeit durch ein Fade-Out aus.
    /// </summary>
    /// <param name="audioSource">AudioSource, die ausgeblendet werden soll</param>
    /// <param name="fadeTime">Zeit, die für das Fade-Out verwendet wird</param>
    /// <returns></returns>
    private static IEnumerator FadeOut(AudioSource audioSource, float fadeTime)
    {
        var startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    /// <summary>
    /// Funktion, blendet den eingegebenen Sound in einer angegebenen Zeit durch ein Fade-In ein.
    /// </summary>
    /// <param name="audioSource">AudioSource, die abgespielt werden soll</param>
    /// <param name="fadeTime">Zeit, die für das Fade-In verwendet wird</param>
    /// <returns></returns>
    private static IEnumerator FadeIn(AudioSource audioSource, float fadeTime)
    {
        var endVolume = audioSource.volume;
        audioSource.volume = 0;
        audioSource.Play();

        while (audioSource.volume < 1.0f - Mathf.Epsilon)
        {
            audioSource.volume += endVolume * Time.deltaTime / fadeTime;

            yield return null;
        }

        audioSource.volume = 1.0f;
    }

    /// <summary>
    /// Spielt den Sound ab, wenn der Spieler mit einem Objekt kollidiert.
    /// </summary>
    private void PlayHitSound()
    {
        PlaySound("HitSound");
    }

    /// <summary>
    /// Spielt den Sound ab, wenn der Spieler einen Punkt einsammelt. 
    /// </summary>
    private void PlayScoreCollectibleSound()
    {
        PlaySound("CollectScoreObj");
    }

    /// <summary>
    /// Spielt das Ticken des Timers ab.
    /// </summary>
    private void PlayTickSound()
    {
        PlaySound("TimerTickSound");
    }

    /// <summary>
    /// Spielt den Sound ab, der erklingt, wenn der Spieler schneller wird.
    /// </summary>
    private void PlayFasterSound()
    {
        PlaySound("Faster");
    }

    /// <summary>
    /// Spielt den Sound der Zackenbewegung ab. 
    /// </summary>
    private void PlayStoneGrind()
    {
        if (!IsSoundPlaying("Stone"))
        {
            PlaySound("Stone");
        }
    }
}