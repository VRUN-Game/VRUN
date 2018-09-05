using System.Collections;
using UnityEngine;

public class TransitionCtrl : MonoBehaviour
{
    private static Animator _transitionAnim;

    // Use this for initialization
    void OnEnable()
    {
        _transitionAnim = GetComponentInChildren<Animator>();
        EventManager.LevelIsLoadedEventMethods += StartTransition;
    }

    private void OnDisable()
    {
        EventManager.LevelIsLoadedEventMethods -= StartTransition;
    }

    public static IEnumerator ShowTransition()
    {
        _transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(.9f);
    }

    private void StartTransition()
    {
        _transitionAnim.SetTrigger("Start");
    }
}