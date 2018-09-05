using UnityEngine;

/// <summary>
/// Klasse, wird an jeden Tunnel gebunden und ist für die Tunneldrehung zuständig.
/// </summary>
public class ControllerTunnel : MonoBehaviour
{
    public const int RotationSpeed = 5;

    /// <summary>
    /// Funktion, wird jeden Frame aufgerufen und lässt die Tunnelinstanz drehen.
    /// </summary>
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, RotationSpeed) * Time.deltaTime);
    }
}