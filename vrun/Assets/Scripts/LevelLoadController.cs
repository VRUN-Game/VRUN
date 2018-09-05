using UnityEngine;

/// <summary>
/// Klasse, wird ausgeführt, wenn ein Level gestartet wird und signalisiert, dass das entsprechende Level geladen
/// worden ist. 
/// </summary>
public class LevelLoadController : MonoBehaviour {

	/// <summary>
	/// Funktion, wird von Unity ausgeführt, wenn das Skript aktiviert wird. Diese Funktion wird vor der 
	/// Update Methode ausgeführt.
	/// </summary>
	void Start () {
		EventManager.LevelIsLoaded();
	}
}
