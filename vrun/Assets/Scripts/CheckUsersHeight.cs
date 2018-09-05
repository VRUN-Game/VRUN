using UnityEngine;
using VRSF.Utils;

namespace VRRun.VRSetup
{
    /// <summary>
    /// Klasse, setzt die Spielerhöhe auf den angegebenen Wert, sofern die Oculus Rift benutzt wird.
    /// </summary>
	public class CheckUsersHeight : MonoBehaviour
    {

        private const float Height = 3.3f; //Höhe auf die der Spieler gesetzt wird
	    
        /// <summary>
        /// Funktion, wird von Unity jeden Frame aufgerufen.
        /// </summary>
        void Update()
        {
            if (VRSF_Components.DeviceLoaded == EDevice.OVR && SetupVR.SetupEnded)
            {
                VRSF_Components.CameraRig.transform.localPosition = new Vector3(0.0f, Height, 0.0f);
                this.enabled = false;
            }
        }
    }
}