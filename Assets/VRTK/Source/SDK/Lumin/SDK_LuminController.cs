// Lumin Controller|SDK_Lumin|004
namespace VRTK
{
    using UnityEngine;
    using System.Collections.Generic;
#if VRTK_DEFINE_SDK_LUMIN && UNITY_2017_2_OR_NEWER
    using UnityEngine.XR.MagicLeap;
    using MagicLeap;
#endif

    /// <summary>
    /// The Lumin Controller SDK script provides a bridge to SDK methods that deal with the input devices.
    /// </summary>
    [SDK_Description(typeof(SDK_Lumin))]
    public class SDK_LuminController
#if VRTK_DEFINE_SDK_LUMIN && UNITY_2017_2_OR_NEWER
        : SDK_BaseController
#else
        : SDK_FallbackController
#endif
    {
#if VRTK_DEFINE_SDK_LUMIN && UNITY_2017_2_OR_NEWER
        protected ControllerConnectionHandler controllerConnectionHandler;
        protected MLInputController cachedLeftController;
        protected VRTK_VelocityEstimator cachedLeftVelocityEstimator;
        //protected VRTK_VelocityEstimator cachedRightVelocityEstimator;

        protected bool homeLeftPressed = false;
        protected bool homeRightPressed = false;
        protected bool bumperLeftPressed = false;
        protected bool bumperRightPressed = false;

        protected Dictionary<MLInputController, GameObject> cachedTrackedGameObjectsByController = new Dictionary<MLInputController, GameObject>();
        //protected Dictionary<uint, MLInputController> cachedTrackedObjectsByIndex = new Dictionary<uint, MLInputController>();

        #region Overriden base functions
        /// <summary>
        /// The GenerateControllerPointerOrigin method can create a custom pointer origin Transform to represent the pointer position and forward.
        /// </summary>
        /// <param name="parent">The GameObject that the origin will become parent of. If it is a controller then it will also be used to determine the hand if required.</param>
        /// <returns>A generated Transform that contains the custom pointer origin.</returns>
        [System.Obsolete("GenerateControllerPointerOrigin has been deprecated and will be removed in a future version of VRTK.")]
        public override Transform GenerateControllerPointerOrigin(GameObject parent)
        {
            return null;
        }

        /// <summary>
        /// The GetAngularVelocity method is used to determine the current angular velocity of the tracked object on the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current angular velocity of the tracked object.</returns>
        public override Vector3 GetAngularVelocity(VRTK_ControllerReference controllerReference)
        {
            if (VRTK_ControllerReference.IsValid(controllerReference))
            {
                if (controllerReference.hand == ControllerHand.Left && cachedLeftVelocityEstimator != null)
                {
                    return cachedLeftVelocityEstimator.GetAngularVelocityEstimate();
                }
                /*else if (controllerReference.hand == ControllerHand.Right && cachedRightVelocityEstimator != null)
                {
                    return cachedRightVelocityEstimator.GetAngularVelocityEstimate();
                }*/
            }
            return Vector3.zero;
        }

        /// <summary>
        /// The GetButtonAxis method retrieves the current X/Y axis values for the given button type on the given controller reference.
        /// </summary>
        /// <param name="buttonType">The type of button to check for the axis on.</param>
        /// <param name="controllerReference">The reference to the controller to check the button axis on.</param>
        /// <returns>A Vector2 of the X/Y values of the button axis. If no axis values exist for the given button, then a Vector2.Zero is returned.</returns>
        public override Vector2 GetButtonAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)
        {
            MLInputController controller = GetInputControllerByReference(controllerReference);
            if (controller == null)
            {
                return Vector2.zero;
            }

            switch (buttonType)
            {
                case ButtonTypes.Trigger:
                    return new Vector2(0, controller.TriggerValue);
                case ButtonTypes.Touchpad:
                    return new Vector2(controller.Touch1PosAndForce.x, controller.Touch1PosAndForce.y);
            }

            return Vector2.zero;
        }

        /// <summary>
        /// The GetButtonSenseAxis method retrieves the current sense axis value for the given button type on the given controller reference.
        /// </summary>
        /// <param name="buttonType">The type of button to check for the sense axis on.</param>
        /// <param name="controllerReference">The reference to the controller to check the sense axis on.</param>
        /// <returns>The current sense axis value.</returns>
        public override float GetButtonSenseAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)
        {
            MLInputController controller = GetInputControllerByReference(controllerReference);
            if (controller == null)
            {
                return 0;
            }

            switch (buttonType)
            {
                case ButtonTypes.Trigger:
                    return controller.TriggerValue;
                case ButtonTypes.Touchpad:
                    return controller.Touch1PosAndForce.z;
            }

            return 0;
        }

        /// <summary>
        /// The GetButtonHairlineDelta method is used to get the difference between the current button press and the previous frame button press.
        /// </summary>
        /// <param name="buttonType">The type of button to get the hairline delta for.</param>
        /// <param name="controllerReference">The reference to the controller to get the hairline delta for.</param>
        /// <returns>The delta between the button presses.</returns>
        public override float GetButtonHairlineDelta(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)
        {
            MLInputController controller = GetInputControllerByReference(controllerReference);
            if (controller == null)
            {
                return 0;
            }
            //TODO: Implement
            return 0;
        }

        /// <summary>
        /// The GetControllerButtonState method is used to determine if the given controller button for the given press type on the given controller reference is currently taking place.
        /// </summary>
        /// <param name="buttonType">The type of button to check for the state of.</param>
        /// <param name="pressType">The button state to check for.</param>
        /// <param name="controllerReference">The reference to the controller to check the button state on.</param>
        /// <returns>Returns true if the given button is in the state of the given press type on the given controller reference.</returns>
        public override bool GetControllerButtonState(ButtonTypes buttonType, ButtonPressTypes pressType, VRTK_ControllerReference controllerReference)
        {
            MLInputController controller = GetInputControllerByReference(controllerReference);
            if (controller == null)
            {
                return false;
            }

            switch (buttonType)
            {
                case ButtonTypes.Trigger:
                    return controller.TriggerValue > 0.0f;

                case ButtonTypes.TriggerHairline:
                    //TODO
                    return false;

                case ButtonTypes.Grip:
                    return false;

                case ButtonTypes.Touchpad:
                    return controller.Touch1PosAndForce.z > 0.0f;

                case ButtonTypes.TouchpadTwo:
                    return controller.Touch2PosAndForce.z > 0.0f;

                case ButtonTypes.ButtonTwo:
                    if (controller.Hand == MLInput.Hand.Left)
                    {
                        return bumperLeftPressed;
                    } else
                    {
                        // return bumperRightPressed;
                        return false;
                    }

                case ButtonTypes.StartMenu:
                    if (controller.Hand == MLInput.Hand.Left)
                    {
                        return homeLeftPressed;
                    }
                    else
                    {
                        //return homeRightPressed;
                        return false;
                    }
            }
            return false;
        }

        /// <summary>
        /// The GetControllerByIndex method returns the GameObject of a controller with a specific index.
        /// </summary>
        /// <param name="index">The index of the controller to find.</param>
        /// <param name="actual">If true it will return the actual controller, if false it will return the script alias controller GameObject.</param>
        /// <returns>The GameObject of the controller</returns>
        public override GameObject GetControllerByIndex(uint index, bool actual = false)
        {
            if (controllerConnectionHandler == null)
            {
                return null;
            }

            if (!controllerConnectionHandler.IsControllerValid((byte) index))
            {
                return null;
            }

            if (index < uint.MaxValue)
            {
                VRTK_SDKManager sdkManager = VRTK_SDKManager.instance;
                if (sdkManager != null)
                {
                    if (cachedLeftController != null && (uint) cachedLeftController.Id == index)
                    {
                        return (actual ? sdkManager.loadedSetup.actualLeftController : sdkManager.scriptAliasLeftController);
                    }

                    /*if (cachedRightTrackedObject != null && (uint)cachedRightTrackedObject.Index == index)
                    {
                        return (actual ? sdkManager.loadedSetup.actualRightController : sdkManager.scriptAliasRightController);
                    }*/
                }
            }
            
            return null;
        }

        /// <summary>
        /// The GetControllerDefaultColliderPath returns the path to the prefab that contains the collider objects for the default controller of this SDK.
        /// </summary>
        /// <param name="hand">The controller hand to check for</param>
        /// <returns>A path to the resource that contains the collider GameObject.</returns>
        public override string GetControllerDefaultColliderPath(ControllerHand hand)
        {
            //TODO: Implement
            return "ControllerColliders/Fallback";
        }

        /// <summary>
        /// The GetControllerElementPath returns the path to the game object that the given controller element for the given hand resides in.
        /// </summary>
        /// <param name="element">The controller element to look up.</param>
        /// <param name="hand">The controller hand to look up.</param>
        /// <param name="fullPath">Whether to get the initial path or the full path to the element.</param>
        /// <returns>A string containing the path to the game object that the controller element resides in.</returns>
        public override string GetControllerElementPath(ControllerElements element, ControllerHand hand, bool fullPath = false)
        {
            /*
            InteractionSourceHandedness handedness;
            switch (hand)
            {
                case ControllerHand.Left:
                    handedness = InteractionSourceHandedness.Left;
                    break;
                case ControllerHand.Right:
                    handedness = InteractionSourceHandedness.Right;
                    break;
                default:
                    handedness = InteractionSourceHandedness.Unknown;
                    break;
            }
            
            return ControllerVisualizer.Instance.GetPathToButton(element, handedness);
            */
            return "";
        }

        /// <summary>
        /// The GetControllerIndex method returns the index of the given controller.
        /// </summary>
        /// <param name="controller">The GameObject containing the controller.</param>
        /// <returns>The index of the given controller.</returns>
        public override uint GetControllerIndex(GameObject controllerGameObject)
        {
            MLInputController controller = GetInputControllerByGameObject(controllerGameObject);
            if (controller == null)
            {
                return uint.MaxValue;
            }

            return controller.Id;
        }

        /// <summary>
        /// The GetControllerLeftHand method returns the GameObject containing the representation of the left hand controller.
        /// </summary>
        /// <param name="actual">If true it will return the actual controller, if false it will return the script alias controller GameObject.</param>
        /// <returns>The GameObject containing the left hand controller.</returns>
        public override GameObject GetControllerLeftHand(bool actual = false)
        {
            GameObject controller = GetSDKManagerControllerLeftHand(actual);
            if (controller == null && actual)
            {
                controller = VRTK_SharedMethods.FindEvenInactiveGameObject<SDK_LuminCameraRig>("Controller (Left)");
            }
            
            return controller;
        }

        /// <summary>
        /// The GetControllerModel method returns the model alias for the given GameObject.
        /// </summary>
        /// <param name="controller">The GameObject to get the model alias for.</param>
        /// <returns>The GameObject that has the model alias within it.</returns>
        public override GameObject GetControllerModel(GameObject controller)
        {
            return GetControllerModelFromController(controller);
        }

        /// <summary>
        /// The GetControllerModel method returns the model alias for the given controller hand.
        /// </summary>
        /// <param name="hand">The hand enum of which controller model to retrieve.</param>
        /// <returns>The GameObject that has the model alias within it.</returns>
        public override GameObject GetControllerModel(ControllerHand hand)
        {
            GameObject model = GetSDKManagerControllerModelForHand(hand);
            if (model == null)
            {
                GameObject controller = null;
                switch (hand)
                {
                    case ControllerHand.Left:
                        controller = GetControllerLeftHand(true);
                        break;
                    case ControllerHand.Right:
                        controller = GetControllerRightHand(true);
                        break;
                }
                if (controller != null)
                {
                    Transform modelTransform = controller.transform.Find("Parts");
                    if (modelTransform != null)
                    {
                        model = modelTransform.gameObject;
                    }
                }
            }
            return model;
        }

        /// <summary>
        /// The GetControllerOrigin method returns the origin of the given controller.
        /// </summary>
        /// <param name="controllerReference">The reference to the controller to retrieve the origin from.</param>
        /// <returns>A Transform containing the origin of the controller.</returns>
        public override Transform GetControllerOrigin(VRTK_ControllerReference controllerReference)
        {
            return VRTK_SDK_Bridge.GetPlayArea();
        }

        /// <summary>
        /// The GetControllerRenderModel method gets the game object that contains the given controller's render model.
        /// </summary>
        /// <param name="controllerReference">The reference to the controller to check.</param>
        /// <returns>A GameObject containing the object that has a render model for the controller.</returns>
        public override GameObject GetControllerRenderModel(VRTK_ControllerReference controllerReference)
        {
            if (cachedLeftController.Id == controllerReference.index)
            {
                return GetControllerModel(ControllerHand.Left);
            }
            return null;
        }

        /// <summary>
        /// The GetControllerRightHand method returns the GameObject containing the representation of the right hand controller.
        /// </summary>
        /// <param name="actual">If true it will return the actual controller, if false it will return the script alias controller GameObject.</param>
        /// <returns>The GameObject containing the right hand controller.</returns>
        public override GameObject GetControllerRightHand(bool actual = false)
        {
            GameObject controller = GetSDKManagerControllerRightHand(actual);
            if (controller == null && actual)
            {
                controller = VRTK_SharedMethods.FindEvenInactiveGameObject<SDK_LuminCameraRig>("Controller (Right)");
            }
            return controller;
        }

        /// <summary>
        /// The GetCurrentControllerType method returns the current used ControllerType based on the SDK and headset being used.
        /// </summary>
        /// <returns>The ControllerType based on the SDK and headset being used.</returns>
        public override ControllerType GetCurrentControllerType(VRTK_ControllerReference controllerReference = null)
        {
            return ControllerType.MagicLeap_Controller;
        }

        /// <summary>
        /// The GetHapticModifiers method is used to return modifiers for the duration and interval if the SDK handles it slightly differently.
        /// </summary>
        /// <returns>An SDK_ControllerHapticModifiers object with a given `durationModifier` and an `intervalModifier`.</returns>
        public override SDK_ControllerHapticModifiers GetHapticModifiers()
        {
            SDK_ControllerHapticModifiers modifiers = new SDK_ControllerHapticModifiers();
            modifiers.durationModifier = 0.4f;
            return modifiers;
        }

        /// <summary>
        /// The GetVelocity method is used to determine the current velocity of the tracked object on the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to check for.</param>
        /// <returns>A Vector3 containing the current velocity of the tracked object.</returns>
        public override Vector3 GetVelocity(VRTK_ControllerReference controllerReference)
        {
            if (VRTK_ControllerReference.IsValid(controllerReference))
            {
                if (controllerReference.hand == ControllerHand.Left && cachedLeftVelocityEstimator != null)
                {
                    return cachedLeftVelocityEstimator.GetVelocityEstimate();
                }
                /*else if (controllerReference.hand == ControllerHand.Right && cachedRightVelocityEstimator != null)
                {
                    return cachedRightVelocityEstimator.GetVelocityEstimate();
                }*/
            }
            return Vector3.zero;
        }

        /// <summary>
        /// The HapticPulse/2 method is used to initiate a simple haptic pulse on the tracked object of the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to initiate the haptic pulse on.</param>
        /// <param name="strength">The intensity of the rumble of the controller motor. `0` to `1`.</param>
        public override void HapticPulse(VRTK_ControllerReference controllerReference, float strength = 0.5F)
        {
            uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);
            GameObject controllerGameObject = GetControllerModel(controllerReference.hand);
            MLInputController controller = GetInputControllerByGameObject(controllerGameObject);
            if (controller != null)
            {
                controller.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.Buzz, MLInputControllerFeedbackIntensity.Medium);
            }
        }

        /// <summary>
        /// The HapticPulse/2 method is used to initiate a haptic pulse based on an audio clip on the tracked object of the given controller reference.
        /// </summary>
        /// <param name="controllerReference">The reference to the tracked object to initiate the haptic pulse on.</param>
        /// <param name="clip">The audio clip to use for the haptic pattern.</param>
        public override bool HapticPulse(VRTK_ControllerReference controllerReference, AudioClip clip)
        {
            return false;
        }

        /// <summary>
        /// The IsControllerLeftHand/1 method is used to check if the given controller is the the left hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <returns>Returns true if the given controller is the left hand controller.</returns>
        public override bool IsControllerLeftHand(GameObject controller)
        {
            return CheckActualOrScriptAliasControllerIsLeftHand(controller);
        }

        /// <summary>
        /// The IsControllerLeftHand/2 method is used to check if the given controller is the the left hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <param name="actual">If true it will check the actual controller, if false it will check the script alias controller.</param>
        /// <returns>Returns true if the given controller is the left hand controller.</returns>
        public override bool IsControllerLeftHand(GameObject controller, bool actual)
        {
            return CheckControllerLeftHand(controller, actual);
        }

        /// <summary>
        /// The IsControllerRightHand/1 method is used to check if the given controller is the the right hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <returns>Returns true if the given controller is the right hand controller.</returns>
        public override bool IsControllerRightHand(GameObject controller)
        {
            return CheckActualOrScriptAliasControllerIsRightHand(controller);
        }

        /// <summary>
        /// The IsControllerRightHand/2 method is used to check if the given controller is the the right hand controller.
        /// </summary>
        /// <param name="controller">The GameObject to check.</param>
        /// <param name="actual">If true it will check the actual controller, if false it will check the script alias controller.</param>
        /// <returns>Returns true if the given controller is the right hand controller.</returns>
        public override bool IsControllerRightHand(GameObject controller, bool actual)
        {
            return CheckControllerRightHand(controller, actual);
        }

        /// <summary>
        /// The IsTouchpadStatic method is used to determine if the touchpad is currently not being moved.
        /// </summary>
        /// <param name="currentAxisValues"></param>
        /// <param name="previousAxisValues"></param>
        /// <param name="compareFidelity"></param>
        /// <returns>Returns true if the touchpad is not currently being touched or moved.</returns>
        public override bool IsTouchpadStatic(bool isTouched, Vector2 currentAxisValues, Vector2 previousAxisValues, int compareFidelity)
        {
            return (!isTouched || VRTK_SharedMethods.Vector2ShallowCompare(currentAxisValues, previousAxisValues, compareFidelity));
        }

        /// <summary>
        /// The ProcessFixedUpdate method enables an SDK to run logic for every Unity FixedUpdate
        /// </summary>
        /// <param name="controllerReference">The reference for the controller.</param>
        /// <param name="options">A dictionary of generic options that can be used to within the fixed update.</param>
        public override void ProcessFixedUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)
        {
        }

        /// <summary>
        /// The ProcessUpdate method enables an SDK to run logic for every Unity Update
        /// </summary>
        /// <param name="controllerReference">The reference for the controller.</param>
        /// <param name="options">A dictionary of generic options that can be used to within the update.</param>
        public override void ProcessUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)
        {
        }

        /// <summary>
        /// The SetControllerRenderModelWheel method sets the state of the scroll wheel on the controller render model.
        /// </summary>
        /// <param name="renderModel">The GameObject containing the controller render model.</param>
        /// <param name="state">If true and the render model has a scroll wheen then it will be displayed, if false then the scroll wheel will be hidden.</param>
        public override void SetControllerRenderModelWheel(GameObject renderModel, bool state)
        {
        }

        /// <summary>
        /// The WaitForControllerModel method determines whether the controller model for the given hand requires waiting to load in on scene start.
        /// </summary>
        /// <param name="hand">The hand to determine if the controller model will be ready for.</param>
        /// <returns>Returns true if the controller model requires loading in at runtime and therefore needs waiting for. Returns false if the controller model will be available at start.</returns>
        public override bool WaitForControllerModel(ControllerHand hand)
        {
            return true;
        }
        #endregion

        public override void OnAfterSetupLoad(VRTK_SDKSetup setup)
        {
            // Start Magic Leap Input
            MLResult result = MLInput.Start();
            if (!result.IsOk)
            {
                Debug.LogError("Couldn't start ML Input");
                return;
            }

            // get the controller. Only one controller is currently supported
            controllerConnectionHandler = FindObjectOfType<ControllerConnectionHandler>();
            if (controllerConnectionHandler == null)
            {
                Debug.LogError("Couldn't find ControllerConnectionHandler in Scene.");
                return;
            }

            cachedLeftController = controllerConnectionHandler.ConnectedController;
            if (cachedLeftController == null)
            {
                controllerConnectionHandler.OnControllerConnected += OnControllerConnected;
            }
            MLInput.OnControllerButtonDown += OnButtonDown;
            MLInput.OnControllerButtonUp += OnButtonUp;
        }

        public override void OnAfterSetupUnload(VRTK_SDKSetup setup)
        {
            MLResult result = MLInput.Start();
            if (!result.IsOk)
            {
                Debug.LogError("Couldn't start ML Input");
                return;
            }

            // get the controller. Only one controller is currently supported
            controllerConnectionHandler = FindObjectOfType<ControllerConnectionHandler>();
            if (controllerConnectionHandler == null)
            {
                Debug.LogError("Couldn't find ControllerConnectionHandler in Scene.");
                return;
            }

            cachedLeftController = controllerConnectionHandler.ConnectedController;
            if (cachedLeftController == null)
            {
                controllerConnectionHandler.OnControllerConnected += OnControllerConnected;
            }
            MLInput.OnControllerButtonDown += OnButtonDown;
            MLInput.OnControllerButtonUp += OnButtonUp;
        }

        private void OnControllerConnected(MLInputController controller)
        {
            if (controller.Hand == MLInput.Hand.Left)
            {
                cachedLeftController = controller;
            } else
            {
                //cachedRightController = controller;
            }
            controllerConnectionHandler.OnControllerConnected -= OnControllerConnected;
            controllerConnectionHandler.OnControllerDisconnected += OnControllerDisconnected;
        }

        private void OnControllerDisconnected(MLInputController controller)
        {
            if (controller.Hand == MLInput.Hand.Left)
            {
                cachedLeftController = null;
                bumperLeftPressed = false;
                homeLeftPressed = false;

            } else
            {
                //cachedRightController = null;
                //bumperRightPressed = false;
                //homeRightPressed = false;
            }
            
            controllerConnectionHandler.OnControllerConnected += OnControllerConnected;
            controllerConnectionHandler.OnControllerDisconnected -= OnControllerDisconnected;
        }

        private void OnButtonDown(byte id, MLInputControllerButton button)
        {
            MLInputController controller = GetInputControllerByIndex(id);
            if (controller == null)
            {
                return;
            }
            switch (button)
            {
                case MLInputControllerButton.Bumper:
                    if (controller.Hand == MLInput.Hand.Left)
                    {
                        bumperLeftPressed = true;
                    } else
                    {
                        bumperRightPressed = true;
                    }
                    break;

                case MLInputControllerButton.HomeTap:
                    if (controller.Hand == MLInput.Hand.Left)
                    {
                        homeLeftPressed = true;
                    } else
                    {
                        homeRightPressed = true;
                    }
                    break;

            }
            
        }

        private void OnButtonUp(byte id, MLInputControllerButton button)
        {
            MLInputController controller = GetInputControllerByIndex(id);
            if (controller == null)
            {
                return;
            }
            switch (button)
            {
                case MLInputControllerButton.Bumper:
                    if (controller.Hand == MLInput.Hand.Left)
                    {
                        bumperLeftPressed = false;
                    } else
                    {
                        bumperRightPressed = false;
                    }
                    break;

                case MLInputControllerButton.HomeTap:
                    if (controller.Hand == MLInput.Hand.Left)
                    {
                        homeLeftPressed = false;
                    } else
                    {
                        homeRightPressed = false;
                    }
                    break;
            }

        }

        private MLInputController GetInputControllerByReference(VRTK_ControllerReference controllerReference)
        {
            if (!VRTK_ControllerReference.IsValid(controllerReference))
            {
                return null;
            }

            uint index = VRTK_ControllerReference.GetRealIndex(controllerReference);
            return GetInputControllerByIndex(index);
        }

        private MLInputController GetInputControllerByIndex(uint index)
        {
            GameObject controllerGO = GetControllerByIndex(index, true);
            if (controllerGO == null)
            {
                return null;
            }

            return GetInputControllerByGameObject(controllerGO);
        }

        private MLInputController GetInputControllerByGameObject(GameObject gameObject)
        {
            MLInputController controller = null;
            foreach (KeyValuePair<MLInputController, GameObject> pair in cachedTrackedGameObjectsByController)
            {
                if (pair.Value == gameObject)
                {
                    controller = pair.Key;
                    break;
                }
            }
            return controller;
        }
#endif
    }
}
