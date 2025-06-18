using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using BecomeSisyphus.Core;
using BecomeSisyphus.Core.Interfaces;
using BecomeSisyphus.Core.GameStateSystem;

namespace BecomeSisyphus.Managers.Systems
{
    public class CameraSystem : ICameraSystem
    {
        private CameraSystemBehaviour behaviour;
        private CinemachineCamera currentCamera;
        private Coroutine transitionCoroutine;
        private Camera mainCamera;

        // Layer constants
        private const int OUTSIDE_LAYER = 6;
        private const int INSIDE_LAYER = 7;

        public void Initialize()
        {
            Debug.Log("CameraSystem: Starting initialization...");
            
            behaviour = GameObject.FindAnyObjectByType<CameraSystemBehaviour>();
            Debug.Log($"CameraSystem: Found CameraSystemBehaviour: {behaviour != null}");
            
            if (!ValidateSystemState()) return;

            // Get the main camera reference
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                mainCamera = GameObject.FindAnyObjectByType<Camera>();
            }
            
            if (mainCamera == null)
            {
                Debug.LogError("CameraSystem: Main camera not found!");
                return;
            }
            
            Debug.Log($"CameraSystem: Found main camera: {mainCamera.name}");
            
            // Use new state system to determine initial camera
            var stateManager = GameStateManager.Instance;
            if (stateManager != null)
            {
                var currentStatePath = stateManager.GetCurrentStatePath();
                Debug.Log($"CameraSystem: Current state path: {currentStatePath}");
                
                // Set initial camera based on current state
                if (currentStatePath.Contains("OutsideWorld"))
                {
                    Debug.Log("CameraSystem: Setting initial camera to OutsideWorldCamera");
                    currentCamera = behaviour.OutsideWorldCamera;
                    behaviour.OutsideWorldCamera.Priority.Value = behaviour.ActivePriority;
                    SetCullingMaskForOutsideWorld();
                }
                else if (currentStatePath.Contains("InsideWorld"))
                {
                    Debug.Log("CameraSystem: Setting initial camera to InsideWorldCamera");
                    currentCamera = behaviour.InsideWorldCamera;
                    behaviour.InsideWorldCamera.Priority.Value = behaviour.ActivePriority;
                    SetCullingMaskForInsideWorld();
                }
                else
                {
                    Debug.LogWarning($"CameraSystem: Unknown state path {currentStatePath}, defaulting to OutsideWorldCamera");
                    currentCamera = behaviour.OutsideWorldCamera;
                    behaviour.OutsideWorldCamera.Priority.Value = behaviour.ActivePriority;
                    SetCullingMaskForOutsideWorld();
                }
            }
            else
            {
                Debug.LogWarning("CameraSystem: GameStateManager not available, defaulting to OutsideWorldCamera");
                currentCamera = behaviour.OutsideWorldCamera;
                behaviour.OutsideWorldCamera.Priority.Value = behaviour.ActivePriority;
                SetCullingMaskForOutsideWorld();
            }
            
            Debug.Log($"CameraSystem: Final currentCamera state: {currentCamera != null}");
            if (currentCamera != null)
            {
                Debug.Log($"CameraSystem: Current camera name: {currentCamera.name}");
            }
            Debug.Log("CameraSystem: Initialization completed");
        }

        /// <summary>
        /// Set culling mask for outside world (exclude inside layer)
        /// </summary>
        private void SetCullingMaskForOutsideWorld()
        {
            if (mainCamera != null)
            {
                int mask = mainCamera.cullingMask;
                mask &= ~(1 << INSIDE_LAYER); // Remove inside layer
                mask |= (1 << OUTSIDE_LAYER); // Ensure outside layer is included
                mainCamera.cullingMask = mask;
                
                Debug.Log($"CameraSystem: Main camera culling mask set for Outside World - excluding layer {INSIDE_LAYER} (Inside). Mask: {mask}");
            }
        }

        /// <summary>
        /// Set culling mask for inside world (exclude outside layer)
        /// </summary>
        private void SetCullingMaskForInsideWorld()
        {
            if (mainCamera != null)
            {
                int mask = mainCamera.cullingMask;
                mask &= ~(1 << OUTSIDE_LAYER); // Remove outside layer
                mask |= (1 << INSIDE_LAYER); // Ensure inside layer is included
                mainCamera.cullingMask = mask;
                
                Debug.Log($"CameraSystem: Main camera culling mask set for Inside World - excluding layer {OUTSIDE_LAYER} (Outside). Mask: {mask}");
            }
        }

        public void SwitchToOutsideWorld()
        {
            Debug.Log("CameraSystem: SwitchToOutsideWorld called");
            
            if (!ValidateSystemState()) 
            {
                Debug.LogError("CameraSystem: ValidateSystemState failed in SwitchToOutsideWorld");
                return;
            }
            
            if (currentCamera == behaviour.OutsideWorldCamera) 
            {
                Debug.Log("CameraSystem: Already using OutsideWorldCamera, skipping switch");
                return;
            }
            
            Debug.Log("CameraSystem: Starting transition to OutsideWorldCamera");
            
            // Set culling mask for outside world
            SetCullingMaskForOutsideWorld();
            
            if (transitionCoroutine != null)
            {
                Debug.Log("CameraSystem: Stopping existing transition coroutine");
                behaviour.StopCoroutine(transitionCoroutine);
            }

            transitionCoroutine = behaviour.StartCoroutine(TransitionToCamera(behaviour.OutsideWorldCamera));
            Debug.Log("CameraSystem: Transition to Outside World Camera started");
        }

        public void SwitchToInsideWorld()
        {
            Debug.Log("CameraSystem: SwitchToInsideWorld called");
            
            if (!ValidateSystemState()) 
            {
                Debug.LogError("CameraSystem: ValidateSystemState failed in SwitchToInsideWorld");
                return;
            }
            
            if (currentCamera == behaviour.InsideWorldCamera) 
            {
                Debug.Log("CameraSystem: Already using InsideWorldCamera, skipping switch");
                return;
            }
            
            Debug.Log("CameraSystem: Starting transition to InsideWorldCamera");

            // Set culling mask for inside world
            SetCullingMaskForInsideWorld();

            if (transitionCoroutine != null)
            {
                Debug.Log("CameraSystem: Stopping existing transition coroutine");
                behaviour.StopCoroutine(transitionCoroutine);
            }

            transitionCoroutine = behaviour.StartCoroutine(TransitionToCamera(behaviour.InsideWorldCamera));
            Debug.Log("CameraSystem: Transition to Inside World Camera started");
        }

        private bool ValidateSystemState()
        {
            if (behaviour == null)
            {
                Debug.LogError($"{nameof(CameraSystemBehaviour)} is not initialized!");
                return false;
            }

            if (behaviour.OutsideWorldCamera == null || behaviour.InsideWorldCamera == null)
            {
                Debug.LogError($"Camera references are missing! OutsideWorldCamera: {behaviour.OutsideWorldCamera != null}, InsideWorldCamera: {behaviour.InsideWorldCamera != null}");
                return false;
            }

            return true;
        }

        private IEnumerator TransitionToCamera(CinemachineCamera targetCamera)
        {
            Debug.Log($"CameraSystem: TransitionToCamera called - Target: {targetCamera?.name}, Current: {currentCamera?.name}");
            
            if (targetCamera == null || currentCamera == null)
            {
                Debug.LogError($"CameraSystem: Target camera or current camera is null! Target: {targetCamera != null}, Current: {currentCamera != null}");
                Debug.LogError($"CameraSystem: This usually means the initialization didn't set currentCamera properly.");
                var stateManager = GameStateManager.Instance;
                if (stateManager != null)
                {
                    Debug.LogError($"CameraSystem: Current state path during transition: {stateManager.GetCurrentStatePath()}");
                }
                else
                {
                    Debug.LogError($"CameraSystem: GameStateManager not available during transition");
                }
                yield break;
            }

            float elapsedTime = 0;
            float startPriority = currentCamera.Priority.Value;
            float oldCameraPriority = startPriority;

            while (elapsedTime < behaviour.TransitionDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / behaviour.TransitionDuration;

                // Smoothly decrease current camera's priority
                oldCameraPriority = Mathf.Lerp(startPriority, behaviour.DefaultPriority, t);
                currentCamera.Priority.Value = Mathf.RoundToInt(oldCameraPriority);

                // Smoothly increase target camera's priority
                float newPriority = Mathf.Lerp(behaviour.DefaultPriority, behaviour.ActivePriority, t);
                targetCamera.Priority.Value = Mathf.RoundToInt(newPriority);

                yield return null;
            }

            // Ensure final priorities are set correctly
            currentCamera.Priority.Value = behaviour.DefaultPriority;
            targetCamera.Priority.Value = behaviour.ActivePriority;
            currentCamera = targetCamera;
        }

        public void Update()
        {
            // No update logic needed for now
        }

        public void Cleanup()
        {
            if (transitionCoroutine != null && behaviour != null)
            {
                behaviour.StopCoroutine(transitionCoroutine);
            }
        }
    }
} 