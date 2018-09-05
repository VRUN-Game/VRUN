using System;
using ScriptableFramework.Util;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRSF.Utils;

/// <summary>
/// Klasse, ist für die Anzeige des Punktestandes des Spielers und des Item-Timers am Horizont zuständig. 
/// </summary>
public class PlayerstatsUIHandler : MonoBehaviour {

    private GameObject _scoreUi; //UI Element des Punktestandes in der Szene
    private GameObject _timerUi; //UI Element des Timers in der Szene
	private Text _scoreLabel; //Textelement des Punktestandes in der Szene
	private Text _timerLabel; //Textelement des Timers in der Szene

	/// <summary>
	/// Funktion, wird von Unity aufgerufen, wenn das dazugehörige GameObject aktiviert wird.
	/// Fügt die entsprechenden Methoden dem Event-Handler-System hinzu.
	/// </summary>
    private void OnEnable()
	{
		_scoreUi = VRSF_Components.CameraRig.transform.FindDeepChild("ScoreUI").gameObject;
		_timerUi = VRSF_Components.CameraRig.transform.FindDeepChild("Timer").gameObject;
		EventManager.GameStartEventMethods += DisplayPlayerStatsAndInventory;
		EventManager.ActivateScoreEventMethods += DisplayScore;
		EventManager.ActivateTimerEventMethods += DisplayTimer;
		EventManager.UpdateTimerDisplayEventMethods += UpdateTimerDisplay;
	}
	
	/// <summary>
	/// Funktion, wird von Unity aufgerufen, wenn das dazugehörige GameObject deaktiviert wird.
	/// Entfernt die entsprechenden Methoden aus dem Event-Handler-System.
	/// </summary>
	private void OnDisable()
	{
		EventManager.GameStartEventMethods -= DisplayPlayerStatsAndInventory;
		EventManager.ActivateScoreEventMethods -= DisplayScore;
		EventManager.ActivateTimerEventMethods -= DisplayTimer;
		EventManager.UpdateTimerDisplayEventMethods -= UpdateTimerDisplay;
	}

	/// <summary>
	/// Funktion, wird von Unity ausgeführt, wenn das Skript aktiviert wird. Diese Funktion wird vor der 
	/// Update Methode ausgeführt.
	/// </summary>
	private void Start()
	{
		_scoreLabel = _scoreUi.GetComponent<Text>();
		_timerLabel = _timerUi.GetComponent<Text>();

		if (!GlobalDataHandler.GetGameModus()) return;
		_scoreLabel.color = new Color(255, 255, 255, 1);
		_timerLabel.color = new Color(178, 0, 0, 1);
	}

	/// <summary>
	/// Funktion, wird von Unity jeden Frame aufgerufen.
	/// </summary>
	private void Update()
	{
		var plyrScore = ScoreHandler.GetPlayerScore();
		
		// Setze die Punktzahl, wenn der Spieler nicht in der HomeScene ist
		if (SceneManager.GetActiveScene().name != "HomeScene")
		{
			_scoreLabel.text = plyrScore <= 0 ? "0" : plyrScore.ToString();
        }
		
		// Zeige die UI Elemente an, wenn der Spieler in einem Level ist 
        if (SceneManager.GetActiveScene().name != "Tutorial" && _scoreUi != null && _timerUi != null)
        {
         	_scoreUi.SetActive(GlobalDataHandler.GetGameModus());
            _timerUi.SetActive(GlobalDataHandler.GetGameModus());
        }
	}

	/// <summary>
	/// Funktion, lässt den Punktestand und den Timer anzeigen, sofern die Szene nicht das Tutorial ist. 
	/// </summary>
	private void DisplayPlayerStatsAndInventory()
	{
        if (SceneManager.GetActiveScene().name != "Tutorial")
        {
            DisplayScore();
            DisplayTimer();
        }
    }

	/// <summary>
	/// Funktion, aktiviert die Anzeige des Punktestandes. 
	/// </summary>
	private void DisplayScore()
	{
        _scoreUi.SetActive(true);    
	}

	/// <summary>
	/// Funktion, aktiviert die Anzeige des Timers. 
	/// </summary>
	private void DisplayTimer()
	{
		_timerUi.SetActive(true);
	}
	
	/// <summary>
	/// Funktion, akualisiert die Timer Anzeige. 
	/// </summary>
	private void UpdateTimerDisplay()
	{
		var timer = InventoryHandler.GetTimer();
		var time = timer == null ? -1 : timer.GetTime();
        TimeSpan t = TimeSpan.FromSeconds(time);
        _timerLabel.color = time > 0.0f ? new Color(178f, 0, 0, 1.0f) : new Color(178f, 0, 0, 0);
        _timerLabel.text = string.Format("{0:0}", t.Seconds);
        
	}
	
}
