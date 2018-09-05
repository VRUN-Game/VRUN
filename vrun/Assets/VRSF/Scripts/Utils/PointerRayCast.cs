using System;
using ScriptableFramework.Variables;
using VRSF.Controllers;
using VRSF.Gaze;
using System.Linq;
using UnityEngine;
using VRSF.Interactions;

namespace VRSF.Utils
{
    /// <summary>
    /// Check the Raycast of the two controllers and the Gaze and reference them in three list of RaycastHit
    /// </summary>
    public class PointerRayCast : MonoBehaviour
    {
        // EMPTY
        #region PUBLIC_VARIABLES
        #endregion PUBLIC_VARIABLES


        #region PRIVATE_VARIABLES
        private ControllersParametersVariable _controllersParameter;
        private GazeParametersVariable _gazeParameters;
        private InteractionVariableContainer _interactionContainer;
        #endregion PRIVATE_VARIABLES


        #region MONOBEHAVIOUR_METHODS
        private void Start()
        {
            _controllersParameter = ControllersParametersVariable.Instance;
            _gazeParameters = GazeParametersVariable.Instance;
            _interactionContainer = InteractionVariableContainer.Instance;

            if (!_controllersParameter.UseControllers && !_gazeParameters.UseGaze)
                this.enabled = false;
        }

        private void Update ()
        {
            if (VRSF_Components.DeviceLoaded == EDevice.SIMULATOR)
                CheckMouseRays();
            else if (VRSF_Components.DeviceLoaded != EDevice.NULL)
                CheckVRRays();
        }
        #endregion MONOBEHAVIOUR_METHODS


        //EMPTY
        #region PUBLIC_METHODS
        #endregion PUBLIC_METHODS
            

        #region PRIVATE_METHODS
        /// <summary>
        /// Check the Rays from the two controllers
        /// </summary>
        private void CheckVRRays()
        {
            // Handle raycasting for both controllers
            if (_controllersParameter.UseControllers)
            {
                try
                {
                    if (_controllersParameter.RightPointerState != EPointerState.OFF)
                    {
                        var startTransform = VRSF_Components.RightController.transform;
                        RightRay = new Ray(startTransform.position, startTransform.TransformDirection(Vector3.forward));

                        RaycastHandler(RightRay, _controllersParameter.MaxDistancePointerRight,
                            _controllersParameter.GetExclusionsLayer(EHand.RIGHT), ref _interactionContainer.RightHit);
                    }

                    if (_controllersParameter.LeftPointerState != EPointerState.OFF)
                    {
                        var startTransform = VRSF_Components.LeftController.transform;
                        LeftRay = new Ray(startTransform.position, startTransform.TransformDirection(Vector3.forward));

                        RaycastHandler(LeftRay, _controllersParameter.MaxDistancePointerLeft,
                            _controllersParameter.GetExclusionsLayer(EHand.LEFT), ref _interactionContainer.LeftHit);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("VRSF : VR Components not setup yet, waiting for next frame.\n" + e);
                }
            }

            // Handle raycast for the Gaze
            try
            {
                if (_gazeParameters.UseGaze)
                {
                    var startTransform = VRSF_Components.VRCamera.transform;
                    GazeRay = new Ray(startTransform.position, startTransform.forward);

                    RaycastHandler(GazeRay, _gazeParameters.DefaultDistance, 
                        _gazeParameters.GetGazeExclusionsLayer(), ref _interactionContainer.GazeHit);
                }
            }
            catch (Exception e)
            {
                Debug.Log("VRSF : VR Components not setup yet, waiting for next frame.\n" + e);
            }
        }

        /// <summary>
        /// Check the Ray from the Mouse. We use the normal camera for the three raycast hit, meaning that they are all equals.
        /// </summary>
        private void CheckMouseRays()
        {
            if (_controllersParameter.UseControllers)
            {
                if (_controllersParameter.RightPointerState != EPointerState.OFF)
                {
                    RightRay = VRSF_Components.VRCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

                    RaycastHandler(RightRay, _controllersParameter.MaxDistancePointerRight, 
                        _controllersParameter.GetExclusionsLayer(EHand.RIGHT), ref _interactionContainer.RightHit);
                }

                if (_controllersParameter.LeftPointerState != EPointerState.OFF)
                {
                    LeftRay = VRSF_Components.VRCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

                    RaycastHandler(LeftRay, _controllersParameter.MaxDistancePointerLeft, 
                        _controllersParameter.GetExclusionsLayer(EHand.LEFT), ref _interactionContainer.LeftHit);
                }
            }

            if (_gazeParameters.UseGaze)
            {
                GazeRay = VRSF_Components.VRCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

                RaycastHandler(GazeRay, _gazeParameters.DefaultDistance, 
                    _gazeParameters.GetGazeExclusionsLayer(), ref _interactionContainer.GazeHit);
            }
        }

        /// <summary>
        /// Check if the Ray from a controller is hitting something
        /// </summary>
        /// <param name="ray">The ray to check</param>
        /// <param name="distance">The maximum distance to which we raycast</param>
        /// <param name="layerToIgnore">The layer(s) to ignore from raycasting</param>
        /// <param name="hitVariable">The RaycastHitVariable in which we store the hit value</param>
        private void RaycastHandler(Ray ray, float distance, int layerToIgnore, ref RaycastHitVariable hitVariable)
        {
            var hits = Physics.RaycastAll(ray, distance, layerToIgnore);
            
            if (hits.Length > 0)
            {
                var first3DHit = hits.OrderBy(x => x.distance).First();
                hitVariable.SetValue(first3DHit);
                hitVariable.isNull = false;
            }
            else
            {
                hitVariable.isNull = true;
            }
        }
        #endregion PRIVATE_METHODS


        #region GETTERS_SETTERS
        public Ray RightRay { get; private set; }

        public Ray LeftRay { get; private set; }

        public Ray GazeRay { get; private set; }
        #endregion GETTERS_SETTERS
    }
}
