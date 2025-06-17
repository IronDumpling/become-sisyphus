using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using BecomeSisyphus.Core;
using BecomeSisyphus.Core.Interfaces;

namespace BecomeSisyphus.Managers.Systems
{
    public class CameraSystem : ICameraSystem
    {
        private CameraSystemBehaviour behaviour;
        private CinemachineCamera currentCamera;
        private Coroutine transitionCoroutine;

        public void Initialize()
        {
            Debug.Log("CameraSystem: Starting initialization...");
            
            behaviour = GameObject.FindAnyObjectByType<CameraSystemBehaviour>();
            Debug.Log($"CameraSystem: Found CameraSystemBehaviour: {behaviour != null}");
            
            if (!ValidateSystemState()) return;

            Debug.Log($"CameraSystem: Current game state: {GameManager.Instance.CurrentState}");
            Debug.Log($"CameraSystem: GameState enum values - Climbing: {(int)GameState.Climbing}, Sailing: {(int)GameState.Sailing}");
            Debug.Log($"CameraSystem: Current state as int: {(int)GameManager.Instance.CurrentState}");
            
            // Set initial camera based on game state
            switch (GameManager.Instance.CurrentState)
            {
                case GameState.Climbing:
                    Debug.Log("CameraSystem: Setting initial camera to OutsideWorldCamera");
                    currentCamera = behaviour.OutsideWorldCamera;
                    Debug.Log($"CameraSystem: currentCamera set to: {currentCamera != null}");
                    behaviour.OutsideWorldCamera.Priority.Value = behaviour.ActivePriority;
                    Debug.Log($"CameraSystem: OutsideWorldCamera priority set to: {behaviour.OutsideWorldCamera.Priority.Value}");
                    break;
                case GameState.Sailing:
                    Debug.Log("CameraSystem: Setting initial camera to InsideWorldCamera");
                    currentCamera = behaviour.InsideWorldCamera;
                    Debug.Log($"CameraSystem: currentCamera set to: {currentCamera != null}");
                    behaviour.InsideWorldCamera.Priority.Value = behaviour.ActivePriority;
                    Debug.Log($"CameraSystem: InsideWorldCamera priority set to: {behaviour.InsideWorldCamera.Priority.Value}");
                    break;
                case GameState.MainMenu:
                default:
                    Debug.LogWarning($"CameraSystem: Game state is {GameManager.Instance.CurrentState}, defaulting to OutsideWorldCamera");
                    currentCamera = behaviour.OutsideWorldCamera;
                    Debug.Log($"CameraSystem: currentCamera set to default: {currentCamera != null}");
                    behaviour.OutsideWorldCamera.Priority.Value = behaviour.ActivePriority;
                    Debug.Log($"CameraSystem: OutsideWorldCamera priority set to: {behaviour.OutsideWorldCamera.Priority.Value}");
                    break;
            }
            
            Debug.Log($"CameraSystem: Final currentCamera state: {currentCamera != null}");
            if (currentCamera != null)
            {
                Debug.Log($"CameraSystem: Current camera name: {currentCamera.name}");
            }
            Debug.Log("CameraSystem: Initialization completed");
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
                Debug.LogError($"CameraSystem: Current GameState during transition: {GameManager.Instance.CurrentState}");
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