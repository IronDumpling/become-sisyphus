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
        private readonly float transitionDuration;
        private readonly PrioritySettings defaultPriority;
        private readonly PrioritySettings activePriority;

        private CinemachineCamera currentCamera;
        private Coroutine transitionCoroutine;

        public CameraSystem(
            float transitionDuration,
            PrioritySettings defaultPriority,
            PrioritySettings activePriority)
        {
            this.transitionDuration = transitionDuration;
            this.defaultPriority = defaultPriority;
            this.activePriority = activePriority;
        }

        public void Initialize()
        {
            behaviour = GameObject.FindAnyObjectByType<CameraSystemBehaviour>();
            if (behaviour == null)
            {
                Debug.LogError($"{nameof(CameraSystemBehaviour)} not found!");
                return;
            }

            if (behaviour.OutsideWorldCamera == null || behaviour.InsideWorldCamera == null)
            {
                Debug.LogError($"Camera references not set in {nameof(CameraSystem)}! OutsideWorldCamera: {behaviour.OutsideWorldCamera != null}, InsideWorldCamera: {behaviour.InsideWorldCamera != null}");
                return;
            }

            // Set initial priorities
            behaviour.OutsideWorldCamera.Priority.Value = defaultPriority.Value;
            behaviour.InsideWorldCamera.Priority.Value = defaultPriority.Value;

            // Set initial camera based on game state
            switch (GameManager.Instance.CurrentState)
            {
                case GameState.Climbing:
                    currentCamera = behaviour.OutsideWorldCamera;
                    behaviour.OutsideWorldCamera.Priority.Value = activePriority.Value;
                    break;
                case GameState.Sailing:
                    currentCamera = behaviour.InsideWorldCamera;
                    behaviour.InsideWorldCamera.Priority.Value = activePriority.Value;
                    break;
            }
        }

        public void SwitchToOutsideWorld()
        {
            if (!ValidateSystemState()) return;
            if (currentCamera == behaviour.OutsideWorldCamera) return;
            
            if (transitionCoroutine != null)
                behaviour.StopCoroutine(transitionCoroutine);

            transitionCoroutine = behaviour.StartCoroutine(TransitionToCamera(behaviour.OutsideWorldCamera));
            Debug.Log("Switching to Outside World Camera");
        }

        public void SwitchToInsideWorld()
        {
            if (!ValidateSystemState()) return;
            if (currentCamera == behaviour.InsideWorldCamera) return;

            if (transitionCoroutine != null)
                behaviour.StopCoroutine(transitionCoroutine);

            transitionCoroutine = behaviour.StartCoroutine(TransitionToCamera(behaviour.InsideWorldCamera));
            Debug.Log("Switching to Inside World Camera");
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
            if (targetCamera == null || currentCamera == null)
            {
                Debug.LogError($"Target camera or current camera is null! Target: {targetCamera != null}, Current: {currentCamera != null}");
                yield break;
            }

            float elapsedTime = 0;
            float startPriority = currentCamera.Priority.Value;
            float oldCameraPriority = startPriority;

            while (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / transitionDuration;

                // Smoothly decrease current camera's priority
                oldCameraPriority = Mathf.Lerp(startPriority, defaultPriority.Value, t);
                currentCamera.Priority.Value = Mathf.RoundToInt(oldCameraPriority);

                // Smoothly increase target camera's priority
                float newPriority = Mathf.Lerp(defaultPriority.Value, activePriority.Value, t);
                targetCamera.Priority.Value = Mathf.RoundToInt(newPriority);

                yield return null;
            }

            // Ensure final priorities are set correctly
            currentCamera.Priority.Value = defaultPriority.Value;
            targetCamera.Priority.Value = activePriority.Value;
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