using System.Collections;
using UnityEngine;

/// <summary>
/// Klasse, die den Übergang der Schwarzmaske kontrolliert und ausführt.
/// </summary>
public class TransitionCtrl : MonoBehaviour
{
    private static Animator _transitionAnim; //Animator des Canvas, der gesteuert wird.
    
    void OnEnable()
    {
        _transitionAnim = GetComponentInChildren<Animator>();
        EventManager.LevelIsLoadedEventMethods += StartTransition;
    }

    private void OnDisable()
    {
        EventManager.LevelIsLoadedEventMethods -= StartTransition;
    }

    /// <summary>
    /// Startet die Animation und somit auch die Transition in Schwarze.
    /// </summary>
    /// <returns>Gibt einen IEnumerator zurück.</returns>
    public static IEnumerator ShowTransition()
    {
        _transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(.9f);
    }

    /// <summary>
    /// Startet die Animation in die Transparenz (Blendet die schwarze Maske aus).
    /// </summary>
    private void StartTransition()
    {
        _transitionAnim.SetTrigger("Start");
    }
}