using System.Collections.Generic;
using UnityEngine;
using VRSF.Utils;

/// <summary>
/// Klasse, wird an jeden Coin gebunden und kontrolliert die einzelne Coininstanz. Zuständig für das Wabern des
/// einzelnen Coins und den Flug Richtung Score bei Einsammeln.
/// </summary>
public class PointController : MonoBehaviour
{
	[Header("Werte für das Wabern")]
	public float Amplitude;
	public float Speed;

	// Variablen für das Wabern
	private float _tempVal;
	private Vector3 _tempPos;
	private float _startValue;

	[Header("Variablen für die Interpolationen")]
	public readonly Vector3 EndPosition = new Vector3(0, 370, 800);

	public readonly Vector3 EndScaling = new Vector3(.1f, .1f, .1f);
	public const float DestroyHeight = 50f;
	private const float InterpolationSpeed = 0.09f;
    
	/// <summary>
    /// Initialisierungen zu Beginn des Spiels.
    /// </summary>
    void Start()
    {
        _tempVal = transform.position.y;
        _tempPos = transform.position;
    }

    /// <summary>
    /// Funktion, wird jeden Frame aufgerufen.
    /// </summary>
    void Update()
    {
        if (VRSF_Components.CameraRig == null) return;

        //Prüfe ob das Inventar das Parent-Element ist 
        if (transform.parent.name == "Inventory")
        {
            //Interpoliere die Position zu dem Score-Textfeld
            transform.localPosition = Vector3.Lerp(transform.localPosition, EndPosition, Time.deltaTime * InterpolationSpeed);

            //Interpoliere die Skalierung des Coins
            transform.localScale = Vector3.Slerp(transform.localScale, EndScaling, Time.deltaTime * InterpolationSpeed);

            //Zerstöre den Coin ab angegebener Höhe
            if (transform.position.y > DestroyHeight) Destroy(this.gameObject);
        }
        else
        {
            //Berechne die schwingende Bewegung
            _tempPos.y = _tempVal + Amplitude * Mathf.Sin(Speed * (Time.time + _startValue));
            transform.position = _tempPos;
        }
    }

    /// <summary>
    /// Setzt den Beginn des Waberns für die Coininstanz. 
    /// </summary>
    /// <param name="value">Anfangswert für das Wabern der Coins</param>
    public void SetStartValue(float value)
    {
        _startValue = value;
    }
}