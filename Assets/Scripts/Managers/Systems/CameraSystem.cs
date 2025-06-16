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
            behaviour = GameObject.FindAnyObjectByType<CameraSystemBehaviour>();
            if (!ValidateSystemState()) return;

            // Set initial camera based on game state
            switch (GameManager.Instance.CurrentState)
            {
                case GameState.Climbing:
                    currentCamera = behaviour.OutsideWorldCamera;
                    behaviour.OutsideWorldCamera.Priority.Value = behaviour.ActivePriority;
                    break;
                case GameState.Sailing:
                    currentCamera = behaviour.InsideWorldCamera;
                    behaviour.InsideWorldCamera.Priority.Value = behaviour.ActivePriority;
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