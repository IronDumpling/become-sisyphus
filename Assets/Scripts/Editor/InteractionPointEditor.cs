using UnityEngine;
using UnityEditor;
using BecomeSisyphus.Core;
using BecomeSisyphus.Core.Data;

namespace BecomeSisyphus.Editor
{
    [CustomEditor(typeof(InteractionPointBehaviour))]
    public class InteractionPointEditor : UnityEditor.Editor
    {
        private SerializedProperty interactionId;
        private SerializedProperty pointType;
        private SerializedProperty displayTitle;
        private SerializedProperty displayDescription;
        private SerializedProperty interactionRadius;
        
        private SerializedProperty restEffectiveness;
        private SerializedProperty restDuration;
        private SerializedProperty canRepairVessel;
        
        private SerializedProperty illuminationRadius;
        private SerializedProperty providesNavigation;
        private SerializedProperty lightkeeperMessage;
        
        private SerializedProperty hasSecrets;
        private SerializedProperty explorationReward;
        
        private SerializedProperty salvageDifficulty;
        private SerializedProperty isExhausted;
        
        private SerializedProperty showGizmos;
        private SerializedProperty gizmoColor;

        private void OnEnable()
        {
            interactionId = serializedObject.FindProperty("interactionId");
            pointType = serializedObject.FindProperty("pointType");
            displayTitle = serializedObject.FindProperty("displayTitle");
            displayDescription = serializedObject.FindProperty("displayDescription");
            interactionRadius = serializedObject.FindProperty("interactionRadius");
            
            restEffectiveness = serializedObject.FindProperty("restEffectiveness");
            restDuration = serializedObject.FindProperty("restDuration");
            canRepairVessel = serializedObject.FindProperty("canRepairVessel");
            
            illuminationRadius = serializedObject.FindProperty("illuminationRadius");
            providesNavigation = serializedObject.FindProperty("providesNavigation");
            lightkeeperMessage = serializedObject.FindProperty("lightkeeperMessage");
            
            hasSecrets = serializedObject.FindProperty("hasSecrets");
            explorationReward = serializedObject.FindProperty("explorationReward");
            
            salvageDifficulty = serializedObject.FindProperty("salvageDifficulty");
            isExhausted = serializedObject.FindProperty("isExhausted");
            
            showGizmos = serializedObject.FindProperty("showGizmos");
            gizmoColor = serializedObject.FindProperty("gizmoColor");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Interaction Point Settings", EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(interactionId);
            EditorGUILayout.PropertyField(pointType);
            EditorGUILayout.PropertyField(displayTitle);
            EditorGUILayout.PropertyField(displayDescription);
            EditorGUILayout.PropertyField(interactionRadius);
            
            EditorGUILayout.Space();
            
            // Show type-specific settings
            InteractionPointType type = (InteractionPointType)pointType.enumValueIndex;
            
            switch (type)
            {
                case InteractionPointType.Harbor:
                    EditorGUILayout.LabelField("Harbor Settings", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(restEffectiveness);
                    EditorGUILayout.PropertyField(restDuration);
                    EditorGUILayout.PropertyField(canRepairVessel);
                    break;
                    
                case InteractionPointType.Lighthouse:
                    EditorGUILayout.LabelField("Lighthouse Settings", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(illuminationRadius);
                    EditorGUILayout.PropertyField(providesNavigation);
                    EditorGUILayout.PropertyField(lightkeeperMessage);
                    break;
                    
                case InteractionPointType.Island:
                    EditorGUILayout.LabelField("Island Settings", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(hasSecrets);
                    EditorGUILayout.PropertyField(explorationReward);
                    break;
                    
                case InteractionPointType.Salvage:
                    EditorGUILayout.LabelField("Salvage Settings", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(salvageDifficulty);
                    EditorGUILayout.PropertyField(isExhausted);
                    break;
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(showGizmos);
            EditorGUILayout.PropertyField(gizmoColor);
            
            // Runtime info
            if (Application.isPlaying)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Runtime Info", EditorStyles.boldLabel);
                
                InteractionPointBehaviour behaviour = (InteractionPointBehaviour)target;
                var info = behaviour.GetInfo();
                
                EditorGUILayout.LabelField($"ID: {info.id}");
                EditorGUILayout.LabelField($"Type: {info.type}");
                EditorGUILayout.LabelField($"Position: {info.position}");
                EditorGUILayout.LabelField($"Radius: {info.radius}");
            }
            
            serializedObject.ApplyModifiedProperties();
        }
        
        private void OnSceneGUI()
        {
            InteractionPointBehaviour behaviour = (InteractionPointBehaviour)target;
            
            // Draw interaction radius handle
            Handles.color = Color.green;
            float newRadius = Handles.RadiusHandle(Quaternion.identity, behaviour.transform.position, behaviour.InteractionRadius);
            
            if (newRadius != behaviour.InteractionRadius)
            {
                Undo.RecordObject(behaviour, "Changed Interaction Radius");
                behaviour.SetInteractionRadius(newRadius);
                EditorUtility.SetDirty(behaviour);
            }
            
            // Draw label
            Handles.Label(behaviour.transform.position + Vector3.up * 2, 
                         $"{behaviour.DisplayTitle}\n({behaviour.PointType})",
                         EditorStyles.whiteLabel);
        }
    }
    
    [System.Serializable]
    public class InteractionPointSetupWindow : EditorWindow
    {
        [MenuItem("Tools/Become Sisyphus/Interaction Point Setup")]
        public static void ShowWindow()
        {
            GetWindow<InteractionPointSetupWindow>("Interaction Point Setup");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("Interaction Points in Scene", EditorStyles.boldLabel);
            
            InteractionPointBehaviour[] interactionPoints = FindObjectsByType<InteractionPointBehaviour>(FindObjectsSortMode.None);
            
            if (interactionPoints.Length == 0)
            {
                EditorGUILayout.HelpBox("No interaction points found in the scene. " +
                                      "Add InteractionPointBehaviour components to GameObjects to create interaction points.",
                                      MessageType.Info);
            }
            else
            {
                foreach (var point in interactionPoints)
                {
                    EditorGUILayout.BeginHorizontal();
                    
                    if (GUILayout.Button($"Select", GUILayout.Width(60)))
                    {
                        Selection.activeGameObject = point.gameObject;
                        SceneView.FrameLastActiveSceneView();
                    }
                    
                    var info = point.GetInfo();
                    GUILayout.Label($"{info.type} - {point.DisplayTitle} ({point.gameObject.name})");
                    
                    EditorGUILayout.EndHorizontal();
                }
            }
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Create New Interaction Point"))
            {
                CreateNewInteractionPoint();
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Tip: Select an InteractionPointBehaviour in the scene to see detailed settings and visual handles.",
                                  MessageType.Info);
        }
        
        private void CreateNewInteractionPoint()
        {
            GameObject newPoint = new GameObject("New Interaction Point");
            newPoint.AddComponent<InteractionPointBehaviour>();
            
            // Position it in front of the scene camera
            if (SceneView.lastActiveSceneView != null)
            {
                Camera cam = SceneView.lastActiveSceneView.camera;
                newPoint.transform.position = cam.transform.position + cam.transform.forward * 5f;
            }
            
            Selection.activeGameObject = newPoint;
            Undo.RegisterCreatedObjectUndo(newPoint, "Create Interaction Point");
        }
    }
} 