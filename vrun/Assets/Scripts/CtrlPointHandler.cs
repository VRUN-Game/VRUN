using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Klasse, ist für die Verwaltung und Steuerung der Kontrollpunkte in der Tutorial Szene zuständig. 
/// </summary>
public class CtrlPointHandler : MonoBehaviour
{
    private List<Transform> _controlPoints; //Liste der Kontrollpunkte
    private int _actualCtrlPointIdx;        // Index des aktuellen Kontrollpunkes an dem sich der Spieler befindet

    /// <summary>
    /// Funktion, wird von Unity aufgerufen, wenn das dazugehörige GameObject aktiviert wird.
    /// Fügt die entsprechenden Methoden dem Event-Handler-System hinzu.
    /// </summary>
    private void OnEnable()
    {
        EventManager.EnterCtrlPointEventMethods += EnterCtrlPoint;
        EventManager.LeaveCtrlPointEventMethods += LeaveCtrlPoint;
        EventManager.TutorialObstacleHitEventMethods += HitObstacle;
    }

    /// <summary>
    /// Funktion, wird von Unity aufgerufen, wenn das dazugehörige GameObject deaktiviert wird.
    /// Entfernt die entsprechenden Methoden aus dem Event-Handler-System.
    /// </summary>
    private void OnDisable()
    {
        EventManager.EnterCtrlPointEventMethods -= EnterCtrlPoint;
        EventManager.LeaveCtrlPointEventMethods -= LeaveCtrlPoint;
        EventManager.TutorialObstacleHitEventMethods -= HitObstacle;
    }


    /// <summary>
    /// Funktion, wird von Unity ausgeführt, wenn das Skript aktiviert wird. Diese Funktion wird vor der 
    /// Update Methode ausgeführt.
    /// </summary>
    private void Start()
    {
        _controlPoints = new List<Transform>();
        _actualCtrlPointIdx = -1;

        foreach (Transform child in transform)
        {
            _controlPoints.Add(child);
        }
    }

    /// <summary>
    /// Funktion, sendet das Ereignis, dass ein Kontrollpunkt verlassen worden ist.
    /// </summary>
    public void LeaveCtrlPointHandler()
    {
        EventManager.LeaveCtrlPoint();
    }

    /// <summary>
    /// Funktion, sendet ein Ereignis, dass ein Kontrollpunkt betreten worden ist. 
    /// </summary>
    public void EnterCtrlPoint()
    {

        if (_actualCtrlPointIdx >= 0)
        {
            _controlPoints[_actualCtrlPointIdx].GetComponent<BoxCollider>().enabled = false;            
        }
        _actualCtrlPointIdx++;
        _controlPoints[_actualCtrlPointIdx].GetComponent<BoxCollider>().enabled = false;
        HandleSpecificCtrlPoint(_controlPoints[_actualCtrlPointIdx].name);

    }

    /// <summary>
    /// Funktion, deaktiviert den aktuellen Kontrollpunkt, wenn dieser Verlassen wird. 
    /// </summary>
    private void LeaveCtrlPoint()
    {
        _controlPoints[_actualCtrlPointIdx].GetChild(0).gameObject.SetActive(false);
    }

    /// <summary>
    /// Funktion, behandelt, je nach Eingabe des spezifischen Kontrollpunktes, welche Ereignisse ausgeführt werden
    /// sollen. 
    /// </summary>
    /// <param name="name">Name des Kontrollpunktes für den die spezifischen Ereignisse ausgeführt werden sollen</param>
    private void HandleSpecificCtrlPoint(string name)
    {
        switch (name)
        {
            case "Score":
                EventManager.ActivateScore();
                break;
            case "Items":
                EventManager.ActivateTimer();
                break;
            case "Finish":
                if (GlobalDataHandler.GetInventoryItem() != null)
                {
					_controlPoints[_actualCtrlPointIdx].GetComponent<BoxCollider>().enabled = true;
					_actualCtrlPointIdx--;
					EventManager.HitTutorialObstacle(_controlPoints[_actualCtrlPointIdx].transform.position.z);
                    _controlPoints[_actualCtrlPointIdx].GetComponent<BoxCollider>().enabled = true;

                } else
				{
					EventManager.ResetInventory();
				}

                break;
        }
    }

    /// <summary>
    /// Funktion aktiviert den letzten Kontrollpunkt wieder, wenn der Spieler mit einem Hindernis kollidiert. 
    /// </summary>
    /// <param name="pos">Parameter, muss aufgrund des Events mit übergeben werden. Wird hier nicht verwendet</param>
    private void HitObstacle(float pos)
    {
        _controlPoints[_actualCtrlPointIdx].GetChild(0).gameObject.SetActive(true);
    }

    /// <summary>
    /// Funktion, gibt die Position des aktuellen Kontrollpunktes wieder. 
    /// </summary>
    /// <returns>Aktuelle Position des Kontrollpunktes</returns>
    public float GetActualCtrlPointPosZ()
    {
        return (_actualCtrlPointIdx == 0) ? 0 : _controlPoints[_actualCtrlPointIdx].transform.position.z;
    }
}