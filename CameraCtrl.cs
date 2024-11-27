using System;
using Unity.Cinemachine;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraCtrl : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private CinemachinePositionComposer cinemachineComposer;

    [Header("Move Settings")]
    [SerializeField] private bool drawBounds;
    [SerializeField] private Bounds cameraBounds;
    [SerializeField] private float camSpeed;
    [SerializeField] private Vector2 screenPercentageDetection;

    [Header("Zoom Settings")]
    [SerializeField] private float minZoomDistance;
    [SerializeField] private float maxZoomDistance;
    [SerializeField] private float zoomSpeed;

    private Vector2 normalScreenPercentage;
    private Vector2 NormalMousePos => new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
    private bool InScreenLeft => NormalMousePos.x < normalScreenPercentage.x && Application.isFocused;
    private bool InScreenRight => NormalMousePos.x > 1 - normalScreenPercentage.x && Application.isFocused;
    private bool InScreenTop => NormalMousePos.y < normalScreenPercentage.y && Application.isFocused;
    private bool InScreenBottom => NormalMousePos.y > 1 - normalScreenPercentage.y && Application.isFocused;

    private bool _cameraSet;

    private void Awake()
    {
        normalScreenPercentage = screenPercentageDetection * 0.01f;
    }

    private void OnValidate()
    {
        normalScreenPercentage = screenPercentageDetection * 0.01f;
    }

    private void Update()
    {
        // SetCameraForAutoAssignTeam();
        MoveCamera();
        ZoomCamera();
    }

    private void MoveCamera()
    {
        if (InScreenLeft)
        {
            transform.position += Vector3.left * (camSpeed * Time.deltaTime);
        }

        if (InScreenRight)
        {
            transform.position += Vector3.right * (camSpeed * Time.deltaTime);
        }

        if (InScreenTop)
        {
            transform.position += Vector3.back * (camSpeed * Time.deltaTime);
        }

        if (InScreenBottom)
        {
            transform.position += Vector3.forward * (camSpeed * Time.deltaTime);
        }

        if (!cameraBounds.Contains(transform.position))
        {
            transform.position = cameraBounds.ClosestPoint(transform.position);
        }
    }

    private void ZoomCamera()
    {
        if (Mathf.Abs(Input.mouseScrollDelta.y) > float.Epsilon)
        {
            cinemachineComposer.CameraDistance -= Input.mouseScrollDelta.y * zoomSpeed * Time.deltaTime;
            cinemachineComposer.CameraDistance =
                Mathf.Clamp(cinemachineComposer.CameraDistance, minZoomDistance, maxZoomDistance);
        }
    }
    private void OnDrawGizmos()
    {
        if (!drawBounds) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(cameraBounds.center, cameraBounds.size);
    }
}