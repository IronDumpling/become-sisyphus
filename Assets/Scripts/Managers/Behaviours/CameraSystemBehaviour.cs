using UnityEngine;
using Unity.Cinemachine;
using BecomeSisyphus;

public class CameraSystemBehaviour : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private CinemachineCamera outsideWorldCamera;
    [SerializeField] private CinemachineCamera insideWorldCamera;

    // Properties to access cameras
    public CinemachineCamera OutsideWorldCamera => outsideWorldCamera;
    public CinemachineCamera InsideWorldCamera => insideWorldCamera;

    private void Awake()
    {
        FindCameraReferences();
    }

    private void OnValidate()
    {
        FindCameraReferences();
    }

    private void FindCameraReferences()
    {
        // First try to find in children
        if (outsideWorldCamera == null)
            outsideWorldCamera = transform.Find("OutsideWorldCamera")?.GetComponent<CinemachineCamera>();
        
        if (insideWorldCamera == null)
            insideWorldCamera = transform.Find("InsideWorldCamera")?.GetComponent<CinemachineCamera>();

        // If not found in children, try global search
        if (outsideWorldCamera == null)
            outsideWorldCamera = GameObject.Find("OutsideWorldCamera")?.GetComponent<CinemachineCamera>();
        
        if (insideWorldCamera == null)
            insideWorldCamera = GameObject.Find("InsideWorldCamera")?.GetComponent<CinemachineCamera>();

        if (outsideWorldCamera == null || insideWorldCamera == null)
        {
            Debug.LogError($"Camera references not set in {nameof(CameraSystemBehaviour)}! OutsideWorldCamera: {outsideWorldCamera != null}, InsideWorldCamera: {insideWorldCamera != null}");
        }
    }
}
