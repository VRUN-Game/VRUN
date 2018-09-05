using UnityEngine;

/// <summary>
/// Klasse, legt die Tastatureingaben für Testzwecke fest. Diese Klasse ist ausschließlich für Testzwecke 
/// entworfen worden und ist nicht relevant für das eigentliche Spiel.
/// </summary>
public class InputManager : MonoBehaviour
{

    /// <summary>
    /// Funktion, wird von Unity jeden Frame aufgerufen.
    /// </summary>
    void Update()
    {
        // Startet das Spiel und der Spieler rennt los
        if (Input.GetKeyDown("g"))
        {
            EventManager.GameStart();
        }
        
        // Beendet das Spiel und führt einen Reset aus 
        if (Input.GetKeyDown("r"))
        {
            EventManager.GameEnd();
            EventManager.GameReset();
        }

        // Aktiviert das eingesammelte Item
        if (Input.GetKeyDown("v") && GlobalDataHandler.GetGameModus())
        {
            EventManager.ActivatePlayersInventoryItem();
        }

        // De- / aktiviert den Kollisionsmodus
        if (Input.GetKeyDown("c"))
        {
            GlobalDataHandler.ToggleCollisionState();
        }

        // Verlässt einen Kontrollpunkt
        if (Input.GetKeyDown("w"))
        {
            EventManager.LeaveCtrlPoint();
        }

        // De- / aktiviert die Spielsounds
        if (Input.GetKeyDown("k"))
        {
            EventManager.MuteAudio();
        }

        // Beendet das Spiel und spawnt den Spieler in der HomeScene
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EventManager.GameEnd();
            EventManager.DeleteVrStuff();
            EventManager.SwitchScene("HomeScene");
        }
    }

    /// <summary>
    /// Funktion, aktiviert das Item des Inventars. 
    /// </summary>
    public void ActivateInventoryItem()
    {
        EventManager.ActivatePlayersInventoryItem();
    }
}