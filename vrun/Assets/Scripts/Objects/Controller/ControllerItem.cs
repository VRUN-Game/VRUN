using UnityEngine;
using NewTypes;
using VRSF.Utils;

/// <summary>
/// Klasse, wird an jedes Item gebunden und kontrolliert die einzelne Iteminstanz. Zuständig für den Flug 
/// Richtung Score bei Einsammeln und die ständige Rotation.
/// </summary>
public class ControllerItem : MonoBehaviour
{
    public const int RotationSpeed = 45;

    [Header("Variablen für die Interpolationen")]
    public const float InterpolationSpeed = 3.5f;

    public readonly Vector3 EndPosition = new Vector3(12.5f, 30f, 56f);
    public readonly Vector3 EndScaling = new Vector3(1.0f, 1.0f, 1.0f);

    public ItemType ItemType;

    /// <summary>
    /// Funktion, wird jeden Frame aufgerufen und lässt das Item drehen und bei Bedarf Richtung Score fliegen.
    /// </summary>
    void Update()
    {
        if (VRSF_Components.CameraRig == null) return;

        //Prüfe ob der Player das Parent-Element ist 
        if (transform.parent.name == GameObject.Find("Inventory").transform.name)
        {
            //Interpoliere die Position neben das Score-Textfeld (ItemBag)
            transform.localPosition = Vector3.Slerp(transform.localPosition, EndPosition,
                Time.deltaTime * InterpolationSpeed);

            //Interpoliere die Skalierung des Items
            transform.localScale =
                Vector3.Slerp(transform.localScale, EndScaling, Time.deltaTime * InterpolationSpeed);
        }

        //Drehe das Item kontinuierlich
        transform.Rotate(Vector3.up * Time.deltaTime * RotationSpeed, Space.World);
    }
}