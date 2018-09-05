using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRSF.Controllers;
using VRSF.Utils;

/// <summary>
/// Klasse, verwaltet und steuert die UI Elemente des Spiels. 
/// </summary>
public class DisplayCtrl : MonoBehaviour
{
    public GameObject EndMenuInterface;     //Prefab des EndScreens
    public GameObject ScoreTableInScene;    //Referenz zur Punktetabelle 

    private GameObject _endMenuInterfaceInstance; //Instanz des Endscreens
    private const int EndScreenOffset = 7; //Abstand des EndScreens vor dem Spieler 

    /// <summary>
    /// Funktion, wird von Unity aufgerufen, wenn das dazugehörige GameObject aktiviert wird.
    /// Fügt die entsprechenden Methoden dem Event-Handler-System hinzu.
    /// </summary>
    private void OnEnable()
    {
        EventManager.GameEndEventMethods += SetupEndMenuInterface;
    }

    /// <summary>
    /// Funktion, wird von Unity aufgerufen, wenn das dazugehörige GameObject deaktiviert wird.
    /// Entfernt die entsprechenden Methoden aus dem Event-Handler-System.
    /// </summary>
    private void OnDisable()
    {
        EventManager.GameEndEventMethods -= SetupEndMenuInterface;
    }

    /// <summary>
    /// Funktion, wird von Unity jeden Frame aufgerufen.
    /// </summary>
    private void Start()
    {
        if (GlobalDataHandler.GetActualSceneName() != "HomeScene") return;
        
        List<int> scoreTable = ScoreTableCtrl.GetScoreTable();

        int i = 0;
        foreach (var textToChange in ScoreTableInScene.GetComponentsInChildren<Text>())
        {
            textToChange.text = scoreTable[i].ToString();
            i++;
        }
    }

    /// <summary>
    /// Funktion, bereitet die Endmenüinstanz vor.
    /// </summary>
    private void SetupEndMenuInterface()
    {
        ControllersParametersVariable.Instance.UsePointerRight = true;
        VRSF_Components.RightController.GetComponent<LineRenderer>().enabled = true;

        
        InstantiateScreen(ref _endMenuInterfaceInstance, EndMenuInterface, transform, true);
        _endMenuInterfaceInstance.transform.GetChild(1).GetComponent<Text>().text = ScoreHandler.GetPlayerScore().ToString();
    }

    /// <summary>
    /// Funktion, instanziiert die übergebenen Objekte.
    /// </summary>
    /// <param name="obj">Speichervariable die nach außen hin sichtbar ist.</param>
    /// <param name="itemToInstantiate">Button Objekte welche Instanziiert werden.</param>
    private void InstantiateScreen(ref GameObject obj, GameObject itemToInstantiate, Transform parentTransform, bool isEndScreen)
    {
        obj = Instantiate(itemToInstantiate);

        if (parentTransform == null) return;
        
        obj.transform.SetParent(parentTransform);

        if (isEndScreen)
        {
            obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, GlobalDataHandler.GetPlayerPosition() + 7);
        }
    }

    /// <summary>
    /// Funktion, führt den Start des Spiels in Level 1 aus, sobald auf das UI Element geklickt worden ist. 
    /// </summary>
    public void TriggerPlayHandle()
    {
        EventManager.DeleteVrStuff();

        ControllersParametersVariable.Instance.UsePointerLeft = false;
        ControllersParametersVariable.Instance.UsePointerRight = false;
        EventManager.GameReset();
        EventManager.SwitchScene("Level 1");
    }

    /// <summary>
    /// Funktion, führt den Start des Spiels im Tutorial aus, sobald auf das UI Element geklickt worden ist.
    /// </summary>
    public void TriggerTutorialHandle()
    {
        EventManager.DeleteVrStuff();
        EventManager.SwitchScene("Tutorial");
    }

    /// <summary>
    /// Funktion, wechselt den Spieler in die HomeScene, sobald auf das UI Element des EndScreens geklickt worden ist.
    /// </summary>
    public void TriggerEndMenuHandle()
    {
        EventManager.DeleteVrStuff();
        Destroy(_endMenuInterfaceInstance);
        EventManager.SwitchScene("HomeScene");
        
    }
    
    
}