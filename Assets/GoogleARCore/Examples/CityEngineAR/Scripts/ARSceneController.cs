//-----------------------------------------------------------------------
// <copyright file="CloudAnchorController.cs" company="Google">
//
// Copyright 2018 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

// -----------------------------------------------------------------------
// <copyright file="CloudAnchorController.cs" company="Esri">
//
// Modifications Copyright 2018 Esri Inc. All Rights Reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCore.Examples.CityEngineARTemplate
{
    using System.Collections.Generic;
    using GoogleARCore;
    using GoogleARCore.CrossPlatform;
    using UnityEngine;
    using UnityEngine.UI;

#if UNITY_EDITOR
    // Set up touch input propagation while using Instant Preview in the editor.
    using Input = InstantPreviewInput;
#endif

    /// <summary>
    /// Controller for the Cloud Anchor Example Project.
    /// </summary>
    public class ARSceneController : MonoBehaviour
    {
        [Header("Google Cloud Anchor Settings")]
        /// <summary>
        /// Manages sharing Anchor Ids across the local network to clients using Unity's NetworkServer.  There
        /// are many ways to share this data and this not part of the ARCore Cloud Anchors API surface.
        /// </summary>
        public RoomSharingServer RoomSharingServer;

        /// <summary>
        /// A controller for managing UI associated with the example.
        /// </summary>
        public ARSceneUIController UIController;

        [Header("Model Settings")]
        /// <summary>
        /// Float multiplier for Elevating the model above the Surface.
        /// </summary>
        [Range(-1000.0f, 1000.0f)]
        public float ModelElevation = 0.5f;

        /// <summary>
        /// Scaling factor to ensure the CityEngine model
        /// Recommended Scaling between 0.001f and 0.005 for a few city blocks
        /// </summary>
        [Range(-0.0001f, 100.0f)]
        public float ModelScalingFactor = 0.0005f;

        [Header("CityEngine Model Prefab")]
        /// <summary>
        /// An CityEngine model Prefab to visually represent anchors in the scene
        /// </summary>
        public GameObject CityengineModel;

        /// <summary>
        /// Input String denoting the Existing Conditions Model
        /// </summary>
        //public string ExistingConditionsName = "ExistingConditions";
        public GameObject ExistingConditionsNestedPrefab;

        /// <summary>
        /// Input String denoting the Demolition Model
        /// </summary>
        public GameObject DemolitionNestedPrefab;

        /// <summary>
        /// Input String denoting Scenario 1 Model
        /// </summary>
        public GameObject Scenario1NestedPrefab;

        /// <summary>
        /// Input String denoting Scenario 2 Model
        /// </summary>
        public GameObject Scenario2NestedPrefab;

        /// <summary>
        /// Input String denoting Scenario 3 Model
        /// </summary>
        public GameObject Scenario3NestedPrefab;

        [Header("ARCore")]

        /// <summary>
        /// The root for ARCore-specific GameObjects in the scene.
        /// </summary>
        public GameObject ARCoreRoot;

        [Header("ARKit")]

        /// <summary>
        /// The root for ARKit-specific GameObjects in the scene.
        /// </summary>
        public GameObject ARKitRoot;

        /// <summary>
        /// The first-person camera used to render the AR background texture for ARKit.
        /// </summary>
        public Camera ARKitFirstPersonCamera;

        /// <summary>
        /// The loopback ip address.
        /// </summary>
        private const string k_LoopbackIpAddress = "127.0.0.1";

        /// <summary>
        /// The rotation in degrees need to apply to model when the model is placed.
        /// </summary>
        private const float k_ModelRotation = 180.0f;

        /// <summary>
        /// A helper object to ARKit functionality.
        /// </summary>
        private ARKitHelper m_ARKit = new ARKitHelper();

        /// <summary>
        /// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
        /// </summary>
        private bool m_IsQuitting = false;

        /// <summary>
        /// The last placed anchor.
        /// </summary>
        private Component m_LastPlacedAnchor = null;

        /// <summary>
        /// The last resolved anchor.
        /// </summary>
        private XPAnchor m_LastResolvedAnchor = null;

        /// <summary>
        /// The current cloud anchor mode.
        /// </summary>
        private ApplicationMode m_CurrentMode = ApplicationMode.Ready;

        /// <summary>
        /// Current local room to attach next Anchor to.
        /// </summary>
        private int m_CurrentRoom;

        /// <summary>
        /// Enumerates modes the example application can be in.
        /// </summary>
        public enum ApplicationMode
        {
            Ready,
            Hosting,
            Resolving,
        }

        /// <summary>
        /// The Unity Start() method.
        /// </summary>
        public void Start()
        {
            if (Application.platform != RuntimePlatform.IPhonePlayer)
            {
                ARCoreRoot.SetActive(true);
                ARKitRoot.SetActive(false);
            }
            else
            {
                ARCoreRoot.SetActive(false);
                ARKitRoot.SetActive(true);
            }
            _ResetStatus();
        }


        /// <summary>
        /// The Unity Update() method.
        /// </summary>
        /// 

        // boolean operations for storing current model status information
        // These booleans trigger which function to run
        private bool modelPlaced = false;
        private bool switchScenario = false;
        private bool modelFirstUpdate = false;
        public void Update()
        {
            _UpdateApplicationLifecycle();

            // If we are not in hosting mode or the user has already placed an anchor then the update is complete.
            if (m_CurrentMode != ApplicationMode.Hosting || m_LastPlacedAnchor != null)
            {
                // Ensure that the model has been placed as a cloud anchor.
                if (modelPlaced == true && switchScenario == true)
                {
                    // Check that game object exists in the scene
                    if (MyLastScenario != null)
                    {
                        // Check that the selected scenario is not already enabled
                        if (MyLastScenario != MyCurrentScenario)
                        {
                            // Obtain Names of Prefab Game Objects attached to the Anchor Game Object
                            // TODO resolve potential issue where explicitly calling prefab game object of interest is required --
                            // ----- Necessary if more than one Prefab is added to Anchor. Example.. multiple city locations in future...
                            foreach (Transform child in m_LastPlacedAnchor.transform)
                            {
                                // Obtain names of first-level Game Objects nested within the Prefab Game Object
                                foreach (Transform nestedChild in child.transform)
                                {

                                    if (nestedChild.name == DemolitionNestedPrefab.name ||
                                        nestedChild.name == Scenario1NestedPrefab.name ||
                                        nestedChild.name == Scenario2NestedPrefab.name ||
                                        nestedChild.name == Scenario3NestedPrefab.name)
                                    {
                                        string model2Disable = m_LastPlacedAnchor.name + "/" + CityengineModel.name + "(Clone)" + "/" + nestedChild.name;
                                        GameObject disableGameObj = GameObject.Find(model2Disable);
                                        disableGameObj.SetActive(false);
                                    }
                                }
                            }
                        }
                    }
                    if (MyLastScenario != MyCurrentScenario)
                    {
                        // Disable Demolition model upon initial scenario change.
                        if (modelFirstUpdate == false)
                        {
                            string demolitionModel2Disable = m_LastPlacedAnchor.name + "/" + CityengineModel.name + "(Clone)" + "/" + DemolitionNestedPrefab.name;
                            GameObject demolitionModel2DisableObj = GameObject.Find(demolitionModel2Disable);
                            demolitionModel2DisableObj.SetActive(false);
                            modelFirstUpdate = true;
                        }
                        // Enable Selected Scenario Model
                        string model2Enable = m_LastPlacedAnchor.name + "/" + CityengineModel.name + "(Clone)" + "/" + MyCurrentScenario;
                        GameObject currentGameObject = GameObject.Find(model2Enable);
                        UIController.ShowCurrentScenarioText(MyCurrentScenario);
                        // Move model up slightly to avoid z-fighting
                        currentGameObject.transform.position += new Vector3(0.0f, 0.0001f, 0.0f);
                        currentGameObject.SetActive(true);

                        // Do something Cool Here!
                        // ex: Bobble Game Object up and down.
                        //currentGameObject.AddComponent<TransformObj>();
                        //currentGameObject.AddComponent<ScaleUpAndDown>();
                        currentGameObject.AddComponent<ToggleGlowShader>();
                        switchScenario = false;
                        return;
                    }
                }
                return;
            }

            Touch touch;
            // If the player has not touched the screen then the update is complete.
            if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
            {
                return;
            }

            // Raycast against the location the player touched to search for planes.
            if (Application.platform != RuntimePlatform.IPhonePlayer)
            {
                TrackableHit hit;
                if (Frame.Raycast(touch.position.x, touch.position.y,
                        TrackableHitFlags.PlaneWithinPolygon, out hit))
                {
                    m_LastPlacedAnchor = hit.Trackable.CreateAnchor(hit.Pose);
                }
            }
            else
            {
                Pose hitPose;
                if (m_ARKit.RaycastPlane(ARKitFirstPersonCamera, touch.position.x, touch.position.y, out hitPose))
                {
                    m_LastPlacedAnchor = m_ARKit.CreateAnchor(hitPose);
                }
            }

            if (m_LastPlacedAnchor != null)
            {
            // Instantiate Model at the hit pose.
            var modelObject = Instantiate(_GetModelPrefab(), m_LastPlacedAnchor.transform.position,
                m_LastPlacedAnchor.transform.rotation);
            
            // Transform Operation Was here!

            // Compensate for the hitPose rotation facing away from the raycast (i.e. camera).
            modelObject.transform.Rotate(0, k_ModelRotation, 0, Space.Self);

            // Make model a child of the anchor.
            //modelObject.transform.parent = m_LastPlacedAnchor.transform; // Original ARCore Code
            //modelObject.transform.SetParent(m_LastPlacedAnchor.transform); // Same as ARCore but Unity-based
            modelObject.transform.SetParent(m_LastPlacedAnchor.transform, false); //Geof7015 Esri (Player retains its Local Orientation)


                _HostLastPlacedAnchor();
            modelPlaced = true;

            // Scale the Model
            float newXScale = (float)(ModelScalingFactor);
            float newYScale = (float)(ModelScalingFactor);
            float newZScale = (float)(ModelScalingFactor);
            modelObject.transform.localScale = new Vector3(newXScale, newYScale, newZScale);

            // Move the GameObject up
            modelObject.transform.position += new Vector3(0, ModelElevation, 0);
            
            // Do Something Crazy to the model!
            // ex: Make it Spin
            //modelObject.AddComponent<SpinY>();
            }
        }

        // Strings for storing current and previously selected Scenario's
        private string MyLastScenario;
        private string MyCurrentScenario;

        // Check if Existing Conditions Button has been selected
        public void OnExistingConditionsClick()
        {
            MyLastScenario = MyCurrentScenario;
            MyCurrentScenario = DemolitionNestedPrefab.name;
            switchScenario = true;
        }

        // Check if Scenario 1 Button has been selected
        public void OnScenario1Click()
        {
            MyLastScenario = MyCurrentScenario;
            MyCurrentScenario = Scenario1NestedPrefab.name;
            switchScenario = true;
        }

        // Check if Scenario 2 Button has been selected
        public void OnScenario2Click()
        {
            MyLastScenario = MyCurrentScenario;
            MyCurrentScenario = Scenario2NestedPrefab.name;
            switchScenario = true;
        }

        // Check if Scenario 3 Button has been selected
        public void OnScenario3Click()
        {
            MyLastScenario = MyCurrentScenario;
            MyCurrentScenario = Scenario3NestedPrefab.name;
            switchScenario = true;
        }

        /// <summary>
        /// Handles user intent to enter a mode where they can place an anchor to host or to exit this mode if
        /// already in it.
        /// </summary>
        public void OnEnterHostingModeClick()
        {
            if (m_CurrentMode == ApplicationMode.Hosting)
            {
                m_CurrentMode = ApplicationMode.Ready;
                _ResetStatus();
                return;
            }
            m_CurrentMode = ApplicationMode.Hosting;
            m_CurrentRoom = Random.Range(1, 9999);
            UIController.SetRoomTextValue(m_CurrentRoom);
            UIController.ShowHostingModeBegin();
        }

        /// <summary>
        /// Handles a user intent to enter a mode where they can input an anchor to be resolved or exit this mode if
        /// already in it.
        /// </summary>
        public void OnEnterResolvingModeClick()
        {
            if (m_CurrentMode == ApplicationMode.Resolving)
            {
                m_CurrentMode = ApplicationMode.Ready;
                _ResetStatus();
                return;
            }
            m_CurrentMode = ApplicationMode.Resolving;
            UIController.ShowResolvingModeBegin();
        }

        /// <summary>
        /// Handles the user intent to resolve the cloud anchor associated with a room they have typed into the UI.
        /// </summary>
        public void OnResolveRoomClick()
        {
            var roomToResolve = UIController.GetRoomInputValue();
            if (roomToResolve == 0)
            {
                UIController.ShowResolvingModeBegin("Invalid room code.");
                return;
            }
            string ipAddress =
                UIController.GetResolveOnDeviceValue() ? k_LoopbackIpAddress : UIController.GetIpAddressInputValue();

            UIController.ShowResolvingModeAttemptingResolve();
            RoomSharingClient roomSharingClient = new RoomSharingClient();
            roomSharingClient.GetAnchorIdFromRoom(roomToResolve, ipAddress, (bool found, string cloudAnchorId) =>
            {
                if (!found)
                {
                    UIController.ShowResolvingModeBegin("Invalid room code.");
                }
                else
                {
                    _ResolveAnchorFromId(cloudAnchorId);
                }
            });
        }

        /// <summary>
        /// Hosts the user placed cloud anchor and associates the resulting Id with the current room.
        /// </summary>
        private void _HostLastPlacedAnchor()
        {
#if !UNITY_IOS
            var anchor = (Anchor)m_LastPlacedAnchor;
#else
            var anchor = (UnityEngine.XR.iOS.UnityARUserAnchorComponent)m_LastPlacedAnchor;
#endif
            UIController.ShowHostingModeAttemptingHost();
            XPSession.CreateCloudAnchor(anchor).ThenAction(result =>
            {
                if (result.Response != CloudServiceResponse.Success)
                {
                    UIController.ShowHostingModeBegin(
                        string.Format("Failed to host cloud anchor: {0}", result.Response));
                    return;
                }
                RoomSharingServer.SaveCloudAnchorToRoom(m_CurrentRoom, result.Anchor);
                UIController.ShowHostingModeBegin("Cloud anchor was created and saved.");
            });
        }

        /// <summary>
        /// Resolves an anchor id and instantiates an model prefab on it.
        /// </summary>
        /// <param name="cloudAnchorId">Cloud anchor id to be resolved.</param>
        private void _ResolveAnchorFromId(string cloudAnchorId)
        {
            XPSession.ResolveCloudAnchor(cloudAnchorId).ThenAction((System.Action<CloudAnchorResult>)(result =>
            {
                if (result.Response != CloudServiceResponse.Success)
                {
                    UIController.ShowResolvingModeBegin(string.Format("Resolving Error: {0}.", result.Response));
                    return;
                }
                m_LastResolvedAnchor = result.Anchor;
                Instantiate(_GetModelPrefab(), result.Anchor.transform);
                UIController.ShowResolvingModeSuccess();
            }));
        }

        /// <summary>
        /// Resets the internal status and UI.
        /// </summary>
        private void _ResetStatus()
        {
            // Reset internal status.
            m_CurrentMode = ApplicationMode.Ready;
            if (m_LastPlacedAnchor != null)
            {
                Destroy(m_LastPlacedAnchor.gameObject);
            }
            m_LastPlacedAnchor = null;
            if (m_LastResolvedAnchor != null)
            {
                Destroy(m_LastResolvedAnchor.gameObject);
            }
            m_LastResolvedAnchor = null;
            UIController.ShowReadyMode();
        }

        /// <summary>
        /// Gets the platform-specific model prefab.
        /// </summary>
        /// <returns>The platform-specific model prefab.</returns>
        private GameObject _GetModelPrefab()
        {
            return CityengineModel;
            //return Application.platform != RuntimePlatform.IPhonePlayer ?
            //    ARCoreCityengineModelPrefab : ARKitCityengineModelPrefab;
        }


        /// <summary>
        /// Check and update the application lifecycle.
        /// </summary>
        private void _UpdateApplicationLifecycle()
        {
            // Exit the app when the 'back' button is pressed.
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }
            var sleepTimeout = SleepTimeout.NeverSleep;

#if !UNITY_IOS
            // Only allow the screen to sleep when not tracking.
            if (Session.Status != SessionStatus.Tracking)
            {
                const int lostTrackingSleepTimeout = 15;
                sleepTimeout = lostTrackingSleepTimeout;
            }
#endif

            Screen.sleepTimeout = sleepTimeout;

            if (m_IsQuitting)
            {
                return;
            }

            // Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
            if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
            {
                _ShowAndroidToastMessage("Camera permission is needed to run this application.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
            else if (Session.Status.IsError())
            {
                _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
        }

        /// <summary>
        /// Actually quit the application.
        /// </summary>
        private void _DoQuit()
        {
            Application.Quit();
        }

        /// <summary>
        /// Show an Android toast message.
        /// </summary>
        /// <param name="message">Message string to show in the toast.</param>
        private void _ShowAndroidToastMessage(string message)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                        message, 0);
                    toastObject.Call("show");
                }));
            }
        }
    }
}
