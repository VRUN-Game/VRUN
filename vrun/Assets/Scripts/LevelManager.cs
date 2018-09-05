using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Klasse, ist für das asynchrone Laden der Szenen zuständig.
/// </summary>
public class LevelManager : MonoBehaviour
{
    /// <summary>
    /// Funktion, wird von Unity aufgerufen, wenn das dazugehörige GameObject aktiviert wird.
    /// Fügt die entsprechenden Methoden dem Event-Handler-System hinzu.
    /// </summary>
    private void OnEnable()
    {
        EventManager.LoadNewSceneEventMethods += LoadSceneAsync;
    }

    /// <summary>
    /// Funktion, wird von Unity aufgerufen, wenn das dazugehörige GameObject deaktiviert wird.
    /// Entfernt die entsprechenden Methoden aus dem Event-Handler-System.
    /// </summary>
    private void OnDisable()
    {
        EventManager.LoadNewSceneEventMethods -= LoadSceneAsync;
    }

    /// <summary>
    /// Lädt die angegebene Szene asynchron im Hintergrund. 
    /// </summary>
    /// <param name="sceneName">Name der Szene, die geladen werden soll</param>
    private void LoadSceneAsync(String sceneName)
    {
		EventManager.StopLevelSound();
        StartCoroutine(LoadActionsAsync(sceneName));
    }

    /// <summary>
    /// Funktion, startet den Szenenübergang. Dabei wird die Szene  geladen und eine schwarze Fläche
    /// vor dem Spieler eingeblendet.
    /// </summary>
    /// <param name="sceneName">Name der Scene, die geladen werden soll</param>
    /// <returns>IEnumator</returns>
    private IEnumerator LoadActionsAsync(String sceneName)
    {
        yield return StartCoroutine(TransitionCtrl.ShowTransition());
        yield return StartCoroutine(LoadAsyncScene(sceneName));
    }

    /// <summary>
    /// Lädt die angegebene Scene asynchron im Hintergrund. 
    /// </summary>
    /// <param name="sceneName">Name der Szene, die geladen werden soll</param>
    private IEnumerator LoadAsyncScene(String sceneName)
    {
        AsyncOperation asyncScene = SceneManager.LoadSceneAsync(sceneName);
        asyncScene.allowSceneActivation = false;

        while (!asyncScene.isDone)
        {
            if (asyncScene.progress >= 0.9f)
            {
                asyncScene.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}