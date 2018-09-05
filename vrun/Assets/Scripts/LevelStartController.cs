using UnityEngine;
using VRSF.Controllers;

/// <summary>
/// Klasse, wird ausgeführt, wenn ein Level gestartet wird und startet das entsprechende Level, deaktiviert den 
/// Laser Pointer und signalisiert, dass das Level geladen ist. 
/// </summary>
public class LevelStartController : MonoBehaviour {

    /// <summary>
    /// Funktion, wird von Unity ausgeführt, wenn das Skript aktiviert wird. Diese Funktion wird vor der 
    /// Update Methode ausgeführt.
    /// </summary>
    void Start () {
        ControllersParametersVariable.Instance.UsePointerRight = false;
        EventManager.LevelIsLoaded();
        EventManager.GameStart();
    }
}
